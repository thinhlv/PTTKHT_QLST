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
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using gMVVM.Views.AssCommon;
using mvvmCommon;

namespace gMVVM.ViewModels.AssCommon
{
    public class ArgumentViewModel:ViewModelBase
    {
          private ARGUMENTClient argumentClient;
          public ArgumentViewModel()
        {
            this.Reload();

            this.actionButton = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            ActionMenuButton.actionControl.SetAllAction(null, null, actionButton, actionButton, actionButton, actionButton, null);
            this.DoubleClickItemCommand = new RelayCommand(DoubleClickItem);

            Messenger.Reset();
            Messenger.Default.Register<ParentMessage>(this, CurrencyEditResponseMessageReceived);
        }

        //save data from database
        private ObservableCollection<SYS_PARAMETER> currentData;
        private ArgumentEdit editChild;
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

        private SYS_PARAMETER currentSelectItem;
        public SYS_PARAMETER CurrentSelectItem
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

        private string arg_Code = "";
        public string Arg_Code
        {
            get
            {
                return this.arg_Code;
            }

            set
            {
                this.arg_Code = value;
                this.OnPropertyChanged("Arg_Code");
            }
        }

        private string arg_Name = "";
        public string Arg_Name
        {
            get
            {
                return this.arg_Name;
            }

            set
            {
                this.arg_Name = value;
                this.OnPropertyChanged("Arg_Name");
            }
        }
        private string dataType = "";
        public string DataType
        {
            get
            {
                return this.dataType;
            }

            set
            {
                this.dataType = value;
                this.OnPropertyChanged("DataType");
            }
        }
        //

        public ICommand DoubleClickItemCommand { get; private set; }

        #endregion

        #region[Action Functions]

        private void Insert()
        {
            if (this.editChild == null)
            {
                this.editChild = new ArgumentEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = null, isEdit = false, action = ActionMenuButton.Insert, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Edit()
        {
            if (this.editChild == null)
            {
                this.editChild = new ArgumentEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem, isEdit = true, action = ActionMenuButton.Edit, level = "Main" });
            PageAnimation.ToBack();
        }

        private void View()
        {
            if (this.editChild == null)
            {
                this.editChild = new ArgumentEdit();
                PageAnimation.back.Children.Add(this.editChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem, isEdit = false, action = ActionMenuButton.View, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Delete()
        {
            this.messagePop.Reset();
            if (this.currentSelectItem.AUTH_STATUS.Equals("A")) this.messagePop.SetError(ValidatorResource.lblErrorDeletePer);
            if (this.messagePop.HasError()) return;
            else
            if (MessageBox.Show(CommonResource.msgDelete, CommonResource.btnDelete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                //call delete item
                MyHelper.IsBusy();
                this.argumentClient.DeleteArgumentAsync(this.currentSelectItem.ParaKey);
            }

        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                string where = " ParaKey like '%" + (this.arg_Code.Equals("") ? "%" : this.arg_Code) + "%' "
                    + " and [ParaValue] like N'%" + (this.arg_Name.Equals("") ? "%" : this.arg_Name) + "%' " + " and [DataType] like N'%" + (this.dataType.Equals("") ? "%" : this.dataType) + "%' ";
                MyHelper.IsBusy();
                this.argumentClient.GetByTopArgumentAsync("200", where, "");
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
                this.currentData.Add(obj.currentObject as SYS_PARAMETER);
            }
            else
            {
                this.messagePop.Reset();
                ActionMenuButton.actionControl.SetAllAction(null, null, actionButton, actionButton, actionButton, actionButton, null);
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
                this.currentData = new ObservableCollection<SYS_PARAMETER>();
                this.argumentClient = new ARGUMENTClient();       
                this.argumentClient.GetByTopArgumentCompleted += new EventHandler<GetByTopArgumentCompletedEventArgs>(searchCompleted);
                this.argumentClient.DeleteArgumentCompleted += new EventHandler<DeleteArgumentCompletedEventArgs>(deleteCompleted);

            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void deleteCompleted(object sender, DeleteArgumentCompletedEventArgs e)
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

        private void searchCompleted(object sender, GetByTopArgumentCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.currentData = e.Result;
                else
                    this.currentData = new ObservableCollection<SYS_PARAMETER>();
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
            private ArgumentViewModel viewModel;
            public ActionButton(ArgumentViewModel viewModel)
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
