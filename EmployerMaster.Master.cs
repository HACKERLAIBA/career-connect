using System;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace CareerConnect.USER.Employer
{
    public partial class EmployerMaster : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["employerCompanyId"] == null)
            {
                Response.Redirect("~/USER/Login.aspx?role=employer");
                return;
            }

            string app = (Request.AppRelativeCurrentExecutionFilePath ?? "").ToLowerInvariant().Replace("%20", " ");
            if (Session["emailVerificationPending"] != null &&
                string.Equals(Session["emailVerificationPending"].ToString(), "1", StringComparison.Ordinal) &&
                !app.Contains("verifyotp.aspx") &&
                !app.Contains("resendverification.aspx"))
            {
                Response.Redirect("~/USER/VerifyOtp.aspx", false);
                return;
            }

            // Block employer portal until admin approves the company
            string status = (Session["employerCompanyStatus"] ?? "").ToString();
            if (string.IsNullOrEmpty(status))
            {
                int cid;
                if (int.TryParse(Session["employerCompanyId"].ToString(), out cid))
                {
                    string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
                    using (var conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        using (var cmd = new MySqlCommand("SELECT status FROM companies WHERE id = @id LIMIT 1", conn))
                        {
                            cmd.Parameters.AddWithValue("@id", cid);
                            object s = cmd.ExecuteScalar();
                            status = s == null ? "" : s.ToString();
                            Session["employerCompanyStatus"] = status;
                        }
                    }
                }
            }

            if (!string.Equals(status, "active", StringComparison.OrdinalIgnoreCase))
            {
                // Prevent redirect loop on the pending page itself
                string current = (Page?.Request?.AppRelativeCurrentExecutionFilePath ?? "").ToLowerInvariant();
                if (!current.EndsWith("/pending.aspx"))
                {
                    Response.Redirect("~/USER/Employer/pending.aspx", false);
                    return;
                }
            }

            lbLoginorLogout.Text = "Logout";

            litCcMsgState.Text =
                "<span id=\"cc-msg-state\" data-logged-in=\"" +
                (Session["employerCompanyId"] != null ? "1" : "0") +
                "\" style=\"display:none\"></span>";

            HighlightEmployerNav();
        }

        private void HighlightEmployerNav()
        {
            var items = new[] { navDash, navMyJobs, navPost, navApps };
            foreach (var li in items)
            {
                if (li != null)
                    li.Attributes.Remove("class");
            }

            string app = (Request.AppRelativeCurrentExecutionFilePath ?? "").ToLowerInvariant().Replace("%20", " ");
            HtmlGenericControl active = null;
            if (app.Contains("employer/default.aspx") || app.Contains("employer/pending.aspx"))
                active = navDash;
            else if (app.Contains("employer/jobs.aspx"))
                active = navMyJobs;
            else if (app.Contains("employer/job_add.aspx"))
                active = navPost;
            else if (app.Contains("employer/job_edit.aspx"))
                active = navMyJobs;
            else if (app.Contains("employer/applications.aspx"))
                active = navApps;
            else if (app.Contains("employer/company.aspx"))
                active = navDash;
            if (active != null)
                active.Attributes["class"] = "cc-nav-active";
        }

        protected void lbCompany_Click(object sender, EventArgs e)
        {
            Response.Redirect("company.aspx");
        }

        protected void lbLoginorLogout_Click(object sender, EventArgs e)
        {
            Session.Remove("employerUserId");
            Session.Remove("employerCompanyId");
            Session.Remove("employerUsername");
            Session.Remove("employerRole");
            Response.Redirect("~/USER/Login.aspx?role=employer");
        }
    }
}
