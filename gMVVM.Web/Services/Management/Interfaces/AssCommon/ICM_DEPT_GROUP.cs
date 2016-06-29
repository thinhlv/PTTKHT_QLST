using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace gMVVM.Web.Services.Management.Interfaces.AssCommon
{
    [ServiceContract]
    public interface ICM_DEPT_GROUP
    {
        [OperationContract]
        CM_DEPT_GROUP_InsResult CM_DEPT_GROUP_Ins(CM_DEPT_GROUP_SearchResult data);

        [OperationContract]
        CM_DEPT_GROUP_UpdResult CM_DEPT_GROUP_Upd(CM_DEPT_GROUP_SearchResult data);

        [OperationContract]
        CM_DEPT_GROUP_DelResult CM_DEPT_GROUP_Del(string id);

        [OperationContract]
        IEnumerable<CM_DEPT_GROUP_SearchResult> SearchCM_DEPT_GROUP(CM_DEPT_GROUP_SearchResult data, int top);

        [OperationContract]
        CM_DEPT_GROUP_ApprResult CM_DEPT_GROUP_Appr(CM_DEPT_GROUP_SearchResult data);       
    }
}
