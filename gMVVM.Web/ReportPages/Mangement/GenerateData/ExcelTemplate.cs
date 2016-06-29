using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;
/*
 * Description : Export Excel Template
 * Create by : GiauHT
 * Company: GSoft
 * Create Date: 14/01/2014
 * Modified Date: 16/01/2014
 */
namespace gMVVM.Web.ReportPages.AssetMangement.GenerateData
{
    public class ExcelTemplate
    {
        public ExcelTemplate()
        {
            storeName = null;
            storeParameterType = new Dictionary<string, SqlDbType>();
            storeParameterValue = new Dictionary<string, string>();
            hasParameterExcel = false;
            hasParameterStore = false;
            parameterExcel = new Dictionary<string, string>();

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
        private string idExcel;
        public string IdExcel
        {
            get { return idExcel; }
            set { idExcel = value; }
        }
        private string storeName;

        public string StoreName
        {
            get { return storeName; }
            set { storeName = value; }
        }
        private string excelTemplateFile;

        public string ExcelTemplateFile
        {
            get { return excelTemplateFile; }
            set { excelTemplateFile = value; }
        }
        private Dictionary<string, string> parameterExcel;

        public Dictionary<string, string> ParameterExcel
        {
            get { return parameterExcel; }
            set { parameterExcel = value; }
        }
        private bool hasParameterExcel;

        public bool HasParameterExcel
        {
            get { return hasParameterExcel; }
            set { hasParameterExcel = value; }
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
        private bool isExcelNameEmpty()
        {
            return (this.idExcel == "" || this.idExcel == null);
        }
        private bool isexcelTemplateFileEmpty()
        {
            return (excelTemplateFile == "" || excelTemplateFile == null);
        }
        private string formatExcel(string CellName, string type, ref ExcelWorksheet wsTemplate, string pathXMLCell)
        {
            try
            {
                type = type.ToLower();
                Dictionary<string, string> Format = ReadXML(pathXMLCell + "/" + type);
                switch (type)
                {
                    case "merge":
                        #region "format merge"
                        using (ExcelRange r = wsTemplate.Cells[Format["MergeRanges"]])
                        {
                            r.Merge = true;
                            switch (Format["HorizontalAlignment"])
                            {
                                case "CenterContinuous":
                                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                                    break;
                                case "Center":
                                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                                    break;
                                case "Right":
                                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                    break;
                                case "Distributed":
                                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Distributed;
                                    break;
                                case "Fill":
                                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Fill;
                                    break;
                                case "General":
                                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.General;
                                    break;
                                case "Justify":
                                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Justify;
                                    break;
                                default:
                                    r.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                                    break;
                            }
                        }
                        #endregion
                        break;
                    case "wraptext":
                        wsTemplate.Cells[CellName].Style.WrapText = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
        private Stream excelPackageStream;


        public Stream ExcelPackageStream
        {
            get { return excelPackageStream; }
        }
        private string formatExcel(int CellRow,int CellCol, string type, ref ExcelWorksheet wsTemplate, string pathXMLCell)
        {
            try
            {
              //  type = type.ToLower();
                
                Dictionary<string, string> Format = ReadXML(pathXMLCell + "/" + type);
                switch (type)
                {
                    
                    case "WrapText":

                        switch (Format.Keys.First())
                        {
                            case "Text":
                                break;
                            case "Date":
                                wsTemplate.Cells[CellRow, CellCol].Style.Numberformat.Format = "dd/mm/yyyy";
                                break;
                            case "Number":
                                wsTemplate.Cells[CellRow, CellCol].Style.Numberformat.Format = "#,##0";
                                break;
                            case "Percent":
                                wsTemplate.Cells[CellRow, CellCol].Style.Numberformat.Format = "#,##0";
                                break;
                            case "FLOAT":
                                wsTemplate.Cells[CellRow, CellCol].Style.Numberformat.Format = "#,##0.00";
                                break;
                                
                        }

                        wsTemplate.Cells[CellRow,CellCol].Style.WrapText
                                = (Format.Values.First().ToLower().Equals( "true"))?true:false;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "";
        }
      
        public string exportToExcel()
        {
            // Step 1. Read data from database
            if (isStoreNameEmpty())
                return "Store Name is empty";
            if (isExcelNameEmpty())
                return "Excel Name id is empty";
            if (isexcelTemplateFileEmpty())
                return "Template File Doesn't exists";
            string xmlError = LoadXMLPath();
            if (xmlError != null)
                return xmlError;
            if (hasParameterStore)
            #region "Export"
            /*{

                try
                {
                    ConnectData cdata = new ConnectData();
                    // get connectString from Sacombank
                    cdata.ConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["gMVVMConnectionString"].ConnectionString;
                    // add type of Paramerter Store
                    //foreach (KeyValuePair<string, SqlDbType> pair in this.storeParameterType)
                    //{
                    //    cdata.Paramerters.Add(pair.Key);
                    //    cdata.ParametersType.Add(pair.Value);
                    //}
                    foreach (KeyValuePair<string, string> pair in this.storeParameterValue)
                    {
                        cdata.Paramerters.Add(pair.Key);
                        cdata.ParamertersValue.Add(pair.Value);
                        cdata.ParametersType.Add(SqlDbType.VarChar);
                    }
                    // get data from database to datatable
                    cdata.Read_Store(StoreName, true);
                    // Step 2. Read XML file
                    // Read ParameterExcel in XML
                    // load Excel Template file                    
                    ExcelPackage pckTemplate = new ExcelPackage(new FileInfo(excelTemplateFile), true);
                    
                    var wsTemplate = pckTemplate.Workbook.Worksheets[1];
                    XmlNode node;
                    string Cellname, type, pathXMLCell;
                    Dictionary<string, string> listFieldData = ReadXML("Template/"
                            + idExcel + "/FieldExcel/StartPoint");
                    int row = int.Parse(listFieldData["Row"]), col = int.Parse(listFieldData["Column"]);
                    string path = "Template/" + idExcel + "/FieldExcel/Fields";
                    listFieldData = ReadXML(path);

                    // Step 3.1 : set value for cell paramerterExcel
                    foreach (KeyValuePair<string, string> pair in parameterExcel)
                    {
                        Cellname = pair.Key;
                        pathXMLCell = "Template/"
                                + idExcel + "/ParameterExcel/" + Cellname;
                        node = xmldoc.SelectSingleNode(pathXMLCell);
                        type = node.ChildNodes[0].Name;
                        formatExcel(Cellname, type, ref wsTemplate, pathXMLCell);
                        wsTemplate.Cells[Cellname].Value = pair.Value;
                    }
                    // Step 3.2 : fill data in field excel
                   // if (cdata.DataSource.Rows.Count > 0) // check if has data or no
                 //   {
                        // Step 3.2.1 : read row and col started - thieuvq 14/11/2014
                        //thieuvq Dictionary<string, string> listFieldData = ReadXML("Template/"
                        //        + idExcel + "/FieldExcel/StartPoint");
                        //thieuvq int row = int.Parse(listFieldData["Row"]), col = int.Parse(listFieldData["Column"]);
                        //thieuvq string path = "Template/"
                        //        + idExcel + "/FieldExcel/Fields";
                        //listFieldData = ReadXML(path);
                        if (listFieldData == null)
                            return "Error when load XML format datafield ";
                        int ncol;
                        foreach (KeyValuePair<string, string> pair in listFieldData)
                        {
                            pathXMLCell = "Template/"
                                + idExcel + "/FieldExcel/Fields/" + pair.Key + "/Prior";
                            node = xmldoc.SelectSingleNode(pathXMLCell);
                            // get prior
                            ncol = int.Parse(node.ChildNodes[0].Value);
                            //pathXMLCell = "Template/"
                            //    + storeName + "/FieldExcel/Fields/" + pair.Key ;
                            //type = node.ChildNodes[0].Name;
                            //formatExcel();
                            for (int i = 0; i < cdata.DataSource.Rows.Count; i++)
                            {
                                
                                wsTemplate.Cells[row + i, col + ncol].Value
                                    = cdata.DataSource.Rows[i][pair.Key];

                                pathXMLCell = "Template/"
                                + idExcel + "/FieldExcel/Fields/" + pair.Key;
                                node = xmldoc.SelectSingleNode(pathXMLCell);
                                type = node.ChildNodes[1].Name;
                                formatExcel(row + i, col + ncol, type, ref wsTemplate, pathXMLCell);

                                wsTemplate.Cells[row + i, col + ncol].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                wsTemplate.Cells[row + i, col + ncol].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                wsTemplate.Cells[row + i, col + ncol].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                wsTemplate.Cells[row + i, col + ncol].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                              //  wsTemplate.Cells[row + i, col + ncol].Style.WrapText = true;
                            }

                        }
                        // shape.Text = "Sample 3 uses a template that is stored in the application cashe.";
                        pckTemplate.Save();
                        excelPackageStream = pckTemplate.Stream;

                        return null;
                  //  }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Passed invalid TemplatePath to Excel Template")
                        return "Chưa có mẫu báo cáo dạng excel.";
                    return ex.Message;
                }


            }*/
            #endregion

            #region "Export2"
            {
                try
                {
                    ConnectData cdata = new ConnectData();
                    // get connectString from Sacombank
                    cdata.ConnectString = System.Configuration.ConfigurationManager.ConnectionStrings["gMVVMConnectionString"].ConnectionString;
                    // add type of Paramerter Store
                    //foreach (KeyValuePair<string, SqlDbType> pair in this.storeParameterType)
                    //{
                    //    cdata.Paramerters.Add(pair.Key);
                    //    cdata.ParametersType.Add(pair.Value);
                    //}

                  //  DateTime start = DateTime.Now;
                    string rootPath = "Template/" + idExcel + "/FieldExcel";
                    foreach (KeyValuePair<string, string> pair in this.storeParameterValue)
                    {
                        cdata.Paramerters.Add(pair.Key);
                        cdata.ParamertersValue.Add(pair.Value);
                        cdata.ParametersType.Add(SqlDbType.VarChar);
                    }
                    // get data from database to datatable
                    cdata.Read_Store(StoreName, true);
                    // Step 2. Read XML file
                    // Read ParameterExcel in XML
                    // load Excel Template file                    
                    ExcelPackage pckTemplate = new ExcelPackage(new FileInfo(excelTemplateFile), true);

                    var wsTemplate = pckTemplate.Workbook.Worksheets[1];
                    XmlNode node;
                    string Cellname, type, pathXMLCell;
                    Dictionary<string, string> listFieldData = ReadXML(rootPath + "/StartPoint");
                    int row = int.Parse(listFieldData["Row"]), col = int.Parse(listFieldData["Column"]);
                    string path = rootPath + "/Fields";
                    listFieldData = ReadXML(path);

                    // Step 3.1 : set value for cell paramerterExcel
                    foreach (KeyValuePair<string, string> pair in parameterExcel)
                    {
                        Cellname = pair.Key;
                        pathXMLCell = "Template/"
                                + idExcel + "/ParameterExcel/" + Cellname;
                        node = xmldoc.SelectSingleNode(pathXMLCell);
                        type = node.ChildNodes[0].Name;
                        formatExcel(Cellname, type, ref wsTemplate, pathXMLCell);
                        wsTemplate.Cells[Cellname].Value = pair.Value;

                    }
                    // Step 3.2 : fill data in field excel
                    // if (cdata.DataSource.Rows.Count > 0) // check if has data or no
                    //   {
                    // Step 3.2.1 : read row and col started - thieuvq 14/11/2014
                    //thieuvq Dictionary<string, string> listFieldData = ReadXML("Template/"
                    //        + idExcel + "/FieldExcel/StartPoint");
                    //thieuvq int row = int.Parse(listFieldData["Row"]), col = int.Parse(listFieldData["Column"]);
                    //thieuvq string path = "Template/"
                    //        + idExcel + "/FieldExcel/Fields";
                    //listFieldData = ReadXML(path);
                    if (listFieldData == null)
                        return "Error when load XML format datafield ";
                    int ncol;

                    //thieuvq 08062015
                    Dictionary<string, int> _values = new Dictionary<string, int>();
                    Dictionary<string, string> _names = new Dictionary<string, string>();
                    foreach (KeyValuePair<string, string> pair in listFieldData)
                    {
                        pathXMLCell = rootPath + "/Fields/" + pair.Key + "/Prior";
                        node = xmldoc.SelectSingleNode(pathXMLCell);                        
                        ncol = int.Parse(node.ChildNodes[0].Value);
                        _values.Add(pair.Key, ncol);

                        pathXMLCell = rootPath + "/Fields/" + pair.Key;
                        node = xmldoc.SelectSingleNode(pathXMLCell);
                        type = node.ChildNodes[1].Name;
                        _names.Add(pair.Key, type);

                    }


                    //pathXMLCell = "Template/"
                    //    + storeName + "/FieldExcel/Fields/" + pair.Key ;
                    //type = node.ChildNodes[0].Name;
                    //formatExcel();
                    int TotalRow = cdata.DataSource.Rows.Count;
                    int TotalCol = listFieldData.Count;
                    for (int i = 0; i < TotalRow; i++)
                    {
                        foreach (KeyValuePair<string, string> pair in listFieldData)
                        {
                            //pathXMLCell = "Template/"
                            //    + idExcel + "/FieldExcel/Fields/" + pair.Key + "/Prior";
                            //node = xmldoc.SelectSingleNode(pathXMLCell);
                            //// get prior
                            //ncol = int.Parse(node.ChildNodes[0].Value);
                            ncol = _values[pair.Key];

                            wsTemplate.Cells[row + i, col + ncol].Value
                                = cdata.DataSource.Rows[i][pair.Key];

                            //pathXMLCell = "Template/"
                            //+ idExcel + "/FieldExcel/Fields/" + pair.Key;
                            //node = xmldoc.SelectSingleNode(pathXMLCell);
                            //type = node.ChildNodes[1].Name;
                           // formatExcel(row + i, col + ncol, type, ref wsTemplate, pathXMLCell);
                            
                        }
                    }

                    if (TotalRow > 0)
                    {
                        //formatExcel                    
                        foreach (KeyValuePair<string, string> pair in listFieldData)
                        {
                            type = _names[pair.Key];
                            ncol = _values[pair.Key];
                            Dictionary<string, string> Format = ReadXML(rootPath + "/Fields/" + pair.Key + "/" + type);
                            switch (type)
                            {

                                case "WrapText":

                                    switch (Format.Keys.First())
                                    {
                                        case "Text":
                                            break;
                                        case "Date":
                                            wsTemplate.Cells[row, col + ncol, row + TotalRow - 1, col + ncol].Style.Numberformat.Format = "dd/mm/yyyy";
                                            break;
                                        case "Number":
                                            wsTemplate.Cells[row, col + ncol, row + TotalRow - 1, col + ncol].Style.Numberformat.Format = "#,##0";
                                            break;
                                        case "Percent":
                                            wsTemplate.Cells[row, col + ncol, row + TotalRow - 1, col + ncol].Style.Numberformat.Format = "#,##0";
                                            break;
                                        case "FLOAT":
                                            wsTemplate.Cells[row, col + ncol, row + TotalRow - 1, col + ncol].Style.Numberformat.Format = "#,##0.00";
                                            break;
                                    }

                                    wsTemplate.Cells[row, col + ncol, row + TotalRow - 1, col + ncol].Style.WrapText
                                            = (Format.Values.First().ToLower().Equals("true")) ? true : false;
                                    break;
                                default:
                                    break;
                            }
                        }
                        //tao border                      
                        wsTemplate.Cells[row, col, row + TotalRow - 1, col + TotalCol - 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        wsTemplate.Cells[row, col, row + TotalRow - 1, col + TotalCol - 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        wsTemplate.Cells[row, col, row + TotalRow - 1, col + TotalCol - 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        wsTemplate.Cells[row, col, row + TotalRow - 1, col + TotalCol - 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        wsTemplate.Cells[row, col, row + TotalRow - 1, col + TotalCol - 1].Style.WrapText = true;
                    }
                    // shape.Text = "Sample 3 uses a template that is stored in the application cashe.";
                    pckTemplate.Save();
                    excelPackageStream = pckTemplate.Stream;

                    //return (start.ToString() + "\n" + DateTime.Now.ToString() + "\n" + (DateTime.Now - start).TotalSeconds.ToString());
                    return null;
                    //  }
                }
                catch (Exception ex)
                {
                    if (ex.Message == "Passed invalid TemplatePath to Excel Template")
                        return "Chưa có mẫu báo cáo dạng excel.";
                    return ex.Message;
                }


            }
            #endregion

            return null;
            //   return "not have data, please check again!";
        }
        private XmlDocument xmldoc;
        private string LoadXMLPath()
        {
            try
            {
                xmldoc = new XmlDocument();
                string xmlfilePath = 
                    HttpContext.Current.Server.MapPath("./ExcelTemplate/ExcelTemplateDescription.xml");
                //D:\Giau\GSoft\Coding\Today\gMVVM\gMVVM.Web\Services
              //  xmlfilePath += @"/Functions/ExcelTemplateDescription.xml";
                xmldoc.Load(xmlfilePath);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return null;
        }
        private Dictionary<string, string> ReadXML(string PathnameNode)
        {
            // read top level child in nodes
            try
            {

                XmlNode xnList = xmldoc.SelectSingleNode(PathnameNode);
                Dictionary<string, string> Data = new Dictionary<string, string>();

                for (int i = 0; i < xnList.ChildNodes.Count; i++)
                {
                    //if(!Data.Keys.Contains(xnList.ChildNodes[i].Name)
                    Data.Add(xnList.ChildNodes[i].Name, xnList.ChildNodes[i].InnerText);
                }
                return Data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}