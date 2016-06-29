using gMVVM.gMVVMService;
using gMVVM.ViewModels.Common;
using gMVVM.CommonClass;
using gMVVM.Resources;
using GalaSoft.MvvmLight.Messaging;
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

namespace gMVVM.ViewModels.SystemRole
{
    public class MenuEditViewModel : ViewModelBase
    {
        private TLMENUClient menuClient;

        public MenuEditViewModel()
        {
            this.actionCommand = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            this.Load();
            Messenger.Default.Register<ChildMessage>(this, CurrencyEditSendMessageRecieve);
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
                if (this.isEdit)
                {
                    this.HeaderText = CommonResource.lblEdit;
                    this.IconPath = DefineClass.IconEdit;
                }
                else
                {
                    this.HeaderText = CommonResource.lblInsert;
                    this.IconPath = DefineClass.IconInsert;
                }
                //this.HeaderText = this.isEdit ? CommonResource.lblEdit : CommonResource.lblInsert;
                this.IsView = true;
            }

            this.messagePop.Reset();
            this.CurrentMenu = obj.currentObject != null ? (obj.currentObject as TL_MENU)
                : new TL_MENU()
                {
                    MENU_NAME = "",
                    MENU_LINK = "/",
                    MENU_PARENT = "",
                    AUTH_STATUS = "U",
                    ISAPPROVE = "0",
                    MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                    MENU_ORDER = 0,
                    MENU_NAME_EL = "",
                    ISAPPROVE_FUNC = "0"
                };

            if (this.currentMenu.AUTH_STATUS.Equals("A"))
                this.IsApproved = Visibility.Visible.ToString();
            else
                this.IsApproved = Visibility.Collapsed.ToString();

            this.OnPropertyChanged("IsView");
            this.OnPropertyChanged("IsApproved");
            this.OnPropertyChanged("IsApproveFuntion");
            this.OnPropertyChanged("");
        }

        private string preAuthStatus = "";        

        #region[All Properties]

        private object iconPath = "/gMVVM;component/Data/Icons/edit_icon.png";
        public object IconPath
        {
            get
            {
                return this.iconPath;
            }
            set
            {
                this.iconPath = value;
                this.OnPropertyChanged("IconPath");
            }
        }

        public bool IsApproveFuntion
        {
            get
            {
                return this.currentMenu.ISAPPROVE_FUNC.Equals("1") ? true : false;
            }

            set
            {
                this.currentMenu.ISAPPROVE_FUNC = value ? "1" : "0";
                this.OnPropertyChanged("IsApproveFuntion");
            }
        }

        public bool IsView { get; set; }

