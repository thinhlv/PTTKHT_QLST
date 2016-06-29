using gMVVM.Resources;
using System;
using System.Collections.Generic;
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
using System.Security.Cryptography;
using System.Text;
using JeffWilcox.Utilities.Silverlight;
using gMVVM.gMVVMService;
using gMVVM.Views.SystemRole;
using gMVVM.ViewModels.SystemRole;
using gMVVM.CommonClass;
using System.Collections.ObjectModel;
using mvvmCommon;
using gMVVM.SessionService;
using System.Windows.Interactivity;
using gMVVM.ViewModels.Common;


namespace gMVVM.ViewModels
{
    public class LoginSystemViewModel : ViewModelBase
    {
        TLUSERClient userClient;
        
        SessionServiceClient sessionClient;
        public LoginSystemViewModel()
        {

            this.userClient = new TLUSERClient();            
            sessionClient = new SessionServiceClient();

            this.login = new LoginComman(this);
            ActionMenuButton.actionControl = new ActionMenuViewModel();
            ActionMenuButton.actionControl.SetEnterAction(login);

            string culture = IsolatedStorageSettings.ApplicationSettings["currentCulture"].ToString();
            if (culture.Equals("vi-VN"))
                this.IsCheckedVN = true;
            else
                this.IsCheckedEL = true;
            this.isCheckedVN = true;
            this.listUnit = new List<ItemData>()
            {
                new ItemData() { Id= "0", Name= SystemRoleResource.lblBachViet}
            };

            this.listClass = new List<ItemData>() 
            {
                new ItemData(){ Id="0", Name="NVNH LOP 1"},
                new ItemData(){ Id="1", Name="NVNH LOP 2"},
                new ItemData(){ Id="2", Name="NVNH LOP 3"},
                new ItemData(){ Id="3", Name="NVNH LOP 4"},
                new ItemData(){ Id="4", Name="NVNH LOP 5"},
            };

            this.CurrentUnit = "0";
            this.CurrentClass = "0";

            this.CaptchaItem = new CaptChaControl();

            this.userName = "";
            this.IsChangePassVisible = Visibility.Visible.ToString();
            this.IsGetCodeVisible = Visibility.Collapsed.ToString();
            this.OnPropertyChanged("IsChangePassVisible");
            this.OnPropertyChanged("IsGetCodeVisible");
            //this.userClient.CheckUserLoginCompleted += new EventHandler<CheckUserLoginCompletedEventArgs>(loginCompleted);
            //this.tillMasterClient.GetByTopTILMASTERCompleted += new EventHandler<GetByTopTILMASTERCompletedEventArgs>(getByTopTILMASTERCompleted);
            this.userClient.CheckLoginCompleted += new EventHandler<CheckLoginCompletedEventArgs>(userLoginCompleted);

            sessionClient.SetSessionCompleted += new EventHandler<SetSessionCompletedEventArgs>(setSessionCompleted);
            this.userClient.SendCodeTLUSERCompleted += new EventHandler<SendCodeTLUSERCompletedEventArgs>(sendCodeCompleted);
            this.userClient.ChangePassTLUSERCompleted += new EventHandler<ChangePassTLUSERCompletedEventArgs>(changePassCompleted);
            this.userClient.LoginADCompleted += new EventHandler<LoginADCompletedEventArgs>(checkLoginADCompleted);
        }         

