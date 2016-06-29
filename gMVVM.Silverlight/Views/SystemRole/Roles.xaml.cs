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
    public partial class Roles : UserControl
    {
        private RolesViewModel viewModel;

        public Roles()
        {
            
            InitializeComponent();
            PageAnimation.SetObject(front, back);
            viewModel = new RolesViewModel();
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }
    }
}
