using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using gMVVM.Web.Reports.Management;
using System.Data;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using gMVVM.Web.ReportPages.AssetMangement.GenerateData;
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;
using ZXing;
using System.Drawing;

namespace gMVVM.Web.ReportPages.ExportReport
{
    public partial class DownloadExcel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string idReport = Page.Request["idReport"];
            string Type = Page.Request["Type"];
            MemoryStream streanm = Application["TemplateCaching"] as MemoryStream;
            byte[] data = streanm.ToArray();
            StreamFileToBrowser(data, idReport, Type);
        }
        public void StreamFileToBrowser(byte[] fileBytes, string idReport,string typeReport)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.ClearContent();
            context.Response.AppendHeader("content-length", fileBytes.Length.ToString());
            if(typeReport=="E")
            {
                Response.AddHeader("content-disposition", "attachment;  filename=" + idReport + "_" + Guid.NewGuid().ToString() + ".xlsx");
                context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                
            }
            else
            {
                Response.AddHeader("content-disposition", "attachment;  filename=" + idReport + "_" + Guid.NewGuid().ToString() + ".doc");
                Response.ContentType = "application/ms-word";
               
            }
                
            context.Response.BinaryWrite(fileBytes);
            context.ApplicationInstance.CompleteRequest();
            
        }
    }

}