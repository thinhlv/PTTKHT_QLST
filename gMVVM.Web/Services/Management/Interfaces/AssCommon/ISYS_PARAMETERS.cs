using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.AssCommon
{
    [ServiceContract]
    public interface ISYS_PARAMETERS
    {
        [OperationContract]
        SYS_PARAMETERS_ByIdResult GetParameterById(string paraKey);
    }
}