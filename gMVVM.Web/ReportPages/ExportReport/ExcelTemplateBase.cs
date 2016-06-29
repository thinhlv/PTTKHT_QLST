using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Aspose.Cells;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using gMVVM.Web.ReportPages.ExportReport;

/// <summary>
/// Author: HTG
/// htangiau12th01@gmail.com
/// </summary>
namespace STB.QLPCD.Web.TemplateController
{
    /// <summary>
    /// Dùng validate format file excel
    /// Các thông số cần khởi tạo: RowStart,ColumStart,TotalColum,Model,PropertiesNotValidate
    /// Thông số Input: MemoryStream
    /// Thông số Output: ModelProperties
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExcelTemplateBase<T> where T : class
    {
        #region "Properties"
        /// <summary>
        /// Dòng bắt đầu đọc file excel, tính từ dòng chứa các field name(cần khởi tạo khi sử dụng) 
        /// </summary>
        public int RowStart = 1;
        /// <summary>
        /// Cột bắt đầu đọc file excel, tính từ cột A trong file excel(cần khởi tạo khi sử dụng) 
        /// </summary>
        public int ColumStart = 0;
        /// <summary>
        /// Tỗng số cột cần đọc của 1 dòng(cần khởi tạo khi sử dụng) 
        /// </summary>
        public int TotalColum = 13;
        /// <summary>
        /// Cần khởi tạo Model trước khi sử dụng(cần khởi tạo khi sử dụng) 
        /// </summary>
        /// <value>
        /// Model.
        /// </value>
        public T Model { get; set; }
        //
        /// <summary>
        /// Cần khởi tạo khi không cần validate danh sách cột
        /// </summary>
        /// <value>
        /// Danh sách properties không cần validate.
        /// </value>
        public List<string> PropertiesNotValidate { get; set; }
        // Danh sach KeyValue cac thuoc tinh , Mã lỗi
        /// <summary>
        /// Chứa danh  sách Model{Properties - Value, IsError, ErrorMessage khi validate}
        /// </summary>
        /// <value>
        /// List of Model
        /// </value>
        public List<ModelProperties> LstModelProperties { get; set; }
        /// <summary>
        /// Chứa các lỗi khi không khởi tạo
        /// </summary>
        /// <value>
        /// The error common.
        /// </value>
        public String ErrorCommon { get; set; }
        public DataTable source { get; set; }
        public List<string> PropertiesMustValidate { get; set; }
        #endregion
        #region "Method"

        /// <summary>
        /// Validates format file excel import.
        /// còn warning iserror cua model luôn có giá trị true
        /// </summary>
        /// <param name="memoryStream">The memory stream.</param>
        /// <returns></returns>
        public bool ValidateFormatImport(Stream memoryStream)
        {
            List<T> ObjectInput = new List<T>();
            Workbook workbook = new Workbook(memoryStream);
         //   workbook.Settings.Region = CountryCode.VietNam;
            // Lay Sheet dau tien
            Worksheet worksheet = workbook.Worksheets[0];
           // DataTable source = new DataTable();
            bool isRecordEmpty;
            LstModelProperties =  new List<ModelProperties>();
            #region "Validate Properties"
            //if (this.RowStart == default(int) )
            //{
            //    ErrorCommon = "Chưa set dòng đọc";
            //    return false;
            //}
            //if (this.ColumStart == default(int))
            //{
            //    ErrorCommon = "Chưa set cột đọc";
            //    return false;
            //}
            if (this.TotalColum == default(int)) { this.TotalColum = 13; }
            #endregion
            if(PropertiesMustValidate != null)
            {
                // Doc danh sach cot file excel
                DataTable dt = worksheet.Cells.ExportDataTable(this.RowStart, this.ColumStart, this.RowStart, this.TotalColum, true);
                bool isError = false;
                // Neu nhung truong bat buoc validate không có trong file excel, thông báo lỗi
                ErrorCommon = "";
                if (dt != null)
                {
                    foreach (string colmstValid in this.PropertiesMustValidate)
                    {
                        if (!dt.Columns.Contains(colmstValid))
                        {
                            ErrorCommon += String.Format("File excel bị thiếu cột {0};", colmstValid);
                            isError = true;
                        }
                    }
                    if (isError)
                        return false;
                }
                else
                {
                    ErrorCommon = "File Rỗng";
                    return false;
                }
            }
            worksheet.Cells.ExportDataTable(source, this.RowStart, this.ColumStart, worksheet.Cells.MaxDataRow - 1 + 1, true);
            if(source.Rows.Count == 0)
            {
                ErrorCommon = "File Rỗng";
                return false;
            }
           
            // Duyet tung dong excel, do du lieu vao ObjectInput
            for (int i = 0; i < source.Rows.Count; i++)
            {
                isRecordEmpty = true;
                #region  Bỏ qua nếu Dòng rỗng
                foreach (DataColumn item in source.Columns)
                {
                    if (!String.IsNullOrEmpty(source.Rows[i][item.ColumnName].ToString().Trim()))
                    {
                        isRecordEmpty = false;
                        break;
                    }
                }
                if (isRecordEmpty == true)
                    continue;
                #endregion
                ModelProperties model = new ModelProperties()
                {
                    KeyValue = new Dictionary<string, object>(),
                    ErrorList = new List<ExcelErrorObject>()
                };
                foreach (PropertyInfo property in Model.GetType().GetProperties())
                {
                    // Loai bo nhung cot khong can gan du lieu
                    #region "Validate"
                    var pro = property.Name;
                    bool isNotValidate = this.PropertiesNotValidate.Contains(property.Name);
                    
                    // Bo qua nhung property ko co trong file excel
                    if (!source.Columns.Contains(property.Name))
                        continue;
                    var value = source.Rows[i][property.Name];
                    var ErrorMessageCurrent = "";
                    // rang buoc kieu du lieu
                    switch (property.PropertyType.FullName.ToLower())
                    {
                        case "system.string":
                            try
                            {
                                model.KeyValue.Add(property.Name, value);
                                if (!isNotValidate)
                                {
                                    // không cho phép rỗng
                                    if (string.IsNullOrEmpty(value.ToString()) || value == null)
                                    {
                                        model.IsError = true;
                                        ErrorMessageCurrent = "Không cho phép nhập rỗng";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                ErrorMessageCurrent = String.Format("Lỗi không xác định: {0}", ex.Message);
                                model.IsError = true;
                            }
                            break;
                        case "system.datetime":
                            try
                            {
                                //Dữ liệu nhập khác rỗng --> Convert
                                if (isNotValidate)
                                {
                                    //var datevalue = (DateTime)value;
                                    var datevalue =
                                        DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                    if (datevalue == new DateTime(1900, 1, 1)){
                                        model.KeyValue.Add(property.Name, null);
                                    }
                                    else{
                                        model.KeyValue.Add(property.Name, datevalue);
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(value.ToString()) && value != null)
                                    {
                                        var datevalue =
                                         DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        
                                     //   var datevalue = (DateTime)value;
                                        if (datevalue == new DateTime(1900, 1, 1)){
                                            model.IsError = true;
                                            ErrorMessageCurrent = "Vui lòng nhập ngày";
                                        }
                                        else {
                                            model.KeyValue.Add(property.Name, datevalue);
                                        }

                                    }
                                    else{
                                        //Nếu rỗng, set là lỗi
                                        model.IsError = true;
                                        ErrorMessageCurrent = "Vui lòng nhập ngày";
                                    }
                                }
                            }
                            catch (Exception) //Dữ liệu nhập không đúng format kiểu DateTime
                            {
                                model.IsError = true;
                                ErrorMessageCurrent = "Định dạng phải là ngày tháng(dd/mm/yyyy)";
                            }
                            break;
                        case "system.decimal":
                            try
                            {
                                if (!isNotValidate)
                                {
                                    //Dữ liệu nhập khác rỗng --> Convert
                                    if (!string.IsNullOrEmpty(value.ToString()) && value != null){
                                        if (value.ToString().Trim().Equals("-"))
                                            value = 0;
                                       // decimal val = Convert.ToDecimal(value);
                                        decimal val = decimal.Parse(value.ToString(), NumberStyles.AllowParentheses | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowTrailingSign | NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.Number,CultureInfo.InvariantCulture);
                                        if (val < 0)
                                        {
                                             model.IsError = true;
                                             ErrorMessageCurrent = "Không được phép nhập số âm";
                                        }
                                        model.KeyValue.Add(property.Name, val);
                                    }
                                    else{
                                        //Nếu rỗng, set là lỗi
                                        model.IsError = true;
                                        ErrorMessageCurrent = "Chưa nhập số liệu";
                                    }
                                }
                                else
                                    model.KeyValue.Add(property.Name, Convert.ToDecimal(value));
                            }
                            catch (Exception) //Dữ liệu nhập không đúng format kiểu decimal
                            {
                                model.IsError = true;
                                ErrorMessageCurrent = "Định dạng phải là số";
                            }
                            break;
                        case "system.int32":
                            try
                            {
                                if (!isNotValidate)
                                {
                                    //Dữ liệu nhập khác rỗng --> Convert
                                    if (!string.IsNullOrEmpty(value.ToString()) && value != null){
                                        model.KeyValue.Add(property.Name, Convert.ToInt32(value));
                                    }
                                    else{
                                        //Nếu rỗng, set là lỗi
                                        model.IsError = true;
                                        ErrorMessageCurrent = "Chưa nhập số liệu";
                                    }
                                }
                                else
                                    model.KeyValue.Add(property.Name, Convert.ToInt32(value));
                            }
                            catch (Exception) //Dữ liệu nhập không đúng format kiểu int32
                            {
                                model.IsError = true;
                                ErrorMessageCurrent = "Định dạng phải là số";
                            }
                            break;
                        case "system.boolean":
                            try
                            {
                                if (!isNotValidate)
                                {
                                    //Dữ liệu nhập khác rỗng --> Convert
                                    if (!string.IsNullOrEmpty(value.ToString()) && value != null){
                                        model.KeyValue.Add(property.Name, Convert.ToInt32(value));
                                    }
                                    else{
                                        //Nếu rỗng, set là lỗi
                                        model.IsError = true;
                                        ErrorMessageCurrent = "Chưa nhập True/False";
                                    }
                                }
                                else
                                    model.KeyValue.Add(property.Name, Convert.ToInt32(value));
                            }
                            catch (Exception) //Dữ liệu nhập không đúng format kiểu int32
                            {
                                model.IsError = true;
                                ErrorMessageCurrent = "Định dạng phải là True/False";
                            }
                            break;
                        // Nullable<Decimal>
                        case "system.nullable`1[[system.decimal, mscorlib, version=4.0.0.0, culture=neutral, publickeytoken=b77a5c561934e089]]":
                            try
                            {

                                //Dữ liệu nhập khác rỗng --> Convert
                                if (!isNotValidate)
                                {
                                    if (!string.IsNullOrEmpty(value.ToString()) && value != null )
                                    {
                                        if (value.ToString().Trim().Equals("-"))
                                            value = 0;
                                       // decimal val = Convert.ToDecimal(value);
                                        decimal val = decimal.Parse(value.ToString(), NumberStyles.AllowParentheses | NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint | NumberStyles.AllowTrailingSign | NumberStyles.Float | NumberStyles.AllowDecimalPoint | NumberStyles.Number,CultureInfo.InvariantCulture);
                                        if (val < 0)
                                        {
                                            model.IsError = true;
                                            ErrorMessageCurrent = "Không được phép nhập số âm";
                                        }
                                        model.KeyValue.Add(property.Name, val);
                                    }
                                    else
                                    {

                                        //Nếu rỗng, set là lỗi
                                        model.IsError = true;
                                        ErrorMessageCurrent = "Chưa nhập số liệu";
                                    }
                                }
                                else
                                    model.KeyValue.Add(property.Name, Convert.ToDecimal(value));

                            }
                            catch (Exception) //Dữ liệu nhập không đúng format kiểu decimal
                            {
                                model.IsError = true;
                                ErrorMessageCurrent = "Định dạng phải là số";
                            }
                            break;
                        case "system.nullable`1[[system.datetime, mscorlib, version=4.0.0.0, culture=neutral, publickeytoken=b77a5c561934e089]]":
                            try
                            {
                                //Dữ liệu nhập khác rỗng --> Convert
                                if (isNotValidate)
                                {
                                    //var datevalue = (DateTime)value;
                                    var datevalue =
                                        DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        if (datevalue == new DateTime(1900, 1, 1)){
                                            model.KeyValue.Add(property.Name, null);
                                        }
                                        else{
                                            model.KeyValue.Add(property.Name, datevalue);
                                        }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(value.ToString()) && value != null)
                                    {
                                        var datevalue = //(DateTime)value;
                                        DateTime.ParseExact(value.ToString(), "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                        if (datevalue == new DateTime(1900, 1, 1)){
                                            model.IsError = true;
                                            ErrorMessageCurrent = "Vui lòng nhập ngày";
                                        }
                                        else{
                                            model.KeyValue.Add(property.Name, datevalue);
                                        }

                                    }
                                    else
                                    {
                                        //Nếu rỗng, set là lỗi
                                        model.IsError = true;
                                        ErrorMessageCurrent = "Vui lòng nhập ngày";
                                    }
                                }
                            }
                            catch (Exception) //Dữ liệu nhập không đúng format kiểu DateTime
                            {
                                model.IsError = true;
                                ErrorMessageCurrent = "Định dạng phải là ngày tháng(dd/mm/yyyy)";
                            }
                            break;
                        //{System.Nullable`1[System.Int32] NamHieuLuc}
                        case "system.nullable`1[[system.int32, mscorlib, version=4.0.0.0, culture=neutral, publickeytoken=b77a5c561934e089]]":
                            try
                            {
                                if (!isNotValidate)
                                {
                                    //Dữ liệu nhập khác rỗng --> Convert
                                    if (!string.IsNullOrEmpty(value.ToString()) && value != null)
                                    {
                                        model.KeyValue.Add(property.Name, Convert.ToInt32(value));
                                    }
                                    else
                                    {
                                        //Nếu rỗng, set là lỗi
                                        model.IsError = true;
                                        ErrorMessageCurrent = "Chưa nhập số liệu";
                                    }
                                }
                                else
                                    model.KeyValue.Add(property.Name, Convert.ToInt32(value));
                            }
                            catch (Exception) //Dữ liệu nhập không đúng format kiểu int32
                            {
                                model.IsError = true;
                                ErrorMessageCurrent = "Định dạng phải là số";
                            }
                            break;
                    }
                    if (model.IsError)
                    {
                        model.ErrorList.Add(new ExcelErrorObject()
                        {
                            Key = property.Name
                            ,
                            Row = (i + 2 + RowStart).ToString()
                            ,
                            Column = property.Name
                            ,
                            ErrorMessage = ErrorMessageCurrent
                        });
                    }

                    #endregion
                }
                // Add vao LstModelProperties
                LstModelProperties.Add(model);
            }
            return true;
        }
        /// <summary>
        /// Gets file is valid.
        /// </summary>
        /// <returns></returns>
        public bool GetIsValid()
        {
            foreach(var model in LstModelProperties)
            {
                if (model.IsError)
                    return false;
            }
            return true;
        }
        #endregion

    }
}