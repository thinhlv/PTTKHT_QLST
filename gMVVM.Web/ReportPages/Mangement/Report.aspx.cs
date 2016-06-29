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
using System.IO;
using System.Windows.Forms;
using OfficeOpenXml;
using ZXing;
using System.Drawing;
using gMVVM.Web.ReportPages.AssetMangement.GenerateData;

namespace gMVVM.Web.ReportPages
{
    public partial class Report_SQL : System.Web.UI.Page
    {
        private ReportDataSet rpds_demo;
        private ReportDocument cryRpt;
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = null;
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
            string isPrintTemp = Request["IsPrintTemp"];
            string isExportExcel = Request["IsExportExcel"];
            string isExportExcelOld = Request["IsExportExcelOld"];
            string IsExportNew = Request["IsExportNew"];
            string isIsMuiltiesStore = Request["IsMuiltiesStore"];
            // Stream pathEx Request["PathExport"];

           

            if (isIsMuiltiesStore == "true" || isIsMuiltiesStore == "True")
            {
                this.ReportMultiesTable();
                return;
            }
          
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            Dictionary<string, string> ParametersReport = new Dictionary<string, string>();
            foreach (String key in Request.QueryString.AllKeys)
            {
                if (key != "ReportName" && key != "StoreName" && key != "IsMuiltiesStore"
                    && key != "HasValue" && key != "TitleName" && key != "IsPrintTemp"
                    && key != "IsExportExcel" && key != "PathExport" && key != "IsExportExcelOld" && key != "IsExportNew" && key != "TypeExport" && key != "IsExportNew"
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
            if (IsExportNew == "true" || IsExportNew == "True")
            {
                funcExportReport(reportName, storeName, parameters, ParametersReport, typeExport);
                return;
            }
            if ((isExportExcel == "True" || isExportExcel == "true") && isExportExcelOld != "Y")
            {
                // string reportNameWithoutExtension = reportName.Substring(0, reportName.IndexOf('.'));
                exportExcelTemplate(reportName, storeName, parameters, ParametersReport);
                return;
            }
           
            load_rpt_FILE(reportName, storeName, parameters, ParametersReport, hasValue, titleName, isPrintTemp, isExportExcel);
        }
        private bool funcExportReport(string reportName, string storeName,
          Dictionary<string, string> parameterStore,
          Dictionary<string, string> parameterExcel, string Type
          )
        {
            try
            {
                ExportReportCalulate Reporttmp = new ExportReportCalulate();
                Reporttmp.ReportName = reportName;
                Reporttmp.StoreName = storeName;
                Reporttmp.HasParameterStore = true;
                Reporttmp.StoreParameterValue = parameterStore;
                Reporttmp.ParameterReport = parameterExcel;

                string error = "";
                if (Type == "E")
                {
                    Reporttmp.FilePath = Server.MapPath("~/ReportPages/ExportReport/Template/" + reportName + ".xlsx");
                    error = Reporttmp.exportToExcel();
                }
                else
                {
                    Reporttmp.FilePath = Server.MapPath("~/ReportPages/ExportReport/Template/" + reportName + ".docx");
                    error = Reporttmp.exportToWord();
                }

                if (error != null)
                    throw new Exception(error);
                Application["TemplateCaching"] = Reporttmp.PackageStream;
                Response.Redirect("~/ReportPages/ExportReport/DownloadExcel.aspx?idReport=" + Reporttmp.ReportName + "&Type=" + Type,false);

            }
            catch (Exception ex)
            {
                Label1.Text = ex.Message;
                return false;
            }
            return true;
        }
        private void ReportMultiesTable()
        {
            try
            {
                Dictionary<string, Dictionary<string, string>> LstValues = new Dictionary<string, Dictionary<string, string>>();
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                Dictionary<string, string> ParametersReport = new Dictionary<string, string>();
                string key_compare = "pfrp_";
                string storeKey = "_stna_";
                int _index = 0;
                string storeName ="";
                string _keyParam = "";
                foreach (String key in Request.QueryString.AllKeys)
                {
                    if (key != "ReportName" && key != "StoreName" && key != "IsMuiltiesStore"
                        && key != "HasValue" && key != "TitleName" && key != "IsPrintTemp"
                        && key != "IsExportExcel" && key != "PathExport" && key != "IsExportExcelOld" && key != "IsExportNew" && key != "TypeExport"
                        )
                    {
                        //Lenght Key > 5 char                        
                        if (key.Substring(0, key_compare.Length).Equals(key_compare))
                            ParametersReport.Add(key.Substring(
                                key_compare.Length
                                , key.Length - key_compare.Length), Request[key]);
                        else
                        {
                            _index = key.IndexOf(storeKey);
                            storeName = key.Substring(0,_index);
                            _keyParam = key.Substring(_index + 6, key.Length - _index - 6);
                            if (!LstValues.ContainsKey(storeName))
                                LstValues.Add(storeName, new Dictionary<string, string>());
                            LstValues[storeName].Add(_keyParam, Request[key]);                            
                        }                        
                    }
                }

                //Goi load report
                AssetMangement.GenerateData.ConnectData sqlData = new AssetMangement.GenerateData.ConnectData();
                rpds_demo = new ReportDataSet();
                try
                {
                    foreach (var store_name in LstValues.Keys)
                    {
                        sqlData.Paramerters = new List<string>();
                        sqlData.ParametersType = new List<SqlDbType>();
                        sqlData.ParamertersValue = new List<string>();

                        if (Request["HasValue"] == "True")
                        {
                            foreach (KeyValuePair<string, string> pair in LstValues[store_name])
                            {
                                sqlData.Paramerters.Add("@" + pair.Key);
                                sqlData.ParametersType.Add(SqlDbType.VarChar);
                                sqlData.ParamertersValue.Add(pair.Value);
                            }
                        }

                        //add paramerters cho store proc
                        //rpds_demo = new ReportDataSet();
                        bool isLoad = sqlData.Read_Store(store_name, true);
                        if (isLoad)
                        {
                            try
                            {
                                DataTable dt = sqlData.DataSource;
                                dt.TableName = storeKey + store_name;
                                rpds_demo.Tables.Add(dt);
                            }
                            catch (Exception ex)
                            {
                                Label1.Text = ex.Message;
                            }
                        }
                        else
                        {
                            Label1.Text = "Không kết nối được";
                        }
                    }

                }
                catch (Exception ex)
                {
                    Label1.Text = "Lỗi data: " + ex.Message;
                    return;
                }
                finally
                {
                    GC.Collect();
                }

                try
                {
                    string reportPath = Server.MapPath("/Reports/Management/" + Request["ReportName"]);
                    cryRpt = new ReportDocument();
                    cryRpt.Load(reportPath);
                    cryRpt.SummaryInfo.ReportTitle = Request["TitleName"];
                    foreach (var keyStoreName in LstValues.Keys)
                    {
                        //reportDocument.OpenSubreport("rptSubReport1.rpt").SetDataSource(dt1);
                        // reportDocument.OpenSubreport("rptSubReport2JNR.rpt").SetDataSource(dt2);
                        cryRpt.Database.Tables[keyStoreName].SetDataSource(rpds_demo.Tables[storeKey + keyStoreName]);
                    }

                    if (ParametersReport.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> pair in ParametersReport)
                        {
                            ParameterDiscreteValue pa = new ParameterDiscreteValue();
                            pa.Value = pair.Value;
                            ParameterFieldDefinition crParameterFieldDefinition = cryRpt.DataDefinition.ParameterFields[pair.Key];
                            crParameterFieldDefinition.CurrentValues.Clear();
                            crParameterFieldDefinition.CurrentValues.Add(pa);
                            crParameterFieldDefinition.ApplyCurrentValues(crParameterFieldDefinition.CurrentValues);
                        }

                    }

                    CrystalReportViewer.ReportSource = cryRpt;

                }
                catch (Exception exr)
                {
                    Label1.Text = "Lỗi parameter Report: " + exr.Message;
                    return;
                }
                finally
                {
                    GC.Collect();
                }
            }
            catch (Exception ex)
            {
                Label1.Text = "Lỗi:" + ex.Message;
                return;
            }
        }


