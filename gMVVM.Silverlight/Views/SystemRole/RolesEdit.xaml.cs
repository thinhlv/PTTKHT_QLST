using gMVVM.ViewModels.SystemRole;
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
    public partial class RolesEdit : UserControl
    {
        private RolesEditViewModel viewModel = new RolesEditViewModel();
        public RolesEdit()
        {
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }
    }
}
