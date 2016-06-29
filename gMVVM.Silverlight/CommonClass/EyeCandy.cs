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
    public class EyeCandy
    {
        #region Image dependency property

        /// <summary>
        /// An attached dependency property which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static readonly DependencyProperty ImageProperty;

        /// <summary>
        /// Gets the <see cref="ImageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static ImageSource GetImage(DependencyObject obj)
        {
            return (ImageSource)obj.GetValue(ImageProperty);
        }

        /// <summary>
        /// Sets the attached <see cref="ImageProperty"/> for a given
        /// <see cref="DependencyObject"/>, which provides an
        /// <see cref="ImageSource" /> for arbitrary WPF elements.
        /// </summary>
        public static void SetImage(DependencyObject obj, ImageSource value)
        {
            obj.SetValue(ImageProperty, value);
        }

        #endregion

        static EyeCandy()
        {

            //register attached dependency property
            var metadata = new PropertyMetadata((ImageSource)null);
            ImageProperty = DependencyProperty.RegisterAttached("Image",
                                                                typeof(ImageSource),
                                                                typeof(EyeCandy), metadata);
        }
    }
}
