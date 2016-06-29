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
using gMVVM.ViewModels.SystemRole;

namespace gMVVM.Views.SystemRole
{
    public partial class UserEdit : UserControl
    {
        UserEditViewModel viewModel = new UserEditViewModel();
        public UserEdit()
        {
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }
      
    }
}

