using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI.WebControls;

namespace CareerConnect.USER
{
    public partial class _default : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCategories();
            }
        }

        private void LoadCategories()
        {
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = @"SELECT 
                    c.id, 
                    c.name as Title, 
                    c.icon as IconClass, 
                    c.color,
                    (SELECT COUNT(*) FROM jobs j WHERE j.category_id = c.id AND j.status = 'active') AS JobCount
                FROM 
                    categories c
                WHERE 
                    c.status = 'active'
                ORDER BY 
                    JobCount DESC";
                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                rptCategories.DataSource = dt;
                rptCategories.DataBind();
            }
        }
    }
}