        public string IsApproved { get; set; }
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
        private TL_MENU currentMenu;
        public TL_MENU CurrentMenu
        {
            get
            {
                return this.currentMenu;
            }

            set
            {
                this.currentMenu = value;
                this.OnPropertyChanged("CurrentMenu");
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

        private ObservableCollection<TL_MENU> parentData;
        public ObservableCollection<TL_MENU> ParentData
        {
            get
            {
                return this.parentData;
            }

            set
            {
                this.parentData = value;
                this.OnPropertyChanged("ParentData");
            }
        }        

        #endregion

        #region[Action Functions]

        private void OkAction()
        {
            RefreshValidator();
            if (!this.messagePop.HasError())
            {

                if (this.isEdit)
                {
                    this.currentMenu.MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
                    this.preAuthStatus = this.currentMenu.AUTH_STATUS;
                    this.currentMenu.AUTH_STATUS = "U";
                    MyHelper.IsBusy();
                    this.menuClient.UpdateTLMENUAsync(this.currentMenu);
                }
                else
                {
                    MyHelper.IsBusy();
                    this.menuClient.InsertTLMENUAsync(this.currentMenu);
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
            if (this.currentMenu.AUTH_STATUS.Equals("A")) return;
            if (this.currentMenu.MAKER_ID.Equals(CurrentSystemLogin.CurrentUser.TLNANME))
            {
                this.messagePop.SetSingleError(ValidatorResource.lblErrorPermission);
                return;
            }
            this.currentMenu.CHECKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
            this.currentMenu.DATE_APPROVE = DateTime.Now;
            //this.menuClient.ApproveCMAGENTAsync(this.currentMenu);
        }        

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.currentMenu.MENU_NAME.Equals(""))
                this.messagePop.SetError(SystemRoleResource.MENU_NAME + notEmpty);
            if (this.currentMenu.MENU_NAME_EL.Equals(""))
                this.messagePop.SetError(SystemRoleResource.MENU_NAME_EL + notEmpty);

            //if (this.currentMenu.MENU_PARENT == null)
            //    this.messagePop.SetError(SystemRoleResource.MENU_PARENT + notEmpty);

            if (this.currentMenu.MENU_LINK.Equals("\\") || this.currentMenu.MENU_LINK.Equals(""))
                this.currentMenu.MENU_LINK = "/";                    
        }
        private void Load()
        {
            try
            {
                this.menuClient = new TLMENUClient();

                this.menuClient.GetParentTLMENUAsync();
                this.menuClient.GetParentTLMENUCompleted += (s, e) =>
                {
                    this.parentData = new ObservableCollection<TL_MENU>();
                    this.parentData.Add(new TL_MENU() { MENU_PARENT = "", MENU_NAME = "---NULL---" });
                    if (e.Result != null && e.Result.Count > 0)
                    {
                        foreach (var item in e.Result)
                            this.parentData.Add(new TL_MENU() { MENU_PARENT = item.MENU_ID.ToString(), MENU_NAME = item.MENU_NAME });                        
                    }
                    
                    this.OnPropertyChanged("ParentData");
                };
                
                this.menuClient.InsertTLMENUCompleted += new EventHandler<InsertTLMENUCompletedEventArgs>(insertCurrencyCompleted);
                this.menuClient.UpdateTLMENUCompleted += new EventHandler<UpdateTLMENUCompletedEventArgs>(updateCurrencyCompleted);
                //this.menuClient.ApproveCMAGENTCompleted += new EventHandler<ApproveCMAGENTCompletedEventArgs>(approveComplete);
                                
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        //private void approveComplete(object sender, ApproveCMAGENTCompletedEventArgs e)
        //{
        //    if (e.Result)
        //    {
        //        this.currentAgent.AUTH_STATUS = "A";
        //        this.currentAgent.ISAPPROVE = "1";
        //        this.IsApproved = Visibility.Visible.ToString();
        //        this.OnPropertyChanged("IsApproved");
        //        this.messagePop.Successful(ValidatorResource.SuccessfulApprove);
        //    }
        //    else
        //        this.messagePop.SetSingleError(ValidatorResource.ErrorApprove);
        //}

        private void updateCurrencyCompleted(object sender, UpdateTLMENUCompletedEventArgs e)
        {
            try
            {
                if (e.Result)
                {
                    this.IsApproved = Visibility.Collapsed.ToString();
                    this.OnPropertyChanged("IsApproved");
                    this.messagePop.Successful(ValidatorResource.UpdateSuccessful);
                }
                else
                {
                    this.currentMenu.AUTH_STATUS = this.preAuthStatus;
                    this.messagePop.SetSingleError(ValidatorResource.ErrorUpdate);
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

        private void insertCurrencyCompleted(object sender, InsertTLMENUCompletedEventArgs e)
        {
            try
            {
                if (!e.Result.Equals(""))
                {
                    this.currentMenu.MENU_ID = int.Parse(e.Result);
                    if (this.currentMenu.MENU_PARENT.Equals(""))
                        this.ParentData.Add(new TL_MENU() { MENU_PARENT = e.Result, MENU_NAME = this.currentMenu.MENU_NAME });
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { isEdit = this.isEdit, currentObject = this.currentMenu });

                }
                else
                {
                    this.messagePop.SetSingleError(ValidatorResource.ErrorInsert);
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

            private MenuEditViewModel viewModel;
            public ActionButton(MenuEditViewModel viewModel)
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
