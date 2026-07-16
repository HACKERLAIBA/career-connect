using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;
using CareerConnect.USER;

namespace CareerConnect.USER.Employer
{
    public partial class EmployerApplications : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["employerCompanyId"] == null)
            {
                Response.Redirect("~/USER/Login.aspx?role=employer");
                return;
            }

            if (!IsPostBack)
                LoadApps();
        }

        protected void rptApps_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;
            var ddl = (DropDownList)e.Item.FindControl("ddlStatus");
            var hid = (HiddenField)e.Item.FindControl("hidAppId");
            if (ddl == null || hid == null)
                return;
            var row = (DataRowView)e.Item.DataItem;
            var st = (row["status"]?.ToString() ?? "pending").ToLowerInvariant();
            var li = ddl.Items.FindByValue(st);
            if (li != null)
            {
                ddl.ClearSelection();
                li.Selected = true;
            }
            else if (ddl.Items.Count > 0)
                ddl.SelectedIndex = 0;
        }

        protected void rptApps_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "UpdateStatus")
                return;

            var item = (RepeaterItem)((Control)e.CommandSource).NamingContainer;
            var ddl = (DropDownList)item.FindControl("ddlStatus");
            var hid = (HiddenField)item.FindControl("hidAppId");
            if (ddl == null || hid == null)
                return;

            int appId;
            if (!int.TryParse(hid.Value, out appId) || appId <= 0)
                return;

            int cid;
            if (!int.TryParse(Session["employerCompanyId"]?.ToString(), out cid) || cid <= 0)
                return;

            string newStatus = ddl.SelectedValue ?? "";
            var allowed = new[] { "pending", "reviewed", "shortlisted", "interviewed", "hired", "rejected" };
            if (Array.IndexOf(allowed, newStatus) < 0)
                return;

            string oldStatus = "";
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var q = new MySqlCommand(
                    @"SELECT a.status FROM applications a
                      INNER JOIN jobs j ON a.job_id = j.id
                      WHERE a.id = @id AND j.company_id = @cid LIMIT 1", conn))
                {
                    q.Parameters.AddWithValue("@id", appId);
                    q.Parameters.AddWithValue("@cid", cid);
                    var o = q.ExecuteScalar();
                    if (o == null || o == DBNull.Value)
                    {
                        ShowMsg("Application not found or access denied.", "danger");
                        return;
                    }
                    oldStatus = o.ToString();
                }

                using (var up = new MySqlCommand(
                    @"UPDATE applications a
                      INNER JOIN jobs j ON a.job_id = j.id
                      SET a.status = @st
                      WHERE a.id = @id AND j.company_id = @cid", conn))
                {
                    up.Parameters.AddWithValue("@st", newStatus);
                    up.Parameters.AddWithValue("@id", appId);
                    up.Parameters.AddWithValue("@cid", cid);
                    up.ExecuteNonQuery();
                }
            }

            if (!string.Equals(oldStatus, newStatus, StringComparison.OrdinalIgnoreCase))
                ApplicationNotificationMail.TryNotifyStatusChanged(connStr, appId, newStatus);

            ShowMsg("Status updated.", "success");
            LoadApps();
        }

        private void ShowMsg(string text, string type)
        {
            lblMsg.Visible = true;
            lblMsg.Text = text;
            lblMsg.CssClass = "alert alert-" + type + " d-block";
        }

        private void LoadApps()
        {
            int cid;
            if (!int.TryParse(Session["employerCompanyId"]?.ToString(), out cid))
                return;

            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string sql = @"
                    SELECT a.id AS application_id, a.status, a.applied_at,
                        CONCAT(u.first_name, ' ', u.last_name) AS user_name,
                        u.email AS user_email,
                        j.title AS job_title
                    FROM applications a
                    INNER JOIN jobs j ON a.job_id = j.id
                    INNER JOIN users u ON a.user_id = u.id
                    WHERE j.company_id = @cid
                    ORDER BY a.applied_at DESC";

                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@cid", cid);
                    var dt = new DataTable();
                    da.Fill(dt);
                    rptApps.DataSource = dt;
                    rptApps.DataBind();
                    pnlEmpty.Visible = dt.Rows.Count == 0;
                }
            }
        }
    }
}
