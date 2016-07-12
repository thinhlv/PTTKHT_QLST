using gMVVM.Web.Services.QuanLySoThu;
using gMVVM.Web.Services.QuanLySoThu.Interfaces.QuanLySoThu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace gMVVM.Web.Services.QuanLySoThu.Implement
{
    public class ImplementInterface : IZOO_BaoCaoTonKho, IZOO_PhieuNhapThuoc, IZOO_LoThuoc
    {
        public const string formatDate = "dd/MM/yyyy HH:mm:ss";

        #region [Public Functions]

        public int GetDayOfYear(int year)
        {
            return (new DateTime(year + 1, 1, 1) - new DateTime(year, 1, 1)).Days;
        }

        public string SetAutoId(string id, int maxlength)
        {
            while (id.Length < maxlength)
            {
                id = "0" + id;
            }
            return id;
        }

        #endregion

        #region IZOO_BaoCaoTonKho
        public IEnumerable<ZOO_BAOCAOTONKHO_SearchResult> LayBaoCaoTonKho(string maThuoc, string tenThuoc, DateTime ngayHetHang)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<ZOO_BAOCAOTONKHO_SearchResult> result = dataContext.ZOO_BAOCAOTONKHO_Search(maThuoc, tenThuoc, ngayHetHang == null ? DateTime.Now.ToString(formatDate) : ngayHetHang.ToString(formatDate), 0).ToList();

                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region IZOO_PhieuNhapThuoc
        public ZOO_PHIEUNHAPTHUOC_InsResult LuuPhieuNhap(ZOO_PHIEUNHAPTHUOC data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    ZOO_PHIEUNHAPTHUOC_InsResult result = dataContext.ZOO_PHIEUNHAPTHUOC_Ins(
                        data.MaLo,
                        data.SoLuong,
                        data.NgayNhap == null ? "" : data.NgayNhap.Value.ToString(formatDate),

                        data.NOTES,
                        data.RECORD_STATUS,
                        data.MAKER_ID,
                        data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate),
                        data.AUTH_STATUS,
                        data.CHECKER_ID,
                        data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)
                        ).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ZOO_PHIEUNHAPTHUOC_InsResult() { Result = "-1", ErrorDesc = ex.Message, MaPhieuNhap = "" };
            }
        }

        public IEnumerable<ZOO_PHIEUNHAPTHUOC_SearchResult> TimPhieuNhap(string maphieu, string tenthuoc, DateTime ngaynhap, int top)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<ZOO_PHIEUNHAPTHUOC_SearchResult> result = dataContext.ZOO_PHIEUNHAPTHUOC_Search(maphieu, tenthuoc, ngaynhap == null ? "" : ngaynhap.ToString(formatDate), top).ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<ZOO_THUOC> DanhSachThuoc()
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    var result = dataContext.ZOO_THUOCs.ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ZOO_PHIEUNHAPTHUOC_UpdResult ChinhSuaPhieuNhap(ZOO_PHIEUNHAPTHUOC data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    ZOO_PHIEUNHAPTHUOC_UpdResult result = dataContext.ZOO_PHIEUNHAPTHUOC_Upd(
                        data.MaPhieuNhap,
                        data.MaLo,
                        data.SoLuong,
                        data.NgayNhap == null ? "" : data.NgayNhap.Value.ToString(formatDate),
                        data.NOTES,
                        data.RECORD_STATUS,
                        data.MAKER_ID,
                        data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate),
                        data.AUTH_STATUS,
                        data.CHECKER_ID,
                        data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)
                        ).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ZOO_PHIEUNHAPTHUOC_UpdResult() { Result = "-1", ErrorDesc = ex.Message, MaPhieuNhap = "" };
            }
        }
        #endregion

        #region IZOO_LOTHUOC
        public ZOO_LOTHUOC_InsResult ThemLoThuocMoi(ZOO_LOTHUOC data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    ZOO_LOTHUOC_InsResult result = dataContext.ZOO_LOTHUOC_Ins(
                        data.MaThuoc,
                        data.SoLo,
                        data.NgaySanXuat == null ? "" : data.NgaySanXuat.Value.ToString(formatDate) ,
                        data.NgayHetHan == null ? "" : data.NgayHetHan.Value.ToString(formatDate),
                        data.SoLuong,


                        data.NOTES,
                        data.RECORD_STATUS,
                        data.MAKER_ID,
                        data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate),
                        data.AUTH_STATUS,
                        data.CHECKER_ID,
                        data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)
                        ).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ZOO_LOTHUOC_InsResult() { Result = "-1", ErrorDesc = ex.Message, MaLo = "" };
            }
        }

        public IEnumerable<ZOO_LOTHUOC_SearchResult> TimLoThuoc(string solo, string tenthuoc)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<ZOO_LOTHUOC_SearchResult> result = dataContext.ZOO_LOTHUOC_Search("", "", tenthuoc, solo,"", "", 0).ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<ZOO_LOTHUOC_SearchResult> TimLoThuocCombobox(string mathuoc)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    IEnumerable<ZOO_LOTHUOC_SearchResult> result = dataContext.ZOO_LOTHUOC_Search("", mathuoc, "", "", "", "", 0).ToList();
                    return result;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public ZOO_LOTHUOC_UpdResult ChinhSuaLoThuoc(ZOO_LOTHUOC data)
        {
            try
            {

                using (var dataContext = new AssetDataContext())
                {
                    ZOO_LOTHUOC_UpdResult result = dataContext.ZOO_LOTHUOC_Upd(
                        data.MaLo,
                        data.MaThuoc,
                        data.SoLo,
                        data.NgaySanXuat == null ? "" : data.NgaySanXuat.Value.ToString(formatDate),
                        data.NgayHetHan == null ? "" : data.NgayHetHan.Value.ToString(formatDate),
                        data.SoLuong,

                        data.NOTES,
                        data.RECORD_STATUS,
                        data.MAKER_ID,
                        data.CREATE_DT == null ? "" : data.CREATE_DT.Value.ToString(formatDate),
                        data.AUTH_STATUS,
                        data.CHECKER_ID,
                        data.APPROVE_DT == null ? "" : data.APPROVE_DT.Value.ToString(formatDate)
                        ).FirstOrDefault();

                    return result;
                }
            }
            catch (Exception ex)
            {
                return new ZOO_LOTHUOC_UpdResult() { Result = "-1", ErrorDesc = ex.Message, MaLo = "" };
            }
        }
        #endregion

        /// <summary>
        /// Goi Store thong bang parameter
        /// </summary>
        /// <typeparam name="T">Doi tuong store tra ve</typeparam>
        /// <param name="parameters">Doi so truyen vao store</param>
        /// <returns></returns>
        public IEnumerable<T> ExecuteStoredProcedure<T>(List<gParam> parameters)
        {
            try
            {
                using (var dataContext = new AssetDataContext())
                {
                    Type genericType = typeof(T);

                    string commandsthing = genericType.Name.Replace("Result", " ");
                    int count = parameters.Count();
                    string[] param = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        if (i > 0) commandsthing += ", ";
                        commandsthing += parameters[i].Name + "={" + i.ToString() + "} ";
                        param[i] = parameters[i].Value == null ? "" : parameters[i].Value;
                    }

                    return dataContext.ExecuteQuery<T>(commandsthing, param).ToList<T>();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }






        


        
    }

    public class gParam
    {
        public gParam(string Name, string Value)
        {
            this.Name = Name;
            this.Value = Value;
        }
        public string Name { get; set; }
        public string Value { get; set; }
    }

}