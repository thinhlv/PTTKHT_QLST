using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using Aspose.Words;
using System.IO;
namespace gMVVM.Web.ExportReport.CodeExport
{
    /// <summary>
    /// Dùng để xuất file word theo template (thường dùng cho xuất báo cáo)
    /// htangiau12th01@gmail.com
    /// </summary>
    public class WordTemplateBase
    {
        /// <summary>
        /// Lưu trữ thông tin cần thiết để xuất báo cáo
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public DataTable Data { get; set; }
        /// <summary>
        /// Đường dẫn đến template cần xuất báo cáo
        /// </summary>
        /// <value>
        /// The template path.
        /// </value>
        public String TemplatePath { get; set; }
        public String ErrorMessage { get; set; }
        /// <summary>
        /// Xuất báo cáo theo phương pháp MailMerge
        /// </summary>
        /// <returns>file và tên file</returns>
        public MemoryStream ExcuteMailMergeSimple()
        {
            MemoryStream dstStream = new MemoryStream();
            try
            {
                if (string.IsNullOrEmpty(TemplatePath))
                {
                    ErrorMessage = "Vui lòng set đường dẫn file template";
                    return null;
                }
                if (Data == null || Data.Rows.Count == 0)
                {
                    ErrorMessage = "Dữ liệu rỗng";
                    return null;
                }
                Document doc = new Document(TemplatePath);
               // doc.MailMerge.ExecuteWithRegions(Data);
                DataView orderDetailsView = new DataView(Data);
                doc.MailMerge.Execute(orderDetailsView);
                doc.Save(dstStream, SaveFormat.Doc);
            }
            catch
            {
                return null;
            }
            return dstStream;
        }
        /// <summary>Xuất báo cáo theo Mail Merge dạng table
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<string, MemoryStream> ExcuteMailMergeTableSimple()
        {
            MemoryStream dstStream = new MemoryStream();
            if (string.IsNullOrEmpty(TemplatePath))
            {
                ErrorMessage = "Vui lòng set đường dẫn file template";
                return new KeyValuePair<string, MemoryStream>();
            }
            if (Data == null || Data.Rows.Count == 0)
            {
                ErrorMessage = "Dữ liệu rỗng";
                return new KeyValuePair<string, MemoryStream>();
            }
            Document doc = new Document(TemplatePath);
            // doc.MailMerge.ExecuteWithRegions(Data);
            DataView orderDetailsView = new DataView(Data);
            doc.MailMerge.ExecuteWithRegions(orderDetailsView);
            doc.Save(dstStream, SaveFormat.Docx);
            return new KeyValuePair<string, MemoryStream>(String.Format("Export_{0}", DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss")), dstStream);
        }
    }
}