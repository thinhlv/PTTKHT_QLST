using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Reflection;
using System.Web;

namespace gMVVM.Web.Services.Management.Implement
{
   
    public partial class BranchLevelContext : AssetDataContext
    {
        [Function(Name = "dbo.CM_BRANCH_Lev")]

        [ResultType(typeof(CM_BRANCH))]

        [ResultType(typeof(CM_BRANCH))]

        [ResultType(typeof(CM_BRANCH))]

        public IMultipleResults GetDetail(string p_Branch_Id)
        {
            object[] paras = { p_Branch_Id };

            IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), paras);

            return (IMultipleResults)result.ReturnValue;
        }
    }
}