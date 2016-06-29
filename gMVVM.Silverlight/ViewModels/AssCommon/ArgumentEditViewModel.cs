using GalaSoft.MvvmLight.Messaging;
using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using mvvmCommon;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;


namespace gMVVM.ViewModels.AssCommon
{
    public class ArgumentEditViewModel:ViewModelBase
    {
           private ARGUMENTClient argumentClient;
           private CM_ALLCODEClient cmAllCodeClient;

           public ArgumentEditViewModel()
           {

               this.actionCommand = new ActionButton(this);
               this.messagePop = new MessageAlarmViewModel();
               Messenger.Default.Register<ChildMessage>(this, CurrencyEditSendMessageRecieve);

               this.Load();
           }

        private void CurrencyEditSendMessageRecieve(ChildMessage obj)
        {

            if (obj.action.Equals(ActionMenuButton.View))
            {
                ActionMenuButton.actionControl.SetAllAction(null, null, null, null, null, null, actionCommand);
                this.HeaderText = CommonResource.lblApprove;
                this.IsView = false;
            }
            else
            {
                ActionMenuButton.actionControl.SetAllAction(null, actionCommand, null, null, null);
                this.isEdit = obj.isEdit;
                this.HeaderText = this.isEdit ? CommonResource.lblEdit : CommonResource.lblInsert;
                this.IsView = true;
                if (isEdit) this.IsEnables =true;
                else
                this.IsEnables = false;
            }

            this.messagePop.Reset();
            this.CurrentItem = obj.currentObject != null ? (obj.currentObject as SYS_PARAMETER)
                : new SYS_PARAMETER()
                {
                    MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                    ParaKey = "",
                    ParaValue = "",
                    DataType = "",

                    Description = "",
                    RECORD_STATUS = "1",
                    AUTH_STATUS = CurrentSystemLogin.IsAppFunction ? "U" : "A"
                };
              //if (this.currentItem.AUTH_STATUS.Equals("A")&&obj.action.Equals(ActionMenuButton.Edit))
              //  {
              //      ActionMenuButton.actionControl.SetAllAction(null, null, null, null, null, null, null);
              //      this.IsView = false;                  
              //  }
            this.IsChecked = this.currentItem.RECORD_STATUS.Equals("1") ? "True" : "False";
            if (this.currentItem.AUTH_STATUS.Equals("A"))
                this.IsApproved = Visibility.Visible.ToString();
            else
                this.IsApproved = Visibility.Collapsed.ToString();

            this.OnPropertyChanged("IsView");
            this.OnPropertyChanged("IsChecked");
            this.OnPropertyChanged("IsApproved");
            this.OnPropertyChanged("");
        }        

        #region[All Properties]

        public bool IsView { get; set; }

        public string IsApproved { get; set; }

        public string IsChecked { get; set; }
        public ObservableCollection<CM_ALLCODE_ByIdResult> DataType { get; set; }
        //Message Alarm validate
        private MessageAlarmViewModel messagePop;
        public MessageAlarmViewModel MessagePop
        {
            get
            {
                return this.messagePop;
            }

            set
            {
                this.messagePop = value;
                this.OnPropertyChanged("MessagePop");
            }
        }
        // function insert or edit
        private bool isEdit;
        public string IsEnable
        {
            get
            {
                return (!this.isEdit).ToString();
            }
        }
        //IsEnables
        private bool isEnables;
        public bool IsEnables
        {
            get { return this.isEnables; }
            set
            {
                this.isEnables = value;
                this.OnPropertyChanged("IsEnables");
            }
        }
        //Action Button Property    
        private ICommand actionCommand;
        public ICommand ActionCommand
        {
            get
            {
                return this.actionCommand;
            }

            set 
            {
                this.actionCommand = value;
                this.OnPropertyChanged("ActionCommand");
            }
        }

        //Current Items edit
        private SYS_PARAMETER currentItem;
        public SYS_PARAMETER CurrentItem
        {
            get
            {
                return this.currentItem;
            }

            set
            {
                this.currentItem = value;
                this.OnPropertyChanged("CurrentItem");
            }
        }                

        //set header title
        private string headerText;
        public string HeaderText
        {
            get
            {
                return this.headerText;
            }

            set
            {
                this.headerText = value;
                this.OnPropertyChanged("HeaderText");
            }
        }
        //ParaValue       
        public string ParaValue
        {
            get
            {
                return this.currentItem.ParaValue;
            }

            set
            {
                
                if (this.currentItem.DataType.Equals("Num")&&(!ValidateTest.IsNumber(value)||!ValidateTest.IsDecimalNumber(value)))
                {
                    this.messagePop.SetSingleError(ValidatorResource.PARAMETERSError);
                    
                }
                else this.currentItem.ParaValue = value;
                if (this.messagePop.HasError())
                    return;
                this.OnPropertyChanged("ParaValue");
            }
        }
        #endregion

        #region[Action Functions]

        private void OkAction()
        {
            RefreshValidator();
            if (!this.messagePop.HasError())
            {
                this.currentItem.RECORD_STATUS = this.IsChecked.Equals("True") ? "1" : "0";
                if (this.isEdit)
                {
                    //Dang o trang thai duyet an duyet
                    if (this.currentItem.AUTH_STATUS.Equals("A"))
                    {
                        this.currentItem.AUTH_STATUS = "U";                        
                    }
                    MyHelper.IsBusy();
                    this.argumentClient.UpdateArgumentAsync(this.currentItem);
                }
                else
                {                    
                    this.currentItem.CREATE_DT = DateTime.Now;
                    MyHelper.IsBusy();
                    this.argumentClient.InsertArgumentAsync(this.currentItem);
                }
            }
        }

