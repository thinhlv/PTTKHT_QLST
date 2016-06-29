using gMVVM;
using gMVVM.Views.SystemRole;
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
using gMVVM.gMVVMService;
using System.Collections.ObjectModel;
using System.Windows.Data;
using gMVVM.Resources;
using GalaSoft.MvvmLight.Messaging;
using System.IO;
using System.Collections.Generic;
using gMVVM.CommonClass;
using gMVVM.ViewModels.Common;
using JeffWilcox.Utilities.Silverlight;
using System.Runtime.InteropServices.Automation;
using Lite.ExcelLibrary.SpreadSheet;
using System.Threading;
using System.Windows.Threading;
using mvvmCommon;
using System.Linq;
namespace gMVVM.ViewModels.Common
{
    public class ProfileViewModel:ViewModelBase
    {       
        public ProfileViewModel()
        {             
             Messenger.Default.Register<TL_USER_SearchResult>(this, EditSendMessageRecieve);
             this.actionCommand = new ActionButton(this);
        }

        private void EditSendMessageRecieve(TL_USER_SearchResult obj)
        {
            this.CurrentUser = new TL_USER_SearchResult();
            this.CurrentUser = obj;
        }
                                     
        #region[All Properties]                     
        private ICommand actionCommand;
        public ICommand ActionCommand
        {
            get
            {
                return this.actionCommand;
            }

            set
            {
                this.actionCommand = value;
                this.OnPropertyChanged("ActionCommand");
            }
        }

        private TL_USER_SearchResult currentUser;
        public TL_USER_SearchResult CurrentUser
        {
            get
            {
                return this.currentUser;
            }

            set
            {
                this.currentUser = value;
                this.OnPropertyChanged("CurrentUser");                
            }
        }       

      
        //RoleData
        private ObservableCollection<TL_SYSROLE> roleData;
        public ObservableCollection<TL_SYSROLE> RoleData
        {
            get
            {
                return this.roleData;
            }

            set
            {
                this.roleData = value;
                this.OnPropertyChanged("RoleData");
            }
        }

        private string tlName = "";
        public string TLName
        {
            get
            {
                return this.tlName;
            }

            set
            {
                this.tlName = value;
                this.OnPropertyChanged("TLName");                
            }
        }


        private string roleName = "";
        public string RoleName
        {
            get
            {
                return this.roleName;
            }

            set
            {
                this.roleName = value;
                this.OnPropertyChanged("RoleName");                
            }
        }
        #endregion     

        public void ClosePopup()
        {
            Messenger.Default.Send(true);
        }

        public class ActionButton : ICommand
        {

            private ProfileViewModel viewModel;
            public ActionButton(ProfileViewModel viewModel)
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
                    case "ClosePopup": this.viewModel.ClosePopup(); break;
                    default: break;
                }

            }
        }

    }

    //Command button action
    
}
