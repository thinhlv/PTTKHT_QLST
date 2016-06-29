using gMVVM.gMVVMService;
using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.SessionService;
using mvvmCommon;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM
{
    public partial class App : Application
    {

        // Declare public variable to hold the param value
       // public static string UserID = string.Empty;
        public App()
        {

            //StreamReader reader = new StreamReader("LISTKEYTRANSACTION.txt");
            //string key = "";
            //while (!reader.EndOfStream)
            //{
            //    key = reader.ReadLine();                

            //}            

            sessionClient = new SessionServiceClient();
            sessionClient.GetSessionCompleted += new EventHandler<GetSessionCompletedEventArgs>(getSessionCompleted);
            sessionClient.GetSessionAsync(CurrentSystemLogin.SessionKeys);

            this.Startup += this.Application_Startup;
            this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }
        
        SessionServiceClient sessionClient;
        private IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //UserID = e.InitParams["UserAccount"];
            CultureInfo culture;
            if (appSettings.Contains("currentCulture"))
                culture = new CultureInfo(appSettings["currentCulture"].ToString());
            else
            {
                appSettings.Add("currentCulture", "vi-VN");
                culture = new CultureInfo(appSettings["currentCulture"].ToString());
            }

            if (!appSettings.Contains("UserLogin"))
            {
                CurrentSystemLogin.CurrentUser = null;
                appSettings.Add("UserLogin", CurrentSystemLogin.CurrentUser);
            }
            else
                CurrentSystemLogin.CurrentUser = appSettings["UserLogin"] as TL_USER_SearchResult;
            if (!appSettings.Contains("MenuId"))
            {
                CurrentSystemInfor.CurrentMenuId = "";
                appSettings.Add("MenuId", CurrentSystemInfor.CurrentMenuId);
            }
            else
                CurrentSystemInfor.CurrentMenuId = appSettings["MenuId"] == null ? "" : appSettings["MenuId"].ToString();
            if (!appSettings.Contains("Roles"))
            {
                CurrentSystemLogin.Roles = null;
                appSettings.Add("Roles", CurrentSystemLogin.Roles);
            }
            else
                CurrentSystemLogin.Roles = appSettings["Roles"] as Dictionary<string, TL_SYSROLEDETAIL>;

            if (!appSettings.Contains("MenuLink"))
            {
                CurrentSystemInfor.AvailableLink = null;
                appSettings.Add("MenuLink", CurrentSystemInfor.AvailableLink);
            }
            else
                CurrentSystemInfor.AvailableLink = appSettings["MenuLink"] as Dictionary<string, string>;

            if (!appSettings.Contains("MenuName"))
            {
                appSettings.Add("MenuName", CommonResource.lblHome);
            }

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            //this.RootVisual = new gMVVM.Views.PlanManager.PLSyn();
            //this.RootVisual = new MainFileUpload();
        }

        private void getSessionCompleted(object sender, GetSessionCompletedEventArgs e)
        {
            //InitializeComponent();
            try
            {
                Grid rootGrid = new Grid();
                if (e.Result == null || e.Result.ToString().Equals("NULL"))
                    rootGrid.Children.Add(new LoginSystem());
                else
                    rootGrid.Children.Add(new MainPage()); // Set default page

                this.RootVisual = rootGrid;
            }
            catch (Exception)
            {
                if (CurrentSystemLogin.CurrentUser== null)
                    this.RootVisual = new LoginSystem();
                else
                    this.RootVisual = new MainPage();
            }

        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            // If the app is running outside of the debugger then report the exception using
            // the browser's exception mechanism. On IE this will display it a yellow alert 
            // icon in the status bar and Firefox will display a script error.
            if (!System.Diagnostics.Debugger.IsAttached)
            {
                // NOTE: This will allow the application to continue running after an exception has been thrown
                // but not handled. 
                // For production applications this error handling should be replaced with something that will 
                // report the error to the website and stop the application.
                e.Handled = true;
                Deployment.Current.Dispatcher.BeginInvoke(delegate { ReportErrorToDOM(e); });
            }
        }

        private void ReportErrorToDOM(ApplicationUnhandledExceptionEventArgs e)
        {
            try
            {
                string errorMsg = e.ExceptionObject.Message + e.ExceptionObject.StackTrace;
                errorMsg = errorMsg.Replace('"', '\'').Replace("\r\n", @"\n");

                //System.Windows.Browser.HtmlPage.Window.Eval("throw new Error(\"Unhandled Error in Silverlight Application " + errorMsg + "\");");
            }
            catch (Exception)
            {
            }
        }
    }
}
