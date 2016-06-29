using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;

namespace gMVVM.CommonClass
{
    public class CommandProperty
    {
        public static readonly DependencyProperty CommandBtn = DependencyProperty.RegisterAttached("Cmd", typeof(ICommand), typeof(CommandProperty), null);
        public static void SetCmd(DependencyObject obj, ICommand vb)
        {

            obj.SetValue(CommandBtn, vb);
        }

        public static ICommand GetCmd(DependencyObject obj)
        {

            return (ICommand)obj.GetValue(CommandBtn);
        }     
    }
}
