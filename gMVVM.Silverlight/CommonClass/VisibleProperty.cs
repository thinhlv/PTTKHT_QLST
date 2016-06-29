using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace gMVVM.CommonClass
{
    public class VisibleProperty
    {
        public static readonly DependencyProperty VisiProperty = DependencyProperty.RegisterAttached("btVib", typeof(Visibility), typeof(VisibleProperty), null);

        public static void SetIcon(DependencyObject obj, Visibility vb)
        {

            obj.SetValue(VisiProperty, vb);
        }

        public static Visibility GetIcon(DependencyObject obj)
        {
            return (Visibility)obj.GetValue(VisiProperty);
        }     
    }
}