        private void userLoginCompleted(object sender, CheckLoginCompletedEventArgs e)
        {
            try
            {
                if (e.Result != null)
                {

                    this.IsErrorVisible = Visibility.Collapsed.ToString();

                    //TL_USER                
                    CurrentSystemLogin.CurrentUser = e.Result.User;
                    IsolatedStorageSettings.ApplicationSettings["UserLogin"] = CurrentSystemLogin.CurrentUser;
                    
                    //TL_SYSROLEDETAIL
                    CurrentSystemLogin.Roles = new Dictionary<string, TL_SYSROLEDETAIL>();
                    foreach (var item in e.Result.Roles)
                        CurrentSystemLogin.Roles.Add(item.MENU_ID, item);
                    IsolatedStorageSettings.ApplicationSettings["Roles"] = CurrentSystemLogin.Roles;

                    if (!IsolatedStorageSettings.ApplicationSettings.Contains("TimeLogin"))
                        IsolatedStorageSettings.ApplicationSettings.Add("TimeLogin", DateTime.Now.ToShortTimeString());
                    else
                        IsolatedStorageSettings.ApplicationSettings["TimeLogin"] = DateTime.Now.ToShortTimeString();
                    //Save Session
                    sessionClient.SetSessionAsync(CurrentSystemLogin.SessionKeys, CurrentSystemLogin.CurrentUser.TLNANME);
                }
                else
                    this.IsErrorVisible = Visibility.Visible.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(CommonResource.errorCannotConnectServer + "\n" + ex.Message);
            }
            finally
            {
                MyHelper.IsFree();
            }
        }        

        private void setSessionCompleted(object sender, SetSessionCompletedEventArgs e)
        {            
            System.Windows.Browser.HtmlPage.Window.Eval("location.reload()");
        }        
       
        private void Acction(object item)
        {
            try
            {
                Storyboard animation = item as Storyboard;
                if (animation == null)//Login button
                {
                    if (item.ToString().Equals("SendPass"))
                    {
                        this.ForgotPassSend();

                    }
                    else
                    {
                        if (this.userName.Equals(""))
                        {
                            MessageBox.Show("Vui lòng nhập tên đăng nhập");
                        }
                        else if (this.password.Equals(""))
                        {
                            MessageBox.Show("Vui lòng nhập mật khẩu");
                        }
                        else
                        {
                            IsolatedStorageSettings.ApplicationSettings.Clear();
                            MyHelper.IsBusy();
                            this.userClient.LoginADAsync(this.userName, this.password); 
                            //this.userClient.CheckLoginAsync(this.userName, MD5.GetMd5String(this.password), int.Parse(this.currentClass));                            
                        }
                    }
                }
                else//Send button
                {
                    this.ResetForgotPas();
                    animation.Begin();
                }
            }
            catch (Exception ex)
            {
                MyHelper.IsBusy();
                MessageBox.Show("Acction" + ex.Message);
            }
        }

        private void checkLoginADCompleted(object sender, LoginADCompletedEventArgs e)
        {
            try
            {
                if (e.Result)
                    this.userClient.CheckLoginAsync(this.userName, MD5.GetMd5String(this.password), int.Parse(this.currentClass));
                else
                    this.IsErrorVisible = Visibility.Visible.ToString();
                MyHelper.IsFree();
            }
            catch (Exception ex)
            {
                MyHelper.IsFree();
                MessageBox.Show(CommonResource.errorCannotConnectServer + "\n" + ex.Message);
            }
        }

        private void ForgotPassSend()
        {
            if (!this.isGetCode)
            {
                if (this.UserNameLast.Equals(""))
                {
                    this.MessageSend = SystemRoleResource.lblUsername + " " + ValidatorResource.NotEmpty;
                    this.IsErrorVisible = Visibility.Visible.ToString();
                    goto End;
                }

                if (this.SecCode.Equals(""))
                {
                    this.MessageSend = SystemRoleResource.lblSecurityCode + " " + ValidatorResource.NotEmpty; ;
                    this.IsErrorVisible = Visibility.Visible.ToString();
                    goto End;
                } 

                if (this.NewPass.Equals(""))
                {
                    this.MessageSend = SystemRoleResource.lblNewPass + " " + ValidatorResource.NotEmpty; ;
                    this.IsErrorVisible = Visibility.Visible.ToString();
                    goto End;
                }                

                if (!this.NewPass.Equals(this.NewPassAgaint))
                {
                    this.MessageSend = SystemRoleResource.lblNotMatch;
                    this.IsErrorVisible = Visibility.Visible.ToString();
                    goto End;
                }
                MyHelper.IsBusy();
                this.userClient.ChangePassTLUSERAsync(this.UserNameLast, this.SecCode, MD5.GetMd5String(this.NewPass));

            End:
                this.OnPropertyChanged("MessageSend");                
            }
            else
            {
                if (this.SendUserName == null || this.SendUserName.Equals(""))
                {
                    this.MessageSend = SystemRoleResource.lblUsername + " " + ValidatorResource.NotEmpty;
                    this.OnPropertyChanged("MessageSend");
                    return;
                }
                if (this.InputCaptcha == null || !this.InputCaptcha.ToUpper().Equals((this.CaptchaItem.DataContext as CaptchaViewModel).CaptchaText))
                {
                    (this.CaptchaItem.DataContext as CaptchaViewModel).CreatNewCaptcha();
                    this.MessageSend = SystemRoleResource.lblSecurityCode + " " + ValidatorResource.lblNotTrue;
                    this.OnPropertyChanged("MessageSend");
                    return;
                }

                (this.CaptchaItem.DataContext as CaptchaViewModel).CreatNewCaptcha();
                MyHelper.IsBusy();
                this.userClient.SendCodeTLUSERAsync(this.SendUserName);
            }
        }

