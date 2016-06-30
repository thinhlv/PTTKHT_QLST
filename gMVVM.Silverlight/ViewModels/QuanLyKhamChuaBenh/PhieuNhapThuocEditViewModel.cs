using gMVVM.gMVVMService;
using gMVVM.ViewModels.Common;
using gMVVM.CommonClass;
using gMVVM.Resources;
using GalaSoft.MvvmLight.Messaging;
using mvvmCommon;
using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM.ViewModels.QuanLyKhamChuaBenh
{
    public class PhieuNhapThuocEditViewModel : ViewModelBase
    {
        ZOO_PhieuNhapThuocClient phieuNhapThuocClient;
        public PhieuNhapThuocEditViewModel()
        {
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
                if (this.isEdit)
                {
                    this.HeaderText = CommonResource.lblEdit;
                    this.IconPath = DefineClass.IconEdit;
                }
                else
                {
                    this.HeaderText = CommonResource.lblInsert;
                    this.IconPath = DefineClass.IconInsert;
                }
                //this.HeaderText = this.isEdit ? CommonResource.lblEdit : CommonResource.lblInsert;
                this.IsView = true;
            }

            this.messagePop.Reset();
            this.CurrentPhieuNhapThuoc = obj.currentObject != null ? (obj.currentObject as ZOO_PHIEUNHAPTHUOC)
                : new ZOO_PHIEUNHAPTHUOC()
                {
                    MaThuoc = "",
                    SoLuong = 0,
                    NgayNhap = DateTime.Now,
                    MaPhieuNhap = "",

                    RECORD_STATUS = "1",
                    AUTH_STATUS = "U",
                    MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                };

            if (this.currentPhieuNhapThuoc.AUTH_STATUS.Equals("A"))
                this.IsApproved = Visibility.Visible.ToString();
            else
                this.IsApproved = Visibility.Collapsed.ToString();

            this.OnPropertyChanged("IsView");
            this.OnPropertyChanged("IsApproved");
            this.OnPropertyChanged("");
        }

        private string preAuthStatus = "";

        #region[All Properties]

        private object iconPath = "/gMVVM;component/Data/Icons/edit_icon.png";
        public object IconPath
        {
            get
            {
                return this.iconPath;
            }
            set
            {
                this.iconPath = value;
                this.OnPropertyChanged("IconPath");
            }
        }

        public bool IsView { get; set; }

        public string IsApproved { get; set; }
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
        private ZOO_PHIEUNHAPTHUOC currentPhieuNhapThuoc;
        public ZOO_PHIEUNHAPTHUOC CurrentPhieuNhapThuoc
        {
            get
            {
                return this.currentPhieuNhapThuoc;
            }

            set
            {
                this.currentPhieuNhapThuoc = value;
                this.OnPropertyChanged("CurrentPhieuNhapThuoc");
            }
        }


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

        //so luong
        private string soLuong;
        public string SoLuong
        {
            get
            {
                return this.soLuong;
            }

            set
            {
                this.soLuong = value;
                this.OnPropertyChanged("SoLuong");
            }
        }

        //ghi chu
        private string ghiChu;
        public string GhiChu
        {
            get
            {
                return this.ghiChu;
            }

            set
            {
                this.ghiChu = value;
                this.OnPropertyChanged("GhiChu");
            }
        }

        private string danhSachThuocSelected = "";
        public string DanhSachThuocSelected
        {
            get
            {
                return this.danhSachThuocSelected;
            }

            set
            {
                this.danhSachThuocSelected = value;
                this.OnPropertyChanged("DanhSachThuocSelected");
            }
        }
        //ngay nhap

        private Nullable<DateTime> ngayNhap = null;
        public Nullable<DateTime> NgayNhap
        {
            get
            {
                if (ngayNhap == null)
                {
                    ngayNhap = DateTime.Today;
                }
                return ngayNhap;
            }
            set
            {
                ngayNhap = value;
                OnPropertyChanged("NgayNhap");
            }
        }

        //combobox danh sach thuoc
        private ObservableCollection<ZOO_THUOC> danhSachThuoc;
        public ObservableCollection<ZOO_THUOC> DanhSachThuoc
        {
            get
            {
                return this.danhSachThuoc;
            }

            set
            {
                this.danhSachThuoc = value;
                this.OnPropertyChanged("DanhSachThuoc");
            }
        }

        #endregion

        #region[Action Functions]

        private void OkAction()
        {
            RefreshValidator();
            if (!this.messagePop.HasError())
            {

                if (this.isEdit)
                {
                    this.currentPhieuNhapThuoc.MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
                    this.preAuthStatus = this.currentPhieuNhapThuoc.AUTH_STATUS;
                    this.currentPhieuNhapThuoc.AUTH_STATUS = "U";
                    MyHelper.IsBusy();
                    //this.phieuNhapThuocClient.UpdatePhieuNhapThuoc(this.currentPhieuNhapThuoc);
                }
                else
                {
                    MyHelper.IsBusy();
                    this.phieuNhapThuocClient.LuuPhieuNhapAsync(this.currentPhieuNhapThuoc);
                }
            }
        }

        private void CancelAction()
        {
            Messenger.Default.Send(new ParentMessage() { currentObject = null });
        }

        private void Approve()
        {
            this.messagePop.Reset();
            if (this.currentPhieuNhapThuoc.AUTH_STATUS.Equals("A")) return;
            if (this.currentPhieuNhapThuoc.MAKER_ID.Equals(CurrentSystemLogin.CurrentUser.TLNANME))
            {
                this.messagePop.SetSingleError(ValidatorResource.lblErrorPermission);
                return;
            }
            this.currentPhieuNhapThuoc.CHECKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
            this.currentPhieuNhapThuoc.APPROVE_DT = DateTime.Now;
            //this.menuClient.ApproveCMAGENTAsync(this.currentMenu);
        }

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.currentPhieuNhapThuoc.MaThuoc.Equals(""))
                //this.messagePop.SetError(SystemRoleResource.MENU_NAME + notEmpty);
                this.messagePop.SetError("Mã thuốc" + notEmpty);
            if (this.currentPhieuNhapThuoc.SoLuong.Equals(""))
                //this.messagePop.SetError(SystemRoleResource.MENU_NAME_EL + notEmpty);
                this.messagePop.SetError("Số lượng" + notEmpty);
        }
        private void Load()
        {
            try
            {
                this.phieuNhapThuocClient = new ZOO_PhieuNhapThuocClient();

                this.phieuNhapThuocClient.DanhSachThuocAsync();
                this.phieuNhapThuocClient.DanhSachThuocCompleted += (s, e) =>
                {
                    if(danhSachThuoc == null)
                        this.danhSachThuoc = new ObservableCollection<ZOO_THUOC>();
                    danhSachThuoc.Clear();
                    this.danhSachThuoc = e.Result;
                    this.OnPropertyChanged("DanhSachThuoc");
                };

                this.phieuNhapThuocClient.LuuPhieuNhapCompleted += new EventHandler<LuuPhieuNhapCompletedEventArgs>(luuPhieuNhapComplete);
                //this.phieuNhapThuocClient. += new EventHandler<UpdateTLMENUCompletedEventArgs>(updateCurrencyCompleted);
                //this.menuClient.ApproveCMAGENTCompleted += new EventHandler<ApproveCMAGENTCompletedEventArgs>(approveComplete);

            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
            }
        }

        //private void updateCurrencyCompleted(object sender, UpdateTLMENUCompletedEventArgs e)
        //{
        //    try
        //    {
        //        if (e.Result)
        //        {
        //            this.IsApproved = Visibility.Collapsed.ToString();
        //            this.OnPropertyChanged("IsApproved");
        //            this.messagePop.Successful(ValidatorResource.UpdateSuccessful);
        //        }
        //        else
        //        {
        //            this.currentMenu.AUTH_STATUS = this.preAuthStatus;
        //            this.messagePop.SetSingleError(ValidatorResource.ErrorUpdate);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
        //    }
        //    finally
        //    {
        //        MyHelper.IsFree();
        //    }
        //}

        private void luuPhieuNhapComplete(object sender, LuuPhieuNhapCompletedEventArgs e)
        {
            try
            {
                if (!e.Result.Equals(""))
                {
                    this.currentPhieuNhapThuoc.MaPhieuNhap = e.Result.MaPhieuNhap;
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { isEdit = this.isEdit, currentObject = this.currentPhieuNhapThuoc });

                }
                else
                {
                    this.messagePop.SetSingleError(ValidatorResource.ErrorInsert);
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

            private PhieuNhapThuocEditViewModel viewModel;
            public ActionButton(PhieuNhapThuocEditViewModel viewModel)
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
    }
}
