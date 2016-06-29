using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gMVVM.Web.Services.Management.Functions
{
    public class MyHelper
    {
        public static bool Copy<T>(T result, T data)
        {
            if (result == null || data == null) return false;
            var list = result.GetType().GetProperties();
            foreach (var prop in list)
                prop.SetValue(result, prop.GetValue(data, null), null);
            return true;
        }
    }

}