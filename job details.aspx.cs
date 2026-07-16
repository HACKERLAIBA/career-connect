using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace CareerConnect.USER
{
    public partial class job_details : System.Web.UI.Page
    {
        string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
        int jobId;
        private string _pendingExternalApplyUrl;

        protected int CcMsgCompanyId { get; set; }
        protected int CcMsgJobId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["jobId"] == null || !int.TryParse(Request.QueryString["jobId"], out jobId))
            {
                Response.Redirect("job listing.aspx");
                return;
            }

            LoadJobDetails();

            if (!string.IsNullOrEmpty(_pendingExternalApplyUrl) &&
                string.Equals(Request.QueryString["redirect"], "apply", StringComparison.OrdinalIgnoreCase))
            {
                Response.Redirect(_pendingExternalApplyUrl);
                return;
            }
        }

        private static object SafeRead(MySqlDataReader r, string col)
        {
            try
            {
                var o = r[col];
                return o == DBNull.Value ? null : o;
            }
            catch
            {
                return null;
            }
        }

        private void LoadJobDetails()
        {
            CcMsgCompanyId = 0;
            CcMsgJobId = jobId;

            pnlSyndicated.Visible = false;
            pnlAlreadyApplied.Visible = false;
            btnShowApply.Visible = true;
            pnlSaveJob.Visible = Session["userId"] != null;
            lblSaveHint.Text = Session["userId"] == null ? "" : "Save this job to view it later in your Saved Jobs list.";
            pnlReport.Visible = Session["userId"] != null;
            lblReportResult.Text = "";

            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                string query = @"SELECT 
                    j.*, 
                    c.id AS CompanyId,
                    c.name AS CompanyName, 
                    c.description AS CompanyDescription,
                    COALESCE(c.is_verified, 0) AS IsCompanyVerified,
                    cat.name AS CategoryTitle,
                    c.logo AS CompanyLogo
                FROM jobs j
                LEFT JOIN companies c ON j.company_id = c.id
                LEFT JOIN categories cat ON j.category_id = cat.id
                WHERE j.id = @jobId AND j.status = 'active'";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@jobId", jobId);

                conn.Open();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (!reader.Read())
                        return;

                    lblTitle.Text = reader["title"].ToString();
                    lblTitleInner.Text = reader["title"].ToString();
                    Page.Title = reader["title"].ToString() + " | Career Connect";
                    var descRaw = reader["description"]?.ToString() ?? "";
                    var descPlainMeta = JobDescriptionToPlainWithBreaks(descRaw);
                    descPlainMeta = Regex.Replace(descPlainMeta, @"\s+", " ").Trim();
                    if (descPlainMeta.Length > 200)
                        descPlainMeta = descPlainMeta.Substring(0, 197).TrimEnd() + "…";
                    Page.MetaDescription = string.IsNullOrEmpty(descPlainMeta) ? "Job listing on Career Connect." : descPlainMeta;
                    lblCompany.Text = reader["CompanyName"].ToString();
                    var isVer = SafeRead(reader, "IsCompanyVerified");
                    bool verified = isVer != null && Convert.ToInt32(isVer) == 1;
                    lblVerified.Text = verified ? "<span class=\"badge bg-success\">Verified employer</span>" : "";
                    lblCategory.Text = reader["CategoryTitle"]?.ToString() ?? "—";
                    lblLocation.Text = reader["location"].ToString();
                    lblSalary.Text = FormatSalaryLine(reader);
                    lblJobType.Text = reader["job_type"].ToString();
                    lblCreatedAt.Text = Convert.ToDateTime(reader["created_at"]).ToString("dd MMM yyyy");

                    var deadline = SafeRead(reader, "application_deadline");
                    if (deadline != null)
                        lblDeadline.Text = Convert.ToDateTime(deadline).ToString("dd MMM yyyy");
                    else
                        lblDeadline.Text = "Not specified";

                    var wa = (SafeRead(reader, "work_arrangement") ?? "onsite").ToString();
                    lblWorkArr.Text = FormatWorkArrangement(wa);

                    var hta = SafeRead(reader, "how_to_apply") as string;
                    if (!string.IsNullOrWhiteSpace(hta))
                        lblHowToApply.Text = FormatMultilinePlainText(hta);
                    else
                        lblHowToApply.Text = "<span class=\"text-muted\">Use Apply on this page and add your message below.</span>";

                    litDescription.Text = FormatJobDescriptionAsBulletsHtml(descRaw);

                    var req = reader["requirements"]?.ToString();
                    lblRequirements.Text = string.IsNullOrWhiteSpace(req)
                        ? "<span class=\"text-muted\">No detailed requirements listed.</span>"
                        : FormatMultilinePlainText(req);
                    lblCompanyInfo.Text = FormatMultilinePlainText(reader["CompanyDescription"]?.ToString() ?? "");

                    object cidObj = SafeRead(reader, "company_id");
                    if (cidObj == null || cidObj == DBNull.Value)
                        cidObj = SafeRead(reader, "CompanyId");
                    if (cidObj == null || cidObj == DBNull.Value)
                        cidObj = SafeRead(reader, "companyid");

                    if (cidObj != null && cidObj != DBNull.Value)
                    {
                        int cid = Convert.ToInt32(cidObj);
                        CcMsgCompanyId = cid;
                        hypCompanyProfile.NavigateUrl = ResolveUrl("~/USER/CompanyPublic.aspx?id=" + cid);
                        hypCompanyProfile.Visible = true;
                        hypMsgEmployer.Visible = true;
                        string msgQ = "default.aspx?msg=1&companyId=" + cid + "&jobId=" + jobId;
                        if (Session["userId"] != null)
                        {
                            hypMsgEmployer.NavigateUrl = "#";
                            hypMsgEmployer.Text = "Message employer";
                            hypMsgEmployer.CssClass = "btn btn-outline-secondary w-100 mb-2 cc-msg-open-trigger";
                            hypMsgEmployer.Attributes["data-cc-company-id"] = cid.ToString();
                            hypMsgEmployer.Attributes["data-cc-job-id"] = jobId.ToString();
                        }
                        else
                        {
                            hypMsgEmployer.NavigateUrl = "Login.aspx?role=user&return=" + Server.UrlEncode(msgQ);
                            hypMsgEmployer.Text = "Sign in to message employer";
                            hypMsgEmployer.CssClass = "btn btn-outline-secondary w-100 mb-2";
                            hypMsgEmployer.Attributes.Remove("data-cc-company-id");
                            hypMsgEmployer.Attributes.Remove("data-cc-job-id");
                        }
                    }
                    else
                    {
                        hypCompanyProfile.Visible = false;
                        hypMsgEmployer.Visible = false;
                    }

                    var extKeyObj = SafeRead(reader, "external_key");
                    string extKey = extKeyObj as string ?? extKeyObj?.ToString();
                    bool syndicated = !string.IsNullOrEmpty(extKey);
                    pnlSyndicated.Visible = syndicated;

                    hypExternalApply.Visible = false;
                    if (syndicated)
                    {
                        btnShowApply.Visible = false;
                        pnlApply.Visible = false;
                        hypMsgEmployer.Visible = false;

                        var applyRaw = SafeRead(reader, "external_apply_url");
                        string applyCol = applyRaw as string ?? applyRaw?.ToString();
                        string applyUrl = ExternalJobLinkHelper.SanitizeHttpUrl(applyCol);
                        if (string.IsNullOrEmpty(applyUrl))
                            applyUrl = ExternalJobLinkHelper.ExtractFirstHttpUrl(hta);
                        _pendingExternalApplyUrl = applyUrl;
                        if (!string.IsNullOrEmpty(applyUrl))
                        {
                            hypExternalApply.NavigateUrl = applyUrl;
                            hypExternalApply.Target = "_blank";
                            hypExternalApply.Attributes["rel"] = "noopener noreferrer";
                            string src = (SafeRead(reader, "external_source_label") as string ?? "").Trim();
                            hypExternalApply.Text = "Apply on official site";
                            hypExternalApply.ToolTip = string.IsNullOrEmpty(src)
                                ? "Opens the employer’s application page in a new tab."
                                : "Source: " + src + " — opens in a new tab.";
                            hypExternalApply.Visible = true;
                        }
                    }
                }

                if (Session["userId"] != null)
                {
                    int uid = Convert.ToInt32(Session["userId"]);
                    UpdateSaveUI(conn, uid, jobId);
                    if (UserAlreadyApplied(conn, uid, jobId))
                    {
                        pnlAlreadyApplied.Visible = true;
                        btnShowApply.Visible = false;
                        pnlApply.Visible = false;
                    }
                }
            }
        }

        private static string FormatSalaryLine(MySqlDataReader reader)
        {
            if (reader["salary_min"] != DBNull.Value && reader["salary_max"] != DBNull.Value)
                return "$" + reader["salary_min"] + " - $" + reader["salary_max"];
            if (reader["salary_min"] != DBNull.Value)
                return "$" + reader["salary_min"];
            if (reader["salary_max"] != DBNull.Value)
                return "$" + reader["salary_max"];
            return "Salary not specified";
        }

        private static string FormatJobDescriptionAsBulletsHtml(string raw)
        {
            var plain = JobDescriptionToPlainWithBreaks(raw ?? "");
            plain = TruncatePlainForSummary(plain, 720);
            var points = SplitIntoSummaryPoints(plain);
            if (points.Count == 0)
                return "<p class=\"text-muted mb-0\">No description provided.</p>";

            const int maxBullets = 7;
            const int maxCharsPerPoint = 160;
            points = CapPoints(points, maxBullets);

            var sb = new StringBuilder();
            sb.Append("<ul class=\"cc-job-desc-list\">");
            foreach (var p in points)
            {
                var t = NormalizeReadableCasing(p.Trim());
                if (t.Length > maxCharsPerPoint)
                    t = TruncateAtWord(t, maxCharsPerPoint) + "…";
                sb.Append("<li>").Append(HttpUtility.HtmlEncode(t)).Append("</li>");
            }
            sb.Append("</ul>");
            return sb.ToString();
        }

        /// <summary>Keep only the start of the description for a short summary.</summary>
        private static string TruncatePlainForSummary(string plain, int maxChars)
        {
            if (string.IsNullOrEmpty(plain) || plain.Length <= maxChars)
                return plain.Trim();
            var cut = plain.Substring(0, maxChars).TrimEnd();
            int lastSpace = cut.LastIndexOf(' ');
            if (lastSpace > maxChars / 2)
                cut = cut.Substring(0, lastSpace).TrimEnd();
            return cut + "…";
        }

        private static string TruncateAtWord(string s, int maxLen)
        {
            if (string.IsNullOrEmpty(s) || s.Length <= maxLen)
                return s;
            var cut = s.Substring(0, maxLen).TrimEnd();
            int sp = cut.LastIndexOf(' ');
            if (sp > maxLen / 3)
                cut = cut.Substring(0, sp).TrimEnd();
            return cut;
        }

        /// <summary>Feeds often use ALL CAPS — convert to normal sentence casing when mostly uppercase.</summary>
        private static string NormalizeReadableCasing(string s)
        {
            if (string.IsNullOrWhiteSpace(s))
                return s;

            int letters = 0, upper = 0;
            foreach (var c in s)
            {
                if (char.IsLetter(c))
                {
                    letters++;
                    if (char.IsUpper(c))
                        upper++;
                }
            }

            if (letters >= 4 && (double)upper / letters >= 0.5)
            {
                var lower = s.ToLowerInvariant();
                lower = Regex.Replace(lower, @"\bi\b", "I");
                if (lower.Length == 0)
                    return s;
                return char.ToUpperInvariant(lower[0]) + (lower.Length > 1 ? lower.Substring(1) : "");
            }

            return Regex.Replace(s, @"(?<=\s)i(?=\s)", "I");
        }

        /// <summary>Decode entities, turn block HTML into newlines, strip remaining tags → plain text with line breaks.</summary>
        private static string JobDescriptionToPlainWithBreaks(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";
            var s = raw.Trim();
            for (var i = 0; i < 6; i++)
            {
                var d = HttpUtility.HtmlDecode(s);
                if (d == s) break;
                s = d;
            }
            s = Regex.Replace(s, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"</p\s*>", "\n\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"</div\s*>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"</li\s*>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"<li[^>]*>", "\n• ", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"<[^>]+>", " ");
            s = HttpUtility.HtmlDecode(s);
            s = Regex.Replace(s, @"[ \t\f\v]+", " ");
            s = Regex.Replace(s, @"\n{3,}", "\n\n").Trim();
            return s;
        }

        private static List<string> SplitIntoSummaryPoints(string plainWithBreaks)
        {
            var list = new List<string>();
            if (string.IsNullOrWhiteSpace(plainWithBreaks))
                return list;

            var byLines = Regex.Split(plainWithBreaks.Trim(), @"\r?\n+")
                .Select(x => Regex.Replace(x.Trim(), @"^\s*•\s*", ""))
                .Where(x => x.Length > 0)
                .ToList();

            if (byLines.Count > 1)
            {
                foreach (var line in byLines)
                {
                    if (line.Length > 2)
                        list.Add(line);
                }
                return CapPoints(list, 12);
            }

            var one = Regex.Replace(plainWithBreaks, @"\s+", " ").Trim();
            if (one.Length < 80)
            {
                list.Add(one);
                return list;
            }

            var byMarkers = Regex.Split(one, @"\s+(?:•|-|—)\s+")
                .Select(x => x.Trim())
                .Where(x => x.Length > 2)
                .ToList();
            if (byMarkers.Count > 1)
                return CapPoints(byMarkers, 12);

            var sentences = Regex.Split(one, @"(?<=[\.\!\?])\s+(?=[A-Z""'(0-9])");
            foreach (var seg in sentences)
            {
                var t = seg.Trim();
                if (t.Length < 10)
                    continue;
                list.Add(t);
            }

            if (list.Count == 0)
                list.Add(one);
            return CapPoints(list, 12);
        }

        private static List<string> CapPoints(List<string> points, int max)
        {
            if (points == null || points.Count <= max)
                return points ?? new List<string>();
            return points.Take(max).ToList();
        }

        /// <summary>Turns feed/HTML snippets into safe display: decode entities, strip tags, encode, line breaks.</summary>
        private static string FormatMultilinePlainText(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";
            var s = raw.Trim();
            for (var i = 0; i < 6; i++)
            {
                var d = HttpUtility.HtmlDecode(s);
                if (d == s) break;
                s = d;
            }
            s = Regex.Replace(s, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"</p\s*>", "\n\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"</div\s*>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"</li\s*>", "\n", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"<[^>]+>", " ");
            s = HttpUtility.HtmlDecode(s);
            s = Regex.Replace(s, @"[ \t\f\v]{2,}", " ");
            s = Regex.Replace(s, @"\n{3,}", "\n\n").Trim();
            s = HttpUtility.HtmlEncode(s);
            return s.Replace("\n", "<br/>");
        }

        private static string StripToPlainPreview(string raw, int maxLen)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return "";
            var s = raw.Trim();
            for (var i = 0; i < 6; i++)
            {
                var d = HttpUtility.HtmlDecode(s);
                if (d == s) break;
                s = d;
            }
            s = Regex.Replace(s, @"<[^>]+>", " ");
            s = HttpUtility.HtmlDecode(s);
            s = Regex.Replace(s, @"\s+", " ").Trim();
            if (s.Length > maxLen)
                s = s.Substring(0, maxLen).TrimEnd() + "…";
            return s;
        }

        private void UpdateSaveUI(MySqlConnection conn, int userId, int jid)
        {
            bool isSaved = false;
            using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM saved_jobs WHERE user_id=@u AND job_id=@j", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@j", jid);
                isSaved = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }

            btnToggleSave.CssClass = isSaved ? "btn btn-primary w-100" : "btn btn-outline-primary w-100";
            litSaveLabel.Text = isSaved ? "Saved" : "Save job";
        }

        private static string FormatWorkArrangement(string wa)
        {
            if (string.IsNullOrWhiteSpace(wa))
                return "On-site";
            switch (wa.Trim().ToLowerInvariant())
            {
                case "remote": return "Remote";
                case "hybrid": return "Hybrid";
                default: return "On-site";
            }
        }

        private static bool UserAlreadyApplied(MySqlConnection conn, int userId, int jid)
        {
            using (var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM applications WHERE user_id = @u AND job_id = @j", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                cmd.Parameters.AddWithValue("@j", jid);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        protected void btnShowApply_Click(object sender, EventArgs e)
        {
            if (Session["userId"] != null)
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    if (UserAlreadyApplied(conn, Convert.ToInt32(Session["userId"]), jobId))
                    {
                        pnlAlreadyApplied.Visible = true;
                        btnShowApply.Visible = false;
                        return;
                    }
                }
            }
            pnlApply.Visible = true;
        }

        protected void btnApply_Click(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
            {
                lblApplyResult.Text = "You must be logged in to apply.";
                return;
            }

            int uid = Convert.ToInt32(Session["userId"]);
            using (MySqlConnection conn = new MySqlConnection(connStr))
            {
                conn.Open();
                if (UserAlreadyApplied(conn, uid, jobId))
                {
                    lblApplyResult.Text = "You have already applied for this job.";
                    pnlApply.Visible = false;
                    pnlAlreadyApplied.Visible = true;
                    btnShowApply.Visible = false;
                    return;
                }

                string insert = @"INSERT INTO applications (user_id, job_id, cover_letter, applied_at)
                                  VALUES (@userId, @jobId, @coverLetter, NOW())";

                MySqlCommand cmd = new MySqlCommand(insert, conn);
                cmd.Parameters.AddWithValue("@userId", uid);
                cmd.Parameters.AddWithValue("@jobId", jobId);
                cmd.Parameters.AddWithValue("@coverLetter", txtMessage.Text.Trim());

                cmd.ExecuteNonQuery();

                if (jobId > 0)
                    ApplicationNotificationMail.TryNotifyApplicationSubmitted(connStr, uid, jobId);

                lblApplyResult.Text = "Application submitted successfully.";
                pnlApply.Visible = false;
                pnlAlreadyApplied.Visible = true;
                btnShowApply.Visible = false;
            }
        }

        protected void btnToggleSave_Click(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
                return;
            int uid = Convert.ToInt32(Session["userId"]);
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                bool isSaved = false;
                using (var cmd = new MySqlCommand("SELECT COUNT(*) FROM saved_jobs WHERE user_id=@u AND job_id=@j", conn))
                {
                    cmd.Parameters.AddWithValue("@u", uid);
                    cmd.Parameters.AddWithValue("@j", jobId);
                    isSaved = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                }

                if (isSaved)
                {
                    using (var cmd = new MySqlCommand("DELETE FROM saved_jobs WHERE user_id=@u AND job_id=@j", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", uid);
                        cmd.Parameters.AddWithValue("@j", jobId);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new MySqlCommand("INSERT IGNORE INTO saved_jobs (user_id, job_id, created_at) VALUES (@u,@j,NOW())", conn))
                    {
                        cmd.Parameters.AddWithValue("@u", uid);
                        cmd.Parameters.AddWithValue("@j", jobId);
                        cmd.ExecuteNonQuery();
                    }
                }

                UpdateSaveUI(conn, uid, jobId);
            }
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            if (Session["userId"] == null)
                return;
            int uid = Convert.ToInt32(Session["userId"]);
            string reason = (ddlReportReason.SelectedValue ?? "other").Trim();
            if (string.IsNullOrWhiteSpace(reason))
                reason = "other";
            string details = (txtReportDetails.Text ?? "").Trim();
            if (details.Length > 2000)
                details = details.Substring(0, 2000);

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();

                int companyId = 0;
                using (var cmd = new MySqlCommand("SELECT company_id FROM jobs WHERE id=@j LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@j", jobId);
                    var o = cmd.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        companyId = Convert.ToInt32(o);
                }

                using (var cmd = new MySqlCommand(
                    @"INSERT INTO reports (reporter_user_id, job_id, company_id, reason, details, status, created_at)
                      VALUES (@u, @j, @c, @r, @d, 'open', NOW())", conn))
                {
                    cmd.Parameters.AddWithValue("@u", uid);
                    cmd.Parameters.AddWithValue("@j", jobId);
                    cmd.Parameters.AddWithValue("@c", companyId > 0 ? (object)companyId : DBNull.Value);
                    cmd.Parameters.AddWithValue("@r", reason);
                    cmd.Parameters.AddWithValue("@d", string.IsNullOrWhiteSpace(details) ? (object)DBNull.Value : details);
                    cmd.ExecuteNonQuery();
                }

                lblReportResult.CssClass = "small text-success d-block mt-2";
                lblReportResult.Text = "Thanks — your report was submitted to the admin team.";
                txtReportDetails.Text = "";
            }
        }
    }
}
