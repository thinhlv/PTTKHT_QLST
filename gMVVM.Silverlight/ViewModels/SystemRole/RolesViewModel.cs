using gMVVM.CommonClass;
using gMVVM.gMVVMService;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using gMVVM.Views.SystemRole;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
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
using mvvmCommon;

namespace gMVVM.ViewModels.SystemRole
{
    public class RolesViewModel : ViewModelBase
    {
        private TLSYSROLEClient roleClient;
        
        public RolesViewModel()
        {            
            this.Reload();

            this.actionButton = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton);
            this.DoubleClickItemCommand = new RelayCommand(DoubleClickItem);

            Messenger.Reset();
            Messenger.Default.Register<ParentMessage>(this, CurrencyEditResponseMessageReceived);

            //this.menuClient.DeleteTLMENUCompleted += new EventHandler<DeleteCMAGENTCompletedEventArgs>(deleteCurrencyCompleted);
           
        }                      

        //save data from database
        private ObservableCollection<TL_SYSROLE> currencyData;
        private RolesEdit roleChild;
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
        
        private PagedCollectionView roleData;
        public PagedCollectionView RoleData
        {
            get
            {
                return this.roleData;
            }

            set
            {
                this.roleData = value;
                OnPropertyChanged("RoleData");
            }
        }

        private TL_SYSROLE currentRole;
        public TL_SYSROLE CurrentRole
        {
            get
            {
                return this.currentRole;
            }

            set
            {
                this.currentRole = value;
                OnPropertyChanged("CurrentRole");
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
            }
        }

        private string roleDesc = "";
        public string RoleDesc
        {
            get
            {
                return this.roleDesc;
            }

            set
            {
                this.roleDesc = value;
                this.OnPropertyChanged("RoleDesc");
            }
        }
        
        public ICommand DoubleClickItemCommand { get; private set; }

        //public List<ItemYesNo> ItemsStatusData { get; set; }
        //public List<ItemYesNo> ItemsAuthData { get; set; }

        //public string Status { get; set; }
        //public string AuthStatus { get; set; }

        #endregion

        #region[Action Functions]        

        private void Insert()
        {
            if (this.roleChild == null)
            {
                this.roleChild = new RolesEdit();
                PageAnimation.back.Children.Add(this.roleChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = null, isEdit = false, action = ActionMenuButton.Insert, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Edit()
        {
            if (this.roleChild == null)
            {
                this.roleChild = new RolesEdit();
                PageAnimation.back.Children.Add(this.roleChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentRole, isEdit = true, action = ActionMenuButton.Edit, level = "Main" });
            PageAnimation.ToBack();
        }

        private void View()
        {
            if (this.roleChild == null)
            {
                this.roleChild = new RolesEdit();
                PageAnimation.back.Children.Add(this.roleChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentRole, isEdit = false, action = ActionMenuButton.View, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Delete()
        {
            this.messagePop.Reset();
            if (this.currentRole.ISAPPROVE.Equals("1"))
            {
                this.messagePop.SetSingleError(CommonResource.lblIsUsing);
                return;
            }

            if (MessageBox.Show(CommonResource.msgDelete, CommonResource.btnDelete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //call delete item
                MyHelper.IsBusy();
                this.roleClient.DeleteTLSYSROLEAsync(this.currentRole);
            }            
            
        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                string where = " ROLE_ID like '%" + (this.roleName.Equals("") ? "%" : this.roleName) + "%' "
                    + " and ROLE_DESC like '%" + (this.roleDesc.Equals("") ? "%" : this.roleDesc) + "%' ";
                MyHelper.IsBusy();
                this.roleClient.GetByTopTLSYSROLEAsync("200", where, "");
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
                TL_SYSROLE newitem = (obj.currentObject as TL_SYSROLE);
                this.currencyData.Add(newitem);                
            }
            else
            {
                this.messagePop.Reset();
                ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
                PageAnimation.ToFront();
            }

        }

        private void searchCompleted(object sender, GetByTopTLSYSROLECompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.currencyData = e.Result;
                else this.currencyData = new ObservableCollection<TL_SYSROLE>();
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

        private void deleteCurrencyCompleted(object sender, DeleteTLSYSROLECompletedEventArgs e)
        {
            try
            {
                //delete successful
                if (e.Result)
                {
                    //remove on current CurrencyData
                    this.currencyData.Remove(this.currentRole);
                    this.Refresh();
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

        //refresh current data
        private void Refresh()
        {
            this.RoleData = new PagedCollectionView(this.currencyData);            
        }

        //reload data from database
        private void Reload()
        {
            try
            {
                this.roleClient = new TLSYSROLEClient();
                this.currencyData = new ObservableCollection<TL_SYSROLE>();
                //this.roleClient.GetByTopTLSYSROLEAsync("200", "", "");

                this.currencyData = new ObservableCollection<TL_SYSROLE>();

                this.roleClient.GetByTopTLSYSROLECompleted += new EventHandler<GetByTopTLSYSROLECompletedEventArgs>(searchCompleted);
                this.roleClient.DeleteTLSYSROLECompleted += new EventHandler<DeleteTLSYSROLECompletedEventArgs>(deleteCurrencyCompleted);
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
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
            private RolesViewModel viewModel;
            public ActionButton(RolesViewModel viewModel)
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
                    return this.viewModel.currentRole != null;
                    
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
