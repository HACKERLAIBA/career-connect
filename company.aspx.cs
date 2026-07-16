using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Web.UI;

namespace CareerConnect.USER.Employer
{
    public partial class EmployerCompany : Page
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
                LoadCompany();
            }
        }

        private void LoadCompany()
        {
            int cid = int.Parse(Session["employerCompanyId"].ToString());
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "SELECT name, industry, website, email, phone, city, country, description FROM companies WHERE id = @id LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@id", cid);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                            return;

                        txtName.Text = r["name"]?.ToString() ?? "";
                        txtIndustry.Text = r["industry"]?.ToString() ?? "";
                        txtWebsite.Text = r["website"]?.ToString() ?? "";
                        txtEmail.Text = r["email"]?.ToString() ?? "";
                        txtPhone.Text = r["phone"]?.ToString() ?? "";
                        txtCity.Text = r["city"]?.ToString() ?? "";
                        txtCountry.Text = r["country"]?.ToString() ?? "";
                        txtDescription.Text = r["description"]?.ToString() ?? "";
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int cid = int.Parse(Session["employerCompanyId"].ToString());
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                    UPDATE companies SET
                        name = @name, industry = @ind, website = @web, email = @email, phone = @phone,
                        city = @city, country = @country, description = @desc
                    WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("@name", txtName.Text.Trim());
                    cmd.Parameters.AddWithValue("@ind", txtIndustry.Text.Trim());
                    cmd.Parameters.AddWithValue("@web", txtWebsite.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@phone", txtPhone.Text.Trim());
                    cmd.Parameters.AddWithValue("@city", txtCity.Text.Trim());
                    cmd.Parameters.AddWithValue("@country", txtCountry.Text.Trim());
                    cmd.Parameters.AddWithValue("@desc", txtDescription.Text.Trim());
                    cmd.Parameters.AddWithValue("@id", cid);
                    cmd.ExecuteNonQuery();
                }
            }

            lblMsg.Visible = true;
            lblMsg.CssClass = "alert alert-success d-block";
            lblMsg.Text = "Company profile saved.";
        }
    }
}
