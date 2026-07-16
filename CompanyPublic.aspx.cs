using System;
using System.Configuration;
using System.Web;
using MySql.Data.MySqlClient;
using System.Web.UI;
namespace CareerConnect.USER
{
    public partial class CompanyPublic : Page
    {
        protected int CcMsgCompanyId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            CcMsgCompanyId = 0;
            int cid;
            if (!int.TryParse(Request.QueryString["id"], out cid) || cid <= 0)
            {
                pnlMissing.Visible = true;
                return;
            }

            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    @"SELECT name, industry, description, website, email, city, country, status, COALESCE(is_verified,0) AS is_verified FROM companies WHERE id = @id LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@id", cid);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                        {
                            pnlMissing.Visible = true;
                            return;
                        }

                        var status = (r["status"]?.ToString() ?? "").ToLowerInvariant();
                        if (status == "inactive")
                        {
                            pnlMissing.Visible = true;
                            return;
                        }

                        pnlBody.Visible = true;
                        var name = HttpUtility.HtmlEncode(r["name"]?.ToString() ?? "Company");
                        litTitle.Text = name;
                        var verObj = r["is_verified"];
                        bool verified = verObj != DBNull.Value && Convert.ToInt32(verObj) == 1;
                        litVerified.Text = verified ? "<span class=\"badge bg-success ms-2\">Verified</span>" : "";
                        var ind = r["industry"]?.ToString();
                        litIndustry.Text = string.IsNullOrWhiteSpace(ind)
                            ? ""
                            : HttpUtility.HtmlEncode(ind);

                        var desc = r["description"]?.ToString() ?? "";
                        litDesc.Text = string.IsNullOrWhiteSpace(desc)
                            ? "<span class=\"text-muted\">No description provided.</span>"
                            : "<p class=\"mb-0\">" + HttpUtility.HtmlEncode(desc).Replace("\r\n", "<br/>").Replace("\n", "<br/>") + "</p>";

                        var site = r["website"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(site))
                        {
                            liSite.Visible = true;
                            hypWebsite.NavigateUrl = site.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? site : "https://" + site;
                            hypWebsite.Text = hypWebsite.NavigateUrl;
                        }

                        var em = r["email"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(em))
                        {
                            liEmail.Visible = true;
                            litEmail.Text = HttpUtility.HtmlEncode(em);
                        }

                        var city = r["city"]?.ToString() ?? "";
                        var country = r["country"]?.ToString() ?? "";
                        var loc = (city + ", " + country).Trim(' ', ',');
                        if (!string.IsNullOrWhiteSpace(loc))
                        {
                            liLoc.Visible = true;
                            litLoc.Text = HttpUtility.HtmlEncode(loc);
                        }

                        CcMsgCompanyId = cid;
                        hypMsgCompany.Visible = true;
                        string msgQ = "default.aspx?msg=1&companyId=" + cid;
                        if (Session["userId"] != null)
                        {
                            hypMsgCompany.NavigateUrl = "#";
                            hypMsgCompany.Text = "Message this company";
                            hypMsgCompany.CssClass = "btn btn-primary cc-msg-open-trigger";
                            hypMsgCompany.Attributes["data-cc-company-id"] = cid.ToString();
                            hypMsgCompany.Attributes["data-cc-job-id"] = "0";
                        }
                        else
                        {
                            hypMsgCompany.NavigateUrl = "Login.aspx?role=user&return=" + Server.UrlEncode(msgQ);
                            hypMsgCompany.Text = "Sign in to message this company";
                            hypMsgCompany.CssClass = "btn btn-primary";
                            hypMsgCompany.Attributes.Remove("data-cc-company-id");
                            hypMsgCompany.Attributes.Remove("data-cc-job-id");
                        }
                    }
                }
            }
        }
    }
}
