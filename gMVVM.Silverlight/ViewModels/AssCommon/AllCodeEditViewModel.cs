using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using gMVVM.Views.AssCommon;
using gMVVM.Views.Search;
using mvvmCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM.ViewModels.AssCommon
{
    public class AllCodeEditViewModel:ViewModelBase
    {
        private  CM_ALLCODEClient cmAllCodeClient;
        public AllCodeEditViewModel()
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
                ActionMenuButton.actionControl.SetAllAction(null, null, null, null, null, null, null);       
                this.IsView = false;
                this.IsEnable = true;
            }
            else
            {
                ActionMenuButton.actionControl.SetAllAction(null, actionCommand, null, null, null);
                this.isEdit = obj.isEdit;
                if (isEdit) this.IsEnable = true;
                else this.IsEnable =false;
                this.HeaderText = this.isEdit ? CommonResource.lblEdit : CommonResource.lblInsert;
                this.IsView = true;
            }

            this.messagePop.Reset();
            this.CurrentItem = obj.currentObject != null ? (obj.currentObject as CM_ALLCODE_SearchResult)
                : new CM_ALLCODE_SearchResult()
                {                    
                    CDVAL = "",
                    CDNAME = "",
                    CONTENT = "",                                      
                    CDTYPE = "",                    
                    LSTODR = 0                    
                };          
            this.OnPropertyChanged("IsView");          
            this.OnPropertyChanged("IsChecked");
            this.OnPropertyChanged("");
        }
   
        #region[All Properties]

        public ObservableCollection<CM_DEPT_GROUP_SearchResult> GroupData
        { get; set; }

        public bool IsView { get; set; }

        //Tinh trang record co dang hoat trong hay khong
        public string IsChecked { get; set; }

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
        public string IsEnable1
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
        private CM_ALLCODE_SearchResult currentItem;
        public CM_ALLCODE_SearchResult CurrentItem
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
        private bool isEnable;
        public bool IsEnable
        {
            get
            {
                return this.isEnable;
            }
            set
            {
                this.isEnable = value;
                this.OnPropertyChanged("IsEnable");
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
        public ObservableCollection<CM_DEPARTMENT_SearchResult> DepartmentData { get; set; }

        #endregion

        #region[Action Functions]

        private void OkAction()
        {
            RefreshValidator();
            if (!this.messagePop.HasError())
            {                
                if (this.isEdit)
                {                    
                    MyHelper.IsBusy();
                    this.cmAllCodeClient.CM_ALLCODE_UpdAsync(this.currentItem);
                }
                else
                {                    
                    MyHelper.IsBusy();
                    this.cmAllCodeClient.CM_ALLCODE_InsAsync(this.currentItem);
                }
            }
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new ParentMessage() { currentObject = null, level = "Main" });
        }       
        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.currentItem.CONTENT == "" || this.currentItem.CONTENT == null)
                this.messagePop.SetError(AssCommonResource.Content + notEmpty);
            if (this.currentItem.CDVAL == "" || this.currentItem.CDVAL == null)
                this.messagePop.SetError(AssCommonResource.CDVal + notEmpty);
            if (this.currentItem.CDTYPE == "" || this.currentItem.CDTYPE == null)
                this.messagePop.SetError(AssCommonResource.CDType + notEmpty);
            if (this.currentItem.CDNAME == "" || this.currentItem.CDNAME == null)
                this.messagePop.SetError(AssCommonResource.CDName + notEmpty);           
        }
        private void Load()
        {
            try
            {               
                this.cmAllCodeClient = new CM_ALLCODEClient();                
                this.cmAllCodeClient.CM_ALLCODE_UpdCompleted += new EventHandler<CM_ALLCODE_UpdCompletedEventArgs>(updateCompleted);
                this.cmAllCodeClient.CM_ALLCODE_InsCompleted += new EventHandler<CM_ALLCODE_InsCompletedEventArgs>(insertCompleted);
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }


        private void getDepartmentCompleted(object sender, SearchDEPARTMENTCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.DepartmentData = e.Result;
                else
                    this.DepartmentData = new ObservableCollection<CM_DEPARTMENT_SearchResult>();

                this.OnPropertyChanged("DepartmentData");
                this.OnPropertyChanged("CurrentItem");                
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);           
            }
        }

        private void updateCompleted(object sender, CM_ALLCODE_UpdCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result.Equals("0"))
                {
                    this.IsApproved = Visibility.Collapsed.ToString();
                    this.OnPropertyChanged("IsApproved");
                    this.messagePop.Successful(ValidatorResource.UpdateSuccessful);
                }
                else
                {                    
                    this.messagePop.SetSingleError(ValidatorResource.ErrorUpdate + "\n " + e.Result.ErrorDesc);
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

        private void insertCompleted(object sender, CM_ALLCODE_InsCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result.Equals("0"))
                {
                    this.CurrentItem.ID = e.Result.ID.Value;
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { isEdit = this.isEdit, currentObject = this.currentItem, action = "Insert", level = "Main" });
                }
                else
                {
                    this.messagePop.SetSingleError(ValidatorResource.ErrorInsert + "\n " + e.Result.ErrorDesc);
                }
            }
            catch (Exception )
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

            private AllCodeEditViewModel viewModel;
            public ActionButton(AllCodeEditViewModel viewModel)
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
                    
                    default: break;
                }

            }
        }        
    }
}
