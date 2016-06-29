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
    public class DelegateCommand<T> : ICommand
    {
        private readonly Action<T> _command;

        public DelegateCommand(Action<T> command)
        {
            _command = command;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _command((T)parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
