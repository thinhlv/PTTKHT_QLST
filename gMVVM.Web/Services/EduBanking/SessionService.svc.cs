using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;

namespace EduBanking.WebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SessionService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SessionService.svc or SessionService.svc.cs at the Solution Explorer and start debugging.    
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class SessionService : ISessionService
    {
        public object GetSession(string key)
        {
            return System.Web.HttpContext.Current.Session[key];
        }

        public bool SetSession(string key, object value)
        {
            if (System.Web.HttpContext.Current.Session[key] == null)
                System.Web.HttpContext.Current.Session.Add(key, value);
            else
                System.Web.HttpContext.Current.Session[key] = value;

            return true;
        }
    }
}
