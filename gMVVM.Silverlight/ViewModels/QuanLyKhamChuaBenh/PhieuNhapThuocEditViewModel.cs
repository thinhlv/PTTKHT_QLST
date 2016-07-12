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
    public class PhieuNhapThuocEditViewModel : ViewModelBase
    {
        //private TLMENUClient menuClient;
        private ZOO_PhieuNhapThuocClient phieuNhapClient;
        private ZOO_LoThuocClient loThuocClient;

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
            this.currentPhieuNhapThuoc = obj.currentObject != null ? (obj.currentObject as ZOO_PHIEUNHAPTHUOC)
                : new ZOO_PHIEUNHAPTHUOC()
                {
                    MaLo = "",
                    SoLuong = 0,
                    NgayNhap = DateTime.Now,
                    MaPhieuNhap = "",

                    RECORD_STATUS = "1",
                    AUTH_STATUS = "U",
                    MAKER_ID = CurrentSystemLogin.CurrentUser.TLNANME,
                    NOTES = ""
                };

            if (this.currentPhieuNhapThuoc.AUTH_STATUS.Equals("A"))
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
                InitComboboxLoThuoc();
                this.OnPropertyChanged("ThuocComboboxSelected");
            }
        }


        private ObservableCollection<ZOO_LOTHUOC_SearchResult> loThuocCombobox;
        public ObservableCollection<ZOO_LOTHUOC_SearchResult> LoThuocCombobox
        {
            get
            {
                return this.loThuocCombobox;
            }

            set
            {
                this.loThuocCombobox = value;
                this.OnPropertyChanged("LoThuocCombobox");
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
                    this.phieuNhapClient.ChinhSuaPhieuNhapAsync(this.currentPhieuNhapThuoc);
                }
                else
                {
                    MyHelper.IsBusy();
                    this.currentPhieuNhapThuoc.NgayNhap = NgayNhap;
                    this.phieuNhapClient.LuuPhieuNhapAsync(this.currentPhieuNhapThuoc);
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
        }        

        private void RefreshValidator()
        {
            this.messagePop.Reset();
            string notEmpty = " " + ValidatorResource.NotEmpty;
            if (this.currentPhieuNhapThuoc.MaLo.Equals(""))
                this.messagePop.SetError("Số lô không được để trống");
            if (this.currentPhieuNhapThuoc.SoLuong.ToString().Equals(""))
                this.messagePop.SetError("Số lượng không được để trống");           
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

                this.phieuNhapClient.LuuPhieuNhapCompleted += new EventHandler<LuuPhieuNhapCompletedEventArgs>(insertCurrencyCompleted);
                this.loThuocClient.TimLoThuocComboboxCompleted += new EventHandler<TimLoThuocComboboxCompletedEventArgs>(timLoThuocComplete);
                this.phieuNhapClient.ChinhSuaPhieuNhapCompleted += new EventHandler<ChinhSuaPhieuNhapCompletedEventArgs>(updateCurrencyCompleted);
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

        private void updateCurrencyCompleted(object sender, ChinhSuaPhieuNhapCompletedEventArgs e)
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
                    this.currentPhieuNhapThuoc.AUTH_STATUS = this.preAuthStatus;
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

        private void insertCurrencyCompleted(object sender, LuuPhieuNhapCompletedEventArgs e)
        {
            try
            {
                if (!e.Result.MaPhieuNhap.Equals(""))
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

        private void timLoThuocComplete(object sender, TimLoThuocComboboxCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null && e.Result.Count > 0)
                    this.LoThuocCombobox = e.Result;
                else
                    this.LoThuocCombobox = new ObservableCollection<ZOO_LOTHUOC_SearchResult>();
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

        private void InitComboboxLoThuoc()
        {
            if(!thuocComboboxSelected.Equals(""))
            {
                loThuocClient.TimLoThuocComboboxAsync(thuocComboboxSelected);
            }
        }
    }
}
