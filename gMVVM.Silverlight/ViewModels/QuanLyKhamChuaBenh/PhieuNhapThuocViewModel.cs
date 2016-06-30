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

namespace gMVVM.ViewModels.QuanLyKhamChuaBenh
{
    public class PhieuNhapThuocViewModel : ViewModelBase
    {
        
        private ZOO_PhieuNhapThuocClient phieuNhapThuocClient;
        public PhieuNhapThuocViewModel()
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
        private ObservableCollection<ZOO_PHIEUNHAPTHUOC> DANHSACHPHIEUNHAP;
        private ObservableCollection<ZOO_PHIEUNHAPTHUOC_SearchResult> ketQuaTimKiem;
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

        private PagedCollectionView danhSachPhieuNhap;
        public PagedCollectionView DanhSachPhieuNhap
        {
            get
            {
                return this.danhSachPhieuNhap;
            }

            set
            {
                this.danhSachPhieuNhap = value;
                OnPropertyChanged("DanhSachPhieuNhap");
            }
        }

        private ZOO_PHIEUNHAPTHUOC currentSelectItem;
        public ZOO_PHIEUNHAPTHUOC CurrentSelectItem
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

        private string maPhieuNhap = "";
        public string MaPhieuNhap
        {
            get
            {
                return this.maPhieuNhap;
            }

            set
            {
                this.maPhieuNhap = value;
                this.OnPropertyChanged("MaPhieuNhap");
            }
        }

        private string tenThuoc = "";
        public string TenThuoc
        {
            get
            {
                return this.tenThuoc;
            }

            set
            {
                this.tenThuoc = value;
                this.OnPropertyChanged("TenThuoc");
            }
        }

        private Nullable<DateTime> ngayNhap = null;
        public Nullable<DateTime> NgayNhap
        {
            get
            {
                if (ngayNhap == null)
                {
                    ngayNhap = DateTime.Today.Add(new TimeSpan(1,12,00,00));
                }
                return ngayNhap;
            }
            set
            {
                ngayNhap = value;
                OnPropertyChanged("NgayNhap");
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
                //this.branchClient.DeleteBranchAsync(this.currentSelectItem.BRANCH_ID);
            }

        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                MyHelper.IsBusy();
                this.phieuNhapThuocClient.TimPhieuNhapAsync(maPhieuNhap, tenThuoc, ngayNhap.Value, 0);
                
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
                this.DANHSACHPHIEUNHAP.Add(obj.currentObject as ZOO_PHIEUNHAPTHUOC);
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
            List<DanhSachPhieuNhapGrid> listtemp = new List<DanhSachPhieuNhapGrid>();
            foreach (var item in ketQuaTimKiem)
            {
                listtemp.Add(new DanhSachPhieuNhapGrid() { ItemContent = item, NgayNhap = item.NgayNhap.Value.ToShortDateString() });
            }
            this.DanhSachPhieuNhap = new PagedCollectionView(listtemp);
            //DANHSACHPHIEUNHAP.Clear();
            //foreach (var item in ketQuaTimKiem)
            //{
            //    listtemp.Add(new DanhSachPhieuNhapGrid() { NgayNhap = item.NgayNhap.Value.ToShortDateString(), ItemContent = item });
            //    DANHSACHPHIEUNHAP.Add(new ZOO_PHIEUNHAPTHUOC()
            //    {
            //        MaPhieuNhap = item.MaPhieuNhap,
            //        MaThuoc = item.MaThuoc,
            //        SoLuong = item.SoLuong,
            //        NgayNhap = item.NgayNhap,
            //        NOTES = item.NOTES,
            //        RECORD_STATUS = item.RECORD_STATUS,
            //        MAKER_ID = item.MAKER_ID,
            //        CREATE_DT = item.CREATE_DT,
            //        AUTH_STATUS = item.AUTH_STATUS,
            //        CHECKER_ID = item.CHECKER_ID,
            //        APPROVE_DT = item.APPROVE_DT
            //    });
            //}

            
        }

        //reload data from database
        private void Reload()
        {
            try
            {
                this.phieuNhapThuocClient = new ZOO_PhieuNhapThuocClient();
                this.DANHSACHPHIEUNHAP = new ObservableCollection<ZOO_PHIEUNHAPTHUOC>();
                ketQuaTimKiem = new ObservableCollection<ZOO_PHIEUNHAPTHUOC_SearchResult>();

                this.phieuNhapThuocClient.TimPhieuNhapCompleted += new EventHandler<TimPhieuNhapCompletedEventArgs>(searchCompleted);
               // this.branchClient.DeleteBranchCompleted += new EventHandler<DeleteBranchCompletedEventArgs>(deleteCompleted);

            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void deleteCompleted(object sender, DeleteBranchCompletedEventArgs e)
        {
            //try
            //{
            //    //delete successful
            //    if (e.Result.Equals(ListMessage.True))
            //    {
            //        //remove on current CurrentData
            //        this.currentData.Remove(this.currentSelectItem);
            //        this.Refresh();
            //    }
            //    else
            //    {
            //        if (ListMessage.Message.ContainsKey(e.Result))
            //            this.messagePop.SetSingleError(CommonResource.errorDelete + "\n " + ListMessage.Message[e.Result]);
            //        else
            //            this.messagePop.SetSingleError(CommonResource.errorDelete + "\n " + e.Result);
            //    }
            //}
            //catch (Exception)
            //{
            //    this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            //}
            //finally
            //{
            //    MyHelper.IsFree();
            //}
        }

        private void searchCompleted(object sender, TimPhieuNhapCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                {
                    ketQuaTimKiem = e.Result;
                }
                else
                {
                    ketQuaTimKiem.Clear();
                }
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
            private PhieuNhapThuocViewModel viewModel;
            public ActionButton(PhieuNhapThuocViewModel viewModel)
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

        public class DanhSachPhieuNhapGrid : ViewModelBase
        {
            public string NgayNhap { get; set; }
            private ZOO_PHIEUNHAPTHUOC_SearchResult itemContent;
            public ZOO_PHIEUNHAPTHUOC_SearchResult ItemContent
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
