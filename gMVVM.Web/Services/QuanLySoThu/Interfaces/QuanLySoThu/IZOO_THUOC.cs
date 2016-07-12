using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace gMVVM.Web.Services.QuanLySoThu.Interfaces.QuanLySoThu
{
    [ServiceContract]
    interface IZOO_THUOC
    {
        [OperationContract]
        IEnumerable<ZOO_THUOC_SearchResult> TimThuoc(string mathuoc, string tenthuoc, string tinhtrang);

        [OperationContract]
        ZOO_THUOC_InsResult ThemThuocMoi(ZOO_THUOC data);

        [OperationContract]
        ZOO_THUOC_UpdResult CapNhatThuoc(ZOO_THUOC data);
    }
}
