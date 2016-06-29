using gMVVM.Resources;
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

namespace gMVVM.CommonClass
{
    public static class ListMessage
    {
        public const string True = "True";
        public const string False = "False";
        public static Dictionary<string, string> Message = new Dictionary<string, string>() 
        {                
            {"NotExisted",CommonResource.NotExisted},
            {"Existed",CommonResource.Existed}
        };
    }   
}
