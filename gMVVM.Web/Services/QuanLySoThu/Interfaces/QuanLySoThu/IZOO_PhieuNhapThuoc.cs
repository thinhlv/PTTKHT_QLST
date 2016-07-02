using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace gMVVM.Web.Services.QuanLySoThu.Interfaces.QuanLySoThu
{
    [ServiceContract]
    interface IZOO_PhieuNhapThuoc
    {
        [OperationContract]
        ZOO_PHIEUNHAPTHUOC_InsResult LuuPhieuNhap(ZOO_PHIEUNHAPTHUOC data);

        [OperationContract]
        IEnumerable<ZOO_PHIEUNHAPTHUOC_SearchResult> TimPhieuNhap(string maphieu, string tenthuoc, DateTime ngaynhap, int top);

        [OperationContract]
        IEnumerable<ZOO_THUOC> DanhSachThuoc();
    }
}
