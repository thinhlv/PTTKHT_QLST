using gMVVM.gMVVMService;
using gMVVM.Resources;
using mvvmCommon;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace gMVVM.CommonClass
{
    public static class CurrentSystemLogin
    {
        public static TL_USER_SearchResult CurrentUser { get; set; }
        public static Dictionary<string, TL_SYSROLEDETAIL> Roles { get; set; }
        public static Dictionary<string, bool> dicApproveFunction { get; set; }
        public static bool IsAppFunction = true;
        public const string SessionKeys = "CurrentUser";
    }

    //Du lieu phan trang trong datagrid
    public class dataCb
    {
        public int value { get; set; }
    }

    public static class DefineClass
    {
        public static string IconEdit = "/gMVVM;component/Data/Icons/edit_icon.png";
        public static string IconInsert = "/gMVVM;component/Data/Icons/insert_icon.png";        
    }

    public class ImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MemoryStream memStream = new MemoryStream((byte[])value);
            memStream.Seek(0, SeekOrigin.Begin);
            BitmapImage empImage = new BitmapImage();
            empImage.SetSource(memStream);
            return empImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class FormatMoneyVN : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return MyHelper.Format_Price(Math.Round(decimal.Parse(value.ToString()), 0).ToString());
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                value = "0";
            return MyHelper.CoverBackMoney(value.ToString());
        }
    }

    public class MyStaticClass
    {
        
        public static List<String> ChuSo= new List<String>(){" không "," một "," hai "," ba "," bốn "," năm "," sáu "," bảy "," tám "," chín "};
         public static List<String> Tien= new List<String>(){ "", " nghìn", " triệu", " tỷ", " nghìn tỷ", " triệu tỷ"};

         //1. Hàm đọc số có ba chữ số;
        public static String DocSo3ChuSo(int baso)
    {
        int tram;
        int chuc;
        int donvi;
        String KetQua="";
        tram= baso/100;
        chuc=(baso%100)/10;
        donvi=baso%10;
        if(tram==0 && chuc==0 && donvi==0) return "";
        if(tram!=0)
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
            String tmp="";
            var ViTri = new List<Double>() { 0,0,0,0,0,0,0};
            if(SoTien<0) return "Số tiền âm";
            if(SoTien==0) return "Không đồng";
            if(SoTien>0)
            {
                so=SoTien;
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
            so -=ViTri[3] * 1000000000;
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
                tmp = MyStaticClass.DocSo3ChuSo((int)ViTri[i]);
                KetQua += tmp;
                if (ViTri[i] > 0) KetQua += Tien[i];
                if ((i > 0) && (tmp.Length > 0)) KetQua += " ";//&& (!string.IsNullOrEmpty(tmp))
            }
            if (KetQua.Substring(KetQua.Length - 1) == " ")
            {
                KetQua = KetQua.Substring(0, KetQua.Length - 1);
            }
            KetQua = KetQua.Substring(1, 1).ToUpper() + KetQua.Substring(2);
            return KetQua;//.substring(0, 1);//.toUpperCase();// + KetQua.substring(1);
        }
    
      
        public static decimal ValidateMoney(string value)
        {
            decimal temp = 0;
            if (!decimal.TryParse(value, out temp))
                temp = 0;
            return Math.Round(temp, 0);
        }

        public static List<dataCb> DataComboboxPageNumber = new List<dataCb>()
        {
            new dataCb(){value=10},
                new dataCb(){value=15},
                new dataCb(){value=25},
                new dataCb(){value=50}
        };

        public static List<dataCb> DataComboboxSmallPageNumber = new List<dataCb>()
        {
            new dataCb(){value=3},
                new dataCb(){value=5},
                new dataCb(){value=10},
                new dataCb(){value=15}
        };

        public static bool IsCoreNoteMatch(string data)
        {
            return Regex.IsMatch(data, "^[0-9a-zA-Z-,( ),(,),-,.]{0,34}$");
        }

        private static readonly string[] VietnameseSigns = new string[]
        {

            "aAeEoOuUiIdDyY",

            "áàạảãâấầậẩẫăắằặẳẵ",

            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

            "éèẹẻẽêếềệểễ",

            "ÉÈẸẺẼÊẾỀỆỂỄ",

            "óòọỏõôốồộổỗơớờợởỡ",

            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

            "úùụủũưứừựửữ",

            "ÚÙỤỦŨƯỨỪỰỬỮ",

            "íìịỉĩ",

            "ÍÌỊỈĨ",

            "đ",

            "Đ",

            "ýỳỵỷỹ",

            "ÝỲỴỶỸ"

        };

        public static string RemoveSign4VietnameseString(string str)
        {

            //Tiến hành thay thế , lọc bỏ dấu cho chuỗi

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {

                for (int j = 0; j < VietnameseSigns[i].Length; j++)

                    str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);

            }

            return str;
        }

        public static decimal? GetAmortMonth(DateTime? Amort_Start_Date, DateTime? Amort_End_Date)
        {
            DateTime _StartDate = Amort_Start_Date.Value;
            DateTime _EndDate = Amort_End_Date.Value;

            if (_StartDate.Date >= _EndDate.Date) return 0;

            decimal dayStart = _StartDate.AddMonths(1).AddDays(-(_StartDate.Day)).Day;
            decimal dayEnd = _EndDate.AddMonths(1).AddDays(-(_EndDate.Day)).Day;

            decimal? _amortMonth = (dayStart - _StartDate.Day + 1) / dayStart +
                MyStaticClass.DateDiff(_StartDate, _EndDate) + (_EndDate.Day / dayEnd);

            return Math.Round(_amortMonth.Value, 2);
        }

        public static int DateDiff(DateTime StartDate, DateTime EndDate)
        {
            int month = 0;
            int year = 0;
            if (EndDate.Year > StartDate.Year)
            {
                year = EndDate.Year - StartDate.Year;
                DateTime temp = StartDate.AddYears(year);
                if (temp.Month == EndDate.Month)
                    month = (year - 1) * 12 + 11;
                else if (temp.Month < EndDate.Month)
                    month = year * 12 + (EndDate.Month - temp.Month) - 1;
                else
                    month = (year - 1) * 12 + (12 - (temp.Month - EndDate.Month)) - 1;

            }
            else
                month = EndDate.Month - StartDate.Month - 1;
            return month;
        }

    }

    public class MyPost
    {
        public string ReportName { get; set; }
        public string Query { get; set; }
        public string TableName { get; set; }

        public string TypeReport { get; set; }
        public string Type { get; set; }

        public override string ToString()
        {
            return string.Format("?ReportName={0}&Query={1}&TableName={2}&TypeReport={3}&Type={4}",
                ReportName, Query, TableName, TypeReport, Type);
        }
    }

    public class MessageInfo
    {
        public string Type { get; set; }
        public string Level { get; set; }
        public string Index { get; set; }
        public string ObjectType { get; set; }
    }

    public class DeleteItem
    {
        public List<int> lstDelete = new List<int>();
        public bool isAll = false;
    }



}
