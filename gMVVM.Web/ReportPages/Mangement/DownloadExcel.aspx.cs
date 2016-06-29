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

namespace gMVVM.Web.ReportPages
{
    public partial class DownloadExcel : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            string idExcel = Page.Request["idExcel"];
            ExcelPackage pck = new ExcelPackage(new MemoryStream(), Application["ExcelTemplateCaching"] as Stream);
            StreamFileToBrowser(idExcel + "_" + Guid.NewGuid().ToString() + ".xlsx", pck.GetAsByteArray(), idExcel);
        }
        public void StreamFileToBrowser(string sFileName, byte[] fileBytes, string idExcel)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            context.Response.Clear();
            context.Response.ClearHeaders();
            context.Response.ClearContent();
            context.Response.AppendHeader("content-length", fileBytes.Length.ToString());
            context.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=" + idExcel + "_" + Guid.NewGuid().ToString() + ".xlsx");
            context.Response.BinaryWrite(fileBytes);

            // use this instead of response.end to avoid thread aborted exception (known issue):
            // http://support.microsoft.com/kb/312629/EN-US
            context.ApplicationInstance.CompleteRequest();
        }
    }

}