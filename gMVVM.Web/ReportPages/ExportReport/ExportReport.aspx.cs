using gMVVM.Web.ReportPages.AssetMangement.GenerateData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace gMVVM.Web.ReportPages
{
    public partial class ExportReportSql : System.Web.UI.Page
    {
        // private bool Flag = true;
        protected void Page_Load(object sender, EventArgs e)
        {
              load_Report();
        }
        private void load_Report()
        {
            string reportName = Request["ReportName"];
            if (reportName == null)
                return;
            string storeName = Request["StoreName"];
            if (storeName == null)
                return;
            string hasValue = Request["HasValue"];
            string titleName = Request["TitleName"];
            string typeExport = Request["TypeExport"];

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Dictionary<string, string> ParametersReport = new Dictionary<string, string>();
            foreach (String key in Request.QueryString.AllKeys)
            {
                if (key != "ReportName" && key != "StoreName" && key != "IsMuiltiesStore"
                    && key != "HasValue" && key != "TitleName" && key != "IsPrintTemp"
                    && key != "IsExportExcel" && key != "PathExport" && key != "IsExportExcelOld" && key != "IsExportNew" && key != "TypeExport"
                    )
                {
                    //Lenght Key > 5 char
                    string key_compare = "pfrp_";
                    if (key.Substring(0, key_compare.Length).Equals(key_compare))
                        ParametersReport.Add(key.Substring(
                            key_compare.Length
                            , key.Length - key_compare.Length), Request[key]);
                    else
                        parameters.Add(key, Request[key]);
                }
            }
            //funcExportReport(reportName, storeName, parameters, ParametersReport, typeExport);
            return;
        }
    }
}
