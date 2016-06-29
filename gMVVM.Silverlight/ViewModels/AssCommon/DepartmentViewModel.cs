using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using gMVVM.ViewModels.Common;
using mvvmCommon;
using System;
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
using gMVVM.Views.AssCommon;
using System.Collections.Generic;

namespace gMVVM.ViewModels.AssCommon
{
    public class DepartmentViewModel : ViewModelBase
    {
         private DEPARTMENTClient departmentClient;
         private CM_DEPT_GROUPClient departmentTypeClient;
         public DepartmentViewModel()
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
         private ObservableCollection<CM_DEPARTMENT_SearchResult> currentData;
        private DepartmentEdit editChild;
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

        private DepartmentItem currentSelectItem;
        public DepartmentItem CurrentSelectItem
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

        public ICommand DoubleClickItemCommand { get; private set; }

        public ObservableCollection<CM_DEPT_GROUP_SearchResult> GroupData
        { get; set; }

        private CM_DEPARTMENT_SearchResult currentSearch = new CM_DEPARTMENT_SearchResult() {AUTH_STATUS = "", GROUP_ID = ""};
        public CM_DEPARTMENT_SearchResult CurrentSearch
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

        public List<ItemYesNo> AuthData { get; set; }

        #endregion

        #region[Action Functions]

        private void Insert()
        {
            if (this.editChild == null)
            {
                this.editChild = new DepartmentEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = null, isEdit = false, action = ActionMenuButton.Insert, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Edit()
        {
            if (this.editChild == null)
            {
                this.editChild = new DepartmentEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem.ItemContent, isEdit = true, action = ActionMenuButton.Edit, level = "Main" });
            PageAnimation.ToBack();
        }

        private void View()
        {
            if (this.editChild == null)
            {
                this.editChild = new DepartmentEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem.ItemContent, isEdit = false, action = ActionMenuButton.View, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Delete()
        {
            this.messagePop.Reset();

            if (MessageBox.Show(CommonResource.msgDelete, CommonResource.btnDelete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //call delete item
                MyHelper.IsBusy();
                this.departmentClient.DeleteDEPARTMENTAsync(this.currentSelectItem.ItemContent.DEP_ID);
            }

        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                MyHelper.IsBusy();
                this.departmentClient.SearchDEPARTMENTAsync(this.currentSearch, 200);
            }
            catch (Exception)
            { }

        }

        #endregion

        #region[Functions]

        //Received Message
        private void CurrencyEditResponseMessageReceived(ParentMessage obj)
        {
            //OK button clicked
            if (obj.currentObject != null)
            {
                this.currentData.Add(obj.currentObject as CM_DEPARTMENT_SearchResult);
                this.DataRefresh();
            }
            else
            {
                this.messagePop.Reset();
                ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
                PageAnimation.ToFront();
            }

        }

        private void DataRefresh()
        {
            List<DepartmentItem> lstTemp = new List<DepartmentItem>();
            int i = 1;
            foreach (var item in this.currentData)
            {
                lstTemp.Add(new DepartmentItem() { ID = i, ItemContent = item });
                i++;
            }
            this.DataItem = new PagedCollectionView(lstTemp);
        }

        //reload data from database
        private void Reload()
        {
            try
            {
                this.departmentClient = new DEPARTMENTClient();
                this.departmentTypeClient = new CM_DEPT_GROUPClient();
                this.departmentTypeClient.SearchCM_DEPT_GROUPAsync(new CM_DEPT_GROUP_SearchResult(), 100);
                this.currentData = new ObservableCollection<CM_DEPARTMENT_SearchResult>();

                this.AuthData = new List<ItemYesNo>()
                {
                    new ItemYesNo(){Id="A", Name=CommonResource.lblApproved},
                    new ItemYesNo(){Id="U", Name=CommonResource.lblDisApprove},
                    new ItemYesNo(){Id="", Name=CommonResource.lblAll}
                };

                this.departmentClient.SearchDEPARTMENTCompleted += new EventHandler<SearchDEPARTMENTCompletedEventArgs>(searchCompleted);
                this.departmentClient.DeleteDEPARTMENTCompleted += new EventHandler<DeleteDEPARTMENTCompletedEventArgs>(deleteCompleted);
                this.departmentTypeClient.SearchCM_DEPT_GROUPCompleted += new EventHandler<SearchCM_DEPT_GROUPCompletedEventArgs>(getGroupDepCompleted);

            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void deleteCompleted(object sender, DeleteDEPARTMENTCompletedEventArgs e)
        {
            try
            {
                //delete successful
                if (e.Result.Result.Equals("0"))
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

        private void searchCompleted(object sender, SearchDEPARTMENTCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.currentData = e.Result;
                else
                    this.currentData = new ObservableCollection<CM_DEPARTMENT_SearchResult>();
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

        private void getGroupDepCompleted(object sender, SearchCM_DEPT_GROUPCompletedEventArgs e)
        {
            try
            {
                ObservableCollection<CM_DEPT_GROUP_SearchResult> tempGroup;
                if (e.Result != null && e.Result.Count > 0)
                    tempGroup = e.Result;
                else
                    tempGroup = new ObservableCollection<CM_DEPT_GROUP_SearchResult>();

                tempGroup.Add(new CM_DEPT_GROUP_SearchResult() { GROUP_ID = "", GROUP_NAME = CommonResource.lblAll });
                this.GroupData = tempGroup;
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
            private DepartmentViewModel viewModel;
            public ActionButton(DepartmentViewModel viewModel)
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

        public class DepartmentItem : ViewModelBase
        {
            public int ID { get; set; }
            private CM_DEPARTMENT_SearchResult itemContent;
            public CM_DEPARTMENT_SearchResult ItemContent
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
