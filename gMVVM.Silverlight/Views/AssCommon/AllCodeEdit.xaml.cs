using gMVVM.ViewModels.AssCommon;
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

namespace gMVVM.Views.AssCommon
{
    public partial class AllCodeEdit : UserControl
    {
        private AllCodeEditViewModel viewModel = new AllCodeEditViewModel();
        public AllCodeEdit()
        {
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }
    }
}
