using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace gMVVM.CommonClass
{
    public class VisibleButton
    {
        public static readonly DependencyProperty ButVisiProperty = DependencyProperty.RegisterAttached("Visi", typeof(Visibility), typeof(VisibleButton), null);
        public static void SetVisi(DependencyObject obj, Visibility vb)
        {

            obj.SetValue(ButVisiProperty, vb);
        }

        public static Visibility GetVisi(DependencyObject obj)
        {
            
            return (Visibility)obj.GetValue(ButVisiProperty);
        }     
    }

}
