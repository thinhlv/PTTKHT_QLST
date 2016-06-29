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
    public class imagebutton
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(ImageSource), typeof(imagebutton), null);

        public static void SetIcon(DependencyObject obj, ImageSource pImageSource)
        {

            obj.SetValue(IconProperty, pImageSource);
        }

        public static ImageSource GetIcon(DependencyObject obj)
        {
            return obj.GetValue(IconProperty) as ImageSource;
        }
    }
}