        private void load_rpt_FILE(String reportName, String storeName, Dictionary<string, string> paramerter,
            Dictionary<string, string> parameterReport = null
            , String hasValue = "false", String title = "Tiêu đề", String isPrintTemp = "False",
            String isExportExcel = "False")
        {
            //ReportDB.exportDataToExcel("ABC", null, "OIUYTR");
            //return;

            AssetMangement.GenerateData.ConnectData sqlData = new AssetMangement.GenerateData.ConnectData();
            if (hasValue == "True")
            {
                foreach (KeyValuePair<string, string> pair in paramerter)
                {
                    sqlData.Paramerters.Add("@" + pair.Key);
                    sqlData.ParametersType.Add(SqlDbType.VarChar);
                    sqlData.ParamertersValue.Add(pair.Value);
                }
            }

            // add paramerters cho store proc
            rpds_demo = new ReportDataSet();
            bool isLoad = sqlData.Read_Store(storeName, true);
            if (isExportExcel == "True" || isExportExcel == "true")
            {
                // HyperLink1.NavigateUrl = HttpContext.Current.Request.PhysicalApplicationPath + "ReportPages/";
                // return;

                string tieude = Request["TitleName"];

                DataTable dt = new DataTable();
                string reportNameWithoutExtension = reportName.Substring(0, reportName.IndexOf('.'));
                dt = sqlData.DataSource;
                Dictionary<string, string> listNewColumnName = ReportDB.ReadXML(reportNameWithoutExtension);
                if (listNewColumnName == null)
                {
                    Label1.Text = "Lỗi đọc XML";
                    return;
                }
                foreach (KeyValuePair<string, string> pair in listNewColumnName)
                {
                    //   values += "&" + pair.Key + "=" + pair.Value;
                    // check name load from xml
                    if (dt.Columns.Contains(pair.Key) && !dt.Columns.Contains(pair.Value))
                        dt.Columns[pair.Key].ColumnName = pair.Value;
                }
                //         string path =  ReportDB.SaveAs(reportNameWithoutExtension);
                string path = HttpContext.Current.Request.PhysicalApplicationPath + "ReportPages/" + reportNameWithoutExtension + ".xls";
                bool isComplete = ReportDB.exportDataToExcel(tieude, dt, path);
                Label1.Text = "Xuất file excel cho report " + tieude;
                if (isComplete)
                {
                    Label1.Text += " thành công ! ";// + "<a id=\"linkdownload\" href=\"" + path + "\">Tải file</a>";
                    HyperLink1.NavigateUrl = "../" + reportNameWithoutExtension + ".xls";
                    HyperLink1.Text = "Tải file";

                    // System.Diagnostics.Process.Start(path);                       

                }
                else
                    Label1.Text += " thất bại !";

                // Label1.Text = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\" + "reportName" + ".xls";                

                return;
            }
            try
            {
                if (isLoad)
                {
                    try
                    {
                        DataTable dt = sqlData.DataSource;
                        if (isPrintTemp.Equals("True"))
                            dt = getDataPrintTemp(dt, parameterReport.First().Value);
                        dt.TableName = "Newrpt";
                        rpds_demo.Tables.Add(dt);
                    }
                    catch (Exception ex)
                    {
                        Label1.Text = ex.Message;
                    }
                }
                else
                {
                    Label1.Text = "Không kết nối được";
                }
            }
            catch (Exception ex)
            {
                Label1.Text = "Lỗi data: " + ex.Message;
                return;
            }
            finally
            {
                GC.Collect();
            }
            try
            {
                string reportPath = Server.MapPath("/Reports/Management/" + reportName);
                cryRpt = new ReportDocument();
                cryRpt.Load(reportPath);
                cryRpt.SummaryInfo.ReportTitle = title;
                cryRpt.SetDataSource(rpds_demo.Tables["Newrpt"]);
                if (parameterReport.Count > 0)
                {
                    foreach (KeyValuePair<string, string> pair in parameterReport)
                    {
                        ParameterDiscreteValue pa = new ParameterDiscreteValue();
                        pa.Value = pair.Value;
                        ParameterFieldDefinition crParameterFieldDefinition = cryRpt.DataDefinition.ParameterFields[pair.Key];
                        crParameterFieldDefinition.CurrentValues.Clear();
                        crParameterFieldDefinition.CurrentValues.Add(pa);
                        crParameterFieldDefinition.ApplyCurrentValues(crParameterFieldDefinition.CurrentValues);
                    }

                }

                //cryRpt.PrintOptions.PrinterName = "Fax";
                //cryRpt.PrintToPrinter(1, false, 0, 0);
                //CrystalReportViewer.ReportSource = cryRpt;

                //System.Drawing.Printing.PrintDocument pDoc = new System.Drawing.Printing.PrintDocument();
                // CrystalDecisions.Shared.PrintLayoutSettings PrintLayout = new CrystalDecisions.Shared.PrintLayoutSettings();
                //System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                // printerSettings.PrinterName = "Default printer";
                // don't use this, use the new button
                //PrintLayout.Scaling = PrintLayoutSettings.PrintScaling.DoNotScale;

                //System.Drawing.Printing.PageSettings pSettings = new System.Drawing.Printing.PageSettings(printerSettings);
                //rpt.PrintOptions.DissociatePageSizeAndPrinterPaperSize = false;
                //cryRpt.PrintOptions.PrinterDuplex = PrinterDuplex.Simplex;
                //System.Drawing.Printing.PageSettings pageSettings = new System.Drawing.Printing.PageSettings(printerSettings);
                //if (cryRpt.DefaultPageSettings.PaperSize.Height > cryRpt.DefaultPageSettings.PaperSize.Width)
                //{
                //  cryRpt.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
                // rptClientDoc.PrintOutputController.ModifyPaperOrientation(CrPaperOrientationEnum.crPaperOrientationPortrait);
                //}
                //else
                //{
                //    cryRpt.PrintOptions.DissociatePageSizeAndPrinterPaperSize = true;
                //    //rptClientDoc.PrintOutputController.ModifyPaperOrientation(CrPaperOrientationEnum.crPaperOrientationLandscape);
                //}
                //cryRpt.PrintToPrinter(printerSettings, pageSettings, false, PrintLayout);

                CrystalReportViewer.ReportSource = cryRpt;

            }
            catch (Exception exr)
            {
                Label1.Text = "Lỗi parameter Report: " + exr.Message;
                return;
            }
            finally
            {
                GC.Collect();
            }

            //  ReportDB.exportDataToExcel("demo", sqlData.DataSource);

        }

