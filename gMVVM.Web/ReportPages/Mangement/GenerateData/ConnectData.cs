using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
/*
 * Author: HTANGIAU
 * Company: GSoft
 * Description: use to connect sql server by sqlClient
 * Properties: connectString: sql connectString to connect sql server
 *          sqlConnect: use to connect sql server by connectString
 *          sqlCommand: command will buid to select data in sql server
 *          dataSource : datatable return data in sql server
 *          paramerter :  with Store Proce has paramerter Name
 *          paramerterType : with Store Proce has paramerter Type
 *          paramerterValue : with Store Proce, set paramerter value
 * Method:
 *  Read_Store(nameStore,hasParameter) use to read Store Proc with name and Paramerter
 *  to use this code, please change connectstring in Web.config in your project
 */
namespace gMVVM.Web.ReportPages.AssetMangement.GenerateData
{
    public class ConnectData
    {
        public ConnectData()
        {
            connectString = ConfigurationManager.ConnectionStrings["gMVVMConnectionString"].ConnectionString;
            sqlConnect = new SqlConnection();
            sqlCommand = new SqlCommand();
            dataSource = new DataTable();
            paramerters = new List<string>();
            paramertersValue = new List<string>();
            parametersType = new List<SqlDbType>();
        }
        private String error;

        public String Error
        {
            get { return error; }
            set { error = value; }
        }
        private String connectString;

        public String ConnectString
        {
            get { return connectString; }
            set { connectString = value; }
        }
        private SqlConnection sqlConnect;

        public SqlConnection SqlConnect
        {
            get { return sqlConnect; }
            set { sqlConnect = value; }
        }
        private SqlCommand sqlCommand;

        public SqlCommand SqlCommand
        {
            get { return sqlCommand; }
            set { sqlCommand = value; }
        }
        private DataTable dataSource;

        public DataTable DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }
        private List<string> paramerters;

        public List<string> Paramerters
        {
            get { return paramerters; }
            set { paramerters = value; }
        }
        private List<SqlDbType> parametersType;

        public List<SqlDbType> ParametersType
        {
            get { return parametersType; }
            set { parametersType = value; }
        }

        private DataSet dataSet;

        public DataSet DataSet
        {
            get { return dataSet; }
            set { dataSet = value; }
        }

        private List<string> paramertersValue;

        public List<string> ParamertersValue
        {
            get { return paramertersValue; }
            set { paramertersValue = value; }
        }
        public bool Read_Store(String storeName,bool hasParameters = false)
        {
            try
            {
                sqlConnect = new SqlConnection(this.connectString);
                sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = storeName;
                if (hasParameters)
                {
                    for (int i = 0; i < paramerters.Count; i++)
                    {
                        sqlCommand.Parameters.Add(paramerters[i],
                            parametersType[i]).Value = paramertersValue[i];

                    }
                }
                sqlCommand.Connection = sqlConnect;
                sqlConnect.Open();
                SqlDataAdapter adap = new SqlDataAdapter(sqlCommand);
                dataSource = new DataTable();
                adap.Fill(dataSource);
            }
            catch (Exception ex)
            {
                sqlConnect.Close();
                sqlConnect.Dispose();
                this.Error = ex.Message;
                return false;
            }
            finally {
                sqlConnect.Close();
                sqlConnect.Dispose();
            }
            return true;
        }

        public bool Read_Store_Execute(String storeName, bool hasParameters = false)
        {
            try
            {
                sqlConnect = new SqlConnection(this.connectString);
                sqlCommand = new SqlCommand();
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.CommandText = storeName;
                if (hasParameters)
                {
                    for (int i = 0; i < paramerters.Count; i++)
                    {
                        sqlCommand.Parameters.Add(paramerters[i],
                            parametersType[i]).Value = paramertersValue[i];

                    }
                }
                sqlCommand.Connection = sqlConnect;
                sqlConnect.Open();
                SqlDataAdapter adap = new SqlDataAdapter(sqlCommand);
                this.dataSet = new DataSet();
                adap.Fill(dataSet);
            }
            catch (Exception ex)
            {
                sqlConnect.Close();
                sqlConnect.Dispose();
                this.Error = ex.Message;
                return false;
            }
            finally
            {
                sqlConnect.Close();
                sqlConnect.Dispose();
            }
            return true;
        }
    }
}