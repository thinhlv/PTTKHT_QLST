using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.AssCommon
{
    [ServiceContract]
    public interface IBranch
    {
        [OperationContract]
        IEnumerable<CM_BRANCH> GetByAllBranch();

        [OperationContract]
        String InsertBranch(CM_BRANCH data);

        [OperationContract]
        String UpdateBranch(CM_BRANCH data);

        [OperationContract]
        String DeleteBranch(string id);

        [OperationContract]
        IEnumerable<CM_BRANCH> GetByTopBranch(string _top, string _where, string _order);

        [OperationContract]
        String ApproveBranch(CM_BRANCH data);

        [OperationContract]
        IEnumerable<CM_BRANCH_GETALLCHILDResult> GetByParentBranchID(string parentId);

        [OperationContract]
        BranchLevel CM_BRANCH_Level(string branchId);

        [OperationContract]
        IEnumerable<CM_BRANCH_SearchResult> BranchSearch(CM_BRANCH_SearchResult data, string branchLogin, int? top);
    }

    public partial class BranchLevel
    {
        public List<CM_BRANCH> RegionList { get; set; }
        public List<CM_BRANCH> BranchList { get; set; }
        public List<CM_BRANCH> TransList { get; set; }
    }
}