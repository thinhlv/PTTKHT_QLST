using gMVVM.ViewModels.AssCommon;
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

namespace gMVVM.Views.AssCommon
{
    public partial class ArgumentList : UserControl
    {
        public ArgumentList()
        {
            InitializeComponent();
            PageAnimation.SetObject(front, back);
            this.Loaded += (s, e) => { this.DataContext = new ArgumentViewModel(); };
        }
    }
}