        private bool exportExcelTemplate(string idExcel, string storeName,
           Dictionary<string, string> parameterStore,
           Dictionary<string, string> parameterExcel
           )
        {
            try
            {
                ExcelTemplate Exceltmp = new ExcelTemplate();
                Exceltmp.IdExcel = idExcel;
                Exceltmp.StoreName = storeName;
                Exceltmp.HasParameterStore = true;
                Exceltmp.HasParameterExcel = true;
                Exceltmp.StoreParameterValue = parameterStore;
                Exceltmp.ParameterExcel = parameterExcel;
                Exceltmp.ExcelTemplateFile = Server.MapPath("./ExcelTemplate/" + idExcel + ".xlsx");
                string error = Exceltmp.exportToExcel();
                if (error != null)
                    throw new Exception(error);

                Application["ExcelTemplateCaching"] = Exceltmp.ExcelPackageStream;
                Response.Redirect("DownloadExcel.aspx?idExcel=" + idExcel,false);
                //Response.Clear();

                //Response.BinaryWrite(pck.GetAsByteArray());
                //Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //Response.AddHeader("content-disposition", "attachment;  filename=" + idExcel + "_" + Guid.NewGuid().ToString() + ".xlsx");
                //Response.End();
                //   Label1.Text = "Export báo cáo " + idExcel + " thành công!";
            }
            catch (Exception ex)
            {
                Label1.Text = ex.Message;
                return false;
            }
            return true;
        }

