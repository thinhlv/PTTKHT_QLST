using gMVVM.ViewModels.Common;
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

namespace gMVVM.Views.Common
{
    public partial class LeftMenu : UserControl
    {
        private LeftMenuViewModel viewModel = new LeftMenuViewModel();
        public LeftMenu()
        {
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }
    }
}
