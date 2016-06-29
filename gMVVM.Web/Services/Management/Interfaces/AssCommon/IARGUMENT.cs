using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.AssCommon
{
    [ServiceContract]
    public interface IARGUMENT
    {
        [OperationContract]
        IEnumerable<SYS_PARAMETER> GetByAllArgument();

        [OperationContract]
        String InsertArgument(SYS_PARAMETER data);

        [OperationContract]
        String UpdateArgument(SYS_PARAMETER data);

        [OperationContract]
        String DeleteArgument(string id);

        [OperationContract]
        IEnumerable<SYS_PARAMETER> GetByTopArgument(string _top, string _where, string _order);

        [OperationContract]
        String ApproveArgument(SYS_PARAMETER data);
    }
}