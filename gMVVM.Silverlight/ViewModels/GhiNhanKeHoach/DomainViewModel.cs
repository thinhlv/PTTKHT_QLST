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
using gMVVM.GhiNhanService;

namespace gMVVM.ViewModels.GhiNhanKeHoach
{
    public class DomainViewModel : ViewModelBase
    {
        private CM_DomainClient domainClient;
        public DomainViewModel()
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

        public ICommand DoubleClickItemCommand { get; private set; }

        //public ObservableCollection<CM_DEPT_GROUP_SearchResult> GroupData
        //{ get; set; }


        //public List<ItemYesNo> AuthData { get; set; }

        #endregion

        #region[Action Functions]

        private void Insert()
        {
            
        }

        private void Edit()
        {
            
        }

        private void View()
        {
        }

        private void Delete()
        {
           

        }

        private void Search()
        {
            this.insertItemDomain.CREATE_DT = DateTime.Now;
            domainClient.InsertDomainAsync(this.insertItemDomain);
        }

        #endregion

        #region[Functions]

        //Received Message
        private void CurrencyEditResponseMessageReceived(ParentMessage obj)
        {
            //OK button clicked
            if (obj.currentObject != null)
            {

            }
            else
            {
                this.messagePop.Reset();
                ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
                PageAnimation.ToFront();
            }

        }


        private CM_DOMAIN insertItemDomain;
        public CM_DOMAIN InsertItemDomain
        {
            get
            {
                return this.insertItemDomain;
            }
            set
            {
                this.insertItemDomain = value;
                this.OnPropertyChanged("InsertItemDomain");
            }
        }

        private string changeBindPath;
        public string ChangeBindPath
        {
            get
            {
                return this.changeBindPath;
            }
            set
            {
                this.changeBindPath = value;
                this.OnPropertyChanged("changeBindPath");
            }
        }

        //reload data from database
        private void Reload()
        {
            try
            {
                domainClient = new CM_DomainClient();
                this.insertItemDomain = new CM_DOMAIN();
                

                //this.AuthData = new List<ItemYesNo>()
                //{
                //    new ItemYesNo(){Id="A", Name=CommonResource.lblApproved},
                //    new ItemYesNo(){Id="U", Name=CommonResource.lblDisApprove},
                //    new ItemYesNo(){Id="", Name=CommonResource.lblAll}
                //};

                domainClient.InsertDomainCompleted += new EventHandler<InsertDomainCompletedEventArgs>(insertDomainCompleted);
                
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void insertDomainCompleted(object sender, InsertDomainCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result == "-1")
                    this.messagePop.SetSingleError(e.Result.ErrorDesc);
                else
                    this.messagePop.Successful("Insert thanh cong");
            }
            catch (Exception)
            {

                throw;
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
            private DomainViewModel viewModel;
            public ActionButton(DomainViewModel viewModel)
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
                    //return this.viewModel.currentSelectItem != null;
                    return false;

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
