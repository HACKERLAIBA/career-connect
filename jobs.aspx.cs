using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI;

namespace CareerConnect.USER.Employer
{
    public partial class EmployerJobs : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["employerCompanyId"] == null)
            {
                Response.Redirect("~/USER/Login.aspx?role=employer");
                return;
            }

            if (!IsPostBack)
            {
                LoadJobs();
            }
        }

        private void LoadJobs()
        {
            int cid = int.Parse(Session["employerCompanyId"].ToString());
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"SELECT j.id, j.title, j.location, j.job_type, j.status,
                    COALESCE(cat.name, '') AS category_name
                    FROM jobs j
                    LEFT JOIN categories cat ON j.category_id = cat.id
                    WHERE j.company_id = @cid
                    ORDER BY j.created_at DESC";
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@cid", cid);
                    var dt = new DataTable();
                    da.Fill(dt);
                    rptJobs.DataSource = dt;
                    rptJobs.DataBind();
                    pnlEmpty.Visible = dt.Rows.Count == 0;
                }
            }
        }
    }
}
