using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.KeHoach.Interfaces.GhiNhanKeHoach
{
     [ServiceContract]
    public interface IGhiNhan
    {
        [OperationContract]
         string BaoCaoGhiNhan(string data);
    }
}