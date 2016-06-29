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

namespace gMVVM.Views.Search
{
    public partial class SearchScreen : ChildWindow
    {
        public SearchScreen()
        {
            InitializeComponent();
        }
        
        public SearchScreen(string title, UserControl content, object viewModel)
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                lblTitle.Content = title;
                FrameChi.Content = content;
                this.DataContext = viewModel;                               
            };            
        }       
    }
}

