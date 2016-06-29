using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace gMVVM.CommonClass
{
    public class PageSizeProperty
    {
        public static readonly DependencyProperty PSProperty = DependencyProperty.RegisterAttached("PS", typeof(int), typeof(PageSizeProperty), null);
        public static void SetPS(DependencyObject obj, int vb)
        {

            obj.SetValue(PSProperty, vb);
        }

        public static int GetPS(DependencyObject obj)
        {

            return (int)obj.GetValue(PSProperty);
        }     
    }
}
