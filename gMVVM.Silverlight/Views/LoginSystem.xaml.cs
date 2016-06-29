using gMVVM.CommonClass;
using gMVVM.Resources;
using gMVVM.ViewModels;
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

namespace gMVVM
{
    public partial class LoginSystem : UserControl
    {
        private LoginSystemViewModel viewModel = new LoginSystemViewModel();
        public LoginSystem()
        {
            InitializeComponent();
            MyHelper.AppProcessing = this.ProcessingLogin;
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
            HtmlPage.Document.SetProperty("title", SystemRoleResource.lblLoginTitle);
        }
    }
}