        private void CancelAction()
        {            
            Messenger.Default.Send(new ParentMessage() { currentObject = null });
        }

        private void Approve()
        {
            this.messagePop.Reset();
            if (this.currentItem.AUTH_STATUS.Equals("A")) return;
            if (this.currentItem.MAKER_ID.Equals(CurrentSystemLogin.CurrentUser.TLNANME))
            {
                this.messagePop.SetSingleError(ValidatorResource.lblErrorPermission);
                return;
            }
            this.currentItem.CHECKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
            this.currentItem.APPROVE_DT = DateTime.Now;
            MyHelper.IsBusy();
            this.argumentClient.ApproveArgumentAsync(this.currentItem);
        }

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;

            if (this.currentItem.ParaKey == null || this.currentItem.ParaKey.Equals(""))
                this.messagePop.SetError(AssCommonResource.Arg_Code + notEmpty);
            if (this.currentItem.ParaValue.Equals(""))
                this.messagePop.SetError(AssCommonResource.Arg_Name + notEmpty);
            if (this.currentItem.DataType.Equals(""))
                this.messagePop.SetError(AssCommonResource.DataType + notEmpty);
            if (this.currentItem.Description.Equals(""))
                this.messagePop.SetError(AssCommonResource.Description + notEmpty);
        }

        private void Load()
        {
            try
            {
                this.argumentClient = new ARGUMENTClient();
                this.cmAllCodeClient = new CM_ALLCODEClient();
                this.cmAllCodeClient.CM_ALLCODE_ByIdAsync("DATA_TYPE", "PARA");
                this.cmAllCodeClient.CM_ALLCODE_ByIdCompleted += (s, e) =>
                {
                    if (e.Result != null)
                    {
                        this.DataType = e.Result;
                        this.OnPropertyChanged("DataType");
                    }
                   
                };
                this.argumentClient.UpdateArgumentCompleted += new EventHandler<UpdateArgumentCompletedEventArgs>(updateCompleted);
                this.argumentClient.InsertArgumentCompleted += new EventHandler<InsertArgumentCompletedEventArgs>(insetCompleted);
                this.argumentClient.ApproveArgumentCompleted += new EventHandler<ApproveArgumentCompletedEventArgs>(approveCompleted);
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void updateCompleted(object sender, UpdateArgumentCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Equals(ListMessage.True))
                {
                    this.IsApproved = Visibility.Collapsed.ToString();
                    this.OnPropertyChanged("IsApproved");
                    this.messagePop.Successful(ValidatorResource.UpdateSuccessful);
                }
                else
                {
                    //this..AUTH_STATUS = this.preAuthStatus;
                    if (ListMessage.Message.ContainsKey(e.Result))
                        this.messagePop.SetSingleError(ValidatorResource.ErrorUpdate + "\n " + ListMessage.Message[e.Result]);
                    else
                        this.messagePop.SetSingleError(ValidatorResource.ErrorUpdate + "\n " + e.Result);
                }
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
            finally
            {
                MyHelper.IsFree();
            }
        }

        private void insetCompleted(object sender, InsertArgumentCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Equals(ListMessage.True))
                {                    
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { isEdit = this.isEdit, currentObject = this.currentItem });
                }
                else
                {
                    if (ListMessage.Message.ContainsKey(e.Result))
                        this.messagePop.SetSingleError(ValidatorResource.ErrorInsert + "\n " + ListMessage.Message[e.Result]);
                    else
                        this.messagePop.SetSingleError(ValidatorResource.ErrorInsert + "\n " );
                }
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
            finally
            {
                MyHelper.IsFree();
            }
        }

        private void approveCompleted(object sender, ApproveArgumentCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Equals(ListMessage.True))
                {
                    this.currentItem.AUTH_STATUS = "A";
                    this.IsApproved = Visibility.Visible.ToString();
                    this.OnPropertyChanged("IsApproved");
                    this.messagePop.Successful(ValidatorResource.SuccessfulApprove);
                }
                else
                {
                    if (ListMessage.Message.ContainsKey(e.Result))
                        this.messagePop.SetSingleError(ValidatorResource.ErrorApprove + "\n " + ListMessage.Message[e.Result]);
                    else
                        this.messagePop.SetSingleError(ValidatorResource.ErrorApprove + "\n " + e.Result);
                }
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
            finally
            {
                MyHelper.IsFree();
            }
        }

        #endregion

        //Command button action
        public class ActionButton : ICommand
        {

            private ArgumentEditViewModel viewModel;
            public ActionButton(ArgumentEditViewModel viewModel)
            {
                this.viewModel = viewModel;
                this.viewModel.PropertyChanged += (s, e) =>
                {
                    if (this.CanExecuteChanged != null)
                    {
                        this.CanExecuteChanged(this, new EventArgs());
                    }
                };
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                switch (parameter.ToString())
                {
                    case "Cancel": this.viewModel.CancelAction(); break;
                    case ActionMenuButton.Update: this.viewModel.OkAction(); break;
                    case ActionMenuButton.Approve: this.viewModel.Approve(); break;                    
                    default: break;
                }

            }
        }
    }
}
