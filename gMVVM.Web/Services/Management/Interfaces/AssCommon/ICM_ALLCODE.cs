using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace gMVVM.Web.Services.Management.Interfaces.AssCommon
{
     [ServiceContract]
    interface ICM_ALLCODE
    {
         [OperationContract]
         IEnumerable<CM_ALLCODE_ByIdResult> CM_ALLCODE_ById(string CDNAME, string CDTYPE);
         [OperationContract]
         CM_ALLCODE_InsResult CM_ALLCODE_Ins(CM_ALLCODE_SearchResult data);

         [OperationContract]
         CM_ALLCODE_UpdResult CM_ALLCODE_Upd(CM_ALLCODE_SearchResult data);

         [OperationContract]
         CM_ALLCODE_DelResult CM_ALLCODE_Del(int id);

         [OperationContract]
         IEnumerable<CM_ALLCODE_By_Type_IdResult> CM_ALLCODE_By_Type_Id(string CDTYPE);

         [OperationContract]
         IEnumerable<CM_ALLCODE_SearchResult> CM_ALLCODE_Search(CM_ALLCODE_SearchResult data, int top);
    }
}
