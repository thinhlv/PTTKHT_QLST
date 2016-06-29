using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gMVVM.Web.ReportPages.ExportReport
{
    public class ExcelErrorObject
    {
        /// <summary>
        /// Lưu trữ Khóa chính thông báo lỗi, thường là property của object
        /// </summary>
        /// <value>
        /// Khóa chính thông báo lỗi, thường là property của object
        /// </value>
        public string Key { get; set; }
        /// <summary>
        ///   Lưu trữ lỗi nếu có
        /// </summary>
        /// <value>
        /// The error message.
        /// </value>
        public string ErrorMessage { get; set; }
        /// <summary>
        /// Lưu trữ dòng bị lỗi trong file excel nếu có
        /// </summary>
        /// <value>
        /// The row.
        /// </value>
        public string Row { get; set; }
        /// <summary>
        /// Lưu trữ cột bị lỗi trong file excel
        /// </summary>
        /// <value>
        /// The column.
        /// </value>
        public string Column { get; set; }
        public override string ToString()
        {
            return (String.IsNullOrEmpty(ErrorMessage)) ? null : String.Format("Cột {2} lỗi: {0}", ErrorMessage, Row, Column);
        }
    }
}