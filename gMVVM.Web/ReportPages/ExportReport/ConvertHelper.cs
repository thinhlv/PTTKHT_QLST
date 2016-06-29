using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using System.ComponentModel;

namespace gMVVM.Web.ReportPages.ExportReport
{
    public partial class ConvertHelper
    {
        public static DataTable ListToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
            {
                //Setting column names as Property names
                dataTable.Columns.Add(prop.Name);
            }
            foreach (T item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    //inserting property values to datatable rows
                    if (Props[i].GetType().Name.ToLower() != "runtimepropertyinfo")
                        values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }
        public static DataTable ToDataTable<T>(List<T> iList, string Name = "default", bool isInsert = true)
        {
            DataTable dataTable = new DataTable() { TableName = Name };
            PropertyDescriptorCollection propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T));
            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                //Type type = propertyDescriptor.PropertyType;
                //if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                //    type = Nullable.GetUnderlyingType(type);

                // gan type of string het
                dataTable.Columns.Add(propertyDescriptor.Name, typeof(string));
            }
            object[] values = new object[propertyDescriptorCollection.Count];
            foreach (T iListItem in iList)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                    Type type = propertyDescriptor.PropertyType;
                    if (type.FullName.ToLower() == "system.nullable`1[[system.datetime, mscorlib, version=4.0.0.0, culture=neutral, publickeytoken=b77a5c561934e089]]"
                        || type.FullName.ToLower() == "system.datetime")
                    {
                        var obj = propertyDescriptorCollection[i].GetValue(iListItem);
                        if (obj != null && (DateTime)obj != DateTime.MinValue)
                            values[i] = (isInsert) ? String.Format("{0:yyyy-MM-dd}", obj) : String.Format("{0:dd/MM/yyyy}", obj);
                        else
                            values[i] = null;
                    }

                    else
                        values[i] = propertyDescriptorCollection[i].GetValue(iListItem);
                }
                dataTable.Rows.Add(values);
            }
            return dataTable;
        }
        public static DataTable ToDataTable<T>(T model)
        {
            DataTable dataTable = new DataTable();
            PropertyDescriptorCollection propertyDescriptorCollection =
                TypeDescriptor.GetProperties(typeof(T));
            for (int i = 0; i < propertyDescriptorCollection.Count; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                dataTable.Columns.Add(propertyDescriptor.Name, typeof(string));
            }
            object[] values = new object[propertyDescriptorCollection.Count];
            for (int i = 0; i < values.Length; i++)
            {
                PropertyDescriptor propertyDescriptor = propertyDescriptorCollection[i];
                Type type = propertyDescriptor.PropertyType;
                if (type.FullName.ToLower() == "system.nullable`1[[system.datetime, mscorlib, version=4.0.0.0, culture=neutral, publickeytoken=b77a5c561934e089]]"
                    || type.FullName.ToLower() == "system.datetime")
                {
                    var obj = propertyDescriptorCollection[i].GetValue(model);
                    if (obj != null && (DateTime)obj != DateTime.MinValue)
                        values[i] = String.Format("{0:dd/MM/yyyy}", obj);
                    else
                        values[i] = null;
                }

                else
                    values[i] = propertyDescriptorCollection[i].GetValue(model);
            }
            dataTable.Rows.Add(values);

            return dataTable;
        }
        //public static String ToMoneyText<T>(T item)
        //{

        //}
        // ham doc tien thanh so
        public static List<String> ChuSo = new List<String>() { " không ", " một ", " hai ", " ba ", " bốn ", " năm ", " sáu ", " bảy ", " tám ", " chín " };
        public static List<String> Tien = new List<String>() { "", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ" };

        //1. Hàm đọc số có ba chữ số;
        public static String DocSo3ChuSo(int baso)
        {
            int tram;
            int chuc;
            int donvi;
            String KetQua = "";
            tram = baso / 100;
            chuc = (baso % 100) / 10;
            donvi = baso % 10;
            if (tram == 0 && chuc == 0 && donvi == 0) return "";
            if (tram != 0)
            {
                KetQua += ChuSo[tram] + " trăm ";
                if ((chuc == 0) && (donvi != 0)) KetQua += " linh ";
            }
            if ((chuc != 0) && (chuc != 1))
            {
                KetQua += ChuSo[chuc] + " mươi";
                if ((chuc == 0) && (donvi != 0)) KetQua = KetQua + " linh ";
            }
            if (chuc == 1) KetQua += " mười ";
            switch (donvi)
            {
                case 1:
                    if ((chuc != 0) && (chuc != 1))
                    {
                        KetQua += " mốt ";
                    }
                    else
                    {
                        KetQua += ChuSo[donvi];
                    }
                    break;
                case 5:
                    if (chuc == 0)
                    {
                        KetQua += ChuSo[donvi];
                    }
                    else
                    {
                        KetQua += " lăm ";
                    }
                    break;
                default:
                    if (donvi != 0)
                    {
                        KetQua += ChuSo[donvi];
                    }
                    break;
            }
            return KetQua;
        }

        //2. Hàm đọc số thành chữ (Sử dụng hàm đọc số có ba chữ số)

        public static String DocTienBangChu(Double SoTien)
        {
            int lan = 0;
            int i = 0;
            Double so = 0;
            String KetQua = "";
            String tmp = "";
            var ViTri = new List<Double>() { 0, 0, 0, 0, 0, 0, 0 };
            if (SoTien < 0) return "Số tiền âm";
            if (SoTien == 0) return "Không đồng";
            if (SoTien > 0)
            {
                so = SoTien;
            }
            else
            {
                so = -SoTien;
            }
            if (SoTien > 8999999999999999)
            {
                //SoTien = 0;
                return "Số quá lớn!";
            }
            ViTri[5] = Math.Floor(so / 1000000000000000);
            so -= ViTri[5] * 1000000000000000;
            ViTri[4] = Math.Floor(so / 1000000000000);
            so -= ViTri[4] * 1000000000000;
            ViTri[3] = Math.Floor(so / 1000000000);
            so -= ViTri[3] * 1000000000;
            ViTri[2] = so / 1000000;
            ViTri[1] = (so % 1000000) / 1000;
            ViTri[0] = so % 1000;
            if (ViTri[5] > 1)
            {
                lan = 5;
            }
            else if (ViTri[4] > 1)
            {
                lan = 4;
            }
            else if (ViTri[3] > 1)
            {
                lan = 3;
            }
            else if (ViTri[2] > 1)
            {
                lan = 2;
            }
            else if (ViTri[1] > 1)
            {
                lan = 1;
            }
            else
            {
                lan = 0;
            }
            for (i = lan; i >= 0; i--)
            {
                tmp = ConvertHelper.DocSo3ChuSo((int)ViTri[i]);
                KetQua += tmp;
                if (ViTri[i] > 0) KetQua += Tien[i];
                if ((i > 0) && (tmp.Length > 0)) KetQua += " ";//&& (!string.IsNullOrEmpty(tmp))
            }
            if (KetQua.Substring(KetQua.Length - 1) == " ")
            {
                KetQua = KetQua.Substring(0, KetQua.Length - 1);
            }
            KetQua = KetQua.Substring(1, 2).ToUpper() + KetQua.Substring(2);
            return KetQua;//.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
        }
    }
}