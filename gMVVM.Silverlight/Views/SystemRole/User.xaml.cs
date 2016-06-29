using gMVVM.ViewModels.SystemRole;
using mvvmCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM.Views.SystemRole
{
    public partial class User : UserControl
    {
        private UserViewModel viewModel;
        public User()
        {
            
            InitializeComponent();
            PageAnimation.SetObject(front, back);
            viewModel = new UserViewModel();
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }
    }
}
