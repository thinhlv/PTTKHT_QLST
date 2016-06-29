using gMVVM;
using gMVVM.CommonClass;
using gMVVM.gMVVMService;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using gMVVM.Views.SystemRole;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
using mvvmCommon;

namespace gMVVM.ViewModels.SystemRole
{
    public class UserViewModel : ViewModelBase
    {
        private TLUSERClient tlUserClient;
        private BranchClient branchClient;
        public UserViewModel()
        {
            this.actionButton = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
            this.DoubleClickItemCommand = new RelayCommand(DoubleClickItem);

            ReLoad();

            Messenger.Reset();
            Messenger.Default.Register<ParentMessage>(this, UserEditResponseMessageReceived);
        }             

        private ActionButton actionButton;
        private ObservableCollection<TL_USER_SearchResult> currentData;
        private UserEdit tlUserChild;
        private DateTime _clickTs;

        #region[All Properties]

        public bool IsSingleData { get; set; }
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

        private PagedCollectionView tlUser;
        public PagedCollectionView TLUser
        {
            get
            {
                return this.tlUser;
            }

            set
            {
                this.tlUser = value;
                OnPropertyChanged("TLUser");
            }
        }

        private UserItem currentUser;
        public UserItem CurrentUser
        {
            get
            {
                return this.currentUser;
            }

            set
            {
                this.currentUser = value;
                OnPropertyChanged("CurrentUser");
            }
        }

        private string brid = "";
        public string BRID
        {
            get
            {
                return this.brid;
            }

            set
            {
                this.brid = value;
                this.OnPropertyChanged("BRID");
            }
        }

        private string subBRDESC = "";
        public string SUBBRDESC
        {
            get
            {
                return this.subBRDESC;
            }

            set
            {
                this.subBRDESC = value;
                this.OnPropertyChanged("SUBBRDESC");
            }
        }

        public ICommand DoubleClickItemCommand { get; private set; }
        
        public List<ItemYesNo> AuthData { get; set; }
        public ObservableCollection<TL_SYSROLE> RoleData { get; set; }

