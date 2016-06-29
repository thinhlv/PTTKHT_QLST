using gMVVM.CommonClass;
using mvvmCommon;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Printing;
using System.Windows.Shapes;

namespace gMVVM.ViewModels.Common
{
    public class ActionMenuViewModel : mvvmActionMenuViewModel
    {
        public override void SetEnterAction(ICommand command)
        {
            this.Insert = command;
        }

        public ActionMenuViewModel()
        {
            ActionMenuButton.actionControl = this;
        }

        public override void SetAllAction(ICommand command)
        {
            SetAction(command);
        }

        private void SetAction(ICommand command)
        {
            ActionMenuButton.actionControl.Approve = CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISAPPROVE ? command : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.View = CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISVIEW ? command : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Edit = CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISEDIT ? command : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Insert = CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISINSERT ? command : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Update = CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISUPDATE ? command : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Close = CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISCLOSE ? command : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Search = CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISSEARCH ? command : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Delete = CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISDELETE ? command : ActionMenuButton.actionControl.defaultAction;
        }

        public override void SetAllAction(ICommand insert, ICommand update, ICommand delete, ICommand search, ICommand edit)
        {
            ActionMenuButton.actionControl.Approve = ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.View = ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Edit = edit != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISEDIT ? edit : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Insert = insert != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISINSERT ? insert : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Update = update != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISUPDATE ? update : ActionMenuButton.actionControl.defaultAction; 
            ActionMenuButton.actionControl.Close = ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Search = search != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISSEARCH ? search : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Delete = delete != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISDELETE ? delete : ActionMenuButton.actionControl.defaultAction; 
        }

        public override void SetAllAction(ICommand insert, ICommand update, ICommand delete, ICommand search, ICommand edit, ICommand view, ICommand approve)
        {
            ActionMenuButton.actionControl.Approve = approve != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISAPPROVE ? approve : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.View = view != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISVIEW ? view : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Edit = edit != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISEDIT ? edit : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Insert = insert != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISINSERT ? insert : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Update = update != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISUPDATE ? update : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Close = ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Search = search != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISSEARCH ? search : ActionMenuButton.actionControl.defaultAction;
            ActionMenuButton.actionControl.Delete = delete != null && CurrentSystemLogin.Roles[CurrentSystemInfor.CurrentMenuId].ISDELETE ? delete : ActionMenuButton.actionControl.defaultAction; 
        }
    }
}
