using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
/*
 * Author: HTANGIAU
 * Company: GSoft
 * Description: generate datatable with n row datable
 * Properties: columns : list Datacolumns of Datatable
 *              typeofColumn: list  type of data in datacolumn
 *              tableName: the name of DataTable
 *              rowCount: the number of rows generate in dataTable
 * Method:
 *      generateData() : fill data and return datatable has data
 */
namespace gMVVM.Web.ReportPages.AssetMangement.GenerateData
{
    public class GenerateDataTable
    {
        public GenerateDataTable()
        {
            this.columns =  new List<string>();
            this.typeofColumns =  new List<string>();
            this.tableName = "";
            this.rowCount = 0;
        }
        private List<string> columns;

        public List<string> Columns
        {
            get { return columns; }
            set { columns = value; }
        }
        private int rowCount;

        public int RowCount
        {
            get { return rowCount; }
            set { rowCount = value; }
        }
        private List<string> typeofColumns;

        public List<string> TypeofColumns
        {
            get { return typeofColumns; }
            set { typeofColumns = value; }
        }
        private String tableName;

        public String TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
        public DataTable generateData()
        {
            DataTable result = new DataTable();
            result.TableName = this.tableName;
            if (this.rowCount <= 0 || this.columns.Count <= 0
                || this.typeofColumns.Count() <= 0)
                return result;
            if (this.columns.Count() != this.typeofColumns.Count)
                return result;
            for (int i = 0; i < columns.Count; i++)
            {
                result.Columns.Add(columns[i],Type.GetType(typeofColumns[i]));
            }
            
            DataRow dr;
            for ( int i = 0; i <= rowCount; i++)
            {
                dr = result.NewRow();
                for (int j = 0; j < this.columns.Count; j++)
                {
                    String clmName = this.columns[j];
                    switch (this.typeofColumns[j])
                    {
                        // you can add more Datatype of columns here
                        case "System.Int32":
                            dr[j] = i; 
                            break;
                        case "System.Double":
                            dr[j] = i;
                            break;
                        case "System.String":
                            dr[j] = clmName + i; 
                            break;

                    }
                }
                    result.Rows.Add(dr);
            }
            return result;
        }
    }
}