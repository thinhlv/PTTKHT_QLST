using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using GalaSoft.MvvmLight.Messaging;
using mvvmCommon;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using gMVVM.gMVVMService;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using gMVVM.Views.Search;

namespace gMVVM.ViewModels.AssCommon
{
    public class DepartmentEditViewModel:ViewModelBase
    {
        private BranchClient branchClient;
        private DEPARTMENTClient departmentClient;
        private SearchScreen branchChild;       
        private CM_DEPT_GROUPClient departmentTypeClient;
        public DepartmentEditViewModel()
        {
            this.actionCommand = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            Messenger.Default.Register<ChildMessage>(this, CurrencyEditSendMessageRecieve);

            this.Load();
        }

        private void CurrencyEditSendMessageRecieve(ChildMessage obj)
        {

            if (obj.level.Equals("Main"))
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
                    this.BRANCH_NAME = "";
                }

                this.messagePop.Reset();
                this.CurrentItem = obj.currentObject != null ? (obj.currentObject as CM_DEPARTMENT_SearchResult)
                    : new CM_DEPARTMENT_SearchResult()
                    {
                        MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                        DEP_ID = "",
                        DEP_CODE = "",
                        DEP_NAME = "",
                        BRANCH_ID = "",
                        GROUP_ID="",
                        TEL = "",
                        NOTES = "",
                        AUTH_STATUS = CurrentSystemLogin.IsAppFunction ? "U" : "A",
                        RECORD_STATUS = "1",
                        CREATE_DT = DateTime.Now
                    };

                if (obj.action.Equals(ActionMenuButton.View) || obj.action.Equals(ActionMenuButton.Edit))
                {
                    this.BRANCH_NAME = this.currentItem.BRANCH_NAME;
                   
                }

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
            if (obj.level.Equals("_BranchSearching"))
            {
                if (obj.currentObject != null)
                {

                    this.BRANCH_NAME = (obj.currentObject as CM_BRANCH_SearchResult).BRANCH_NAME;
                    this.currentItem.BRANCH_ID = (obj.currentObject as CM_BRANCH_SearchResult).BRANCH_ID;
                    this.branchChild.Close();
                    
                }
            }
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
        //Ten don vi
        private string branchName;
        public string BRANCH_NAME
        {
            get
            {
                return this.branchName;
            }
            set
            {
                this.branchName = value;
                this.OnPropertyChanged("BRANCH_NAME");
            }
        }
        private string dep_GROUP_NAME;
        public string DEP_GROUP_NAME
        {
            get
            {
                return this.dep_GROUP_NAME;
            }
            set
            {
                this.dep_GROUP_NAME = value;
                this.OnPropertyChanged("DEP_GROUP_NAME");
            }
        }
        //Current Items edit
        private CM_DEPARTMENT_SearchResult currentItem;
        public CM_DEPARTMENT_SearchResult CurrentItem
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
                if (this.isEdit)
                {
                    //Cap nhat lai thong tin khi sua chua de duyet
                    if (CurrentSystemLogin.IsAppFunction)
                    {
                        this.preStatus = this.currentItem.AUTH_STATUS;
                        this.currentItem.AUTH_STATUS = "U";
                        this.currentItem.AUTH_STATUS_NAME = "Chờ Duyệt";
                        this.currentItem.MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
                        this.currentItem.CHECKER_ID = "";
                    }         
                    MyHelper.IsBusy();
                    this.departmentClient.UpdateDEPARTMENTAsync(this.currentItem);
                }
                else
                {                    
                    this.currentItem.CREATE_DT = DateTime.Now;                    
                    MyHelper.IsBusy();
                    this.departmentClient.InsertDEPARTMENTAsync(this.currentItem);
                }
            }
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new ParentMessage() { currentObject = null });
        }
        private void BranchSearch()
        {
            if (this.branchChild == null)
            {
                BranchSearching branchSearch = new BranchSearching();
                this.branchChild = new SearchScreen(SearchResource.lblBranchSearching, branchSearch, branchSearch.viewModel);
            }

            Messenger.Default.Send("Child");         
            this.branchChild.Show();
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
            this.departmentClient.ApproveDEPARTMENTAsync(this.currentItem);
        }

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.currentItem.DEP_CODE.Equals(""))
                this.messagePop.SetError(AssCommonResource.Dep_Code + notEmpty);
            if (this.currentItem.DEP_NAME.Equals(""))
                this.messagePop.SetError(AssCommonResource.Dep_Name + notEmpty);
            if (this.branchName == null || this.branchName.Equals(""))
                this.messagePop.SetError(AssCommonResource.BranchID + notEmpty);
            if (this.currentItem.GROUP_ID == null || this.currentItem.GROUP_ID.Equals(""))
                this.messagePop.SetError(AssCommonResource.GroupDep + notEmpty);
            if (!this.currentItem.TEL.Equals("") && (!ValidateTest.IsPhoneNumber(this.currentItem.TEL) || this.currentItem.TEL.Length < 7))
                this.messagePop.SetError(AssCommonResource.Tel + " " + ValidatorResource.lblNotTrue);
           

        }
        private void Load()
        {
            try
            {
                this.branchClient = new BranchClient();
                this.departmentClient = new DEPARTMENTClient();
                this.departmentTypeClient = new CM_DEPT_GROUPClient();
                 this.departmentTypeClient.SearchCM_DEPT_GROUPAsync(new CM_DEPT_GROUP_SearchResult(), 100);

                this.departmentClient.UpdateDEPARTMENTCompleted += new EventHandler<UpdateDEPARTMENTCompletedEventArgs>(updateCompleted);
                this.departmentClient.InsertDEPARTMENTCompleted += new EventHandler<InsertDEPARTMENTCompletedEventArgs>(insertCompleted);
                this.departmentClient.ApproveDEPARTMENTCompleted += new EventHandler<ApproveDEPARTMENTCompletedEventArgs>(approveCompleted);
                this.departmentClient.SearchDEPARTMENTCompleted += new EventHandler<SearchDEPARTMENTCompletedEventArgs>(getDepartmentCompleted);
                this.departmentTypeClient.SearchCM_DEPT_GROUPCompleted += new EventHandler<SearchCM_DEPT_GROUPCompletedEventArgs>(getGroupDepCompleted);
               
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

        private void updateCompleted(object sender, UpdateDEPARTMENTCompletedEventArgs e)
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

        private void insertCompleted(object sender, InsertDEPARTMENTCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result.Equals("0"))
                {
                    this.CurrentItem.DEP_ID = e.Result.DEP_ID;
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { isEdit = this.isEdit, currentObject = this.currentItem });
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

        private void approveCompleted(object sender, ApproveDEPARTMENTCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result.Equals("0"))
                {
                    this.currentItem.AUTH_STATUS = "A";
                    this.currentItem.AUTH_STATUS_NAME = "Đã duyệt";
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

        private void getGroupDepCompleted(object sender, SearchCM_DEPT_GROUPCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.GroupData = e.Result;
                else
                    this.GroupData = new ObservableCollection<CM_DEPT_GROUP_SearchResult>();

                this.OnPropertyChanged("GroupData");
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

            private DepartmentEditViewModel viewModel;
            public ActionButton(DepartmentEditViewModel viewModel)
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
                    case "SearchBranch": this.viewModel.BranchSearch(); break;                 
                    default: break;
                }

            }
        }        
    }
}
