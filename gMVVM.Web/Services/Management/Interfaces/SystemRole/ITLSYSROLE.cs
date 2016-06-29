using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.SystemRole
{
    [ServiceContract]
    public interface ITLSYSROLE
    {

        [OperationContract]
        IEnumerable<TL_SYSROLE> GetByAllTLSYSROLE();

        [OperationContract]
        String InsertTLSYSROLE(TL_SYSROLE data, List<TL_SYSROLEDETAIL> lstRoleDetail);

        [OperationContract]
        Boolean UpdateTLSYSROLE(TL_SYSROLE data, List<TL_SYSROLEDETAIL> lstUpdate, List<TL_SYSROLEDETAIL> lstInsert, List<string> lstDelete);

        [OperationContract]
        Boolean DeleteTLSYSROLE(TL_SYSROLE data);

        [OperationContract]
        IEnumerable<TL_SYSROLE> GetByTopTLSYSROLE(string _top, string _where, string _order);

        [OperationContract]
        IEnumerable<TL_SYSROLEDETAIL> GetByTopTLROLEDETAIL(string _top, string _where, string _order);
    }
}