using EduBanking.Silverlight.ViewModels.Common;
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

namespace EduBanking.Silverlight.Views.Common
{
    public partial class CalendarEdit : UserControl
    {
        private CalendarEditViewModel viewModel = new CalendarEditViewModel();
        public CalendarEdit()
        {
            InitializeComponent();
            this.Loaded += (s, e) => { this.DataContext = this.viewModel; };
        }        
    }
}
