using gMVVM.Web.Services.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace gMVVM.Web.Services.Management.Interfaces.QuanLySoThu
{
    [ServiceContract]
    public interface IZOO_BaoCaoTonKho
    {
        [OperationContract]
        IEnumerable<ZOO_BAOCAOTONKHO_SearchResult> LayBaoCaoTonKho(string maThuoc, string tenThuoc, DateTime ngayHetHang);
    }
}
