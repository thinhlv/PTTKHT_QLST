using gMVVM.QLSoThu;
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
    public class LoThuocEditViewModel : ViewModelBase
    {
        //private TLMENUClient menuClient;
        private ZOO_PhieuNhapThuocClient phieuNhapClient;
        private ZOO_LoThuocClient loThuocClient;

        public LoThuocEditViewModel()
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
            this.currentLoThuoc = obj.currentObject != null ? (obj.currentObject as ZOO_LOTHUOC)
                : new ZOO_LOTHUOC()
                {
                    MaThuoc = "",
                    SoLuong = 0,
                    NgaySanXuat = DateTime.Now,
                    NgayHetHan = DateTime.Now,
                    MaLo = "",
                    SoLo = "",

                    RECORD_STATUS = "1",
                    AUTH_STATUS = "U",
                    MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                    NOTES = ""
                };

            if (this.currentLoThuoc.AUTH_STATUS.Equals("A"))
                this.IsApproved = Visibility.Visible.ToString();
            else
                this.IsApproved = Visibility.Collapsed.ToString();

            this.OnPropertyChanged("IsView");
            this.OnPropertyChanged("IsApproved");
            this.OnPropertyChanged("IsApproveFuntion");
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
        private ZOO_LOTHUOC currentLoThuoc;
        public ZOO_LOTHUOC CurrentLoThuoc
        {
            get
            {
                return this.currentLoThuoc;
            }

            set
            {
                this.currentLoThuoc = value;
                this.OnPropertyChanged("CurrentLoThuoc");
            }
        }

        private ObservableCollection<ZOO_THUOC> thuocCombobox;
        public ObservableCollection<ZOO_THUOC> ThuocCombobox
        {
            get
            {
                return this.thuocCombobox;
            }

            set
            {
                this.thuocCombobox = value;
                this.OnPropertyChanged("ThuocCombobox");
            }
        }

        private string thuocComboboxSelected;

        public string ThuocComboboxSelected
        {
            get 
            {
                return this.thuocComboboxSelected; 
            }
            set 
            {
                this.thuocComboboxSelected = value;
                this.OnPropertyChanged("ThuocComboboxSelected");
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

        //ngay san xuat

        private Nullable<DateTime> ngaySanXuat = null;
        public Nullable<DateTime> NgaySanXuat
        {
            get
            {
                if (ngaySanXuat == null)
                {
                    ngaySanXuat = DateTime.Today;
                }
                return ngaySanXuat;
            }
            set
            {
                ngaySanXuat = value;
                OnPropertyChanged("NgaySanXuat");
            }
        }

        //ngay san xuat

        private Nullable<DateTime> ngayhetHan = null;
        public Nullable<DateTime> NgayHetHan
        {
            get
            {
                if (ngayhetHan == null)
                {
                    ngayhetHan = DateTime.Today;
                }
                return ngayhetHan;
            }
            set
            {
                ngayhetHan = value;
                OnPropertyChanged("NgayHetHan");
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
                    this.currentLoThuoc.MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
                    this.preAuthStatus = this.currentLoThuoc.AUTH_STATUS;
                    this.currentLoThuoc.AUTH_STATUS = "U";
                    MyHelper.IsBusy();
                    this.loThuocClient.ChinhSuaLoThuocAsync(this.currentLoThuoc);
                }
                else
                {
                    MyHelper.IsBusy();
                    this.currentLoThuoc.NgaySanXuat = NgaySanXuat;
                    this.currentLoThuoc.NgayHetHan = NgayHetHan;
                    this.currentLoThuoc.MaThuoc = ThuocComboboxSelected;
                    this.loThuocClient.ThemLoThuocMoiAsync(this.currentLoThuoc);
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
            if (this.currentLoThuoc.AUTH_STATUS.Equals("A")) return;
            if (this.currentLoThuoc.MAKER_ID.Equals(CurrentSystemLogin.CurrentUser.TLNANME))
            {
                this.messagePop.SetSingleError(ValidatorResource.lblErrorPermission);
                return;
            }
            this.currentLoThuoc.CHECKER_ID = CurrentSystemLogin.CurrentUser.TLNANME;
            this.currentLoThuoc.APPROVE_DT = DateTime.Now;
        }        

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.thuocComboboxSelected.Equals(""))
                this.messagePop.SetError("tên thuốc không được để trống");
            if (this.currentLoThuoc.SoLo.ToString().Equals(""))
                this.messagePop.SetError("Số lô không được để trống");
            if (ngaySanXuat.Value.CompareTo(ngayhetHan.Value) >= 0)
            {
                this.messagePop.SetError("Sai ngày sản xuất và hết hạn");
            }
        }
        private void Load()
        {
            try
            {
                this.phieuNhapClient = new ZOO_PhieuNhapThuocClient();
                this.loThuocClient = new ZOO_LoThuocClient();

                this.phieuNhapClient.DanhSachThuocAsync();
                this.phieuNhapClient.DanhSachThuocCompleted += (s, e) =>
                {
                    this.thuocCombobox = e.Result;
                    this.OnPropertyChanged("ThuocCombobox");
                };

                this.loThuocClient.ThemLoThuocMoiCompleted += new EventHandler<ThemLoThuocMoiCompletedEventArgs>(insertCurrencyCompleted);
                this.loThuocClient.ChinhSuaLoThuocCompleted += new EventHandler<ChinhSuaLoThuocCompletedEventArgs>(updateCurrencyCompleted);
                //this.menuClient.ApproveCMAGENTCompleted += new EventHandler<ApproveCMAGENTCompletedEventArgs>(approveComplete);
                                
            }
            catch (Exception)
            {
                this.messagePop.SetSingleError(CommonResource.errorCannotConnectServer);
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

        private void updateCurrencyCompleted(object sender, ChinhSuaLoThuocCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result == "0")
                {
                    this.IsApproved = Visibility.Collapsed.ToString();
                    this.OnPropertyChanged("IsApproved");
                    this.messagePop.Successful(ValidatorResource.UpdateSuccessful);
                }
                else
                {
                    this.currentLoThuoc.AUTH_STATUS = this.preAuthStatus;
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

        private void insertCurrencyCompleted(object sender, ThemLoThuocMoiCompletedEventArgs e)
        {
            try
            {
                if (!e.Result.MaLo.Equals(""))
                {
                    this.currentLoThuoc.MaLo = e.Result.MaLo;
                    this.messagePop.Successful(ValidatorResource.InsertSuccessful);
                    Messenger.Default.Send(new ParentMessage() { isEdit = this.isEdit, currentObject = this.currentLoThuoc });

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

            private LoThuocEditViewModel viewModel;
            public ActionButton(LoThuocEditViewModel viewModel)
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
