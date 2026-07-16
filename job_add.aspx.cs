using System;
using System.Configuration;
using CareerConnect.USER;
using MySql.Data.MySqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CareerConnect.USER.Employer
{
    public partial class EmployerJobAdd : Page
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
                LoadCategories();
            }
        }

        private void LoadCategories()
        {
            ddlCategory.Items.Clear();
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    // Match admin add_job.php: all categories (no status filter — works if `status` column is missing)
                    using (var cmd = new MySqlCommand("SELECT id, name FROM categories ORDER BY name", conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        ddlCategory.DataSource = r;
                        ddlCategory.DataTextField = "name";
                        ddlCategory.DataValueField = "id";
                        ddlCategory.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.CssClass = "alert alert-danger d-block";
                lblMsg.Text = "Could not load categories. Check MySQL and run database_seed_categories.sql. " + ex.Message;
            }

            ddlCategory.Items.Insert(0, new ListItem("-- Select category --", ""));
            if (ddlCategory.Items.Count <= 1)
            {
                lblMsg.Visible = true;
                lblMsg.CssClass = "alert alert-warning d-block";
                lblMsg.Text = "No categories in the database. Import database_seed_categories.sql in phpMyAdmin, or add categories from the Admin panel.";
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int cid;
            int uid;
            if (!int.TryParse(Session["employerCompanyId"]?.ToString(), out cid) || cid <= 0 ||
                !int.TryParse(Session["employerUserId"]?.ToString(), out uid) || uid <= 0)
            {
                lblMsg.Visible = true;
                lblMsg.CssClass = "alert alert-danger d-block";
                lblMsg.Text = "Session expired. Please sign in again.";
                return;
            }

            int catId;
            if (!int.TryParse(ddlCategory.SelectedValue, out catId) || catId <= 0)
            {
                lblMsg.Visible = true;
                lblMsg.CssClass = "alert alert-warning d-block";
                lblMsg.Text = "Please select a valid job category.";
                return;
            }

            decimal? salMin = null, salMax = null;
            decimal sm, sx;
            if (!string.IsNullOrWhiteSpace(txtSalaryMin.Text) && decimal.TryParse(txtSalaryMin.Text.Trim(), out sm))
                salMin = sm;
            if (!string.IsNullOrWhiteSpace(txtSalaryMax.Text) && decimal.TryParse(txtSalaryMax.Text.Trim(), out sx))
                salMax = sx;

            string desc = txtDescription.Text.Trim();
            if (string.IsNullOrEmpty(desc))
                desc = "";

            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                object deadlineObj = DBNull.Value;
                if (!string.IsNullOrWhiteSpace(txtAppDeadline.Text))
                {
                    DateTime d;
                    if (DateTime.TryParse(txtAppDeadline.Text.Trim(), out d))
                        deadlineObj = d.Date;
                }

                string wa = ddlWorkArrangement.SelectedValue ?? "onsite";
                if (wa != "remote" && wa != "hybrid")
                    wa = "onsite";

                string sql = @"INSERT INTO jobs (
                    title, description, requirements, responsibilities, company_id, category_id, location, job_type,
                    experience_level, salary_min, salary_max, skills_required, work_arrangement, how_to_apply, application_deadline, status, created_by)
                    VALUES (@title, @desc, @req, '', @cid, @cat, @loc, @jtype, 'mid', @smin, @smax, @skills, @wa, @hta, @deadline, 'pending', @uid)";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@desc", desc);
                    cmd.Parameters.AddWithValue("@req", txtRequirements.Text.Trim());
                    cmd.Parameters.AddWithValue("@cid", cid);
                    cmd.Parameters.AddWithValue("@cat", catId);
                    cmd.Parameters.AddWithValue("@loc", txtLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@jtype", ddlJobType.SelectedValue);
                    cmd.Parameters.AddWithValue("@smin", (object)salMin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@smax", (object)salMax ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@skills", SkillNormalizer.NormalizeCommaSeparated(txtSkills.Text));
                    cmd.Parameters.AddWithValue("@wa", wa);
                    cmd.Parameters.AddWithValue("@hta", string.IsNullOrWhiteSpace(txtHowToApply.Text) ? (object)DBNull.Value : txtHowToApply.Text.Trim());
                    cmd.Parameters.AddWithValue("@deadline", deadlineObj);
                    cmd.Parameters.AddWithValue("@uid", uid);
                    cmd.ExecuteNonQuery();
                }
            }

            lblMsg.Visible = true;
            lblMsg.CssClass = "alert alert-info d-block";
            lblMsg.Text = "Job submitted for admin approval.";
            Response.Redirect("jobs.aspx");
        }
    }
}
