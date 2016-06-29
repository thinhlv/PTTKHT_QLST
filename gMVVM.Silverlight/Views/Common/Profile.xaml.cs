using gMVVM.ViewModels.Common;
using gMVVM.ViewModels.SystemRole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM.Views.Common
{
    public partial class Profile : UserControl
    {
        public ProfileViewModel viewModel = new ProfileViewModel();
        public Profile()
        {
            InitializeComponent();
            this.Loaded += (s, e) => this.DataContext = this.viewModel;
        }
        private Popup currentParent;
        public Profile(Popup parent) : this()
        {
            this.currentParent = parent;
        }
        private void ButtonClose_Click_1(object sender, RoutedEventArgs e)
        {
            this.currentParent.IsOpen = false;
        }
    }
}
