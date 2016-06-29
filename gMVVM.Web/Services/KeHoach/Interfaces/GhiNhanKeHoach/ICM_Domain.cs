using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.KeHoach.Interfaces.GhiNhanKeHoach
{
    [ServiceContract]
    public interface ICM_Domain
    {
        [OperationContract]
        CM_DOMAIN_InsResult InsertDomain(CM_DOMAIN data);
        CM_DOMAIN_UpdResult UpdateDomain(CM_DOMAIN data);
    }
}