        private static byte[] GenerateQrCode(string data)
        {
            var writer = new BarcodeWriter { Format = BarcodeFormat.QR_CODE };
            var result = writer.Write(data);
            writer.Options.Height = 300;
            writer.Options.Width = 300;
            writer.Options.Margin = 0;
            return Report_SQL.ImageToByte2(new Bitmap(result));
        }
        public static byte[] ImageToByte2(Bitmap img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }

        private DataTable getDataPrintTemp(DataTable dtIn, string Qty)
        {
            int _Qty = int.Parse(Qty);
            DataTable dt = new DataTable();
            dt.Columns.Add("TSCD", typeof(string));
            dt.Columns.Add("MSTS", typeof(string));
            dt.Columns.Add("TGSD", typeof(DateTime));
            dt.Columns.Add("DVSD", typeof(string));
            dt.Columns.Add("DEPT_CODE", typeof(string));            

            for (int i = 0; i < dtIn.Rows.Count; i++)
            {
                for (int j = 0; j < _Qty; j++)
                {
                    dt.Rows.Add(dtIn.Rows[i]["TYPE_ID"].ToString(),
                        dtIn.Rows[i]["ASSET_CODE"].ToString(),
                        DateTime.Parse(dtIn.Rows[i]["USE_DATE"].ToString()).Date,
                        dtIn.Rows[i]["DVSD"].ToString(),
                        dtIn.Rows[i]["DEPT_CODE"].ToString()
                        );
                }
            }
            DataTable dt1 = new DataTable();
            dt1.Columns.Add("TSCD1", typeof(string));
            dt1.Columns.Add("TSCD2", typeof(string));
            dt1.Columns.Add("MSTS1", typeof(string));
            dt1.Columns.Add("MSTS2", typeof(string));
            dt1.Columns.Add("TGSD1", typeof(DateTime));
            dt1.Columns.Add("TGSD2", typeof(DateTime));
            dt1.Columns.Add("DVSD1", typeof(string));
            dt1.Columns.Add("DVSD2", typeof(string));
            dt1.Columns.Add("BarCode1", typeof(byte[]));
            dt1.Columns.Add("BarCode2", typeof(byte[]));
            dt1.Columns.Add("DEPT_CODE1", typeof(string));
            dt1.Columns.Add("DEPT_CODE2", typeof(string));

            int rowcount = dt.Rows.Count;
            int row = rowcount / 2;
            string barcode1 = "";
            string barcode2 = "";

            //if (row * 2 + 1 == rowcount)
            //{
            //    barcode1 = dt.Rows[row * 2][1] + ";" + dt.Rows[row * 2][2]
            //       + ";" + dt.Rows[row * 2][4] + ";" + dt.Rows[row * 2][0];
            //    dt1.Rows.Add(dt.Rows[row * 2][0],
            //        null, dt.Rows[row * 2][1], null,
            //        dt.Rows[row * 2][2], null,
            //        dt.Rows[row * 2][3], null,
            //         GenerateQrCode(barcode1),
            //        null,
            //        dt.Rows[row * 2][4],
            //        null
            //        );
            //}
            //else
            //{
                //ASS_CODE;USE_DATE;BRANCH_ID;TYPE_ID
                for (int i = 0; i < row; i++)
                {
                    barcode1 = dt.Rows[i * 2][1] + ";" + dt.Rows[i * 2][2]
                        + ";" + dt.Rows[i * 2][4] + ";" + dt.Rows[i * 2][0];
                    barcode2 = dt.Rows[i * 2 + 1][1] + ";" + dt.Rows[i * 2 + 1][2] +
                        ";" + dt.Rows[i * 2 + 1][4] + ";" + dt.Rows[i * 2 + 1][0];
                    dt1.Rows.Add(
                        dt.Rows[i * 2][0],
                        dt.Rows[i * 2 + 1][0],
                        dt.Rows[i * 2][1],
                        dt.Rows[i * 2 + 1][1],
                        dt.Rows[i * 2][2],
                        dt.Rows[i * 2 + 1][2],
                        dt.Rows[i * 2][3],
                        dt.Rows[i * 2 + 1][3]
                        , GenerateQrCode(barcode1),
                        GenerateQrCode(barcode2),
                        dt.Rows[i * 2][4],
                        dt.Rows[i * 2 + 1][4]

                        );
                }

                if (row * 2 + 1 == rowcount)
                {
                    barcode1 = dt.Rows[row * 2][1] + ";" + dt.Rows[row * 2][2]
                       + ";" + dt.Rows[row * 2][4] + ";" + dt.Rows[row * 2][0];
                    dt1.Rows.Add(dt.Rows[row * 2][0],
                        null, dt.Rows[row * 2][1], null,
                        dt.Rows[row * 2][2], null,
                        dt.Rows[row * 2][3], null,
                         GenerateQrCode(barcode1),
                        null,
                        dt.Rows[row * 2][4],
                        null
                        );
                }
            //}
            return dt1;
        }


