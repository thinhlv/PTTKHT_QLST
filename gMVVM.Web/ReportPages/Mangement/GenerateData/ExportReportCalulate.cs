using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
using gMVVM.Web.ExportReport.CodeExport;
using gMVVM.Web.ExportReport.CodeExport;
using gMVVM.Web.ReportPages.ExportReport;

namespace gMVVM.Web.ReportPages.AssetMangement.GenerateData
{

    public class ExportReportCalulate
    {
        public ExportReportCalulate()
        {
            storeName = null;
            storeParameterType = new Dictionary<string, SqlDbType>();
            storeParameterValue = new Dictionary<string, string>();
            hasParameterStore = false;
            parameterReport = new Dictionary<string, string>();

        }
        private Dictionary<string, SqlDbType> storeParameterType;

        public Dictionary<string, SqlDbType> StoreParameterType
        {
            get { return storeParameterType; }
            set { storeParameterType = value; }
        }
        private Dictionary<string, string> storeParameterValue;

        public Dictionary<string, string> StoreParameterValue
        {
            get { return storeParameterValue; }
            set { storeParameterValue = value; }
        }
        private string reportName;
        public string ReportName
        {
            get { return reportName; }
            set { reportName = value; }
        }
        private string storeName;

        public string StoreName
        {
            get { return storeName; }
            set { storeName = value; }
        }
        private string filePath;

        public string FilePath
        {
            get { return filePath; }
            set { filePath = value; }
        }
        private Dictionary<string, string> parameterReport;

        public Dictionary<string, string> ParameterReport
        {
            get { return parameterReport; }
            set { parameterReport = value; }
        }
        private bool hasParameterStore;

        public bool HasParameterStore
        {
            get { return hasParameterStore; }
            set { hasParameterStore = value; }
        }
        private bool isStoreNameEmpty()
        {
            return (storeName == "" || storeName == null);
        }
        private bool isReportNameEmpty()
        {
            return (this.reportName == "" || this.reportName == null);
        }
        private bool isfilePathTemplateFileEmpty()
        {
            return (filePath == "" || filePath == null);
        }
        private MemoryStream packageStream;
        public MemoryStream PackageStream
        {
            get { return packageStream; }
        }

        public string exportToExcel()
        {
            // Step 1. Read data from database
            if (isStoreNameEmpty())
                return "Store Name is empty";
            if (isReportNameEmpty())
                return "Excel Name id is empty";
            if (isfilePathTemplateFileEmpty())
                return "Template File Doesn't exists";
            if (hasParameterStore)
            {
                try
                {
                    ConnectData cdata = new ConnectData();
                    // get connectString from Sacombank
                    cdata.ConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["gMVVMConnectionString"].ConnectionString;

                    foreach (KeyValuePair<string, string> pair in this.storeParameterValue)
                    {
                        cdata.Paramerters.Add(pair.Key);
                        cdata.ParamertersValue.Add(pair.Value);
                        cdata.ParametersType.Add(SqlDbType.VarChar);
                    }
                    // get data from database to datatable
                    if (!cdata.Read_Store(StoreName, true)) return "Gọi Store thất bại";
                    //Bat dat export
                    //Khoi tao voi duong dan excel truyen vao          
                    ExcelTemplateExportBase excel = new ExcelTemplateExportBase()
                    {
                        TemplatePath = this.FilePath
                    };

                    //gan du lieu doc tu store do vo excel
                    excel.SmartmarkersObjData = new Dictionary<string, object>();
                    excel.SmartmarkersObjData.Add("obj", cdata.DataSource.DefaultView);
                    //cac doi so truyen vao
                    foreach (KeyValuePair<string, string> pair in this.ParameterReport)
                    {
                        excel.SmartmarkersObjData.Add(pair.Key, pair.Value);
                    }
                    this.packageStream = excel.ExprortSmartmarkers();
                    
                    return null;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Passed invalid TemplatePath to Excel Template")
                        return "Chưa có mẫu báo cáo dạng excel.";
                    return ex.Message;
                }


            }
            return null;
        }

        public string exportToWord()
        {
            // Step 1. Read data from database
            if (isStoreNameEmpty())
                return "Store Name is empty";
 
            if (hasParameterStore)
            {
                try
                {
                    ConnectData cdata = new ConnectData();
                    // get connectString from Sacombank
                    cdata.ConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["gMVVMConnectionString"].ConnectionString;

                    foreach (KeyValuePair<string, string> pair in this.storeParameterValue)
                    {
                        cdata.Paramerters.Add(pair.Key);
                        cdata.ParamertersValue.Add(pair.Value);
                        cdata.ParametersType.Add(SqlDbType.VarChar);
                    }
                    // get data from database to datatable
                    if (!cdata.Read_Store(StoreName, true)) return "Gọi Store thất bại";
                    //bat dau export
                    WordTemplateBase word = new WordTemplateBase()
                    {
                        TemplatePath = this.FilePath
                    };

                    word.Data = cdata.DataSource;
                    this.packageStream = word.ExcuteMailMergeSimple();
                    return null;
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Passed invalid TemplatePath to Excel Template")
                        return "Chưa có mẫu báo cáo dạng excel.";
                    return ex.Message;
                }


            }
            return null;
        }

    }
}