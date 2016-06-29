using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Excel = Microsoft.Office.Interop.Excel;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using System.IO;
using Microsoft.Office.Interop.Excel;
using System.Xml;
using System.Web;
using System.Windows.Forms;
using System.Net;
using System.ComponentModel;

namespace gMVVM.Web.ReportPages.AssetMangement.GenerateData
{
    public class ReportDB
    {
        
        static public bool exportDataToExcel(string tieude, System.Data.DataTable dt,String pathExport)
        {
            bool result = false;
            //khoi tao cac doi tuong Com Excel de lam viec
            Excel.Application xlApp;
            Excel.Worksheet xlSheet;
            Excel.Workbook xlBook;
            //doi tuong Trống để thêm  vào xlApp sau đó lưu lại sau
            object missValue = System.Reflection.Missing.Value;

            try
            {
                
                //khoi tao doi tuong Com Excel moi
                xlApp = new Excel.Application();
                xlBook = xlApp.Workbooks.Add(missValue);
                //su dung Sheet dau tien de thao tac
                xlSheet = (Excel.Worksheet)xlBook.Worksheets.get_Item(1);
                //không cho hiện ứng dụng Excel lên để tránh gây đơ máy
                xlApp.Visible = false;
                int socot = dt.Columns.Count;
                int sohang = dt.Rows.Count;
                int i, j;

                //SaveFileDialog f = new SaveFileDialog();
                //f.Filter = "Excel file (*.xls)|*.xls";            
                //if (f.ShowDialog() == DialogResult.OK)
                //{


                //Max column 26 = Z
                string columnHeaderName = "";
                if (socot < 26)
                    columnHeaderName = Convert.ToChar(socot + 65).ToString();
                else
                {
                    columnHeaderName = Convert.ToChar((int)(socot / 26) + 64).ToString();
                    columnHeaderName += Convert.ToChar(socot % 26 + 65).ToString();
                }

                //set thuoc tinh cho tieu de
                xlSheet.get_Range("A1", columnHeaderName + "1").Merge(false);
                Excel.Range caption = xlSheet.get_Range("A1", columnHeaderName + "1");
                caption.Select();
                caption.FormulaR1C1 = tieude;
                //căn lề cho tiêu đề
                caption.HorizontalAlignment = Excel.Constants.xlLeft ;
                caption.Font.Bold = true;
                caption.VerticalAlignment = Excel.Constants.xlCenter;
                caption.Font.Size = 15;
                //màu nền cho tiêu đề
                caption.Interior.ColorIndex = 20;
                caption.RowHeight = 30;
                //set thuoc tinh cho cac header
                Excel.Range header = xlSheet.get_Range("A2", columnHeaderName + "2");
                header.Select();

                header.HorizontalAlignment = Excel.Constants.xlCenter;
                header.Font.Bold = true;
                header.Font.Size = 10;
                //điền tiêu đề cho các cột trong file excel
                for (i = 0; i < socot; i++)
                    xlSheet.Cells[2, i + 2] = dt.Columns[i].ColumnName;
                //dien cot stt
                xlSheet.Cells[2, 1] = "STT";
                
                //for (i = 0; i < sohang; i++)
                //    xlSheet.Cells[i + 3, 1] = i + 1;
                //dien du lieu vao sheet

                //for (j = 0; j < socot; j++)
                //{
                    //Ten cot
                    //xlSheet.Cells[2, j + 2] = dt.Columns[j].ColumnName;
                    for (i = 0; i < sohang; i++)
                    {
                        //STT
                        xlSheet.Cells[i + 3, 1] = i + 1;
                        //gia tri cac dong
                       // xlSheet.Cells[i + 3, j + 2] = dt.Rows[i][j];
                        Range row = xlSheet.get_Range("B" + (i + 3).ToString(), columnHeaderName + (i + 3).ToString());
                        row.Value = dt.Rows[i].ItemArray;
                    }
                //}
                //autofit độ rộng cho các cột
                for (i = 0; i < socot; i++)
                    ((Excel.Range)xlSheet.Cells[1, i + 1]).EntireColumn.AutoFit();

                //save file
                //   xlBook.SaveAs(f.FileName, Excel.XlFileFormat.xlWorkbookNormal, missValue, missValue, missValue, missValue, Excel.XlSaveAsAccessMode.xlExclusive, missValue, missValue, missValue, missValue, missValue);
                string path = pathExport;
                //pathExport 
                if (File.Exists(path))
                    File.Delete(path);
                
                xlBook.SaveAs(path, Excel.XlFileFormat.xlWorkbookNormal, missValue, missValue, missValue, missValue, Excel.XlSaveAsAccessMode.xlExclusive, missValue, missValue, missValue, missValue, missValue);
               // xlBook.Close(true, missValue, missValue);
                xlApp.Quit();
                
                // release cac doi tuong COM
                releaseObject(xlSheet);
                releaseObject(xlBook);
                releaseObject(xlApp);

                //string url = path;
                //// Create an instance of WebClient
                //WebClient client = new WebClient();
                //// Hookup DownloadFileCompleted Event
                //client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

                //// Start the download and copy the file to c:\temp
                //client.DownloadFileAsync(new Uri(url),"NEWFILE");

                result = true;
                //}
            }
            catch (Exception)
            {
                //if (xlBook != null)
                //    xlBook.Close(true, missValue, missValue);
                //if (xlApp != null)
                //    xlApp.Quit();                
                
                //// release cac doi tuong COM
                //releaseObject(xlSheet);
                //releaseObject(xlBook);
                //releaseObject(xlApp);
            }
            return result;
        }

        static void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //MessageBox.Show("File downloaded");
        }

        static public void releaseObject(object obj)
        {
            try
            {
                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                throw new Exception("Exception Occured while releasing object " + ex.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        static public Dictionary<string,string> ReadXML(string reportName)
        {
          // read top level child in nodes
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                string xmlfilePath = HttpContext.Current.Server.MapPath(".");
                //D:\Giau\GSoft\Coding\Today\gMVVM\gMVVM.Web\Services
                xmlfilePath += @"\GenerateData\ReportDataColumn.xml";
                
           //xmldoc.Load(Server.MapPath("QUERYCONFIG.xml"));
                xmldoc.Load(xmlfilePath);
                XmlNode xnList = xmldoc.SelectSingleNode("Report/" + reportName);
                Dictionary<string,string> Data =  new Dictionary<string,string>();
                //foreach (XmlNode xn in xnList)
                //{
                //    //if(xn.Name != reportName)
                //    xn.
                //      Data.Add( xn.Name, xn.InnerText);
                //}
                for (int i = 0; i < xnList.ChildNodes.Count; i++)
                {
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