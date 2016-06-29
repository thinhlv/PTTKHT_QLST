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
    public class MenuViewModel : ViewModelBase
    {
        private TLMENUClient menuClient;
        
        public MenuViewModel()
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
        private ObservableCollection<TL_MENU> currencyData;
        private MenuEdit menuChild;
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
        
        private PagedCollectionView menuData;
        public PagedCollectionView MenuData
        {
            get
            {
                return this.menuData;
            }

            set
            {
                this.menuData = value;
                OnPropertyChanged("MenuData");
            }
        }

        private TL_MENU currentMenu;
        public TL_MENU CurrentMenu
        {
            get
            {
                return this.currentMenu;
            }

            set
            {
                this.currentMenu = value;
                OnPropertyChanged("CurrentMenu");
            }
        }

        private string menuName = "";
        public string MenuName
        {
            get
            {
                return this.menuName;
            }

            set
            {
                this.menuName = value;
                this.OnPropertyChanged("MenuName");
            }
        }

        private string menuParent = "";
        public string MenuParent
        {
            get
            {
                return this.menuParent;
            }

            set
            {
                this.menuParent = value;
                this.OnPropertyChanged("MenuParent");
            }
        }

        //TransactionType
        private ObservableCollection<TL_MENU> parentData;
        public ObservableCollection<TL_MENU> ParentData
        {
            get
            {
                return this.parentData;
            }

            set
            {
                this.parentData = value;
                this.OnPropertyChanged("ParentData");
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
            if (this.menuChild == null)
            {
                this.menuChild = new MenuEdit();
                PageAnimation.back.Children.Add(this.menuChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = null, isEdit = false, action = ActionMenuButton.Insert, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Edit()
        {
            if (this.menuChild == null)
            {
                this.menuChild = new MenuEdit();
                PageAnimation.back.Children.Add(this.menuChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentMenu, isEdit = true, action = ActionMenuButton.Edit, level = "Main" });
            PageAnimation.ToBack();
        }

        private void View()
        {
            if (this.menuChild == null)
            {
                this.menuChild = new MenuEdit();
                PageAnimation.back.Children.Add(this.menuChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentMenu, isEdit = false, action = ActionMenuButton.View, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Delete()
        {
            this.messagePop.Reset();
            if (this.currentMenu.ISAPPROVE.Equals("1"))
            {
                this.messagePop.SetSingleError(CommonResource.lblIsUsing);
                return;
            }

            if (MessageBox.Show(CommonResource.msgDelete, CommonResource.btnDelete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //call delete item
                MyHelper.IsBusy();
                this.menuClient.DeleteTLMENUAsync(this.currentMenu);
            }            
            
        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                string where = " MENU_NAME like '%" + (this.menuName.Equals("") ? "%" : this.menuName) + "%' ";
                if (this.menuParent.Equals(""))
                    where += " and MENU_PARENT like '%' ";
                else if (this.menuParent.Equals(" "))
                    where += " and MENU_PARENT = '' ";
                else where += " and MENU_PARENT = '" + this.menuParent + "' ";

                MyHelper.IsBusy();
                this.menuClient.GetByTopTLMENUAsync("200", where, "");
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
                TL_MENU newitem = (obj.currentObject as TL_MENU);
                this.currencyData.Add(newitem);
                if (newitem.MENU_PARENT.Equals(""))
                {
                    this.ParentData.Add(new TL_MENU() { MENU_PARENT = newitem.MENU_ID.ToString(), MENU_NAME = newitem.MENU_NAME });
                }
            }
            else
            {
                this.messagePop.Reset();
                ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
                PageAnimation.ToFront();
            }

        }

        private void searchCompleted(object sender, GetByTopTLMENUCompletedEventArgs e)
        {
            try
            {
                this.currencyData = e.Result;
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

        private void deleteCurrencyCompleted(object sender, DeleteTLMENUCompletedEventArgs e)
        {
            try
            {
                //delete successful
                if (e.Result)
                {
                    //remove on current CurrencyData
                    this.currencyData.Remove(this.currentMenu);
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
            this.MenuData = new PagedCollectionView(this.currencyData);            
        }

        //reload data from database
        private void Reload()
        {
            try
            {
                this.menuClient = new TLMENUClient();
                this.parentData = new ObservableCollection<TL_MENU>();
                //this.menuClient.GetByTopTLMENUAsync("200", "", "");

                this.currencyData = new ObservableCollection<TL_MENU>();

                //this.menuParent = new TL_MENU() { MENU_ID = -1, MENU_NAME = CommonResource.lblAll };
                this.menuClient.GetParentTLMENUAsync();
                this.menuClient.GetParentTLMENUCompleted += (s, e) =>
                    {
                        this.parentData = new ObservableCollection<TL_MENU>();
                        this.parentData.Add(new TL_MENU() { MENU_PARENT = "", MENU_NAME = " " + CommonResource.lblAll });
                        this.parentData.Add(new TL_MENU() { MENU_PARENT = " ", MENU_NAME = "---NULL---" });
                        if (e.Result != null && e.Result.Count > 0)
                        {
                            foreach (var item in e.Result)
                                this.parentData.Add(new TL_MENU() { MENU_PARENT = item.MENU_ID.ToString(), MENU_NAME = item.MENU_NAME });                           
                        }
                        
                        this.OnPropertyChanged("ParentData");
                    };

                this.menuClient.GetByTopTLMENUCompleted += new EventHandler<GetByTopTLMENUCompletedEventArgs>(searchCompleted);
                this.menuClient.DeleteTLMENUCompleted += new EventHandler<DeleteTLMENUCompletedEventArgs>(deleteCurrencyCompleted);
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
            private MenuViewModel viewModel;
            public ActionButton(MenuViewModel viewModel)
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
                    return this.viewModel.currentMenu != null;
                    
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
