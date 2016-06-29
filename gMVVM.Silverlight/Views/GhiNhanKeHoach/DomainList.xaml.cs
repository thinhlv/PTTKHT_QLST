using gMVVM.ViewModels.GhiNhanKeHoach;
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

namespace gMVVM.Views.GhiNhanKeHoach
{
    public partial class DomainList : UserControl
    {
        public DomainList()
        {
            InitializeComponent();
            PageAnimation.SetObject(front, back);
            this.Loaded += (s, e) => { this.DataContext = new DomainViewModel(); };
        }
    }
}
