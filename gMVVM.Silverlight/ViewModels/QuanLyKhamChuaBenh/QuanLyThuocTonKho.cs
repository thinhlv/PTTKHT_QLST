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
using gMVVM.QLSoThu;

namespace gMVVM.ViewModels.QuanLyKhamChuaBenh
{
    public class QuanLyThuocTonKho : ViewModelBase
    {
        private ZOO_BaoCaoTonKhoClient baoCaoTonKhoClient;
        List<DanhSachTonKhoGrid> danhsach;
        public QuanLyThuocTonKho()
        {
            this.Reload();

            this.actionButton = new ActionButton(this);
            this.messagePop = new MessageAlarmViewModel();
            ActionMenuButton.actionControl.SetAllAction(actionButton, null, actionButton, actionButton, actionButton, actionButton, null);
            this.DoubleClickItemCommand = new RelayCommand(DoubleClickItem);

            Messenger.Reset();
        }

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

        //DatePicjer
        private Nullable<DateTime> ngayHetHan = null;
        public Nullable<DateTime> NgayHetHan
        {
            get
            {
                if (ngayHetHan == null)
                {
                    ngayHetHan = DateTime.Today;
                }
                return ngayHetHan;
            }
            set
            {
                ngayHetHan = value;
                OnPropertyChanged("NgayHetHan");
            }
        }

        private Nullable<DateTime> ngayHienTai = null;
        public Nullable<DateTime> NgayHienTai
        {
            get
            {
                if (ngayHienTai == null)
                {
                    ngayHienTai = DateTime.Today;
                }
                return ngayHienTai;
            }
            set
            {
                ngayHienTai = value;
                OnPropertyChanged("{");
            }
        }

        private ObservableCollection<ZOO_BAOCAOTONKHO_SearchResult> danhSachTonKho;
        public ObservableCollection<ZOO_BAOCAOTONKHO_SearchResult> DanhSachTonKho
        {
            get
            {
                return danhSachTonKho;
            }
            set
            {
                danhSachTonKho = value;
                OnPropertyChanged("DanhSachTonKho");
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

        private string tenThuoc = "";
        public string TenThuoc
        {
            get
            {
                return tenThuoc;
            }
            set
            {
                tenThuoc = value;
                OnPropertyChanged("TenThuoc");
            }
        }

        private string maThuoc = "";
        public string MaThuoc
        {
            get
            {
                return maThuoc;
            }
            set
            {
                maThuoc = value;
                OnPropertyChanged("MaThuoc");
            }
        }

        //Code catch Icommand Click action.
        public ICommand DoubleClickItemCommand { get; private set; }

        public ICommand ActionCommand
        {
            get
            {
                return this.actionButton;
            }
        }

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
            try
            {
                this.messagePop.Reset();
                MyHelper.IsBusy();
                this.baoCaoTonKhoClient.LayBaoCaoTonKhoAsync(maThuoc, tenThuoc, NgayHetHan.Value);
            }
            catch (Exception)
            { }
        }
        /*
        //Crystal report
        private gMVVM.Views.Statistics.POST.MyPost post;
        private void ViewReport()
        {
            if (reportKindSelected.Equals("0"))
            {
                //Validate
                this.messagePop.Reset();
                post = new gMVVM.Views.Statistics.POST.MyPost();
                post.ReportName = "PL_G7_GhiNhanDoanhThu.rpt";
                post.StoreName = "PL_DOANHTHU";
                post.HasValue = true;
                post.HasParameter = true;

                post.ParametersField.Add("branchID", CurrentSystemLogin.CurrentUser.BRANCH_CODE);
                post.ParametersField.Add("brandName", CurrentSystemLogin.CurrentUser.BRANCH_NAME);

                post.Values.Add("MaChiNhanh", branchSelected);
                post.Values.Add("Ngay_Phat_Sinh", fromDateSelected.Value.ToShortDateString());
                post.Values.Add("Ngay_Ket_Thuc", toDateSelected.Value.ToShortDateString());
                System.Windows.Browser.HtmlPage.Window.Invoke("showPopupReport", new string[] { post.ToString() });
            }
            else if (reportKindSelected.Equals("1"))
            {
                //Validate
                this.messagePop.Reset();
                post = new gMVVM.Views.Statistics.POST.MyPost();
                post.ReportName = "PL_G7_GhiNhanChiPhi.rpt";
                post.StoreName = "PL_CHIPHI";
                post.HasValue = true;
                post.HasParameter = true;

                post.ParametersField.Add("branchID", CurrentSystemLogin.CurrentUser.BRANCH_CODE);
                post.ParametersField.Add("brandName", CurrentSystemLogin.CurrentUser.BRANCH_NAME);

                post.Values.Add("MaChiNhanh", branchSelected);
                post.Values.Add("Ngay_Phat_Sinh", fromDateSelected.Value.ToShortDateString());
                post.Values.Add("Ngay_Ket_Thuc", toDateSelected.Value.ToShortDateString());
                System.Windows.Browser.HtmlPage.Window.Invoke("showPopupReport", new string[] { post.ToString() });
            }

        }*/

        #endregion

        #region[Functions]
        //refresh current data
        private void Refresh()
        {
        }

        //reload data from database
        private void Reload()
        {
            try
            {
                danhsach = new List<DanhSachTonKhoGrid>();
                this.NgayHetHan = DateTime.Now;
                baoCaoTonKhoClient = new ZOO_BaoCaoTonKhoClient();
                this.baoCaoTonKhoClient.LayBaoCaoTonKhoCompleted += new EventHandler<LayBaoCaoTonKhoCompletedEventArgs>(searchBaoCaoTonKhoComplete);
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        private void searchBaoCaoTonKhoComplete(object sender, LayBaoCaoTonKhoCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.danhSachTonKho = e.Result;
                else
                    this.danhSachTonKho = new ObservableCollection<ZOO_BAOCAOTONKHO_SearchResult>();

                danhsach.Clear();
                int itemIndex = 0;
                foreach (var item in this.danhSachTonKho)
                {
                    itemIndex++;
                    if(item.NgayHetHan.Value < DateTime.Now)
                    {
                        this.danhsach.Add(new DanhSachTonKhoGrid() { NgayHetHan = item.NgayHetHan.Value.ToShortDateString(), ItemContent = item, GhiChu = "Hết hạn"});
                    }
                    else
                    {
                        this.danhsach.Add(new DanhSachTonKhoGrid() { NgayHetHan = item.NgayHetHan.Value.ToShortDateString(), ItemContent = item, GhiChu = ""});
                    }
                    
                }
                this.DataItem = new PagedCollectionView(danhsach);
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
            private QuanLyThuocTonKho viewModel;
            public ActionButton(QuanLyThuocTonKho viewModel)
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
                    //case "ViewReport": this.viewModel.ViewReport();
                    //    break;
                    default: break;
                }
            }
        }

        public class DanhSachTonKhoGrid : ViewModelBase
        {
            public string NgayHetHan { get; set; }
            public int ID { get; set; }
            public string GhiChu { get; set; }
            private ZOO_BAOCAOTONKHO_SearchResult itemContent;
            public ZOO_BAOCAOTONKHO_SearchResult ItemContent
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
