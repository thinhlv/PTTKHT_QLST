using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gMVVM.Web.ReportPages.ExportReport
{
    /// <summary>
    /// Chứa danh sách property của object, có lỗi hay không, danh sách lỗi theo từng property của object
    /// </summary>
    public class ModelProperties
    {
        /// <summary>
        /// Từ điển lưu key là property, Value là giá trị của property
        /// </summary>
        /// <value>
        /// The key value.
        /// </value>
        public Dictionary<string, object> KeyValue { get; set; }
        public bool IsError { get; set; }
        /// <summary>
        /// Danh sách lỗi theo từng property của object
        /// </summary>
        /// <value>
        /// The error list.
        /// </value>
        public List<ExcelErrorObject> ErrorList { get; set; }
    }
}