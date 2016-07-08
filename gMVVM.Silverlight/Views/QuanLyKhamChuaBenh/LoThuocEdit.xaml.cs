using gMVVM.ViewModels.QuanLyKhamChuaBenh;
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

namespace gMVVM.Views.QuanLyKhamChuaBenh
{
    public partial class LoThuocEdit : UserControl
    {
        private LoThuocEditViewModel viewModel = new LoThuocEditViewModel();
        public LoThuocEdit()
        {
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }  
    }
}
