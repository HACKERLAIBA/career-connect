using System;
using System.Configuration;
using CareerConnect.USER;
using MySql.Data.MySqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace CareerConnect.USER.Employer
{
    public partial class EmployerJobEdit : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["employerCompanyId"] == null)
            {
                Response.Redirect("~/USER/Login.aspx?role=employer");
                return;
            }

            int jobId;
            if (!int.TryParse(Request.QueryString["id"], out jobId) || jobId <= 0)
            {
                Response.Redirect("jobs.aspx");
                return;
            }

            hidJobId.Value = jobId.ToString();

            if (!IsPostBack)
            {
                LoadCategories();
                LoadJob(jobId);
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

        private void LoadJob(int jobId)
        {
            int cid;
            if (!int.TryParse(Session["employerCompanyId"]?.ToString(), out cid) || cid <= 0)
                return;
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "SELECT * FROM jobs WHERE id = @id AND company_id = @cid LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@id", jobId);
                    cmd.Parameters.AddWithValue("@cid", cid);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            lblMsg.Visible = true;
                            lblMsg.CssClass = "alert alert-danger d-block";
                            lblMsg.Text = "Job not found or access denied.";
                            pnlForm.Visible = false;
                            return;
                        }

                        txtTitle.Text = r["title"].ToString();
                        ddlCategory.SelectedValue = r["category_id"].ToString();
                        txtLocation.Text = r["location"]?.ToString() ?? "";
                        ddlJobType.SelectedValue = r["job_type"].ToString();
                        ddlStatus.SelectedValue = r["status"].ToString();
                        txtDescription.Text = r["description"]?.ToString() ?? "";
                        txtRequirements.Text = r["requirements"]?.ToString() ?? "";
                        txtSalaryMin.Text = r["salary_min"] != DBNull.Value ? r["salary_min"].ToString() : "";
                        txtSalaryMax.Text = r["salary_max"] != DBNull.Value ? r["salary_max"].ToString() : "";
                        txtSkills.Text = SkillNormalizer.NormalizeCommaSeparated(r["skills_required"]?.ToString() ?? "");

                        try
                        {
                            var wa = (r["work_arrangement"]?.ToString() ?? "onsite").ToLowerInvariant();
                            if (wa == "remote" || wa == "hybrid")
                                ddlWorkArrangement.SelectedValue = wa;
                            else
                                ddlWorkArrangement.SelectedValue = "onsite";
                        }
                        catch { ddlWorkArrangement.SelectedValue = "onsite"; }

                        try
                        {
                            txtHowToApply.Text = r["how_to_apply"] == DBNull.Value ? "" : r["how_to_apply"].ToString();
                        }
                        catch { txtHowToApply.Text = ""; }

                        try
                        {
                            if (r["application_deadline"] != DBNull.Value)
                                txtAppDeadline.Text = Convert.ToDateTime(r["application_deadline"]).ToString("yyyy-MM-dd");
                            else
                                txtAppDeadline.Text = "";
                        }
                        catch { txtAppDeadline.Text = ""; }
                    }
                }
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            int jobId;
            int cid;
            int catId;
            if (!int.TryParse(hidJobId.Value, out jobId) || jobId <= 0 ||
                !int.TryParse(Session["employerCompanyId"]?.ToString(), out cid) || cid <= 0 ||
                !int.TryParse(ddlCategory.SelectedValue, out catId) || catId <= 0)
            {
                lblMsg.Visible = true;
                lblMsg.CssClass = "alert alert-warning d-block";
                lblMsg.Text = "Invalid job or category. Please select a category and try again.";
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

                string sql = @"UPDATE jobs SET
                    title = @title, description = @desc, requirements = @req,
                    category_id = @cat, location = @loc, job_type = @jtype, status = @st,
                    salary_min = @smin, salary_max = @smax, skills_required = @skills,
                    work_arrangement = @wa, how_to_apply = @hta, application_deadline = @deadline
                    WHERE id = @jid AND company_id = @cid";
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@title", txtTitle.Text.Trim());
                    cmd.Parameters.AddWithValue("@desc", desc);
                    cmd.Parameters.AddWithValue("@req", txtRequirements.Text.Trim());
                    cmd.Parameters.AddWithValue("@cat", catId);
                    cmd.Parameters.AddWithValue("@loc", txtLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@jtype", ddlJobType.SelectedValue);
                    cmd.Parameters.AddWithValue("@st", ddlStatus.SelectedValue);
                    cmd.Parameters.AddWithValue("@smin", (object)salMin ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@smax", (object)salMax ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@skills", SkillNormalizer.NormalizeCommaSeparated(txtSkills.Text));
                    cmd.Parameters.AddWithValue("@wa", wa);
                    cmd.Parameters.AddWithValue("@hta", string.IsNullOrWhiteSpace(txtHowToApply.Text) ? (object)DBNull.Value : txtHowToApply.Text.Trim());
                    cmd.Parameters.AddWithValue("@deadline", deadlineObj);
                    cmd.Parameters.AddWithValue("@jid", jobId);
                    cmd.Parameters.AddWithValue("@cid", cid);
                    cmd.ExecuteNonQuery();
                }
            }

            Response.Redirect("jobs.aspx");
        }
    }
}
