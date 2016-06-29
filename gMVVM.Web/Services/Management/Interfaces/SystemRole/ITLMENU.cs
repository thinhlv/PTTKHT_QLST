using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.SystemRole
{
    [ServiceContract]
    public interface ITLMENU
    {
               
        [OperationContract]
        IEnumerable<TL_MENU> GetByAllTLMENU();

        [OperationContract]
        String InsertTLMENU(TL_MENU data);

        [OperationContract]
        Boolean UpdateTLMENU(TL_MENU data);

        [OperationContract]
        Boolean DeleteTLMENU(TL_MENU data);

        [OperationContract]
        IEnumerable<TL_MENU> GetByTopTLMENU(string _top, string _where, string _order);

        [OperationContract]
        IEnumerable<TL_MENU> GetParentTLMENU();
    }
}