        private TL_USER_SearchResult currentSearch = new TL_USER_SearchResult() { TLNANME = "", TLSUBBRID = "", AUTH_STATUS = "", RoleName = CommonResource.lblAll };
        public TL_USER_SearchResult CurrentSearch
        {
            get
            {
                return this.currentSearch;
            }

            set
            {
                this.currentSearch = value;
                this.OnPropertyChanged("CurrentSearch");
            }
        }

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
                    this.BranchData = new ObservableCollection<CM_BRANCH>();
                    this.dicBranch.Add(value, this.BranchData);
                }
                this.PGDData = new ObservableCollection<CM_BRANCH>();
                this.OnPropertyChanged("BranchData");
                this.OnPropertyChanged("PGDData");
            }
        }

        public ObservableCollection<CM_BRANCH> KVData
        { get; set; }

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
                    this.PGDData = new ObservableCollection<CM_BRANCH>();
                    this.dicTrans.Add(value, this.PGDData);
                }

                this.OnPropertyChanged("PGDData");
            }
        }
        public ObservableCollection<CM_BRANCH> BranchData
        { get; set; }

        public string PGDId { get; set; }
        public ObservableCollection<CM_BRANCH> PGDData
        { get; set; }

        private Dictionary<string, ObservableCollection<CM_BRANCH>> dicBranch;
        private Dictionary<string, ObservableCollection<CM_BRANCH>> dicTrans;
        private bool isInsert = false;

        #endregion

        #region[Action Functions]

        private void Insert()
        {
            if (this.tlUserChild == null)
            {
                this.tlUserChild = new UserEdit();
                PageAnimation.back.Children.Add(this.tlUserChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = null, isEdit = false, action = ActionMenuButton.Insert });
            PageAnimation.ToBack();        
        }

        private void Edit()
        {
            if (this.tlUserChild == null)
            {
                this.tlUserChild = new UserEdit();
                PageAnimation.back.Children.Add(this.tlUserChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentUser.ItemContent, isEdit = true, action = ActionMenuButton.Edit });
            PageAnimation.ToBack();   
        }

        private void View()
        {
            if (this.tlUserChild == null)
            {
                this.tlUserChild = new UserEdit();
                PageAnimation.back.Children.Add(this.tlUserChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentUser.ItemContent, isEdit = false, action = ActionMenuButton.View });
            PageAnimation.ToBack();
        }

        private void Delete()
        {
            this.messagePop.Reset();
            if (this.currentUser.ItemContent.ISAPPROVE.Equals("1"))
            {
                this.messagePop.SetSingleError(CommonResource.lblIsUsing);
                return;
            }

            if (MessageBox.Show(CommonResource.msgDelete, CommonResource.btnDelete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //call delete item
                MyHelper.IsBusy();
                this.tlUserClient.DeleteTLUSERAsync(this.currentUser.ItemContent.TLNANME);
            }    
        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                //string where = " TLNANME like '%" + (this.currentSearch.TLNANME.Equals("") ? "%" : this.currentSearch.TLNANME) + "%' "
                //    + " and RoleName like '%" + (this.currentSearch.RoleName.Equals(CommonResource.lblAll) ? "%" : this.currentSearch.RoleName) + "%' "
                //    + " and TLSUBBRID like '%" + (this.currentSearch.TLSUBBRID.Equals("") ? "%" : this.currentSearch.TLSUBBRID) + "%' "
                //    + " and AUTH_STATUS like '%" + (this.currentSearch.AUTH_STATUS.Equals("") ? "%" : this.currentSearch.AUTH_STATUS) + "%' "
                //    + " and TLNANME != 'admin' ";
                bool isAll = false;
                if (this.PGDId != null && !this.PGDId.Equals(""))
                    this.currentSearch.TLSUBBRID = this.PGDId;
                else
                    if (this.branchId != null && !this.branchId.Equals(""))
                    {
                        this.currentSearch.TLSUBBRID = this.branchId;
                        isAll = true;
                    }
                    else if (this.kvId != null && !this.kvId.Equals(""))
                    {
                        this.currentSearch.TLSUBBRID = this.kvId;
                        isAll = true;
                    }
                    else
                    {
                        this.currentSearch.TLSUBBRID = CurrentSystemLogin.CurrentUser.TLSUBBRID;
                        isAll = true;
                    }
                if (currentSearch.RoleName.Equals(CommonResource.lblAll))
                    this.currentSearch.RoleName = "";
                MyHelper.IsBusy();
                this.tlUserClient.SearchTLUSERAsync(this.currentSearch, 200, this.IsSingleData);
            }
            catch (Exception)
            { }
        }

        private void DataRefresh()
        {
            List<UserItem> lstTemp = new List<UserItem>();
            int i = 1;
            foreach (var item in this.currentData)
            {
                lstTemp.Add(new UserItem() { ID = i, ItemContent = item });
                i++;
            }
            this.TLUser = new PagedCollectionView(lstTemp);
        }

        #endregion

        #region[All Funtions]

        private void UserEditResponseMessageReceived(ParentMessage obj)
        {
            if (obj.currentObject != null)
            {
                //this.currentData.Add(obj.currentObject as TL_USER_SearchResult);
                this.isInsert = true;                    
                //this.DataRefresh();
            }
            else
            {
                if (this.isInsert)
                {
                    this.isInsert = false;
                    this.Search();    
                }
                this.messagePop.Reset();
                ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
                PageAnimation.ToFront();
            }
        }   

        private void ReLoad()
        {
            try
            {
                this.tlUserClient = new TLUSERClient();
                this.branchClient = new BranchClient();
                TLSYSROLEClient roleClient = new TLSYSROLEClient();
                LoadBranch();

                this.IsSingleData = true;

                roleClient.GetByTopTLSYSROLEAsync("", "", "");
                roleClient.GetByTopTLSYSROLECompleted += (s, e) =>
                {
                    ObservableCollection<TL_SYSROLE> tempRole;
                    if (e.Result != null && e.Result.Count > 0)                    
                        tempRole = e.Result;                                           
                    else                    
                        tempRole = new ObservableCollection<TL_SYSROLE>();                        
                    
                    tempRole.Add(new TL_SYSROLE() { ROLE_ID = CommonResource.lblAll });
                    this.RoleData = tempRole;
                    this.OnPropertyChanged("RoleData");
                };                

                this.AuthData = new List<ItemYesNo>()
                {
                    new ItemYesNo(){Id="A", Name=CommonResource.lblApproved},
                    new ItemYesNo(){Id="U", Name=CommonResource.lblDisApprove},
                    new ItemYesNo(){Id="", Name=CommonResource.lblAll}
                };

                this.tlUserClient.SearchTLUSERCompleted += new EventHandler<SearchTLUSERCompletedEventArgs>(searchTLUserCompleted);
                this.tlUserClient.DeleteTLUSERCompleted += new EventHandler<DeleteTLUSERCompletedEventArgs>(deletedCompleted);
            }
            catch (Exception ex)
            {
                this.messagePop.SetSingleError(ex.Message);
            }
        }

        private void deletedCompleted(object sender, DeleteTLUSERCompletedEventArgs e)
        {
            try
            {
                //delete successful
                if (e.Result.Result.Equals("0"))
                {
                    //remove on current CurrencyData
                    this.currentData.Remove(this.currentUser.ItemContent);
                    this.DataRefresh();
                }
                else
                    this.messagePop.SetSingleError(CommonResource.errorDelete);
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

        private void searchTLUserCompleted(object sender, SearchTLUSERCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.currentData = e.Result;
                else
                    this.currentData = new ObservableCollection<TL_USER_SearchResult>();
                this.DataRefresh();
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

        private void DoubleClickItem()
        {
            DateTime now = DateTime.Now;
            if (now.Subtract(_clickTs).TotalMilliseconds <= 200)//
            {
                if (ActionMenuButton.actionControl.Edit.CanExecute(ActionMenuButton.Edit))
                    this.Edit();
                else if (ActionMenuButton.actionControl.View.CanExecute(ActionMenuButton.View))
                    this.View();
            }
            else
            {
                _clickTs = now;
            }
        }

        // Load CM_Branch
        private void LoadBranch()
        {
            branchClient.CM_BRANCH_LevelAsync(CurrentSystemLogin.CurrentUser.TLSUBBRID);
            branchClient.CM_BRANCH_LevelCompleted += (s, e) =>
            {
                try
                {
                    if (e.Result != null)
                    {
                        this.dicBranch = new Dictionary<string, ObservableCollection<CM_BRANCH>>();
                        this.dicTrans = new Dictionary<string, ObservableCollection<CM_BRANCH>>();

                        //Gan danh sach KV
                        if (e.Result.RegionList != null)
                            this.KVData = e.Result.RegionList;
                        else this.KVData = new ObservableCollection<CM_BRANCH>();

                        //Luu danh sach chi nhanh vao dic
                        if (e.Result.BranchList == null)
                            this.BranchData = new ObservableCollection<CM_BRANCH>();
                        foreach (var item in e.Result.BranchList)
                            if (this.dicBranch.ContainsKey(item.FATHER_ID))
                                this.dicBranch[item.FATHER_ID].Add(item);
                            else
                            {
                                this.dicBranch.Add(item.FATHER_ID, new ObservableCollection<CM_BRANCH>());
                                this.dicBranch[item.FATHER_ID].Add(item);
                            }

                        //Luu danh sach PGD vao dic
                        if (e.Result.TransList != null)
                            this.PGDData = new ObservableCollection<CM_BRANCH>();
                        foreach (var item in e.Result.TransList)
                            if (this.dicTrans.ContainsKey(item.FATHER_ID))
                                this.dicTrans[item.FATHER_ID].Add(item);
                            else
                            {
                                this.dicTrans.Add(item.FATHER_ID, new ObservableCollection<CM_BRANCH>());
                                this.dicTrans[item.FATHER_ID].Add(item);
                            }
                    }
                    if (this.KVData.Count > 1)
                        this.KVData.Add(new CM_BRANCH() { BRANCH_ID = "", BRANCH_CODE = "", BRANCH_NAME = CommonResource.lblAll });
                    foreach (var key in this.dicBranch.Keys)
                        if (this.dicBranch[key].Count > 1)
                            this.dicBranch[key].Add(new CM_BRANCH() { BRANCH_ID = "", BRANCH_CODE = "", BRANCH_NAME = CommonResource.lblAll });
                    foreach (var key in this.dicTrans.Keys)
                        if (this.dicTrans[key].Count > 1)
                            this.dicTrans[key].Add(new CM_BRANCH() { BRANCH_ID = "", BRANCH_CODE = "", BRANCH_NAME = CommonResource.lblAll });

                    this.OnPropertyChanged("KVData");
                    //Set gia tri mac dinh tren combobox
                    if (this.KVData.Count > 0)
                        this.KVId = this.KVData[0].BRANCH_ID;
                    if (this.BranchData.Count > 0)
                        this.BranchId = this.BranchData[0].BRANCH_ID;
                    if (this.PGDData.Count > 0)
                        this.PGDId = this.PGDData[0].BRANCH_ID;
                    this.OnPropertyChanged("PGDId");

                    if (CurrentSystemLogin.CurrentUser.BRANCH_TYPE.Equals("HS"))
                    {
                        this.KVId = "";
                        this.OnPropertyChanged("KVId");
                    }
                    else if (CurrentSystemLogin.CurrentUser.BRANCH_TYPE.Equals("KV"))
                    {
                        this.BranchId = "";
                        this.OnPropertyChanged("BranchId");
                    }
                    else if (CurrentSystemLogin.CurrentUser.BRANCH_TYPE.Equals("CN"))
                    {
                        this.PGDId = "";
                        this.OnPropertyChanged("PGDId");
                    }
                }
                catch (Exception ex)
                {
                    this.messagePop.SetSingleError(ex.Message);
                }
            };

        }
        
        #endregion

        //Command button action
        public class ActionButton : ICommand
        {
            private UserViewModel viewModel;
            public ActionButton(UserViewModel viewModel)
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
                if (parameter.ToString().Equals(ActionMenuButton.Delete)
                    || parameter.ToString().Equals(ActionMenuButton.Edit)
                    || parameter.ToString().Equals(ActionMenuButton.View))
                    return this.viewModel.currentUser != null;
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                switch (parameter.ToString())
                {
                    case ActionMenuButton.Edit: this.viewModel.Edit();
                        break;
                    case ActionMenuButton.Delete: this.viewModel.Delete();
                        break;
                    case ActionMenuButton.Insert: this.viewModel.Insert();
                        break;
                    case ActionMenuButton.Search: this.viewModel.Search();
                        break;
                    case ActionMenuButton.View: this.viewModel.View();
                        break;
                    default: break;
                }
            }
        }

        public class UserItem
        {
            public int ID { get; set; }
            public TL_USER_SearchResult ItemContent { get; set; }
        }
    }
}
