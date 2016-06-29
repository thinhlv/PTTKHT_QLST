using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
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

namespace gMVVM.ViewModels.Search
{
    public class BranchSearchingViewModel : ViewModelBase
    { 
        private BranchClient branchClient;
        public BranchSearchingViewModel()
        {
            this.Reload();

            this.actionButton = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();            
            this.DoubleClickItemCommand = new RelayCommand(DoubleClickItem);
            
            Messenger.Default.Register<string>(this, CurrencyEditResponseMessageReceived);
            Messenger.Default.Register<Dictionary<string, string>>(this, ResponseMessageReceived);
        }

        private void ResponseMessageReceived(Dictionary<string, string> obj)
        {
            this.index = int.Parse(obj["index"]);
        }

        private void CurrencyEditResponseMessageReceived(string obj)
        {
            this.level = obj;
        }

        //save data from database
        private ObservableCollection<CM_BRANCH_SearchResult> currentData;
        private ActionButton actionButton;
        private DateTime _clickTs;
        private string level = "";
        private int index = 0;

        #region[All Properties]

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

        private PagedCollectionView dataItem;
        public PagedCollectionView DataItem
        {
            get
            {
                return this.dataItem;
            }

            set
            {
                this.dataItem = value;
                OnPropertyChanged("DataItem");
            }
        }

        private CM_BRANCH_SearchResult currentSelectItem;
        public CM_BRANCH_SearchResult CurrentSelectItem
        {
            get
            {
                return this.currentSelectItem;
            }

            set
            {
                this.currentSelectItem = value;
                this.OnPropertyChanged("CurrentSelectItem");
            }
        }

        private string branchIdSearch = "";
        public string BranchIdSearch
        {
            get
            {
                return this.branchIdSearch;
            }

            set
            {
                this.branchIdSearch = value;
                this.OnPropertyChanged("BranchIdSearch");
            }
        }

        private string branchname = "";
        public string BranchName
        {
            get
            {
                return this.branchname;
            }

            set
            {
                this.branchname = value;
                this.OnPropertyChanged("BranchName");
            }
        }

        //Su kien button
        public ICommand ActionCommand { get { return this.actionButton; } }

        public ICommand DoubleClickItemCommand { get; private set; }

        #endregion

        #region[Action Functions]

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                string branch = "";
                //if (this.PGDId != null && !this.PGDId.Equals(""))
                //    branch = this.PGDId;
                //else
                    if (this.branchId != null && !this.branchId.Equals(""))
                    {
                        branch = this.branchId;
                    }
                    else if (this.kvId != null && !this.kvId.Equals(""))
                    {
                        branch = this.kvId;
                    }
                    else
                    {
                        branch = CurrentSystemLogin.CurrentUser.TLSUBBRID;
                    }
                //string where = " BRANCH_CODE like '%" + (this.branchId.Equals("") ? "%" : this.branchId) + "%' "
                //    + " and [BRANCH_NAME] like N'%" + (this.branchname.Equals("") ? "%" : this.branchname) + "%' ";
                this.branchClient.BranchSearchAsync(new CM_BRANCH_SearchResult() { BRANCH_CODE = this.branchIdSearch, BRANCH_NAME = this.branchname }, branch, 300);
                //this.branchClient.GetByTopBranchAsync("200", where, "");
            }
            catch (Exception)
            { }

        }

        #endregion

        #region[Functions]        

        //refresh current data
        private void Refresh()
        {
            this.DataItem = new PagedCollectionView(this.currentData);
        }

        //reload data from database
        private void Reload()
        {
            try
            {
                this.branchClient = new BranchClient();
                
                branchClient.CM_BRANCH_LevelAsync(CurrentSystemLogin.CurrentUser.TLSUBBRID);
                branchClient.CM_BRANCH_LevelCompleted += (s, e) =>
                {
                    try
                    {
                        this.dicBranch = new Dictionary<string, ObservableCollection<CM_BRANCH>>();
                        this.dicTrans = new Dictionary<string, ObservableCollection<CM_BRANCH>>();
                        if (e.Result != null)
                        {
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
                        else
                        {
                            this.KVData = new ObservableCollection<CM_BRANCH>();
                            this.BranchData = new ObservableCollection<CM_BRANCH>();
                            this.PGDData = new ObservableCollection<CM_BRANCH>();
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

                this.branchClient.BranchSearchCompleted += new EventHandler<BranchSearchCompletedEventArgs>(searchCompleted);
                //this.branchClient.GetByTopBranchCompleted += new EventHandler<GetByTopBranchCompletedEventArgs>(searchCompleted);                
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void searchCompleted(object sender, BranchSearchCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.currentData = e.Result;
                else
                    this.currentData = new ObservableCollection<CM_BRANCH_SearchResult>();
                this.Refresh();
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
                if (this.currentSelectItem != null)
                    this.AcceptItem();  
            }
            else
            {
                _clickTs = now;
            }
        } 

        private void AcceptItem()
        {
            if (this.level.Equals("Child"))
                Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem, level = "_BranchSearching", index = this.index });
            else if (this.level.Equals("Main"))
                Messenger.Default.Send(new ParentMessage() { currentObject = this.currentSelectItem, level = "_BranchSearching", index = this.index });
        }

        #endregion

        //Command button action
        public class ActionButton : ICommand
        {
            private BranchSearchingViewModel viewModel;
            public ActionButton(BranchSearchingViewModel viewModel)
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
                    case ActionMenuButton.Search: this.viewModel.Search();
                        break;
                    case "Ok": this.viewModel.AcceptItem();
                        break;
                    default: break;
                }
            }
        }
    }
}
