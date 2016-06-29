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
    public class ValidateReport
    {
        public static bool isNULL(string text)
        {
            return (text == null || text == "");
        }
       
        public static bool isDateTimeGreaterThan(DateTime source, DateTime destionation)
        {
            return (source.CompareTo(destionation) > 0);
        }
        public static string getDefaultBranchCode()
        {
            return CurrentSystemLogin.CurrentUser.BRANCH_CODE;
        }
        public static string getDefalutBranchId()
        {
            return CurrentSystemLogin.CurrentUser.TLSUBBRID;
        }
        public static string getDefalutBranchName()
        {
            return CurrentSystemLogin.CurrentUser.BRANCH_NAME;
        }
        public static DateTime getDefalutDate()
        {
            return DateTime.Now;
        }
        public static string getDelaultYear()
        {
            return DateTime.Now.Year.ToString();
        }
    }
}
