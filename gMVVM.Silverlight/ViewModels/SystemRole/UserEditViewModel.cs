using gMVVM;
using gMVVM.Views.SystemRole;
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
using System.Collections.ObjectModel;
using System.Windows.Data;
using gMVVM.Resources;
using GalaSoft.MvvmLight.Messaging;
using System.IO;
using System.Collections.Generic;
using gMVVM.CommonClass;
using gMVVM.ViewModels.Common;
using JeffWilcox.Utilities.Silverlight;
using System.Runtime.InteropServices.Automation;
using Lite.ExcelLibrary.SpreadSheet;
using System.Threading;
using System.Windows.Threading;
using mvvmCommon;
using System.Linq;

namespace gMVVM.ViewModels.SystemRole
{
    public class UserEditViewModel : ViewModelBase
    {
        private TLUSERClient tluserClient;
        private BranchClient branchClient;
        private string action = "";

        public UserEditViewModel()
        {
            this.actionCommand = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();

            Messenger.Default.Register<ChildMessage>(this, EditSendMessageRecieve);
            Reload();
        }

        private void Reload()
        {
            try
            {
                this.tluserClient = new TLUSERClient();
                this.branchClient = new BranchClient();
                //CMBRANCHClient branchClient = new CMBRANCHClient();
                TLSYSROLEClient roleClient = new TLSYSROLEClient();                

                this.IsAuto = Visibility.Collapsed.ToString();
                this.IsCustomer = Visibility.Visible.ToString();

                //LoadBranch();
                this.branchClient.GetByParentBranchIDAsync(CurrentSystemLogin.CurrentUser.TLSUBBRID);

                roleClient.GetByTopTLSYSROLEAsync("","","");
                roleClient.GetByTopTLSYSROLECompleted += (s, e) =>
                    {
                        if (e.Result != null && e.Result.Count > 0)
                            this.RoleData = e.Result;
                        else
                            this.RoleData = new ObservableCollection<TL_SYSROLE>();
                    };                

                this.tluserClient.InsertTLUSERCompleted += new EventHandler<InsertTLUSERCompletedEventArgs>(insertCompleted);
                this.tluserClient.UpdateTLUSERCompleted += new EventHandler<UpdateTLUSERCompletedEventArgs>(updateCompleted);
                this.tluserClient.InsertAllTLUSERCompleted += new EventHandler<InsertAllTLUSERCompletedEventArgs>(insertAllCompleted);
                this.branchClient.CM_BRANCH_LevelCompleted += new EventHandler<CM_BRANCH_LevelCompletedEventArgs>(getBranchCompleted);
                this.tluserClient.ApproveTLUSERCompleted += new EventHandler<ApproveTLUSERCompletedEventArgs>(approveComplete);
                this.branchClient.GetByParentBranchIDCompleted += new EventHandler<GetByParentBranchIDCompletedEventArgs>(getAllBranch);
                this.tluserClient.GetUserInfomationCompleted += new EventHandler<GetUserInfomationCompletedEventArgs>(getUserInfoComplete);

                this.IsError = Visibility.Collapsed.ToString();
                this.IsOk = Visibility.Collapsed.ToString();
            }
            catch (Exception ex)
            {
                this.messagePop.SetSingleError(ex.Message);
            }
        }

        private void getAllBranch(object sender, GetByParentBranchIDCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    this.BranchByParentData = e.Result;                    
                }
                else
                    this.BranchByParentData = new ObservableCollection<CM_BRANCH_GETALLCHILDResult>();

