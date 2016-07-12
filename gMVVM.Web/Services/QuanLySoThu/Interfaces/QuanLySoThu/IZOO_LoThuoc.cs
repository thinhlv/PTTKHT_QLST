using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace gMVVM.Web.Services.QuanLySoThu.Interfaces.QuanLySoThu
{
    [ServiceContract]
    interface IZOO_LoThuoc
    {
        [OperationContract]
        ZOO_LOTHUOC_InsResult ThemLoThuocMoi(ZOO_LOTHUOC data);
        [OperationContract]
        IEnumerable<ZOO_LOTHUOC_SearchResult> TimLoThuoc(string solo, string tenthuoc);
        [OperationContract]
        IEnumerable<ZOO_LOTHUOC_SearchResult> TimLoThuocCombobox(string mathuoc);
        [OperationContract]
        ZOO_LOTHUOC_UpdResult ChinhSuaLoThuoc(ZOO_LOTHUOC data);
    }
}
