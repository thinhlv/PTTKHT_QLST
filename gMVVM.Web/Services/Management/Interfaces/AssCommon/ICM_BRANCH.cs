using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.AssCommon
{
    [ServiceContract]
    public interface ICM_BRANCH
    {
        [OperationContract]
        IEnumerable<CM_BRANCH_ListResult> CM_BRANCH_List();        
    }   
}
