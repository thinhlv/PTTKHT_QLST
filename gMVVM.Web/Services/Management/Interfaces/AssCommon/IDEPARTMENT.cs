using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.AssCommon
{
    [ServiceContract]
    public interface IDEPARTMENT
    {
        [OperationContract]
        CM_DEPARTMENT_InsResult InsertDEPARTMENT(CM_DEPARTMENT_SearchResult data);

        [OperationContract]
        CM_DEPARTMENT_UpdResult UpdateDEPARTMENT(CM_DEPARTMENT_SearchResult data);

        [OperationContract]
        CM_DEPARTMENT_DelResult DeleteDEPARTMENT(string id);

        [OperationContract]
        IEnumerable<CM_DEPARTMENT_SearchResult> SearchDEPARTMENT(CM_DEPARTMENT_SearchResult data, int top);

        [OperationContract]
        CM_DEPARTMENT_AppResult ApproveDEPARTMENT(CM_DEPARTMENT_SearchResult data);

        [OperationContract]
        IEnumerable<CM_DEPARTMENT_LstResult> CM_DEPARTMENT_Lst();

    }
}