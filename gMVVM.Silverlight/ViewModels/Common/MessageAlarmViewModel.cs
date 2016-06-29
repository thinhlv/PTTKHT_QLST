using mvvmCommon;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace gMVVM.ViewModels.Common
{
    public class MessageAlarmViewModel : ViewModelBase 
    {
        public MessageAlarmViewModel()
        {
            this.lstError = new List<MessageInfo>();
            this.dptime = new DispatcherTimer();
            this.dptime.Interval = TimeSpan.FromSeconds(3);
            this.dptime.Tick += new EventHandler(someSecond);
            this.actionCommand = new ActionButton(this);
        }

        private DispatcherTimer dptime;

        #region [Properties]

        //visible Error
        private string isError = Visibility.Collapsed.ToString();
        public string  IsError 
        {
            get
            {
                return this.isError;
            }

            set
            {
                this.isError = value;
                this.OnPropertyChanged("IsError");
            }
        }

        //visible Successful
        private string isSuccessful = Visibility.Collapsed.ToString();
        public string IsSuccessful
        {
            get
            {
                return this.isSuccessful;
            }

            set
            {
                this.isSuccessful = value;
                this.OnPropertyChanged("IsSuccessful");
            }
        }

        //visible Warning
        private string isWarning = Visibility.Collapsed.ToString();
        public string IsWarning
        {
            get
            {
                return this.isWarning;
            }

            set
            {
                this.isWarning = value;
                this.OnPropertyChanged("IsWarning");
            }
        }

        //List error message
        private List<MessageInfo> lstError;
        public List<MessageInfo> LstError 
        {
            get 
            {
                return this.lstError;
            }

            set
            {
                this.lstError = value;
                this.OnPropertyChanged("LstError");
            }
        }

        private ICommand actionCommand;
        public ICommand ActionCommand { get { return this.actionCommand; } }

        #endregion

        #region [Function]

        private void someSecond(object sender, EventArgs e)
        {
            this.IsSuccessful = Visibility.Collapsed.ToString();
            this.dptime.Stop();
        }

        //Set list error - call show error by HasError() function
        public void SetError(string message)
        {
            this.lstError.Add(new MessageInfo() { MessageText = message });
        }

        public void SetSingleError(string message)
        {
            this.lstError.Add(new MessageInfo() { MessageText = message });
            this.IsSuccessful = Visibility.Collapsed.ToString();
            this.IsWarning = Visibility.Collapsed.ToString();
            this.IsError = Visibility.Visible.ToString();
            this.OnPropertyChanged("LstError");
        }

        public bool HasError()
        {
            if (this.lstError.Count > 0)
            {
                if (!this.isError.Equals(Visibility.Visible.ToString()))
                {
                    this.IsSuccessful = Visibility.Collapsed.ToString();
                    this.IsWarning = Visibility.Collapsed.ToString();
                    this.IsError = Visibility.Visible.ToString();
                }
                this.OnPropertyChanged("LstError");
                return true;
            }
            else
            {
                this.IsError = Visibility.Collapsed.ToString();
                return false;
            }
        }

        public void Reset()
        {
            this.IsError = Visibility.Collapsed.ToString();
            this.IsSuccessful = Visibility.Collapsed.ToString();
            this.IsWarning = Visibility.Collapsed.ToString();
            this.lstError = new List<MessageInfo>();
            this.OnPropertyChanged("LstError");
        }

        public void ClearError()
        {
            this.lstError = new List<MessageInfo>();
        }

        public void Successful(string message)
        {
            //this.dptime.Stop();
            this.lstError.Add(new MessageInfo() { MessageText = message });
            this.IsSuccessful = Visibility.Visible.ToString();
            this.IsWarning = Visibility.Collapsed.ToString();
            this.IsError = Visibility.Collapsed.ToString();
            this.OnPropertyChanged("LstError");
            //this.dptime.Start();
        }

        public void Warning(string message)
        {
            this.lstError.Add(new MessageInfo() { MessageText = message });
            this.IsError = Visibility.Collapsed.ToString();
            this.IsSuccessful = Visibility.Collapsed.ToString();
            this.IsWarning = Visibility.Visible.ToString();
            this.OnPropertyChanged("LstError");
        }

        #endregion

        public class MessageInfo : ViewModelBase
        {            
            public string MessageText { get; set; }            
        }

        //Command button action
        public class ActionButton : ICommand
        {

            private MessageAlarmViewModel viewModel;
            public ActionButton(MessageAlarmViewModel viewModel)
            {
                this.viewModel = viewModel;
                this.viewModel.PropertyChanged += (s, e) =>
                {
                    if (this.CanExecuteChanged != null)
                    {
                        this.CanExecuteChanged(this, new EventArgs());
                    }
                };
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged;

            public void Execute(object parameter)
            {
                switch (parameter.ToString())
                {
                    case "DeleteMsg": this.viewModel.Reset(); break;                    
                    default: break;
                }

            }
        }        
    }
}
