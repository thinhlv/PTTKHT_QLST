using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using GalaSoft.MvvmLight.Command;
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
using System.Collections.ObjectModel;
using System.Windows.Data;
using gMVVM.Views.AssCommon;
using System.Collections.Generic;

namespace gMVVM.ViewModels.AssCommon
{
    public class BranchViewModel: ViewModelBase
    {
        private BranchClient branchClient;
        public BranchViewModel()
        {
            this.Reload();

            this.actionButton = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
            this.DoubleClickItemCommand = new RelayCommand(DoubleClickItem);

            Messenger.Reset();
            Messenger.Default.Register<ParentMessage>(this, CurrencyEditResponseMessageReceived);
        }

        //save data from database
        private ObservableCollection<CM_BRANCH> currentData;
        private BranchEdit editChild;
        private ActionButton actionButton;
        private DateTime _clickTs;

        #region[All Properties]
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

        private CM_BRANCH currentSelectItem;
        public CM_BRANCH CurrentSelectItem
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

        private string branchId = "";
        public string BranchID
        {
            get
            {
                return this.branchId;
            }

            set
            {
                this.branchId = value;
                this.OnPropertyChanged("BranchID");
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

        private string authStatus = "";
        public string AuthStatus
        {
            get
            {
                return this.authStatus;
            }

            set
            {
                this.authStatus = value;
                this.OnPropertyChanged("AuthStatus");
            }
        }

        public ICommand DoubleClickItemCommand { get; private set; }

        #endregion

        #region[Action Functions]

        private void Insert()
        {
            if (this.editChild == null)
            {
                this.editChild = new BranchEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = null, isEdit = false, action = ActionMenuButton.Insert, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Edit()
        {
            if (this.editChild == null)
            {
                this.editChild = new BranchEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem, isEdit = true, action = ActionMenuButton.Edit, level = "Main" });
            PageAnimation.ToBack();
        }

        private void View()
        {
            if (this.editChild == null)
            {
                this.editChild = new BranchEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem, isEdit = false, action = ActionMenuButton.View, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Delete()
        {
            this.messagePop.Reset();
            //if (this.currentSelectItem.ISAPPROVE.Equals("1"))
            //{
            //    this.messagePop.SetSingleError(CommonResource.lblIsUsing);
            //    return;
            //}

            if (MessageBox.Show(CommonResource.msgDelete, CommonResource.btnDelete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //call delete item
                MyHelper.IsBusy();
                this.branchClient.DeleteBranchAsync(this.currentSelectItem.BRANCH_ID);
            }

        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                string where = " BRANCH_CODE like '%" + (this.branchId.Equals("") ? "%" : this.branchId) + "%' "
                    + " and [BRANCH_NAME] like N'%" + (this.branchname.Equals("") ? "%" : this.branchname) + "%' "
                    + " and [AUTH_STATUS] like '%" + (this.authStatus.Equals("") ? "%" : this.authStatus) + "%' "; 
                MyHelper.IsBusy();
                this.branchClient.GetByTopBranchAsync("200", where, "");
            }
            catch (Exception)
            { }

        }

        //Combobox data
        private List<ItemYesNo> itemsData;
        public List<ItemYesNo> ItemsData
        {
            get
            {
                return this.itemsData;
            }

            set
            {
                this.itemsData = value;
                this.OnPropertyChanged("ItemsData");
            }
        }

        #endregion

        #region[Functions]

        //Received Message
        private void CurrencyEditResponseMessageReceived(ParentMessage obj)
        {
            //OK button clicked
            if (obj.currentObject != null)
            {
                this.currentData.Add(obj.currentObject as CM_BRANCH);
            }
            else
            {
                this.messagePop.Reset();
                ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
                PageAnimation.ToFront();
            }

        }

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
                this.currentData = new ObservableCollection<CM_BRANCH>();
                this.ItemsData = new List<ItemYesNo>()
                {
                    new ItemYesNo(){ Id= "A", Name=CommonResource.lblApproved},
                    new ItemYesNo(){ Id= "U", Name=CommonResource.lblDisApprove},
                    new ItemYesNo(){ Id= "", Name=CommonResource.lblAll}
                };

                this.branchClient.GetByTopBranchCompleted += new EventHandler<GetByTopBranchCompletedEventArgs>(searchCompleted);
                this.branchClient.DeleteBranchCompleted += new EventHandler<DeleteBranchCompletedEventArgs>(deleteCompleted);

            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void deleteCompleted(object sender, DeleteBranchCompletedEventArgs e)
        {
            try
            {
                //delete successful
                if (e.Result.Equals(ListMessage.True))
                {
                    //remove on current CurrentData
                    this.currentData.Remove(this.currentSelectItem);
                    this.Refresh();
                }
                else
                {
                    if (ListMessage.Message.ContainsKey(e.Result))
                        this.messagePop.SetSingleError(CommonResource.errorDelete + "\n " + ListMessage.Message[e.Result]);
                    else
                        this.messagePop.SetSingleError(CommonResource.errorDelete + "\n " + e.Result);
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

        private void searchCompleted(object sender, GetByTopBranchCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.currentData = e.Result;
                else
                    this.currentData = new ObservableCollection<CM_BRANCH>();
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

        #endregion

        //Command button action
        public class ActionButton : ICommand
        {
            private BranchViewModel viewModel;
            public ActionButton(BranchViewModel viewModel)
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
                    return this.viewModel.currentSelectItem != null;

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
    }
}
