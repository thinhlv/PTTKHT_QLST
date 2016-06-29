using GalaSoft.MvvmLight.Messaging;
using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.SessionService;
using gMVVM.Views.Common;
using JeffWilcox.Utilities.Silverlight;
using mvvmCommon;
using System;
using System.IO.IsolatedStorage;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM.ViewModels.Common
{
    public class ToolHeaderViewModel : ViewModelBase
    {
        private TLUSERClient userClient;
        private SessionServiceClient sessionClient;
        private Popup simplePopup = new Popup();
        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        public ToolHeaderViewModel()
        {
            try
            {                                

                this.IsChangeAccount = Visibility.Collapsed.ToString();
                
                this.userClient = new TLUSERClient();
                
                this.userClient.UpdateTLUSERCompleted += new EventHandler<UpdateTLUSERCompletedEventArgs>(changePassCompleted);

                

                sessionClient = new SessionServiceClient();
                sessionClient.SetSessionCompleted += new EventHandler<SetSessionCompletedEventArgs>(setSessionCompleted);

                this.currentCulture = new ChangeCulture(this);
                this.currentUser = CurrentSystemLogin.CurrentUser;
                this.BranchId = this.currentUser.TLSUBBRID;
                this.RoleName = this.currentUser.RoleName;
                this.UserName = this.currentUser.TLNANME;             

                ////DOI MAT KHAU NEU DANG NHAP LAN DAU TIEN
                //if (CurrentSystemLogin.CurrentUser.ISFIRSTTIME.Equals("1"))
                //{
                //    this.ChangePassShow();
                //    this.IsCancelVisi = Visibility.Collapsed.ToString();
                //    this.IsFirstTimeLogin = Visibility.Visible.ToString();
                //    this.OnPropertyChanged("IsCancelVisi");
                //    this.OnPropertyChanged("IsFirstTimeLogin");
                //}

                this.TimeLogin = CommonResource.lblTimeLogin + " " + appSettings["TimeLogin"].ToString();

                Messenger.Default.Register<bool>(this, ReciveClosePopup);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ToolHeaderViewModel() " + ex.Message);
            }
        }

        private void ReciveClosePopup(bool obj)
        {
            if(obj)
                this.simplePopup.IsOpen = false;
        }        
                
        private void setSessionCompleted(object sender, SetSessionCompletedEventArgs e)
        {
            try
            {
                IsolatedStorageSettings.ApplicationSettings["UserLogin"] = null;
                System.Windows.Browser.HtmlPage.Window.Eval("location.reload()");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ChangeCurrentCulture(string culture)
        {
            if (culture.Equals("LOGOUT"))
            {
                sessionClient.SetSessionAsync(CurrentSystemLogin.SessionKeys, "NULL");
            }
            else if (culture.Equals("ChangePass"))
            {
                this.ChangePassShow();
            }
            else if (culture.Equals("Cancel"))
            {
                this.IsChangeAccount = Visibility.Collapsed.ToString();
                this.OnPropertyChanged("IsChangeAccount");
            }
            else if (culture.Equals("OkChange"))
            {
                this.ChangePassword();
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings["currentCulture"] = culture;
                System.Windows.Browser.HtmlPage.Window.Eval("location.reload()");
            }
        }

        private void ChangePassShow()
        {
            this.NewPass = "";
            this.CurrentPass = "";
            this.NewPassAgaint = "";
            this.IsChangeAccount = Visibility.Visible.ToString();
            this.IsErrorVisible = Visibility.Collapsed.ToString();
            this.IsCancelVisi = Visibility.Visible.ToString();
            this.IsFirstTimeLogin = Visibility.Collapsed.ToString();
            this.OnPropertyChanged("IsFirstTimeLogin");
            this.OnPropertyChanged("NewPass");
            this.OnPropertyChanged("CurrentPass");
            this.OnPropertyChanged("NewPassAgaint");
            this.OnPropertyChanged("IsChangeAccount");
            this.OnPropertyChanged("IsErrorVisible");
            this.OnPropertyChanged("IsCancelVisi");
        }

        private void ChangePassword()
        {
            if (this.CurrentPass.Equals(""))
            {
                this.ErrorMessage = SystemRoleResource.Password + " " + ValidatorResource.NotEmpty;
                this.IsErrorVisible = Visibility.Visible.ToString();
                goto End;
            }

            if (this.NewPass.Equals(""))
            {
                this.ErrorMessage = SystemRoleResource.lblNewPass + " " + ValidatorResource.NotEmpty; ;
                this.IsErrorVisible = Visibility.Visible.ToString();
                goto End;
            }

            if (!MD5.GetMd5String(this.CurrentPass).Equals(CurrentSystemLogin.CurrentUser.Password))
            {
                this.ErrorMessage = SystemRoleResource.Password + " " + ValidatorResource.lblNotTrue;
                this.IsErrorVisible = Visibility.Visible.ToString();
                goto End;
            }

            if (!this.NewPass.Equals(this.NewPassAgaint))
            {
                this.ErrorMessage = SystemRoleResource.lblNotMatch;
                this.IsErrorVisible = Visibility.Visible.ToString();
                goto End;
            }

            MyHelper.IsBusy();
            this.preFirsTime = CurrentSystemLogin.CurrentUser.ISFIRSTTIME;
            CurrentSystemLogin.CurrentUser.Password = MD5.GetMd5String(this.NewPass);
            CurrentSystemLogin.CurrentUser.ISFIRSTTIME = "0";
            this.userClient.UpdateTLUSERAsync(CurrentSystemLogin.CurrentUser);
            this.IsErrorVisible = Visibility.Collapsed.ToString();

        End:
            this.OnPropertyChanged("ErrorMessage");
            this.OnPropertyChanged("IsErrorVisible");

        }

        private void changePassCompleted(object sender, UpdateTLUSERCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Result.Equals("0"))
                {
                    this.IsErrorVisible = Visibility.Collapsed.ToString();
                    this.OnPropertyChanged("IsErrorVisible");
                    this.IsChangeAccount = Visibility.Collapsed.ToString();
                    this.OnPropertyChanged("IsChangeAccount");                    
                    IsolatedStorageSettings.ApplicationSettings["UserLogin"] = CurrentSystemLogin.CurrentUser;                    
                }
                else
                {                    
                    CurrentSystemLogin.CurrentUser.Password = MD5.GetMd5String(this.CurrentPass);
                    CurrentSystemLogin.CurrentUser.ISFIRSTTIME = this.preFirsTime;
                    this.ErrorMessage = ValidatorResource.lblErrorChange;
                    this.IsErrorVisible = Visibility.Visible.ToString();
                    this.OnPropertyChanged("IsErrorVisible");
                    this.OnPropertyChanged("ErrorMessage");
                }
            }
            catch (Exception)
            {
                CurrentSystemLogin.CurrentUser.Password = MD5.GetMd5String(this.CurrentPass);
                CurrentSystemLogin.CurrentUser.ISFIRSTTIME = this.preFirsTime;
            }
            finally
            {
                MyHelper.IsFree();
            }
        }

        private TL_USER_SearchResult currentUser;
        private string preFirsTime = "";

        #region [Properties]

        private string timeLogin;
        public string TimeLogin
        {
            get
            {
                return this.timeLogin;
            }

            set
            {
                this.timeLogin = value;
                this.OnPropertyChanged("TimeLogin");
            }
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public string IsFirstTimeLogin { get; set; }

        public string IsErrorVisible { get; set; }

        public string IsCancelVisi { get; set; }

        public string NewPass
        {
            get;
            set;
        }

        public string NewPassAgaint
        {
            get;
            set;
        }

        public string CurrentPass
        {
            get;
            set;
        }

        public string IsChangeAccount
        {
            get;
            set;
        }

        //public string CurrentUserName { get; set; }

        private ChangeCulture currentCulture;
        public ChangeCulture CurrentCulture
        {
            get
            {
                return this.currentCulture;
            }
        }

        private string currentDate;
        public string CurrentDate
        {
            get
            {
                return this.currentDate;
            }

            set
            {
                this.currentDate = value;
                this.OnPropertyChanged("CurrentDate");
            }
        }

        private string currentTime;
        public string CurrentTime
        {
            get
            {
                return this.currentTime;
            }

            set
            {
                this.currentTime = value;
                this.OnPropertyChanged("CurrentTime");
            }
        }

        private string roleName;
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

        private string branchId;
        public string BranchId
        {
            get
            {
                return this.branchId;
            }
            set
            {
                this.branchId = value;
                this.OnPropertyChanged("BranchId");
            }
        }

        private string userName;
        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
                this.OnPropertyChanged("UserName");
            }
        }
        public void View_Profile()
        {
            this.simplePopup.Child = new Profile(this.simplePopup);
            this.simplePopup.HorizontalOffset = (double)System.Windows.Browser.HtmlPage.Window.Eval("screen.width") - 455;
            this.simplePopup.VerticalOffset = 100;
            Messenger.Default.Send(new TL_USER_SearchResult() {TLNANME=CurrentSystemLogin.CurrentUser.TLNANME,BRANCH_NAME=CurrentSystemLogin.CurrentUser.BRANCH_NAME
                ,RoleName=CurrentSystemLogin.CurrentUser.RoleName,TLFullName=CurrentSystemLogin.CurrentUser.TLFullName,
                EMAIL=CurrentSystemLogin.CurrentUser.EMAIL,PHONE=CurrentSystemLogin.CurrentUser.PHONE,
                ADDRESS=CurrentSystemLogin.CurrentUser.ADDRESS
            });               
            this.simplePopup.IsOpen = true;
        }       
        #endregion

        public class ChangeCulture : ICommand
        {

            private ToolHeaderViewModel viewmodel;
            public ChangeCulture(ToolHeaderViewModel viewmodel)
            {
                this.viewmodel = viewmodel;
                this.viewmodel.PropertyChanged += (s, e) =>
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
                this.viewmodel.ChangeCurrentCulture(parameter.ToString());
            }
        }  
    }
}
