using Aspose.Cells;
using Aspose.Cells.Tables;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace gMVVM.Web.ExportReport.CodeExport
{
    /// <summary>
    /// Dùng để xuất template excel(thường dùng cho xuất báo cáo)
    /// htangiau12th01@gmail.com
    /// </summary>
    public class ExcelTemplateExportBase
    {
        #region "Properties"
      
        public Dictionary<string,object> SmartmarkersObjData{get;set;}
        /// <summary>
        /// Lưu trữ thông tin đường dẫn teplate
        /// </summary>
        /// <value>
        /// The template path.
        /// </value>
        public String TemplatePath { get; set; }
        public String ErrorMessage { get; set; }
        #endregion
        #region Method
        /// <summary>
        /// Xuất báo cáo theo Smartmarkers
        /// </summary>
        /// <returns></returns>
        public  MemoryStream ExprortSmartmarkers()
        {
            MemoryStream dstStream = new MemoryStream();
            WorkbookDesigner designer = new WorkbookDesigner()
            {
                Workbook = new Workbook(this.TemplatePath)
            };
            #region [Smart markers]
            foreach (var smark in this.SmartmarkersObjData)
            {
                designer.SetDataSource(smark.Key, smark.Value);
            }
            designer.Process();
            #endregion [Smart markers]
            designer.Workbook.CalculateFormula();
            designer.Workbook.Save(dstStream, SaveFormat.Xlsx);
            return dstStream;
        }
        #endregion
    }
}