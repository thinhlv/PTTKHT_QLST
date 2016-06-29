using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using GalaSoft.MvvmLight.Messaging;
using mvvmCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM.ViewModels.SystemRole
{
    public class RolesEditViewModel : ViewModelBase
    {
        private TLSYSROLEClient roleClient;
        
        public RolesEditViewModel()
        {
            if (!IsolatedStorageSettings.ApplicationSettings["currentCulture"].ToString().Equals("vi-VN"))
                this.isVN = false;
            this.actionCommand = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            this.Load();
            Messenger.Default.Register<ChildMessage>(this, CurrencyEditSendMessageRecieve);
        }

        private void CurrencyEditSendMessageRecieve(ChildMessage obj)
        {            
            if (obj.action.Equals(ActionMenuButton.View))
            {
                ActionMenuButton.actionControl.SetAllAction(null, null, null, null, null, null, actionCommand);
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
            this.CurrentRole = obj.currentObject != null ? (obj.currentObject as TL_SYSROLE)
                : new TL_SYSROLE()
                {
                    AUTH_STATUS = "U",
                    ISAPPROVE = "0",
                    MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                    ROLE_ID = "",
                    ROLE_DESC = ""
                };
            if (obj.action.Equals(ActionMenuButton.View) || obj.action.Equals(ActionMenuButton.Edit))
            {
                this.roleClient.GetByTopTLROLEDETAILAsync("", " ROLE_ID = '" + this.currentRole.ROLE_ID + "' ", "");
            }
            else if (obj.action.Equals(ActionMenuButton.Insert) && this.currentMenuData != null)
                this.LoadMenuRole(this.currentMenuData);

            if (this.currentRole.AUTH_STATUS.Equals("A"))
                this.IsApproved = Visibility.Visible.ToString();
            else
                this.IsApproved = Visibility.Collapsed.ToString();

            this.OnPropertyChanged("IsView");
            this.OnPropertyChanged("IsApproved");
            this.OnPropertyChanged("");

        }

        private string preAuthStatus = "";
        private TreeViewItem currentItem;
        private TreeViewItem currentParentItem;
        private bool isVN = true;
        private Dictionary<string, ActionMenuRole> dicMenuRole;
        private Dictionary<string, CheckBox> dicCheckBoxItem;
        private ObservableCollection<TL_MENU> currentMenuData;
        private ObservableCollection<TL_SYSROLEDETAIL> currentRoleDetailData;
        private List<string> lstInsertRoleDetail;
        private ObservableCollection<string> lstDeleteRoleDetail;
        private List<string> lstRootRoleDetail;
        private bool isFirstTime = true;
        private bool isLoading = true;

        #region[All Properties]

        public bool IsView { get; set; }

        public string IsApproved { get; set; }

        public bool IsEditFalse { get { return !this.isEdit; } }
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
                this.OnPropertyChanged("CurrentRole");
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

        //List Menu treeview
        private List<TreeViewItem> menuData;
        public List<TreeViewItem> MenuData
        {
            get
            {
                return this.menuData;
            }

            set
            {
                this.menuData = value;
                this.OnPropertyChanged("MenuData");
            }
        }

        private ActionMenuRole currentMenuRole;
        public ActionMenuRole CurrentMenuRole
        {
            get
            {
                return this.currentMenuRole;
            }

            set
            {
                this.currentMenuRole = value;
                this.OnPropertyChanged("CurrentMenuRole");
            }
        }
        
        #endregion

        #region[Action Functions]

        private void OkAction()
        {
            try
            {
                RefreshValidator();
                if (!this.messagePop.HasError())
                {

                    if (this.isEdit)
                    {
                        this.currentRole.MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
                        this.preAuthStatus = this.currentRole.AUTH_STATUS;
                        this.currentRole.AUTH_STATUS = "U";

                        ObservableCollection<TL_SYSROLEDETAIL> lstRoleDetailUpdate = new ObservableCollection<TL_SYSROLEDETAIL>();
                        ObservableCollection<TL_SYSROLEDETAIL> lstRoleDetailInsert = new ObservableCollection<TL_SYSROLEDETAIL>();
                        ActionMenuRole actionRole;
                        TL_SYSROLEDETAIL newItemRoleDetail;
                        bool? isNull = true;
                        foreach (var key in this.dicCheckBoxItem.Keys)
                        {
                            isNull = this.dicCheckBoxItem[key].IsChecked;
                            if (isNull == null || isNull.Value)
                            {
                                actionRole = this.dicMenuRole[key];
                                newItemRoleDetail = new TL_SYSROLEDETAIL()
                                {
                                    ISAPPROVE = actionRole.Approve,
                                    ISCLOSE = actionRole.Close,
                                    ISDELETE = actionRole.Delete,
                                    ISEDIT = actionRole.Edit,
                                    ISINSERT = actionRole.Insert,
                                    ISSEARCH = actionRole.Search,
                                    ISUPDATE = actionRole.Update,
                                    ISVIEW = actionRole.View,
                                    MENU_ID = key,
                                    ROLE_ID = this.currentRole.ROLE_ID,
                                    ISCHECKNULL = (isNull == null ? true : false)
                                };
                                if (this.lstInsertRoleDetail.Contains(key))
                                    lstRoleDetailInsert.Add(newItemRoleDetail);
                                else
                                    lstRoleDetailUpdate.Add(newItemRoleDetail);
                            }
                        }
                        MyHelper.IsBusy();
                        this.roleClient.UpdateTLSYSROLEAsync(this.currentRole, lstRoleDetailUpdate, lstRoleDetailInsert, lstDeleteRoleDetail);
                    }
                    else
                    {
                        ObservableCollection<TL_SYSROLEDETAIL> lstRoleDetail = new ObservableCollection<TL_SYSROLEDETAIL>();
                        ActionMenuRole actionRole;
                        bool? isNull = true;
                        foreach (var key in this.dicCheckBoxItem.Keys)
                        {
                            isNull = this.dicCheckBoxItem[key].IsChecked;
                            if (isNull == null || isNull.Value)
                            {                                
                                actionRole = this.dicMenuRole[key];
                                lstRoleDetail.Add(new TL_SYSROLEDETAIL()
                                {
                                    ISAPPROVE = actionRole.Approve,
                                    ISCLOSE = actionRole.Close,
                                    ISDELETE = actionRole.Delete,
                                    ISEDIT = actionRole.Edit,
                                    ISINSERT = actionRole.Insert,
                                    ISSEARCH = actionRole.Search,
                                    ISUPDATE = actionRole.Update,
                                    ISVIEW = actionRole.View,
                                    MENU_ID = key,
                                    ROLE_ID = this.currentRole.ROLE_ID,
                                    ISCHECKNULL = (isNull == null ? true : false)
                                });
                            }
                        }
                        MyHelper.IsBusy();
                        this.roleClient.InsertTLSYSROLEAsync(this.currentRole, lstRoleDetail);
                    }
                    //    this.roleClient.InsertTLSYSROLEAsync(this.currentRole);
                }
            }
            catch (Exception ex)
            {
                this.messagePop.SetSingleError(ex.Message);
            }
        }

        private void CancelAction()
        {            
            Messenger.Default.Send(new ParentMessage() { currentObject = null });
        }

        private void Approve()
        {
            this.messagePop.Reset();
            if (this.currentRole.AUTH_STATUS.Equals("A")) return;
            if (this.currentRole.MAKER_ID.Equals(CurrentSystemLogin.CurrentUser.TLNANME))
            {
                this.messagePop.SetSingleError(ValidatorResource.lblErrorPermission);
                return;
            }
            this.currentRole.CHECKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
            this.currentRole.DATE_APPROVE = DateTime.Now;
            //this.menuClient.ApproveCMAGENTAsync(this.currentMenu);
        }        

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.currentRole.ROLE_ID.Equals(""))
                this.messagePop.SetError(SystemRoleResource.ROLE_ID + notEmpty);
            if (this.currentRole.ROLE_DESC.Equals(""))
                this.messagePop.SetError(SystemRoleResource.ROLE_DESC + notEmpty);                       
        }
        private void Load()
        {
            try
            {
                this.roleClient = new TLSYSROLEClient();

                TLMENUClient menuClient = new TLMENUClient();
                menuClient.GetByTopTLMENUAsync("", "", " MENU_PARENT, MENU_ORDER ");
                menuClient.GetByTopTLMENUCompleted += new EventHandler<GetByTopTLMENUCompletedEventArgs>(getMenuCompleted);

                this.roleClient.InsertTLSYSROLECompleted += new EventHandler<InsertTLSYSROLECompletedEventArgs>(insertCurrencyCompleted);
                this.roleClient.UpdateTLSYSROLECompleted += new EventHandler<UpdateTLSYSROLECompletedEventArgs>(updateCurrencyCompleted);
                //this.menuClient.ApproveCMAGENTCompleted += new EventHandler<ApproveCMAGENTCompletedEventArgs>(approveComplete);
                this.roleClient.GetByTopTLROLEDETAILCompleted += new EventHandler<GetByTopTLROLEDETAILCompletedEventArgs>(getRoleDetailCompleted);
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void getRoleDetailCompleted(object sender, GetByTopTLROLEDETAILCompletedEventArgs e)
        {
            this.currentRoleDetailData = e.Result;
            if (!this.isFirstTime)
            {
                this.isLoading=true;
                this.LoadMenuRole(this.currentMenuData);
                this.LoadEditRole();
                this.isLoading=false;
            }
        }

        private void getMenuCompleted(object sender, GetByTopTLMENUCompletedEventArgs e)
        {
            this.currentMenuData = e.Result;
            this.isLoading=true;
            this.LoadMenuRole(this.currentMenuData);
            if (this.isFirstTime)
            {
                this.LoadEditRole();
                this.isFirstTime = false;
            }
            this.isLoading=false;

        }

        private void LoadEditRole()
        {
            //string error = "";
            try
            {
                this.lstRootRoleDetail = new List<string>();
                this.lstInsertRoleDetail = new List<string>();
                this.lstDeleteRoleDetail = new ObservableCollection<string>();

                if (this.currentRoleDetailData != null && this.currentRoleDetailData.Count > 0)
                {
                    foreach (var item in this.currentRoleDetailData)
                    {
                        //error = item.MENU_ID;
                        this.dicMenuRole[item.MENU_ID] = new ActionMenuRole(new ActionMenuRole()
                        {
                            View = item.ISVIEW,
                            Update = item.ISUPDATE,
                            Search = item.ISSEARCH,
                            Insert = item.ISINSERT,
                            Edit = item.ISEDIT,
                            Delete = item.ISDELETE,
                            Close = item.ISCLOSE,
                            Approve = item.ISAPPROVE
                        });
                        if (item.ISCHECKNULL.Value)
                            this.dicCheckBoxItem[item.MENU_ID].IsChecked = null;
                        else
                            this.dicCheckBoxItem[item.MENU_ID].IsChecked = true;

                        this.lstRootRoleDetail.Add(item.MENU_ID);
                    }

                    this.OnPropertyChanged("MenuData");
                }
            }
            catch (Exception ex)
            {
                this.messagePop.SetSingleError(ex.Message );
            }
        }

        private void LoadMenuRole(ObservableCollection<TL_MENU> data)
        {
            if (data != null && data.Count > 0)
            {

                List<TreeViewItem> menuDataTemp = new List<TreeViewItem>();
                this.dicCheckBoxItem = new Dictionary<string, CheckBox>();

                ObservableCollection<TL_MENU> lstMenu = data;
                Dictionary<string, List<TreeViewItem>> dicAccParent = new Dictionary<string, List<TreeViewItem>>();
                Dictionary<string, string> dicParentName = new Dictionary<string, string>();
                this.dicMenuRole = new Dictionary<string, ActionMenuRole>();

                string menuName = "";
                foreach (var item in lstMenu)
                {
                    menuName = this.isVN ? item.MENU_NAME : item.MENU_NAME_EL;
                    if (item.MENU_PARENT.Equals(""))
                    {
                        dicAccParent.Add(item.MENU_ID.ToString(), new List<TreeViewItem>());
                        dicParentName.Add(item.MENU_ID.ToString(), menuName);
                    }
                    else
                    {
                        dicAccParent[item.MENU_PARENT].Add(CreateItem(item.MENU_ID.ToString(), menuName, null));
                    }
                    this.dicMenuRole.Add(item.MENU_ID.ToString(), new ActionMenuRole());
                }

                foreach (var key in dicAccParent.Keys)
                    menuDataTemp.Add(CreateItem(key, dicParentName[key], dicAccParent[key]));

                this.MenuData = menuDataTemp;
            }
        }

        private TreeViewItem CreateItem(string menuId, string header, List<TreeViewItem> itemSource)
        {
            TreeViewItem item = new TreeViewItem();
            StackPanel spHeader = new StackPanel();
            spHeader.Orientation = Orientation.Horizontal;

            CheckBox chbItem = new CheckBox();
            chbItem.Margin = new Thickness(2);
            //chbItem.Content = header;
            chbItem.Checked += new RoutedEventHandler(checkItem);
            chbItem.Unchecked += new RoutedEventHandler(uncheckItem);
            if (itemSource != null)
            {
                chbItem.IsThreeState = true;
            }
            spHeader.Children.Add(chbItem);
            //Luu lai checkbox de xu ly de hon
            this.dicCheckBoxItem.Add(menuId, chbItem);
            TextBlock txbContent = new TextBlock();
            txbContent.Text = header;
            //txbContent.Margin = new Thickness();
            txbContent.HorizontalAlignment = HorizontalAlignment.Left;
            txbContent.VerticalAlignment = VerticalAlignment.Center;
            spHeader.Children.Add(txbContent);

            item.Header = spHeader;
            item.TabIndex = int.Parse(menuId);
            item.ItemsSource = itemSource;
            item.Selected += new RoutedEventHandler(selectItem);
            return item;
        }

        private void selectItem(object sender, RoutedEventArgs e)
        {
            this.currentItem = (sender as TreeViewItem);
            this.currentParentItem = this.currentItem.GetParentTreeViewItem();
            this.CurrentMenuRole = this.dicMenuRole[this.currentItem.TabIndex.ToString()];
        }

        private void checkItem(object sender, RoutedEventArgs e)
        {
            if (!this.isLoading && this.isEdit)
            {
                string menuId = this.currentItem.TabIndex.ToString();
                if (!this.lstRootRoleDetail.Contains(menuId))
                    this.lstInsertRoleDetail.Add(menuId);
                else
                    this.lstDeleteRoleDetail.Remove(menuId);
            }

            if (this.currentItem != null && this.currentItem.ItemsSource != null)
            {
                List<TreeViewItem> lstItem = this.currentItem.ItemsSource as List<TreeViewItem>;
                foreach (var item in lstItem)
                {
                    ((item.Header as StackPanel).Children[0] as CheckBox).IsChecked = true;
                    this.dicMenuRole[item.TabIndex.ToString()] = new ActionMenuRole(this.currentMenuRole);
                    if (!this.isLoading && this.isEdit)
                    {
                        string menuId = item.TabIndex.ToString();
                        if (!this.lstRootRoleDetail.Contains(menuId) && !this.lstInsertRoleDetail.Contains(menuId))
                            this.lstInsertRoleDetail.Add(menuId);
                        else if (this.lstDeleteRoleDetail.Contains(menuId))
                            this.lstDeleteRoleDetail.Remove(menuId);
                    }
                }
            }
            else if (this.currentParentItem != null)
            {
                List<TreeViewItem> lstItem = this.currentParentItem.ItemsSource as List<TreeViewItem>;
                foreach (var item in lstItem)
                {
                    if (!((item.Header as StackPanel).Children[0] as CheckBox).IsChecked.Value)
                    {
                        ((this.currentParentItem.Header as StackPanel).Children[0] as CheckBox).IsChecked = null;
                        if (!this.isLoading && this.isEdit)
                        {
                            string menuId = this.currentParentItem.TabIndex.ToString();
                            if (!this.lstRootRoleDetail.Contains(menuId) && !this.lstInsertRoleDetail.Contains(menuId))
                                this.lstInsertRoleDetail.Add(menuId);
                            else if (this.lstDeleteRoleDetail.Contains(menuId))
                                this.lstDeleteRoleDetail.Remove(menuId);
                        }
                        return;
                    }
                }

                ((this.currentParentItem.Header as StackPanel).Children[0] as CheckBox).IsChecked = true;

                if (!this.isLoading && this.isEdit)
                {
                    string menuId = this.currentParentItem.TabIndex.ToString();
                    if (!this.lstRootRoleDetail.Contains(menuId) && !this.lstInsertRoleDetail.Contains(menuId))
                        this.lstInsertRoleDetail.Add(menuId);
                    else if (this.lstDeleteRoleDetail.Contains(menuId))
                        this.lstDeleteRoleDetail.Remove(menuId);
                }
            }
        }

        private void uncheckItem(object sender, RoutedEventArgs e)
        {
            if (!this.isLoading && this.isEdit)
            {
                string menuId = this.currentItem.TabIndex.ToString();
                if (this.lstRootRoleDetail.Contains(menuId))
                    this.lstDeleteRoleDetail.Add(menuId);
                else
                    this.lstInsertRoleDetail.Remove(menuId);
            }

            if (this.currentItem != null && this.currentItem.ItemsSource != null)
            {
                List<TreeViewItem> lstItem = this.currentItem.ItemsSource as List<TreeViewItem>;
                foreach (var item in lstItem)
                {
                    ((item.Header as StackPanel).Children[0] as CheckBox).IsChecked = false;
                    if (!this.isLoading && this.isEdit)
                    {
                        string menuId = item.TabIndex.ToString();
                        if (this.lstRootRoleDetail.Contains(menuId) && !this.lstDeleteRoleDetail.Contains(menuId))
                            this.lstDeleteRoleDetail.Add(menuId);
                        else if (this.lstInsertRoleDetail.Contains(menuId))
                            this.lstInsertRoleDetail.Remove(menuId);
                    }
                }
            }
            else if (this.currentParentItem != null)
            {
                List<TreeViewItem> lstItem = this.currentParentItem.ItemsSource as List<TreeViewItem>;
                foreach (var item in lstItem)
                {
                    if (((item.Header as StackPanel).Children[0] as CheckBox).IsChecked.Value)
                    {
                        ((this.currentParentItem.Header as StackPanel).Children[0] as CheckBox).IsChecked = null;                        
                        return;
                    }
                }

                ((this.currentParentItem.Header as StackPanel).Children[0] as CheckBox).IsChecked = false;
                if (!this.isLoading && this.isEdit)
                {
                    string menuId = this.currentParentItem.TabIndex.ToString();
                    if (this.lstRootRoleDetail.Contains(menuId) && !this.lstDeleteRoleDetail.Contains(menuId))
                        this.lstDeleteRoleDetail.Add(menuId);
                    else if (this.lstInsertRoleDetail.Contains(menuId))
                        this.lstInsertRoleDetail.Remove(menuId);
                }
            }

            
        }


        //private void approveComplete(object sender, ApproveCMAGENTCompletedEventArgs e)
        //{
        //    if (e.Result)
        //    {
        //        this.currentAgent.AUTH_STATUS = "A";
        //        this.currentAgent.ISAPPROVE = "1";
        //        this.IsApproved = Visibility.Visible.ToString();
        //        this.OnPropertyChanged("IsApproved");
        //        this.messagePop.Successful(ValidatorResource.SuccessfulApprove);
        //    }
        //    else
        //        this.messagePop.SetSingleError(ValidatorResource.ErrorApprove);
        //}

        private void updateCurrencyCompleted(object sender, UpdateTLSYSROLECompletedEventArgs e)
        {
            try
            {
                if (e.Result)
                {
                    this.IsApproved = Visibility.Collapsed.ToString();
                    this.OnPropertyChanged("IsApproved");
                    this.messagePop.Successful(ValidatorResource.UpdateSuccessful);
                }
                else
                {
                    this.currentRole.AUTH_STATUS = this.preAuthStatus;
                    this.messagePop.SetSingleError(ValidatorResource.ErrorUpdate);
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

        private void insertCurrencyCompleted(object sender, InsertTLSYSROLECompletedEventArgs e)
        {
            try
            {
                if (e.Result.Equals("True"))
                {
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { isEdit = this.isEdit, currentObject = this.currentRole });
                }
                else
                {
                    if (MyHelper.ListMessage.ContainsKey(e.Result))
                        this.messagePop.SetSingleError(MyHelper.ListMessage[e.Result]);
                    else
                        this.messagePop.SetSingleError(ValidatorResource.ErrorInsert + "\n" + e.Result);
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

            private RolesEditViewModel viewModel;
            public ActionButton(RolesEditViewModel viewModel)
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

        public class ActionMenuRole : ViewModelBase
        {
            public ActionMenuRole()
            {
                this.Insert = false;
                this.Update = false;
                this.View = true;
                this.Edit = false;
                this.Delete = false;
                this.Search = true;
                this.Approve = false;
                this.Close = false;
            }

            public ActionMenuRole(ActionMenuRole item)
            {
                this.Insert = item.Insert;
                this.Update = item.Update;
                this.View = item.View;
                this.Edit = item.Edit;
                this.Delete = item.Delete;
                this.Search = item.Search;
                this.Approve = item.Approve;
                this.Close = item.Close;
            }

            public bool Insert { get; set; }
            public bool Update { get; set; }
            public bool Edit { get; set; }
            public bool Delete { get; set; }
            public bool View { get; set; }
            public bool Search { get; set; }
            public bool Approve { get; set; }
            public bool Close { get; set; }
        }
    }
}
