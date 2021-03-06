﻿using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using gMVVM.gMVVMService;
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
    public class DepartmentTypeViewModel : ViewModelBase
    {
        private CM_DEPT_GROUPClient departmentTypeClient;
        public DepartmentTypeViewModel()
        {
            this.Load();
            this.actionButton = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
            this.DoubleClickItemCommand = new RelayCommand(DoubleClickItem);
            Messenger.Reset();
            Messenger.Default.Register<ParentMessage>(this, CurrencyEditResponseMessageReceived);
        }

        //save data Master search   
        private ObservableCollection<CM_DEPT_GROUP_SearchResult> currentData;
        private DepartmentTypeEdit editChild;
        private ActionButton actionButton;
        private DateTime _clickTs;
        private bool isInsert = false;
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
        private DepartmentTypeItem currentSelectItem;
        public DepartmentTypeItem CurrentSelectItem
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
        //Item hien tai
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
        //Load Trang thai duyet  
        private string authStatus="";
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

        //Click Item
        public ICommand DoubleClickItemCommand { get; private set; }
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
        #endregion

        #region[Action Functions]

        private void Insert()
        {
            if (this.editChild == null)
            {
                this.editChild = new DepartmentTypeEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }
            Messenger.Default.Send(new ChildMessage() { currentObject = null, isEdit = false, action = ActionMenuButton.Insert, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();              
                this.currentItem.AUTH_STATUS = this.authStatus;
                MyHelper.IsBusy();
                this.departmentTypeClient.SearchCM_DEPT_GROUPAsync(this.currentItem, 200);
            }
            catch (Exception)
            {
                MyHelper.IsFree();
            }

        }
        private void Edit()
        {
            if (this.editChild == null)
            {
                this.editChild = new DepartmentTypeEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }
            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem.ItemContent, isEdit = true, action = ActionMenuButton.Edit, level = "Main" });
            PageAnimation.ToBack();
        }

        private void View()
        {
            if (this.editChild == null)
            {
                this.editChild = new DepartmentTypeEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }
            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem.ItemContent, isEdit = false, action = ActionMenuButton.View, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Delete()
        {
            this.messagePop.Reset();
            if (this.currentSelectItem.ItemContent.AUTH_STATUS.Equals("A")) this.messagePop.SetError(ValidatorResource.lblErrorDeletePer);
            if (this.messagePop.HasError()) return;
            else if (MessageBox.Show(CommonResource.msgDelete, CommonResource.btnDelete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //call delete item
                MyHelper.IsBusy();
                this.departmentTypeClient.CM_DEPT_GROUP_DelAsync(this.currentSelectItem.ItemContent.GROUP_ID);
            }
        }

        public ICommand ActionCommand
        {
            get
            {
                return this.actionButton;
            }
        }

        #endregion

        #region[Functions]
        //Received Message
        private void CurrencyEditResponseMessageReceived(ParentMessage obj)
        {
            if (obj.level.Equals("Main"))
            {
                ////OK button clicked
                if (obj.currentObject != null)
                {
                    if (obj.action.Equals("Update"))
                        this.CurrentSelectItem.ItemContent = (obj.currentObject as CM_DEPT_GROUP_SearchResult);
                    else
                        this.isInsert = true;
                }
                else
                {
                    this.messagePop.Reset();
                    ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
                    PageAnimation.ToFront();
                    if (this.isInsert)
                    {
                        this.isInsert = false;
                        this.Search();
                    }
                }
            }
        }
        //reload data from database
        private void Load()
        {
            try
            {
                this.currentData = new ObservableCollection<CM_DEPT_GROUP_SearchResult>();
                this.CurrentItem = new CM_DEPT_GROUP_SearchResult();
                this.departmentTypeClient = new CM_DEPT_GROUPClient();
                this.ItemsData = new List<ItemYesNo>()
                    {
                        new ItemYesNo(){ Id= "A", Name=CommonResource.lblApproved},
                        new ItemYesNo(){ Id= "U", Name=CommonResource.lblDisApprove},
                        new ItemYesNo(){ Id= "", Name=CommonResource.lblAll}
                    };
                this.departmentTypeClient.CM_DEPT_GROUP_DelCompleted += new EventHandler<CM_DEPT_GROUP_DelCompletedEventArgs>(deleteCompleted);
                this.departmentTypeClient.SearchCM_DEPT_GROUPCompleted += new EventHandler<SearchCM_DEPT_GROUPCompletedEventArgs>(searchComplete);
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void searchComplete(object sender, SearchCM_DEPT_GROUPCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.currentData = e.Result;
                else
                    this.currentData = new ObservableCollection<CM_DEPT_GROUP_SearchResult>();

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
        private void DataRefresh()
        {
            List<DepartmentTypeItem> lstTemp = new List<DepartmentTypeItem>();
            int i = 1;
            foreach (var item in this.currentData)
            {
                lstTemp.Add(new DepartmentTypeItem() { ID = i, ItemContent = item });
                i++;
            }
            this.DataItem = new PagedCollectionView(lstTemp);
        }
        private void deleteCompleted(object sender, CM_DEPT_GROUP_DelCompletedEventArgs e)
        {
            try
            {
                //delete successful
                if (e.Result.Result == "0")
                {
                    //remove on current CurrentData
                    this.currentData.Remove(this.currentSelectItem.ItemContent);
                    this.DataRefresh();                    
                }
                else
                {
                    this.messagePop.SetSingleError(CommonResource.errorDelete + "\n " + e.Result.ErrorDesc);
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
            private DepartmentTypeViewModel viewModel;
            public ActionButton(DepartmentTypeViewModel viewModel)
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
                if (parameter != null)
                {
                    if (parameter.ToString().Equals(ActionMenuButton.Delete)
                        || parameter.ToString().Equals(ActionMenuButton.Edit)
                        || parameter.ToString().Equals(ActionMenuButton.View))
                        return this.viewModel.currentSelectItem != null;
                }

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

        public class DepartmentTypeItem : ViewModelBase
        {
            public int ID { get; set; }
            private CM_DEPT_GROUP_SearchResult itemContent;
            public CM_DEPT_GROUP_SearchResult ItemContent
            {
                get { return this.itemContent; }
                set
                {
                    this.itemContent = value;
                    this.OnPropertyChanged("ItemContent");
                }
            }
        }
    }
}
