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
using gMVVM.Views.QuanLyKhamChuaBenh;
using System.Collections.Generic;
using gMVVM.QLSoThu;

namespace gMVVM.ViewModels.QuanLyKhamChuaBenh
{
    public class LoThuocViewModel : ViewModelBase
    {
        private ZOO_LoThuocClient loThuocClient;
        
        public LoThuocViewModel()
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

        //
        private ObservableCollection<ZOO_LOTHUOC> DANHSACHLOTHUOC;
        private ObservableCollection<ZOO_LOTHUOC_SearchResult> ketQuaTimKiem;
        private List<DanhSachLoThuocGridData> listtemp = new List<DanhSachLoThuocGridData>();
        private LoThuocEdit LoThuocEditChild;

        //save data from database
        //private ObservableCollection<TL_SYSROLE> currencyData;
        //private RolesEdit PhieuNhapThuocEditChild;
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

        private PagedCollectionView danhSachLoThuoc;
        public PagedCollectionView DanhSachLoThuoc
        {
            get
            {
                return this.danhSachLoThuoc;
            }

            set
            {
                this.danhSachLoThuoc = value;
                OnPropertyChanged("DanhSachLoThuoc");
            }
        }

        private ZOO_LOTHUOC currentSelectItem;
        public ZOO_LOTHUOC CurrentSelectItem
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

        private DanhSachLoThuocGridData currentSelectItemGrid;
        public DanhSachLoThuocGridData CurrentSelectItemGrid
        {
            get
            {
                return this.currentSelectItemGrid;
            }

            set
            {
                this.currentSelectItemGrid = value;
                this.currentSelectItem = new ZOO_LOTHUOC()
                {
                    MaLo = value.ItemContent.MaLo,
                    MaThuoc = value.ItemContent.MaThuoc,
                    SoLo = value.ItemContent.SoLo,
                    NgaySanXuat = value.ItemContent.NgaySanXuat,
                    NgayHetHan = value.ItemContent.NgayHetHan,
                    SoLuong = value.ItemContent.SoLuong,

                    NOTES = value.ItemContent.NOTES,
                    RECORD_STATUS = value.ItemContent.RECORD_STATUS,
                    MAKER_ID = value.ItemContent.MAKER_ID,
                    CREATE_DT = value.ItemContent.CREATE_DT,
                    AUTH_STATUS = value.ItemContent.AUTH_STATUS,
                    CHECKER_ID = value.ItemContent.CHECKER_ID,
                    APPROVE_DT = value.ItemContent.APPROVE_DT
                };
                OnPropertyChanged("CurrentSelectItem");
                this.OnPropertyChanged("CurrentSelectItemGrid");
            }
        }

