using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EduBanking.WebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISessionService" in both code and config file together.
    //[ServiceContract]
    [ServiceContract]
    public interface ISessionService
    {
        [OperationContract]
        Object GetSession(string key);

        [OperationContract]
        Boolean SetSession(string key, object value);
    }
}