        //private DataTable getDataPrintTemp(DataTable dtIn, string Qty)
        //{
        //    int _Qty = int.Parse(Qty);
        //    DataTable dt = new DataTable();
        //    dt.Columns.Add("TSCD", typeof(string));
        //    dt.Columns.Add("MSTS", typeof(string));
        //    dt.Columns.Add("TGSD", typeof(DateTime));
        //    dt.Columns.Add("DVSD", typeof(string));

        //    for (int i = 0; i < dtIn.Rows.Count; i++)
        //    {
        //        for (int j = 0; j < _Qty; j++)
        //        {
        //            dt.Rows.Add(dtIn.Rows[i]["TYPE_ID"].ToString(), dtIn.Rows[i]["ASSET_CODE"].ToString(), DateTime.Parse(dtIn.Rows[i]["USE_DATE"].ToString()), dtIn.Rows[i]["DEPT_ID"].ToString());
        //        }
        //    }
        //    DataTable dt1 = new DataTable();
        //    dt1.Columns.Add("TSCD1", typeof(string));
        //    dt1.Columns.Add("TSCD2", typeof(string));
        //    dt1.Columns.Add("MSTS1", typeof(string));
        //    dt1.Columns.Add("MSTS2", typeof(string));
        //    dt1.Columns.Add("TGSD1", typeof(DateTime));
        //    dt1.Columns.Add("TGSD2", typeof(DateTime));
        //    dt1.Columns.Add("DVSD1", typeof(string));
        //    dt1.Columns.Add("DVSD2", typeof(string));

        //    int rowcount = dt.Rows.Count;
        //    int row = rowcount / 2;
        //    for (int i = 0; i < row; i++)
        //    {
        //        dt1.Rows.Add(dt.Rows[i * 2][0], dt.Rows[i * 2 + 1][0], dt.Rows[i * 2][1], dt.Rows[i * 2 + 1][1], dt.Rows[i * 2][2], dt.Rows[i * 2 + 1][2], dt.Rows[i * 2][3], dt.Rows[i * 2 + 1][3]);
        //    }

        //    if (row * 2 + 1 == rowcount)
        //    {
        //        dt1.Rows.Add(dt.Rows[row * 2][0], "", dt.Rows[row * 2][1], "", dt.Rows[row * 2][2], DateTime.Now, dt.Rows[row * 2][3], "");
        //    }

        //    return dt1;
        //}

        protected void CrystalReportViewer_Unload(object sender, EventArgs e)
        {
            try
            {
                if (this.cryRpt != null)
                {
                    this.cryRpt.Close();
                    this.cryRpt.Dispose();
                }
            }
            catch (Exception)
            {
            }

        }
    }

}