        private string soLo = "";
        public string SoLo
        {
            get
            {
                return this.soLo;
            }

            set
            {
                this.soLo = value;
                this.OnPropertyChanged("SoLo");
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

       
        public ICommand DoubleClickItemCommand { get; private set; }

        #endregion

        #region[Action Functions]        

        private void Insert()
        {
            if (this.LoThuocEditChild == null)
            {
                this.LoThuocEditChild = new LoThuocEdit();
                PageAnimation.back.Children.Add(this.LoThuocEditChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = null, isEdit = false, action = ActionMenuButton.Insert, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Edit()
        {
            if (this.LoThuocEditChild == null)
            {
                this.LoThuocEditChild = new LoThuocEdit();
                PageAnimation.back.Children.Add(this.LoThuocEditChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem, isEdit = true, action = ActionMenuButton.Edit, level = "Main" });
            PageAnimation.ToBack();
        }

        private void View()
        {
            if (this.LoThuocEditChild == null)
            {
                this.LoThuocEditChild = new LoThuocEdit();
                PageAnimation.back.Children.Add(this.LoThuocEditChild);
            }

            Messenger.Default.Send(new ChildMessage() { currentObject = this.currentSelectItem, isEdit = false, action = ActionMenuButton.View, level = "Main" });
            PageAnimation.ToBack();
        }

        private void Delete()
        {
//             this.messagePop.Reset();
//             if (this.currentSelectItem.ISAPPROVE.Equals("1"))
//             {
//                 this.messagePop.SetSingleError(CommonResource.lblIsUsing);
//                 return;
//             }
// 
//             if (MessageBox.Show(CommonResource.msgDelete, CommonResource.btnDelete, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
//             {
//                 //call delete item
//                 MyHelper.IsBusy();
//                 this.phieuNhapThuocClient.DeleteTLSYSROLEAsync(this.currentSelectItem);
//             }            
            
        }

        private void Search()
        {
            try
            {
                this.messagePop.Reset();
                MyHelper.IsBusy();
                this.loThuocClient.TimLoThuocAsync(soLo, tenThuoc);

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
                ZOO_LOTHUOC newitem = (obj.currentObject as ZOO_LOTHUOC);
                this.DANHSACHLOTHUOC.Add(newitem);
            }
            else
            {
                this.messagePop.Reset();
                ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
                PageAnimation.ToFront();
            }

        }

        private void searchCompleted(object sender, TimLoThuocCompletedEventArgs e)
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

        private void deleteCurrencyCompleted(object sender, DeleteTLSYSROLECompletedEventArgs e)
        {
            try
            {
                //delete successful
                if (e.Result)
                {
                    //remove on current DANHSACHPHIEUNHAP
                    this.DANHSACHLOTHUOC.Remove(this.currentSelectItem);
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
            
            DANHSACHLOTHUOC.Clear();
            listtemp.Clear();
            int i = 0;
            foreach (var item in ketQuaTimKiem)
            {
                i++;
                string ghichu = "";
                if (item.NgayHetHan.Value.CompareTo(DateTime.Now) <= 0)
                {
                    ghichu = "Hết hạn";
                }
                listtemp.Add(new DanhSachLoThuocGridData() { ItemContent = item, NgaySanXuat = item.NgaySanXuat.Value.ToShortDateString(), NgayHetHan = item.NgayHetHan.Value.ToShortDateString(), GhiChu = ghichu, ID = i});

                DANHSACHLOTHUOC.Add(new ZOO_LOTHUOC()
                {
                    MaLo = item.MaLo,
                    MaThuoc = item.MaThuoc,
                    NgayHetHan = item.NgayHetHan,
                    NgaySanXuat = item.NgaySanXuat,
                    SoLuong = item.SoLuong,
                    SoLo = item.SoLo,

                    NOTES = item.NOTES,
                    RECORD_STATUS = item.RECORD_STATUS,
                    MAKER_ID = item.MAKER_ID,
                    CREATE_DT = item.CREATE_DT,
                    AUTH_STATUS = item.AUTH_STATUS,
                    CHECKER_ID = item.CHECKER_ID,
                    APPROVE_DT = item.APPROVE_DT
                });
            }
            this.DanhSachLoThuoc = new PagedCollectionView(listtemp);
        }

        //reload data from database
        private void Reload()
        {
            try
            {
                this.loThuocClient = new ZOO_LoThuocClient();
                this.DANHSACHLOTHUOC = new ObservableCollection<ZOO_LOTHUOC>();
                ketQuaTimKiem = new ObservableCollection<ZOO_LOTHUOC_SearchResult>();

                this.loThuocClient.TimLoThuocCompleted += new EventHandler<TimLoThuocCompletedEventArgs>(searchCompleted);
                //this.phieuNhapThuocClient.DeleteTLSYSROLECompleted += new EventHandler<DeleteTLSYSROLECompletedEventArgs>(deleteCurrencyCompleted);
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
            private LoThuocViewModel viewModel;
            public ActionButton(LoThuocViewModel viewModel)
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

        public class DanhSachLoThuocGridData : ViewModelBase
        {
            public string NgaySanXuat { get; set; }
            public string NgayHetHan { get; set; }
            public string GhiChu { get; set; }
            public int ID { get; set; }
            private ZOO_LOTHUOC_SearchResult itemContent;
            public ZOO_LOTHUOC_SearchResult ItemContent
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
