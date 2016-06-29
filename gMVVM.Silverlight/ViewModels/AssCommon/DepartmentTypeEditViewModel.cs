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
    public class DepartmentTypeEditViewModel:ViewModelBase
    {
        private CM_DEPT_GROUPClient departmentTypeClient;
        public DepartmentTypeEditViewModel()
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
            }

            this.messagePop.Reset();
            this.CurrentItem = obj.currentObject != null ? (obj.currentObject as CM_DEPT_GROUP_SearchResult)
                : new CM_DEPT_GROUP_SearchResult()
                {
                    MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                    GROUP_ID = "",
                    GROUP_CODE = "",
                    GROUP_NAME = "",                                      
                    NOTES = "",
                    AUTH_STATUS = CurrentSystemLogin.IsAppFunction ? "U" : "A",
                    RECORD_STATUS = "1",
                    CREATE_DT = DateTime.Now
                };
           
            this.IsChecked = this.currentItem.RECORD_STATUS.Equals("1") ? "True" : "False";

            if (this.currentItem.AUTH_STATUS.Equals("A") && CurrentSystemLogin.IsAppFunction)
                this.IsApproved = Visibility.Visible.ToString();
            else
                this.IsApproved = Visibility.Collapsed.ToString();

            this.OnPropertyChanged("IsView");
            this.OnPropertyChanged("IsApproved");
            this.OnPropertyChanged("IsChecked");
            this.OnPropertyChanged("");
        }

        private string preStatus;//Gia tri AUTH_STATUS truoc khi duyet, cap nhat

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
        private CM_DEPT_GROUP_SearchResult currentItem;
        public CM_DEPT_GROUP_SearchResult CurrentItem
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
        public ObservableCollection<CM_DEPARTMENT_SearchResult> DepartmentData { get; set; }

        #endregion

        #region[Action Functions]

        private void OkAction()
        {
            RefreshValidator();
            if (!this.messagePop.HasError())
            {
                this.currentItem.RECORD_STATUS = this.IsChecked.Equals("True") ? "1" : "0";
                this.currentItem.RECORD_STATUS_NAME=this.currentItem.RECORD_STATUS.Equals("0")?"Không hoạt động":"Hoạt động";
                this.currentItem.AUTH_STATUS_NAME = this.currentItem.AUTH_STATUS.Equals("A") ? "Đang duyệt" : "Chờ duyệt";
                if (this.isEdit)
                {
                    this.preStatus = this.currentItem.AUTH_STATUS;
                    if (this.currentItem.AUTH_STATUS.Equals("A"))
                    {
                        this.currentItem.AUTH_STATUS = "U";
                        this.currentItem.AUTH_STATUS_NAME = "Chờ Duyệt";
                    }
                    MyHelper.IsBusy();
                    this.departmentTypeClient.CM_DEPT_GROUP_UpdAsync(this.currentItem);
                }
                else
                {                    
                    this.currentItem.CREATE_DT = DateTime.Now;                    
                    MyHelper.IsBusy();
                    this.departmentTypeClient.CM_DEPT_GROUP_InsAsync(this.currentItem);
                }
            }
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new ParentMessage() { currentObject = null, level = "Main" });
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
            this.currentItem.AUTH_STATUS = "A";
            this.currentItem.CHECKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
            this.currentItem.APPROVE_DT = DateTime.Now;
            MyHelper.IsBusy();
            this.departmentTypeClient.CM_DEPT_GROUP_ApprAsync(this.currentItem);
        }
        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.currentItem.GROUP_CODE == "" || this.currentItem.GROUP_CODE == null)
                this.messagePop.SetError(AssCommonResource.DepartmentTypeID + notEmpty);
            if (this.currentItem.GROUP_NAME == "" || this.currentItem.GROUP_NAME == null)
                this.messagePop.SetError(AssCommonResource.DepartmentTypeName + notEmpty);
        }
        private void Load()
        {
            try
            {               
                this.departmentTypeClient = new CM_DEPT_GROUPClient();                
                this.departmentTypeClient.CM_DEPT_GROUP_UpdCompleted += new EventHandler<CM_DEPT_GROUP_UpdCompletedEventArgs>(updateCompleted);
                this.departmentTypeClient.CM_DEPT_GROUP_InsCompleted += new EventHandler<CM_DEPT_GROUP_InsCompletedEventArgs>(insertCompleted);
                this.departmentTypeClient.CM_DEPT_GROUP_ApprCompleted += new EventHandler<CM_DEPT_GROUP_ApprCompletedEventArgs>(approveCompleted);                               
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

        private void updateCompleted(object sender, CM_DEPT_GROUP_UpdCompletedEventArgs e)
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
                    this.currentItem.AUTH_STATUS = this.preStatus;
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

        private void insertCompleted(object sender, CM_DEPT_GROUP_InsCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result.Equals("0"))
                {
                    this.CurrentItem.GROUP_ID = e.Result.GROUP_ID;
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

        private void approveCompleted(object sender, CM_DEPT_GROUP_ApprCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result.Equals("0"))
                {
                    this.currentItem.AUTH_STATUS = "A";
                    this.currentItem.AUTH_STATUS_NAME = "Đã Duyệt";
                    this.IsApproved = Visibility.Visible.ToString();
                    this.OnPropertyChanged("IsApproved");
                    this.messagePop.Successful(ValidatorResource.SuccessfulApprove);
                }
                else
                {
                    this.messagePop.SetSingleError(ValidatorResource.ErrorApprove + "\n " + e.Result.ErrorDesc);
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

            private DepartmentTypeEditViewModel viewModel;
            public ActionButton(DepartmentTypeEditViewModel viewModel)
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