        private void sendCodeCompleted(object sender, SendCodeTLUSERCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Equals("NotExisted"))
                {
                    this.MessageSend = SystemRoleResource.lblNotExist;
                    this.OnPropertyChanged("MessageSend");
                }
                else if (e.Result.Equals("True"))
                {
                    this.MessageSend = SystemRoleResource.lblSendSuccessful;
                    this.OnPropertyChanged("MessageSend");
                }
                else
                {
                    this.MessageSend = e.Result;
                    this.OnPropertyChanged("MessageSend");
                }

            }
            catch (Exception)
            {
                this.MessageSend = CommonResource.errorCannotConnectServer;
                this.OnPropertyChanged("MessageSend");
            }
            finally
            {
                MyHelper.IsFree();
            }
        }

        private void changePassCompleted(object sender, ChangePassTLUSERCompletedEventArgs e)
        {
            try
            {
                if (e.Result.Equals("NotExisted"))
                {
                    this.MessageSend = SystemRoleResource.lblNotExist;
                    this.OnPropertyChanged("MessageSend");
                }
                else if (e.Result.Equals("CodeFail"))
                {
                    this.MessageSend = SystemRoleResource.lblSecurityCode + " " + ValidatorResource.lblNotTrue;
                    this.OnPropertyChanged("MessageSend");
                }
                else if (e.Result.Equals("True"))
                {
                    this.MessageSend = ValidatorResource.UpdateSuccessful;
                    this.OnPropertyChanged("MessageSend");
                }
                else
                {
                    this.MessageSend = e.Result;
                    this.OnPropertyChanged("MessageSend");
                }

            }
            catch (Exception)
            {
                this.MessageSend = CommonResource.errorCannotConnectServer;
                this.OnPropertyChanged("MessageSend");
            }
            finally
            {
                MyHelper.IsFree();
            }
        }  

        private void ResetForgotPas()
        {
            this.IsChangePassVisible = Visibility.Visible.ToString();
            this.IsGetCodeVisible = Visibility.Collapsed.ToString();
            this.MessageSend = "";
            this.UserNameLast = "";
            this.SecCode = "";
            this.NewPass = "";
            this.NewPassAgaint = "";
            this.OnPropertyChanged("IsChangePassVisible");
            this.OnPropertyChanged("IsGetCodeVisible");
            this.OnPropertyChanged("MessageSend");
            this.OnPropertyChanged("UserNameLast");
            this.OnPropertyChanged("SecCode");
            this.OnPropertyChanged("NewPass");
            this.OnPropertyChanged("NewPassAgaint");
        }        

        private bool isGetCode;
        public bool IsGetCode
        {
            get
            {
                return this.isGetCode;
            }

            set
            {
                this.isGetCode = value;
                this.OnPropertyChanged("IsGetCode");
                if (this.isGetCode)
                {
                    this.IsChangePassVisible = Visibility.Collapsed.ToString();
                    this.IsGetCodeVisible = Visibility.Visible.ToString();
                }
                else
                {
                    this.IsChangePassVisible = Visibility.Visible.ToString();
                    this.IsGetCodeVisible = Visibility.Collapsed.ToString();
                }
                this.MessageSend = "";
                this.OnPropertyChanged("IsChangePassVisible");
                this.OnPropertyChanged("IsGetCodeVisible");
                this.OnPropertyChanged("MessageSend");
            }
        }

        public string IsGetCodeVisible { get; set; }
        public string IsChangePassVisible { get; set; }

        public string UserNameLast { get; set; }
        public string SecCode { get; set; }
        public string NewPass { get; set; }
        public string NewPassAgaint { get; set; }

        public CaptChaControl CaptchaItem { get; set; }

        public string InputCaptcha { get; set; }

        public string MessageSend { get; set; }

        public string SendUserName { get; set; }

        private string isErrorVisible = Visibility.Collapsed.ToString();
        public string IsErrorVisible
        {
            get
            {
                return this.isErrorVisible;
            }

            set
            {
                this.isErrorVisible = value;
                this.OnPropertyChanged("IsErrorVisible");                
            }
        }

        private string userName = "";
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
                if (value == null || value.Equals("")) throw new Exception("Input value");    
            }
        }

        private string password = "";
        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {                
                this.password = value;
                this.OnPropertyChanged("Password");
                if (value == null || value.Equals("")) throw new Exception("Input value");
            }
        }

        private bool isCheckedVN;
        public bool IsCheckedVN
        {
            get
            {
                return this.isCheckedVN;
            }

            set
            {
                this.isCheckedVN = value;                
                this.OnPropertyChanged("IsCheckedVN");
                IsolatedStorageSettings.ApplicationSettings["currentCulture"] = "vi-VN";
            }
        }

        private bool isCheckedEL;
        public bool IsCheckedEL
        {
            get
            {
                return this.isCheckedEL;
            }

            set
            {
                this.isCheckedEL = value;
                this.OnPropertyChanged("IsCheckedEL");
                IsolatedStorageSettings.ApplicationSettings["currentCulture"] = "";
            }
        }

        private LoginComman login;
        public LoginComman Login
        {
            get
            {
                return this.login;
            }
        }

        private List<ItemData> listUnit;
        public List<ItemData> ListUnit
        {
            get
            {
                return this.listUnit;
            }
        }

        private List<ItemData> listClass;
        public List<ItemData> ListClass
        {
            get
            {
                return this.listClass;
            }
        }

        private string currentUnit;
        public string CurrentUnit
        {
            get
            {
                return this.currentUnit;
            }

            set
            {
                this.currentUnit = value;
                this.OnPropertyChanged("CurrentUnit");
            }
        }

        //CurrentClass
        private string currentClass;
        public string CurrentClass
        {
            get
            {
                return this.currentClass;
            }

            set
            {
                this.currentClass = value;
                this.OnPropertyChanged("CurrentClass");
            }
        }

        public class LoginComman : ICommand
        {
            private LoginSystemViewModel viewmodel;
            public LoginComman(LoginSystemViewModel viewmodel)
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
                this.viewmodel.Acction(parameter);
            }
        }
           
    }

    public class ItemData
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public static class Test
        {
            public static object preControl;
            public static bool active = false;
        }     

    public class EnterKey : Behavior<Control>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.KeyDown += KeyDown;
            AssociatedObject.LostFocus += LostFocus;
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("L" + (sender as Control).ToString());
            Test.preControl = sender;
            if (Test.active)
            {
                ActionMenuButton.actionControl.Insert.Execute("");
                Test.active = false;
            }
        }
        //private object preControl;
        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Test.preControl == null) return;

                //Test.active = true;
                switch ((Test.preControl as Control).GetType().Name)
                {
                    case "TextBox":
                        (Test.preControl as TextBox).Focus();
                        (Test.preControl as TextBox).SelectAll();
                        break;
                    case "PasswordBox":
                        (Test.preControl as PasswordBox).Focus();
                        (Test.preControl as PasswordBox).SelectAll();
                        break;
                }
                Test.active = true;
            }
            else
                Test.active = false;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.KeyDown += KeyDown;
        }
    }
}