                this.OnPropertyChanged("BranchByParentData");
                this.CurBranchId = this.currentUser.TLSUBBRID;
            }
            catch (Exception)
            {
                
            }
        }                
        
        private void EditSendMessageRecieve(ChildMessage obj)
        {
            this.IsOptionInsert = Visibility.Collapsed.ToString();
            this.action = obj.action;            

            //branchClient.CM_BRANCH_LevelAsync(CurrentSystemLogin.CurrentUser.TLSUBBRID);

            if (obj.action.Equals(ActionMenuButton.View))
            {
                ActionMenuButton.actionControl.SetAllAction(null, null, null, null, null, null, actionCommand);
                this.HeaderText = CommonResource.lblApprove;
                this.IsView = false;
            }
            else
            {

                if (obj.action.Equals(ActionMenuButton.Insert))
                    this.ExistUser = false;
                else
                    this.ExistUser = true;

                ActionMenuButton.actionControl.SetAllAction(null, actionCommand, null, null, null);
                this.isEdit = obj.isEdit;
                this.HeaderText = this.isEdit ? CommonResource.lblEdit : CommonResource.lblInsert;
                this.IsView = true;
            }
            
            this.messagePop.Reset();
            this.CurrentUser = obj.currentObject != null ? (obj.currentObject as TL_USER_SearchResult)
                : new TL_USER_SearchResult()
                {
                    AUTH_STATUS = CurrentSystemLogin.IsAppFunction ? "U" : "A",
                    MARKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                    PHONE = "",
                    ADDRESS = "",
                    EMAIL = "",
                    TLFullName = "",
                    Password = "",
                    RoleName = "",
                    TLSUBBRID = CurrentSystemLogin.CurrentUser.TLSUBBRID,
                    TLNANME = "",
                    ISAPPROVE = "0",
                    ISFIRSTTIME = "1"
                };
            this.prePass = this.currentUser.Password;
            this.TLName = this.currentUser.TLNANME;

            if (this.currentUser.AUTH_STATUS.Equals("A"))
                this.IsApproved = Visibility.Visible.ToString();
            else
                this.IsApproved = Visibility.Collapsed.ToString();

            this.CurBranchId = this.currentUser.TLSUBBRID;

            this.IsError = Visibility.Collapsed.ToString();
            this.IsOk = Visibility.Collapsed.ToString();
            this.isCheckedStatus = -1;
            this.OnPropertyChanged("IsError");
            this.OnPropertyChanged("IsOk");

            this.OnPropertyChanged("CurrentUser");
            this.OnPropertyChanged("CurBranchId");
            this.OnPropertyChanged("Email");
            //this.OnPropertyChanged("");
        }

        private string preAuthStatus = "";
        private string prePass = "";
        private int isCheckedStatus = -1;
        private bool isUpdate = false;

        public bool ExistUser = false;

        #region[All Properties]       
 
        public CM_BRANCH_GETALLCHILDResult CurrentItemBranch { get; set; }

        public string IsError { get; set; }
        public string IsOk { get; set; }

        public string Email
        {
            get
            {
                return this.currentUser.EMAIL;
            }

            set
            {
                this.currentUser.EMAIL = value;
                this.OnPropertyChanged("Email");

                //Lay thong tin user_name
                //if (value != null && value.Contains("@"))
                //    this.TLName = value.Split('@')[0];
                //else
                //    this.TLName = "";

                //this.IsError = Visibility.Collapsed.ToString();
                //this.IsOk = Visibility.Collapsed.ToString();
                //this.isCheckedStatus = -1;
                //this.OnPropertyChanged("IsError");
                //this.OnPropertyChanged("IsOk");
            }
        }

        public bool IsView { get; set; }

        public string IsApproved { get; set; }

        public bool IsEnabledEdit 
        { 
            get 
            { 
                return !this.isEdit; 
            } 
        }

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
        private TL_USER_SearchResult currentUser;
        public TL_USER_SearchResult CurrentUser
        {
            get
            {
                return this.currentUser;
            }

            set
            {
                this.currentUser = value;
                this.OnPropertyChanged("CurrentUser");                
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

        //RoleData
        private ObservableCollection<TL_SYSROLE> roleData;
        public ObservableCollection<TL_SYSROLE> RoleData
        {
            get
            {
                return this.roleData;
            }

            set
            {
                this.roleData = value;
                this.OnPropertyChanged("RoleData");
            }
        }

        private string tlName = "";
        public string TLName
        {
            get
            {
                return this.tlName;
            }

            set
            {
                this.tlName = value;
                this.OnPropertyChanged("TLName");
                //if (value != null && !value.Equals("") && this.action.Equals("Insert"))
                //    this.tluserClient.GetUserInfomationAsync(this.tlName);
            }
        }


        private string roleName = "";
        public string RoleName
        {
            get
            {
                return this.roleName;
            }

            set
            {
                this.roleName = value;
                this.OnPropertyChanged("RoleName");

                if (value != null || value.Equals(""))
                    this.SetRoleList();
            }
        }

        private bool isFromFile = false;
        public bool IsFromFile
        {
            get
            {
                return this.isFromFile;
            }

            set
            {
                this.isFromFile = value;
                this.OnPropertyChanged("IsFromFile");
                if (value)
                {
                    this.IsAuto = Visibility.Visible.ToString();
                    this.IsCustomer = Visibility.Collapsed.ToString();
                }
                else
                {
                    this.IsAuto = Visibility.Collapsed.ToString();
                    this.IsCustomer = Visibility.Visible.ToString();
                }
                this.OnPropertyChanged("IsCustomer");
                this.OnPropertyChanged("IsAuto");
                this.messagePop.Reset();
            }
        }

        public string IsCustomer { get; set; }
        public string IsAuto { get; set; }

        public ObservableCollection<TL_USER> DataExcel { get; set; }
        public PagedCollectionView PageData
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public string IsOptionInsert { get; set; }

        private string kvId = "";
        public string KVId
        {
            get { return this.kvId; }
            set
            {
                this.kvId = value; this.OnPropertyChanged("KVId");
                if (value != null && this.dicBranch.ContainsKey(value))
                    this.BranchData = this.dicBranch[value];
                else
                {
                    CM_BRANCH temp = new CM_BRANCH() { BRANCH_ID = "", BRANCH_CODE = "", BRANCH_NAME = "---NULL---" };
                    this.BranchData = new ObservableCollection<CM_BRANCH>();
                    this.BranchData.Add(temp);
                    this.dicBranch.Add(value, this.BranchData);
                    if (value.Equals(""))
                        this.BranchId = "";
                }
                this.PGDData = new ObservableCollection<CM_BRANCH>();
                this.OnPropertyChanged("BranchData");
                this.OnPropertyChanged("PGDData");
                this.OnPropertyChanged("BranchId");
            }
        }

        public ObservableCollection<CM_BRANCH> KVData
        { get; set; }


        private string curBranchId;
        public string CurBranchId
        {
            get { return this.curBranchId; }
            set
            {
                this.curBranchId = value;
                this.OnPropertyChanged("CurBranchId");
            }
        }

        private string branchId = "";
        public string BranchId
        {
            get { return this.branchId; }
            set
            {
                this.branchId = value; this.OnPropertyChanged("BranchId");
                if (value != null && this.dicTrans.ContainsKey(value))
                    this.PGDData = this.dicTrans[value];
                else
                {
                    CM_BRANCH temp = new CM_BRANCH() { BRANCH_ID = "", BRANCH_CODE = "", BRANCH_NAME = "---NULL---"};
                    this.PGDData = new ObservableCollection<CM_BRANCH>();
                    this.PGDData.Add(temp);
                    this.dicTrans.Add(value, this.PGDData);
                    if (value.Equals(""))
                        this.PGDId = "";
                    this.OnPropertyChanged("PGDId");
                }

                this.OnPropertyChanged("PGDData");
                this.OnPropertyChanged("PGDId");
            }
        }

        public ObservableCollection<CM_BRANCH_GETALLCHILDResult> BranchByParentData
        { get; set; }

        public ObservableCollection<CM_BRANCH> BranchData
        { get; set; }

        public string PGDId { get; set; }
        public ObservableCollection<CM_BRANCH> PGDData
        { get; set; }

        private Dictionary<string, ObservableCollection<CM_BRANCH>> dicBranch;
        private Dictionary<string, ObservableCollection<CM_BRANCH>> dicTrans;

        public CM_BRANCH KvItem { get; set; }
        public CM_BRANCH CnItem { get; set; }
        public CM_BRANCH PgdItem { get; set; }

        #endregion

        #region[Action Functions]

        private void approveComplete(object sender, ApproveTLUSERCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Result.Equals("0"))
                {
                    this.currentUser.AUTH_STATUS = "A";
                    this.currentUser.AUTH_STATUS_NAME = CommonResource.lblApproved;
                    this.IsApproved = Visibility.Visible.ToString();
                    this.OnPropertyChanged("IsApproved");
                    this.messagePop.Successful(ValidatorResource.SuccessfulApprove);
                }
                else
                    this.messagePop.SetSingleError(ValidatorResource.ErrorApprove);
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

        private void updateCompleted(object sender, UpdateTLUSERCompletedEventArgs e)
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
                    this.currentUser.AUTH_STATUS = this.preAuthStatus;
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

        private void insertCompleted(object sender, InsertTLUSERCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result.Equals("0"))
                {
                    this.currentUser.TLID = e.Result.TLID;
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { currentObject = this.currentUser });
                }
                else
                {
                    this.messagePop.SetSingleError(ValidatorResource.ErrorInsert + "\n" + e.Result.ErrorDesc);
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

        

        private int doneCount = 0;
        private void insertAllCompleted(object sender, InsertAllTLUSERCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Equals("True"))
                {
                    //Insert Full
                    if (this.currentIndex >= this.DataExcel.Count - 1)
                    {
                        //Messenger.Default.Send(new ParentMessage() { currentObject = this.currentUser });
                        this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                        MyHelper.IsFree();
                    }
                    else
                    {
                        this.doneCount = this.currentIndex;
                        this.tempData = new ObservableCollection<TL_USER>();
                        int max = this.maxRows + this.currentIndex;
                        if (this.DataExcel.Count - max < 0)
                            max = this.DataExcel.Count;
                        for (int i = this.currentIndex; i < max; i++)
                        {
                            this.tempData.Add(this.DataExcel[i]);
                            this.currentIndex++;
                        }
                        this.tluserClient.InsertAllTLUSERAsync(this.tempData);
                    }

                }
                else
                {
                    if (ListMessage.Message.ContainsKey(e.Result))
                        this.messagePop.SetSingleError(ListMessage.Message[e.Result]);
                    else
                        this.messagePop.SetSingleError(SystemRoleResource.lblDoSome + ": " + this.doneCount + "\n" + e.Result);                    
                    MyHelper.IsFree();
                }
            }
            catch (Exception ex)
            {
                this.messagePop.SetSingleError(SystemRoleResource.lblDoSome + ": " + this.doneCount + "\n" + ex.Message);
                MyHelper.IsFree();
            }            
        }

        private void getBranchCompleted(object sender, CM_BRANCH_LevelCompletedEventArgs e)
        {
            try
            {
                ObservableCollection<CM_BRANCH> dicRegionAll = new ObservableCollection<CM_BRANCH>();
                ObservableCollection<CM_BRANCH> dicBranchAll = new ObservableCollection<CM_BRANCH>();
                ObservableCollection<CM_BRANCH> dicTransAll = new ObservableCollection<CM_BRANCH>();

                if (e.Result != null)
                {
                    this.dicBranch = new Dictionary<string, ObservableCollection<CM_BRANCH>>();
                    this.dicTrans = new Dictionary<string, ObservableCollection<CM_BRANCH>>();
                    
                    //Gan danh sach KV
                    if (e.Result.RegionList != null)
                    {
                        this.KVData = e.Result.RegionList;
                        foreach (var item in this.KVData)
                            dicRegionAll.Add(item);
                    }
                    else this.KVData = new ObservableCollection<CM_BRANCH>();

                    //Luu danh sach chi nhanh vao dic
                    if (e.Result.BranchList == null)
                        this.BranchData = new ObservableCollection<CM_BRANCH>();
                    foreach (var item in e.Result.BranchList)
                    {
                        if (this.dicBranch.ContainsKey(item.FATHER_ID))
                            this.dicBranch[item.FATHER_ID].Add(item);
                        else
                        {
                            this.dicBranch.Add(item.FATHER_ID, new ObservableCollection<CM_BRANCH>());
                            this.dicBranch[item.FATHER_ID].Add(item);
                        }
                        dicBranchAll.Add(item);
                    }

                    //Luu danh sach PGD vao dic
                    if (e.Result.TransList != null)
                        this.PGDData = new ObservableCollection<CM_BRANCH>();
                    foreach (var item in e.Result.TransList)
                    {
                        if (this.dicTrans.ContainsKey(item.FATHER_ID))
                            this.dicTrans[item.FATHER_ID].Add(item);
                        else
                        {
                            this.dicTrans.Add(item.FATHER_ID, new ObservableCollection<CM_BRANCH>());
                            this.dicTrans[item.FATHER_ID].Add(item);
                        }
                        dicTransAll.Add(item);
                    }
                }
                if (this.KVData.Count > 1)
                    this.KVData.Add(new CM_BRANCH() { BRANCH_ID = "", BRANCH_CODE = "", BRANCH_NAME = "---NULL---" });
                foreach (var key in this.dicBranch.Keys)
                    if (this.dicBranch[key].Count > 1)
                        this.dicBranch[key].Add(new CM_BRANCH() { BRANCH_ID = "", BRANCH_CODE = "", BRANCH_NAME = "---NULL---" });
                foreach (var key in this.dicTrans.Keys)
                    if (this.dicTrans[key].Count > 1)
                        this.dicTrans[key].Add(new CM_BRANCH() { BRANCH_ID = "", BRANCH_CODE = "", BRANCH_NAME = "---NULL---" });

                this.OnPropertyChanged("KVData");
                //Set gia tri mac dinh tren combobox
                if (this.KVData.Count > 0)
                    this.KVId = this.KVData[0].BRANCH_ID;
                if (this.BranchData.Count > 0)
                    this.BranchId = this.BranchData[0].BRANCH_ID;
                if (this.PGDData.Count > 0)
                    this.PGDId = this.PGDData[0].BRANCH_ID;
                this.OnPropertyChanged("PGDId");

                if (!this.action.Equals("Insert"))
                {
                    //Khu vuc
                    var _res1 = dicRegionAll.FirstOrDefault(p => p.BRANCH_ID == this.currentUser.TLSUBBRID);
                    //Chi nhanh
                    var _res2 = dicBranchAll.FirstOrDefault(p=>p.BRANCH_ID == this.currentUser.TLSUBBRID);
                    //Phong giao dich
                    var _res3 = dicTransAll.FirstOrDefault(p=>p.BRANCH_ID == this.currentUser.TLSUBBRID);
                    if (_res2 != null)
                    {
                        this.KVId = _res2.FATHER_ID;
                        this.BranchId = this.currentUser.TLSUBBRID;
                    }
                    else if (_res3 != null)
                    {
                        var _res4 = dicBranchAll.FirstOrDefault(p => p.BRANCH_ID == _res3.FATHER_ID);
                        this.KVId = _res4.FATHER_ID;
                        this.BranchId = _res3.FATHER_ID;
                        this.PGDId = this.currentUser.TLSUBBRID;
                    }
                    else if (_res1 != null)
                    {
                        this.KVId = this.currentUser.TLSUBBRID;
                        this.BranchId = "";
                        this.PGDId = "";
                    }
                    else
                    {
                        this.KVId = "";
                        this.BranchId = "";
                        this.PGDId = "";
                    }
                    this.OnPropertyChanged("KVId");
                    this.OnPropertyChanged("BranchId");
                    this.OnPropertyChanged("PGDId");

                }
            }
            catch (Exception ex)
            {
                this.messagePop.SetSingleError(ex.Message);
            }
        }

        private void getUserInfoComplete(object sender, GetUserInfomationCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {
                    this.IsOk = Visibility.Visible.ToString();
                    this.IsError = Visibility.Collapsed.ToString();
                    this.isCheckedStatus = 1;
                    //this.currentUser.TLNANME = e.Result.TLNANME;
                    //this.currentUser.APPROVE_DT = e.Result.APPROVE_DT;
                    this.currentUser.TLFullName = e.Result.TLFullName;
                    this.currentUser.EMAIL = e.Result.EMAIL;
                    //this.currentUser.PHONE = e.Result.PHONE;
                    //this.currentUser.ADDRESS = e.Result.ADDRESS;                    
                    //this.Email = e.Result.EMAIL;
                    //this.ExistUser = true;
                    //if (!this.isUpdate)
                      //  this.CurrentUser.TLFullName = e.Result.TLFullName;
                }
                else
                {
                    this.IsOk = Visibility.Collapsed.ToString();
                    this.IsError = Visibility.Visible.ToString();
                    this.isCheckedStatus = 0;
                    //this.currentUser.APPROVE_DT = DateTime.Now;
                    this.currentUser.TLFullName = "";
                    this.currentUser.EMAIL = "";
                    //this.Email = "";
                    //this.currentUser.PHONE = "";
                    //this.currentUser.ADDRESS = "";
                    //this.ExistUser = false;
                }

                if (this.isUpdate)
                {
                    this.isUpdate = false;
                    this.OkAction();
                }

                this.OnPropertyChanged("IsError");
                this.OnPropertyChanged("IsOk");
                this.OnPropertyChanged("Email");
                //this.OnPropertyChanged("CurrentUser");
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

        private void Approve()
        {
            this.messagePop.Reset();
            if (this.currentUser.AUTH_STATUS.Equals("A")) return;
            if (this.currentUser.MARKER_ID.Equals(CurrentSystemLogin.CurrentUser.TLNANME))
            {
                this.messagePop.SetSingleError(ValidatorResource.lblErrorPermission);
                return;
            }
            this.currentUser.AUTH_STATUS = "A";
            this.currentUser.AUTH_ID = CurrentSystemLogin.CurrentUser.TLNANME;
            this.currentUser.APPROVE_DT = DateTime.Now;
            MyHelper.IsBusy();
            this.tluserClient.ApproveTLUSERAsync(this.currentUser);
        }

        private void OkAction()
        {
            try
            {
                RefreshValidator();
                if (!this.messagePop.HasError())
                {
                    //if (this.PGDId != null && !this.PGDId.Equals(""))
                    //{
                    //    this.currentUser.TLSUBBRID = this.PGDId;
                    //    this.currentUser.BRANCH_TYPE = PgdItem.BRANCH_TYPE;
                    //    this.currentUser.BRANCH_NAME = PgdItem.BRANCH_NAME;
                    //}
                    //else
                    //    if (this.branchId != null && !this.branchId.Equals(""))
                    //    {
                    //        this.currentUser.TLSUBBRID = this.branchId;
                    //        this.currentUser.BRANCH_TYPE = CnItem.BRANCH_TYPE;
                    //        this.currentUser.BRANCH_NAME = CnItem.BRANCH_NAME;
                    //    }
                    //    else if (this.kvId != null && !this.kvId.Equals(""))
                    //    {
                    //        this.currentUser.TLSUBBRID = this.kvId;
                    //        this.currentUser.BRANCH_TYPE = KvItem.BRANCH_TYPE;
                    //        this.currentUser.BRANCH_NAME = KvItem.BRANCH_NAME;
                    //    }
                    //    else
                    //    {
                    //        this.currentUser.TLSUBBRID = CurrentSystemLogin.CurrentUser.TLSUBBRID;
                    //        this.currentUser.BRANCH_TYPE = CurrentSystemLogin.CurrentUser.BRANCH_TYPE;
                    //        this.currentUser.BRANCH_NAME = CurrentSystemLogin.CurrentUser.BRANCH_NAME;
                    //    }

                    this.currentUser.TLSUBBRID = this.curBranchId;
                    this.currentUser.BRANCH_TYPE = this.CurrentItemBranch.BRANCH_TYPE;
                    if (this.isEdit)
                    {
                        //if (this.currentUser.AUTH_STATUS.Equals("A"))
                        //{
                        //    this.messagePop.SetSingleError(ValidatorResource.lblPerEdit);
                        //    MyHelper.IsFree();
                        //    return;
                        //}
                        if (CurrentSystemLogin.IsAppFunction)
                        {
                            this.preAuthStatus = this.currentUser.AUTH_STATUS;
                            this.currentUser.AUTH_STATUS = "U";
                            this.currentUser.MARKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
                        }
                        
                        this.currentUser.TLNANME = this.tlName;
                        //this.preAuthStatus = this.currentUser.AUTH_STATUS;
                        //this.currentUser.AUTH_STATUS = "U";
                        if (!this.currentUser.Password.Equals(this.prePass))
                            this.currentUser.Password = MD5.GetMd5String(this.currentUser.Password);
                        MyHelper.IsBusy();
                        this.tluserClient.UpdateTLUSERAsync(this.currentUser);
                    }
                    else
                    {
                        if (!this.isFromFile)
                        {
                            //MessageBox.Show(this.currentUser.Password);
                            //MessageBox.Show(MD5.GetMd5String(this.currentUser.Password));
                            this.currentUser.TLNANME = this.tlName;
                            this.currentUser.Password = MD5.GetMd5String(this.currentUser.Password);
                            this.tluserClient.InsertTLUSERAsync(this.currentUser);
                        }
                        else
                        {                            
                            this.currentIndex = 0;
                            if (this.DataExcel.Count <= this.maxRows)
                            {
                                this.tluserClient.InsertAllTLUSERAsync(this.DataExcel);
                                this.currentIndex = this.DataExcel.Count;
                            }
                            else
                            {
                                this.tempData = new ObservableCollection<TL_USER>();
                                for (int i = 0; i < this.maxRows; i++)
                                {
                                    this.tempData.Add(this.DataExcel[i]);
                                    this.currentIndex++;
                                }
                                this.tluserClient.InsertAllTLUSERAsync(this.tempData);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.messagePop.SetSingleError(ex.Message);
                MyHelper.IsFree();
            }
        }
        
        private int maxRows = 100;
        private int currentIndex = 0;
        private ObservableCollection<TL_USER> tempData;

        private void SetRoleList()
        {
            foreach (var item in this.DataExcel)
                item.RoleName = this.roleName;
        }

        private void LoadFile()
        {
            try
            {
                this.messagePop.Reset();

                ObservableCollection<TL_USER> populationData = new ObservableCollection<TL_USER>();
                OpenFileDialog flDialog = new OpenFileDialog();
                flDialog.Filter = "Excel Files(*.xls)|*.xls";
                bool res = (bool)flDialog.ShowDialog();

                if (res)
                {
                    FileStream fs = flDialog.File.OpenRead();
                    this.FileName = flDialog.File.Name;
                    this.OnPropertyChanged("FileName");
                    // Simply call the Open method of Workbook and you are done
                    Workbook book = Workbook.Open(fs);
                    // All of the worksheet will be populated with data 
                    // currently we will read only first for this sample
                    Worksheet sheet = book.Worksheets[0];
                    Dictionary<string, int> dicPosision = new Dictionary<string, int>();
                    List<string> lstKeys = new List<string>() { "MSSV", "HOTEN", "EMAIL", "DIENTHOAI" };
                    dicPosision.Add(lstKeys[0], 0);
                    dicPosision.Add(lstKeys[1], 0);
                    dicPosision.Add(lstKeys[2], 0);
                    dicPosision.Add(lstKeys[3], 0);
                    string BrId = sheet.Name;
                    int firstRow = sheet.Cells.FirstRowIndex;
                    //Luu lai vi tri cac cot
                    for (int i = sheet.Cells.FirstColIndex; i < sheet.Cells.LastColIndex; i++)
                    {
                        if (lstKeys.Contains(sheet.Cells[firstRow, i].StringValue))
                        {
                            dicPosision[sheet.Cells[firstRow, i].StringValue] = i;
                        }
                    }

                    //Lay thong tin
                    for (int i = sheet.Cells.FirstRowIndex + 1; i <= sheet.Cells.LastRowIndex; i++)
                    {
                        populationData.Add(new TL_USER()
                        {
                            AUTH_STATUS = "U",
                            MARKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                            PHONE = sheet.Cells[i, dicPosision[lstKeys[3]]].StringValue,
                            ADDRESS = "",
                            EMAIL = sheet.Cells[i, dicPosision[lstKeys[2]]].StringValue,
                            TLFullName = sheet.Cells[i, dicPosision[lstKeys[1]]].StringValue,
                            Password = MD5.GetMd5String(sheet.Cells[i, dicPosision[lstKeys[0]]].StringValue),//MSSV
                            RoleName = this.roleName,
                            TLSUBBRID = BrId,
                            TLNANME = sheet.Cells[i, dicPosision[lstKeys[0]]].StringValue,
                            ISAPPROVE = "0",
                            ISFIRSTTIME = "1"
                        });
                    }

                    DataExcel = populationData;
                    this.PageData = new PagedCollectionView(this.DataExcel);
                    this.OnPropertyChanged("PageData");
                    book = null;
                }
            }
            catch (Exception ex)
            {
                this.messagePop.SetSingleError(ex.Message);
            }
        }

        private bool CheckUserName()
        {
            try
            {
                this.tluserClient.GetUserInfomationAsync(this.tlName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void CheckUserNameExist()
        {
            MyHelper.IsBusy();
            this.CheckUserName();
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new ParentMessage() { currentObject = null });
        }

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;

            if (!this.isFromFile)
            {
                //if (!this.ExistUser)
                //{
                //    this.messagePop.SetError(SystemRoleResource.ExistUser);
                //    return;
                //}

                //if (this.tlName.Equals(""))
                //    this.messagePop.SetError(SystemRoleResource.lblUsername + notEmpty);
                //if (this.currentUser.Password.Equals(""))
                //    this.messagePop.SetError(SystemRoleResource.Password + notEmpty);

                //if (this.currentUser.TLSUBBRID == null || this.currentUser.TLSUBBRID.Equals(""))
                //    this.messagePop.SetError(SystemRoleResource.Branch + notEmpty);
                
                if (this.currentUser.TLFullName == null || this.currentUser.TLFullName.Equals(""))
                    this.messagePop.SetError(SystemRoleResource.TLFullName + notEmpty);
                if (this.curBranchId == "" || this.curBranchId == null)
                    this.messagePop.SetSingleError(SystemRoleResource.TLSUBBRID + notEmpty);
                if (this.currentUser.EMAIL == null || this.currentUser.EMAIL.Equals(""))
                    this.messagePop.SetError(SystemRoleResource.EMAIL + notEmpty);
                else if (!ValidateTest.IsEmail(this.currentUser.EMAIL))
                    this.messagePop.SetError(SystemRoleResource.EMAIL + " " + ValidatorResource.ErrorFormat);
                if (this.currentUser.RoleName == null || this.currentUser.RoleName.Equals(""))
                    this.messagePop.SetError(SystemRoleResource.lblRole + notEmpty);

                if (this.isCheckedStatus == -1)
                {
                    this.isUpdate = true;
                    this.CheckUserName();
                    this.messagePop.SetError("Kiểm tra username?");
                }
                else if (this.isCheckedStatus == 0)
                    this.messagePop.SetError("Username không tồn tại.");

                //if (this.currentUser.PHONE == null || this.currentUser.PHONE.Equals(""))
                //    this.messagePop.SetError(SystemRoleResource.PHONE + notEmpty);
                //else if (!this.currentUser.PHONE.Equals("") && !ValidateTest.IsNumber(this.currentUser.PHONE))
                //    this.messagePop.SetError(SystemRoleResource.PHONE + " " + ValidatorResource.ErrorFormat);
            }
            else
            {

                if (this.FileName == null || this.FileName.Equals(""))
                    this.messagePop.SetError(SystemRoleResource.lblFile + notEmpty);

                if (this.roleName == null || this.roleName.Equals(""))
                    this.messagePop.SetError(SystemRoleResource.lblRole + notEmpty);

                
            }
        }

        #endregion

        //Command button action
        public class ActionButton : ICommand
        {

            private UserEditViewModel viewModel;
            public ActionButton(UserEditViewModel viewModel)
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
                    case "GetFile": this.viewModel.LoadFile(); break;
                    case "CheckUserName": this.viewModel.CheckUserNameExist(); break;
                    default: break;
                }
            }
        }
    }
}
