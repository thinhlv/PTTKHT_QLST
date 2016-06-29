using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels.Common;
using gMVVM.ViewModels.Customer;
using mvvmCommon;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace gMVVM
{
    public partial class MainPage : UserControl
    {
        private MainPageViewModel viewModel = new MainPageViewModel();
        public MainPage()
        {
            InitializeComponent();            
            PageAnimation.LoadResource();
            MyHelper.AppProcessing = Processing;
            CurrentSystemLogin.dicApproveFunction = new Dictionary<string, bool>();
            ContentFrame.Navigating += new System.Windows.Navigation.NavigatingCancelEventHandler(BeforeChange);
            ContentFrame.Navigated += new System.Windows.Navigation.NavigatedEventHandler(AfterChange);
            CurrentSystemInfor.CurrentFrame = ContentFrame;
            this.Loaded += (s, e) =>
            {
                this.DataContext = this.viewModel;
            };
        }

        private bool isFalse = false;
        private void AfterChange(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (isFalse)
            {
                CurrentSystemInfor.CurrentFrame.Navigate(new Uri("/Home", UriKind.RelativeOrAbsolute));
                HtmlPage.Document.SetProperty("title", CommonResource.lblHome);
                return;
            }

            HtmlPage.Document.SetProperty("title", IsolatedStorageSettings.ApplicationSettings["MenuName"].ToString());            
        }

        private void BeforeChange(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            try
            {
                if (!CurrentSystemInfor.AvailableLink.ContainsKey(e.Uri.ToString()))
                {
                    CurrentSystemInfor.CurrentFrame.Navigate(new Uri("/Home", UriKind.RelativeOrAbsolute));
                    isFalse = true;
                    return;
                }
                else
                {
                    CurrentSystemInfor.CurrentMenuId = CurrentSystemInfor.AvailableLink[e.Uri.ToString()];
                    if (CurrentSystemLogin.dicApproveFunction.ContainsKey(CurrentSystemInfor.CurrentMenuId))
                        CurrentSystemLogin.IsAppFunction = CurrentSystemLogin.dicApproveFunction[CurrentSystemInfor.CurrentMenuId];
                }
                isFalse = false;
            }
            catch (Exception)
            { }
        }
    }
}
