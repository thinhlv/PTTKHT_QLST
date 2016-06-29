
using gMVVM.Web.ReportPages.AssetMangement.GenerateData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Web.Tools
{
    public partial class QuerryForm : System.Web.UI.Page
    {
        private ConnectData sqlData { get; set; }
        protected void Page_Load(object sender, EventArgs e)
        {
            this.sqlData = new ConnectData();
            this.Label1.Text = "";
        }
        protected void Button1_Click1(object sender, EventArgs e)
        {
            try
            {
                sqlData.Paramerters.Add("@l_QUERY");
                sqlData.ParametersType.Add(SqlDbType.NText);
                sqlData.ParamertersValue.Add(TextBox1.Text);
                bool isLoad = sqlData.Read_Store_Execute("EXECSQL", true);
                if (isLoad)
                {
                    
                    DataSet a = sqlData.DataSet;
                    if (a.Tables.Count >= 1)
                    {
                        foreach(var item in a.Tables)
                        {
                            System.Web.UI.WebControls.GridView grView = new GridView();
                            
                            grView.DataSource = item;
                            grView.DataBind();

                            Panel1.Controls.Add(grView);
                        }

                        
                       
                        this.Label1.Text = "Trạng thái: Select thành công";
                    }
                    //else if (a.Rows[0].ItemArray[0].Equals("ERROR"))
                    //{
                    //    this.Label1.Text = "Trạng thái:" + a.Rows[0].ItemArray[1].ToString();
                    //    this.GridView1.DataSource = null;
                    //    this.GridView1.DataBind();
                    //}
                    //else
                    //{
                    //    this.Label1.Text = "Trạng thái:cập nhật thành công";
                    //    this.GridView1.DataSource = null;
                    //    this.GridView1.DataBind();
                    //}

                }
                else this.Label1.Text = "Trạng thái: Store không tồn tại trong database";
            }
            catch (Exception ex)
            {
                this.Label1.Text = "Trạng thái: " + ex.Message;
            }
        }

        protected void Button2_Click1(object sender, EventArgs e)
        {
            try
            {
                this.Label1.Text = "";
                this.TextBox1.Text = "";
                this.GridView1.DataSource = null;
            }
            catch (Exception ex)
            {
                this.Label1.Text = ex.Message;
            }
        }
    }
}