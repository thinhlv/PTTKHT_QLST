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

namespace gMVVM.ViewModels.AssCommon
{
    public class BranchEditViewModel : ViewModelBase
    {
        private BranchClient branchClient;

        public BranchEditViewModel()
        {
            this.actionCommand = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            Messenger.Default.Register<ChildMessage>(this, CurrencyEditSendMessageRecieve);

            this.Load();
        }

        private string preParentId = "";

        private void CurrencyEditSendMessageRecieve(ChildMessage obj)
        {

            if (obj.action.Equals(ActionMenuButton.View))
            {
                ActionMenuButton.actionControl.SetAllAction(null, null, null, null, null, null, this.actionCommand);
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
            this.CurrentItem = obj.currentObject != null ? (obj.currentObject as CM_BRANCH)
                : new CM_BRANCH()
                {
                    MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                    BRANCH_ID = "",
                    BRANCH_NAME = "",
                    BRANCH_TYPE = "PGD",
                    BRANCH_CODE = "",
                    REGION_ID = "",
                    TEL = "",
                    ADDR = "",
                    NOTES = "",
                    AUTH_STATUS = CurrentSystemLogin.IsAppFunction ? "U" : "A",
                    RECORD_STATUS = "1",
                    FATHER_ID = "",
                    IS_POTENTIAL = "N",
                    TAX_NO = ""
                };
            this.preParentId = this.currentItem.FATHER_ID;

            if (!obj.action.Equals(ActionMenuButton.Insert))
                this.LoadBranchParent();

            this.IsChecked = this.currentItem.RECORD_STATUS.Equals("1") ? true : false;
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

        public bool IsView { get; set; }

        public bool IsPotential
        {
            get
            {
                return this.currentItem.IS_POTENTIAL == "Y" ? true : false;
            }

            set
            {
                this.currentItem.IS_POTENTIAL = (value ? "Y" : "N");
                this.OnPropertyChanged("IsPotential");
            }
        }

        //Tinh trang record co dang hoat trong hay khong
        public bool IsChecked { get; set; }

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
        private CM_BRANCH currentItem;
        public CM_BRANCH CurrentItem
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

        public List<ItemData> BranchTypeData { get; set; }
        
        public string BranchTypeId
        {
            get
            {
                return this.currentItem.BRANCH_TYPE;
            }

            set
            {
                this.currentItem.BRANCH_TYPE = value;
                this.OnPropertyChanged("BranchTypeId");
                this.LoadBranchParent();
            }
        }


        public string RegionID
        {
            get
            {
                return this.currentItem.REGION_ID;
            }

            set
            {
                this.currentItem.REGION_ID = value;
                this.OnPropertyChanged("RegionID");
                this.LoadBranchParent();
            }
        }

        public ObservableCollection<CM_BRANCH> BranchData { get; set; }

        #endregion

        #region[Action Functions]

        private void OkAction()
        {
            RefreshValidator();
            if (!this.messagePop.HasError())
            {
                this.currentItem.RECORD_STATUS = this.IsChecked ? "1" : "0";
                if (this.isEdit)
                {
                    if (CurrentSystemLogin.IsAppFunction)
                    {
                        this.preStatus = this.currentItem.AUTH_STATUS;
                        this.currentItem.AUTH_STATUS = "U";
                    }
                    //Dang o trang thai duyet an duyet
                    if (this.currentItem.AUTH_STATUS.Equals("A"))
                    {
                        this.currentItem.AUTH_STATUS = "U";                        
                    }
                    MyHelper.IsBusy();
                    this.branchClient.UpdateBranchAsync(this.currentItem);
                }
                else
                {                    
                    this.currentItem.CREATE_DT = DateTime.Now;                    
                    MyHelper.IsBusy();
                    this.branchClient.InsertBranchAsync(this.currentItem);
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
            this.branchClient.ApproveBranchAsync(this.currentItem);
        }

        private void LoadBranchParent()
        {
            if (this.currentItem.BRANCH_TYPE != null && !this.currentItem.BRANCH_TYPE.Equals("")
                && this.currentItem.REGION_ID != null && !this.currentItem.REGION_ID.Equals(""))
            {
                //Them moi Hoi so
                if (this.currentItem.BRANCH_TYPE.Equals("HS"))
                {
                    this.BranchData = new ObservableCollection<CM_BRANCH>();
                    this.OnPropertyChanged("BranchData");
                }//Them moi Khu vuc
                else if (this.currentItem.BRANCH_TYPE.Equals("KV"))
                {
                    this.branchClient.GetByTopBranchAsync("", " RECORD_STATUS='1' and AUTH_STATUS='A' and BRANCH_TYPE = 'HS' ", "");
                }//Them moi Chi nhanh
                else if (this.currentItem.BRANCH_TYPE.Equals("CN"))
                {
                    //this.branchClient.GetByTopBranchAsync("", " RECORD_STATUS='1' and AUTH_STATUS='A' and (BRANCH_TYPE = 'HS' or ( BRANCH_TYPE = 'KV' and REGION_ID = '" + this.currentItem.REGION_ID + "'))", "");
                    this.branchClient.GetByTopBranchAsync("", " RECORD_STATUS='1' and AUTH_STATUS='A' and ( BRANCH_TYPE = 'KV' and REGION_ID = '" + this.currentItem.REGION_ID + "')", "");
                }//Them moi PGD
                else
                {
                    this.branchClient.GetByTopBranchAsync("", " RECORD_STATUS='1' and AUTH_STATUS='A' and BRANCH_TYPE in ('KV', 'CN') and REGION_ID = '" + this.currentItem.REGION_ID + "'", "");
                }
            }
        }

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.currentItem.BRANCH_CODE.Equals(""))
                this.messagePop.SetError(AssCommonResource.BranchID + notEmpty);
            if (this.currentItem.BRANCH_NAME.Equals(""))
                this.messagePop.SetError(AssCommonResource.BranchName + notEmpty);
            if (this.currentItem.REGION_ID == null || this.currentItem.REGION_ID.Equals(""))
                this.messagePop.SetError(AssCommonResource.RegionID + notEmpty);

            if (!this.currentItem.TEL.Equals("") && (!ValidateTest.IsPhoneNumber(this.currentItem.TEL) || this.currentItem.TEL.Length < 7))
                this.messagePop.SetError(AssCommonResource.Tel + " " + ValidatorResource.lblNotTrue);
            if (this.currentItem.BRANCH_TYPE != null && !this.currentItem.BRANCH_TYPE.Equals("HS") && this.currentItem.FATHER_ID == "")
                this.messagePop.SetError(AssCommonResource.FatherID + notEmpty);

        }
        private void Load()
        {
            try
            {
                this.branchClient = new BranchClient();
            

                this.branchClient.UpdateBranchCompleted += new EventHandler<UpdateBranchCompletedEventArgs>(updateCompleted);
                this.branchClient.InsertBranchCompleted += new EventHandler<InsertBranchCompletedEventArgs>(insetCompleted);
                this.branchClient.ApproveBranchCompleted += new EventHandler<ApproveBranchCompletedEventArgs>(approveCompleted);
                this.branchClient.GetByTopBranchCompleted += new EventHandler<GetByTopBranchCompletedEventArgs>(getBranchCompleted);

               

                this.BranchTypeData = new List<ItemData>()
                {
                    new ItemData(){ Id="HS", Name= AssCommonResource.lblHeadquarter},
                    new ItemData(){ Id="KV", Name=AssCommonResource.lblRegion},
                    new ItemData(){ Id="CN", Name=AssCommonResource.lblBranch1},
                    new ItemData(){ Id="PGD", Name=AssCommonResource.lblTransactionOffice}
                };
  
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void getBranchCompleted(object sender, GetByTopBranchCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.BranchData = e.Result;
                else
                    this.BranchData = new ObservableCollection<CM_BRANCH>();
                
                this.OnPropertyChanged("BranchData");
                this.CurrentItem.FATHER_ID = this.preParentId;
                this.OnPropertyChanged("CurrentItem");                
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);           
            }
        }

        private void updateCompleted(object sender, UpdateBranchCompletedEventArgs e)
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
                    this.currentItem.AUTH_STATUS = this.preStatus;
                    if (ListMessage.Message.ContainsKey(e.Result))
                        this.messagePop.SetSingleError(ValidatorResource.ErrorUpdate + "\n " + ListMessage.Message[e.Result]);
                    else
                        this.messagePop.SetSingleError(ValidatorResource.ErrorUpdate + "\n " + e.Result);
                }
            }
            catch (Exception)
            {
                this.currentItem.AUTH_STATUS = this.preStatus;
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
            finally
            {
                MyHelper.IsFree();
            }
        }

        private void insetCompleted(object sender, InsertBranchCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Length == 15 && e.Result.Contains("BRN"))
                {
                    this.currentItem.BRANCH_ID = e.Result;
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { isEdit = this.isEdit, currentObject = this.currentItem });
                }
                else
                {
                    if (ListMessage.Message.ContainsKey(e.Result))
                        this.messagePop.SetSingleError(ValidatorResource.ErrorInsert + "\n " + ListMessage.Message[e.Result]);
                    else
                        this.messagePop.SetSingleError(ValidatorResource.ErrorInsert + "\n " + e.Result);
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

        private void approveCompleted(object sender, ApproveBranchCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Equals(ListMessage.True))
                {
                    this.currentItem.AUTH_STATUS ="A";
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

            private BranchEditViewModel viewModel;
            public ActionButton(BranchEditViewModel viewModel)
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
