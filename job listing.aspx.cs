using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;

namespace CareerConnect.USER
{
    public partial class job_listing : System.Web.UI.Page
    {
        private const int RecommendationTopK = 12;
        private const double MinCosineToShow = 0.012;
        private const double MinCosineLoose = 0.006;
        private const int PageSize = 10;

        private HashSet<int> SavedJobIds
        {
            get { return ViewState["sj"] as HashSet<int> ?? new HashSet<int>(); }
            set { ViewState["sj"] = value; }
        }

        private int CurrentPage
        {
            get { return ViewState["cp"] as int? ?? 1; }
            set { ViewState["cp"] = value < 1 ? 1 : value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            BindCategoryFilter();
            if (!IsPostBack)
                CurrentPage = 1;
            LoadJobs();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            CurrentPage = 1;
            LoadJobs();
        }

        protected void btnPrev_Click(object sender, EventArgs e)
        {
            CurrentPage--;
            LoadJobs();
        }

        protected void btnNext_Click(object sender, EventArgs e)
        {
            CurrentPage++;
            LoadJobs();
        }

        private void BindCategoryFilter()
        {
            if (ddlCategoryFilter.Items.Count > 0)
                return;

            ddlCategoryFilter.Items.Add(new ListItem("All categories", ""));
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            try
            {
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand("SELECT id, name FROM categories ORDER BY name", conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                            ddlCategoryFilter.Items.Add(new ListItem(r["name"].ToString(), r["id"].ToString()));
                    }
                }
            }
            catch
            {
                /* ignore */
            }
        }

        private void LoadJobs()
        {
            string connStr = ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;
            pnlRecommendations.Visible = false;
            rptRecommended.DataSource = null;
            rptRecommended.DataBind();
            pnlPager.Visible = false;
            SavedJobIds = new HashSet<int>();

            string q = (txtKeyword.Text ?? "").Trim();
            string loc = (txtLocation.Text ?? "").Trim();
            string jt = ddlJobTypeFilter.SelectedValue ?? "";
            int catId;
            int.TryParse(ddlCategoryFilter.SelectedValue, out catId);
            decimal? smin = null, smax = null;
            decimal d1, d2;
            if (decimal.TryParse(txtSalMin.Text.Trim(), out d1))
                smin = d1;
            if (decimal.TryParse(txtSalMax.Text.Trim(), out d2))
                smax = d2;
            int days = 0;
            int.TryParse(ddlPosted.SelectedValue, out days);

            using (var conn = new MySqlConnection(connStr))
            {
                try
                {
                    conn.Open();

                    var where = new StringBuilder(" WHERE j.status = 'active' ");
                    var cmdCount = new MySqlCommand { Connection = conn };
                    var cmdList = new MySqlCommand { Connection = conn };

                    if (!string.IsNullOrEmpty(q))
                    {
                        where.Append(@" AND (
 j.title LIKE @q OR j.description LIKE @q OR j.requirements LIKE @q
                            OR j.skills_required LIKE @q OR c.name LIKE @q OR cat.name LIKE @q) ");
                        cmdCount.Parameters.AddWithValue("@q", "%" + q + "%");
                        cmdList.Parameters.AddWithValue("@q", "%" + q + "%");
                    }

                    if (catId > 0)
                    {
                        where.Append(" AND j.category_id = @cat ");
                        cmdCount.Parameters.AddWithValue("@cat", catId);
                        cmdList.Parameters.AddWithValue("@cat", catId);
                    }

                    if (!string.IsNullOrEmpty(loc))
                    {
                        where.Append(" AND j.location LIKE @loc ");
                        cmdCount.Parameters.AddWithValue("@loc", "%" + loc + "%");
                        cmdList.Parameters.AddWithValue("@loc", "%" + loc + "%");
                    }

                    if (!string.IsNullOrEmpty(jt))
                    {
                        where.Append(" AND j.job_type = @jt ");
                        cmdCount.Parameters.AddWithValue("@jt", jt);
                        cmdList.Parameters.AddWithValue("@jt", jt);
                    }

                    if (smin.HasValue)
                    {
                        where.Append(" AND (j.salary_max IS NULL OR j.salary_max >= @smin OR j.salary_min >= @smin) ");
                        cmdCount.Parameters.AddWithValue("@smin", smin.Value);
                        cmdList.Parameters.AddWithValue("@smin", smin.Value);
                    }

                    if (smax.HasValue)
                    {
                        where.Append(" AND (j.salary_min IS NULL OR j.salary_min <= @smax) ");
                        cmdCount.Parameters.AddWithValue("@smax", smax.Value);
                        cmdList.Parameters.AddWithValue("@smax", smax.Value);
                    }

                    if (days > 0)
                    {
                        where.Append(" AND j.created_at >= DATE_SUB(NOW(), INTERVAL " + days + " DAY) ");
                    }

                    string fromSql = @"
            FROM jobs j
            LEFT JOIN companies c ON j.company_id = c.id
            LEFT JOIN categories cat ON j.category_id = cat.id ";

                    cmdCount.CommandText = "SELECT COUNT(*) " + fromSql + where;
                    int total = Convert.ToInt32(cmdCount.ExecuteScalar());

                    if (total == 0)
                    {
                        pnlNoJobs.Visible = true;
                        rptJobs.Visible = false;
                        litResultSummary.Text = "No jobs match your filters.";
                        return;
                    }

                    int totalPages = Math.Max(1, (int)Math.Ceiling(total / (double)PageSize));
                    if (CurrentPage > totalPages)
                        CurrentPage = totalPages;

                    int offset = (CurrentPage - 1) * PageSize;

                    string query = @"SELECT 
                j.id, 
                j.title,
                j.description,
                j.requirements,
                j.skills_required,
                COALESCE(c.name, 'Unknown Company') AS CompanyName, 
                COALESCE(cat.name, 'Uncategorized') AS CategoryTitle, 
                COALESCE(j.location, 'Location not specified') AS location, 
                COALESCE(c.is_verified, 0) AS is_verified,
                j.external_key AS ExternalKey,
                j.external_source_label AS ExternalSourceLabel,
                j.external_apply_url AS ExternalApplyUrl,
                CASE 
                    WHEN j.salary_min IS NOT NULL AND j.salary_max IS NOT NULL THEN CONCAT(j.salary_min, ' - ', j.salary_max)
                    WHEN j.salary_min IS NOT NULL THEN CONCAT(j.salary_min)
                    WHEN j.salary_max IS NOT NULL THEN CONCAT(j.salary_max)
                    ELSE 'Salary not specified'
                END AS salary,
                COALESCE(j.job_type, 'Full-time') AS job_type, 
                COALESCE(c.logo, '../assets/img/logo/default-company.png') AS company_logo, 
                j.created_at
            " + fromSql + where + " ORDER BY j.created_at DESC LIMIT @lim OFFSET @off";

                    cmdList.CommandText = query;
                    cmdList.Parameters.AddWithValue("@lim", PageSize);
                    cmdList.Parameters.AddWithValue("@off", offset);

                    var dt = new DataTable();
                    using (var da = new MySqlDataAdapter(cmdList))
                    {
                        da.Fill(dt);
                    }

                    HydrateSavedJobs(conn, dt);

                    pnlNoJobs.Visible = false;
                    rptJobs.Visible = true;
                    rptJobs.DataSource = dt;
                    rptJobs.DataBind();

                    litResultSummary.Text = string.Format("Showing {0}–{1} of {2} jobs", offset + 1, offset + dt.Rows.Count, total);
                    pnlPager.Visible = totalPages > 1;
                    btnPrev.Enabled = CurrentPage > 1;
                    btnNext.Enabled = CurrentPage < totalPages;
                    litPageLabel.Text = "Page " + CurrentPage + " / " + totalPages;

                    var dtAll = LoadAllActiveJobsForRecommendations(conn);
                    if (dtAll != null && dtAll.Rows.Count > 0)
                    {
                        HydrateSavedJobs(conn, dtAll);
                        TryBindRecommendations(conn, dtAll);
                    }
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('Error loading jobs: " + ex.Message.Replace("'", "\\'") + "');</script>");
                }
            }
        }

        private void HydrateSavedJobs(MySqlConnection conn, DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return;
            if (Session["userId"] == null)
                return;

            int uid = Convert.ToInt32(Session["userId"]);

            if (!dt.Columns.Contains("IsSaved"))
                dt.Columns.Add("IsSaved", typeof(bool));

            var ids = new List<int>();
            foreach (DataRow row in dt.Rows)
            {
                int id;
                if (int.TryParse(row["id"].ToString(), out id))
                    ids.Add(id);
            }

            var saved = LoadSavedIds(conn, uid, ids);
            SavedJobIds = saved;

            foreach (DataRow row in dt.Rows)
            {
                int id;
                if (int.TryParse(row["id"].ToString(), out id))
                    row["IsSaved"] = saved.Contains(id);
                else
                    row["IsSaved"] = false;
            }
        }

        private static HashSet<int> LoadSavedIds(MySqlConnection conn, int userId, List<int> jobIds)
        {
            var result = new HashSet<int>();
            if (jobIds == null || jobIds.Count == 0)
                return result;

            var uniq = jobIds.Distinct().Take(200).ToList();
            var ps = new List<string>();
            using (var cmd = new MySqlCommand())
            {
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("@u", userId);
                for (int i = 0; i < uniq.Count; i++)
                {
                    string p = "@j" + i;
                    ps.Add(p);
                    cmd.Parameters.AddWithValue(p, uniq[i]);
                }
                cmd.CommandText = "SELECT job_id FROM saved_jobs WHERE user_id=@u AND job_id IN (" + string.Join(",", ps) + ")";
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                        result.Add(Convert.ToInt32(r["job_id"]));
                }
            }
            return result;
        }

        private void ToggleSave(MySqlConnection conn, int userId, int jobId)
        {
            if (userId <= 0 || jobId <= 0)
                return;

            bool isSaved = SavedJobIds.Contains(jobId);
            if (isSaved)
            {
                using (var cmd = new MySqlCommand("DELETE FROM saved_jobs WHERE user_id=@u AND job_id=@j", conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@j", jobId);
                    cmd.ExecuteNonQuery();
                }
                SavedJobIds.Remove(jobId);
            }
            else
            {
                using (var cmd = new MySqlCommand("INSERT IGNORE INTO saved_jobs (user_id, job_id, created_at) VALUES (@u,@j,NOW())", conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    cmd.Parameters.AddWithValue("@j", jobId);
                    cmd.ExecuteNonQuery();
                }
                SavedJobIds.Add(jobId);
            }
        }

        protected void rptJobs_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;
            var row = e.Item.DataItem as DataRowView;
            if (row == null)
                return;
            var btn = e.Item.FindControl("btnSave") as LinkButton;
            var lit = e.Item.FindControl("litSaveText") as Literal;
            var litVerified = e.Item.FindControl("litVerifiedBadge") as Literal;
            if (btn == null || lit == null)
                return;

            bool isSaved = false;
            if (row.Row.Table.Columns.Contains("IsSaved"))
                isSaved = Convert.ToBoolean(row["IsSaved"]);

            if (litVerified != null && row.Row.Table.Columns.Contains("is_verified"))
            {
                var v = Convert.ToString(row["is_verified"]);
                bool verified = v == "1" || string.Equals(v, "true", StringComparison.OrdinalIgnoreCase);
                litVerified.Text = verified ? "<span class=\"badge bg-success ms-1\">Verified</span>" : "";
            }

            var litExt = e.Item.FindControl("litExternalSrc") as Literal;
            if (litExt != null && row.Row.Table.Columns.Contains("ExternalKey"))
            {
                var ek = row["ExternalKey"] as string ?? (row["ExternalKey"] == DBNull.Value ? "" : Convert.ToString(row["ExternalKey"]));
                if (!string.IsNullOrEmpty(ek))
                {
                    string src = row.Row.Table.Columns.Contains("ExternalSourceLabel") && row["ExternalSourceLabel"] != DBNull.Value
                        ? Convert.ToString(row["ExternalSourceLabel"]).Trim()
                        : "";
                    litExt.Text = string.IsNullOrEmpty(src)
                        ? " <span class=\"badge bg-info text-dark ms-1\">External listing</span>"
                        : " <span class=\"badge bg-info text-dark ms-1\">" + System.Web.HttpUtility.HtmlEncode(src) + "</span>";
                }
            }

            if (Session["userId"] == null)
            {
                btn.Visible = false;
                return;
            }

            btn.CssClass = isSaved ? "btn btn-primary btn-sm" : "btn btn-outline-primary btn-sm";
            lit.Text = isSaved ? "Saved" : "Save";
        }

        protected void rptRecommended_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            rptJobs_ItemDataBound(sender, e);
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;
            var litRec = e.Item.FindControl("litRecMatch") as Literal;
            if (litRec == null)
                return;
            var row = e.Item.DataItem as DataRowView;
            if (row == null)
                return;
            bool profile = row.Row.Table.Columns.Contains("IsProfileMatch") &&
                           row["IsProfileMatch"] != DBNull.Value &&
                           Convert.ToBoolean(row["IsProfileMatch"]);
            int pct = row.Row.Table.Columns.Contains("MatchPct") && row["MatchPct"] != DBNull.Value
                ? Convert.ToInt32(row["MatchPct"])
                : 0;
            if (profile && pct > 0)
                litRec.Text = "<span class=\"badge bg-success ms-1\">" + pct + "% match</span>";
            else
                litRec.Text = "<span class=\"badge bg-secondary ms-1\">Featured</span>";
        }

        protected void rptJobs_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (!string.Equals(e.CommandName, "toggle_save", StringComparison.OrdinalIgnoreCase))
                return;
            if (Session["userId"] == null)
                return;

            int jobId;
            if (!int.TryParse(Convert.ToString(e.CommandArgument), out jobId))
                return;

            int uid = Convert.ToInt32(Session["userId"]);
            using (var conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString))
            {
                conn.Open();
                ToggleSave(conn, uid, jobId);
            }
            LoadJobs();
        }

        protected void rptRecommended_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            rptJobs_ItemCommand(source, e);
        }

        private static DataTable LoadAllActiveJobsForRecommendations(MySqlConnection conn)
        {
            string query = @"SELECT 
                j.id, 
                j.title,
                j.description,
                j.requirements,
                j.skills_required,
                COALESCE(c.name, 'Unknown Company') AS CompanyName, 
                COALESCE(cat.name, 'Uncategorized') AS CategoryTitle, 
                COALESCE(j.location, 'Location not specified') AS location, 
                COALESCE(c.is_verified, 0) AS is_verified,
                j.external_key AS ExternalKey,
                j.external_source_label AS ExternalSourceLabel,
                j.external_apply_url AS ExternalApplyUrl,
                CASE 
                    WHEN j.salary_min IS NOT NULL AND j.salary_max IS NOT NULL THEN CONCAT(j.salary_min, ' - ', j.salary_max)
                    WHEN j.salary_min IS NOT NULL THEN CONCAT(j.salary_min)
                    WHEN j.salary_max IS NOT NULL THEN CONCAT(j.salary_max)
                    ELSE 'Salary not specified'
                END AS salary,
                COALESCE(j.job_type, 'Full-time') AS job_type, 
                COALESCE(c.logo, '../assets/img/logo/default-company.png') AS company_logo, 
                j.created_at
            FROM jobs j
            LEFT JOIN companies c ON j.company_id = c.id
            LEFT JOIN categories cat ON j.category_id = cat.id
            WHERE j.status = 'active'
            ORDER BY j.created_at DESC";

            var dt = new DataTable();
            using (var cmd = new MySqlCommand(query, conn))
            using (var da = new MySqlDataAdapter(cmd))
            {
                da.Fill(dt);
            }
            return dt;
        }

        private void TryBindRecommendations(MySqlConnection conn, DataTable allJobs)
        {
            if (Session["userId"] == null)
                return;
            if (allJobs == null || allJobs.Rows.Count == 0)
                return;

            int userId = Convert.ToInt32(Session["userId"]);
            string bio = "";
            using (var cmd = new MySqlCommand("SELECT bio FROM users WHERE id = @u LIMIT 1", conn))
            {
                cmd.Parameters.AddWithValue("@u", userId);
                var o = cmd.ExecuteScalar();
                bio = o != null && o != DBNull.Value ? o.ToString() : "";
            }

            var skills = JobRecommendationQueries.LoadUserSkillNames(conn, userId);
            string corpus = TfIdfJobRecommender.BuildUserProfileCorpus(bio, skills);
            var applied = JobRecommendationQueries.LoadAppliedJobIds(conn, userId);

            DataTable dtRec = null;

            if (!string.IsNullOrWhiteSpace(corpus))
            {
                var ranked = TfIdfJobRecommender.RankJobs(corpus, allJobs, applied, RecommendationTopK);
                if (ranked.Count > 0)
                {
                    double best = ranked[0].Item2;
                    double floor = best >= MinCosineToShow ? MinCosineToShow : MinCosineLoose;
                    if (best >= floor)
                        dtRec = BuildProfileRecommendationTable(ranked, allJobs, floor);
                }
            }

            if (dtRec == null || dtRec.Rows.Count == 0)
                dtRec = BuildRecentFallbackRecommendations(allJobs, applied, 6);

            if (dtRec == null || dtRec.Rows.Count == 0)
                return;

            pnlRecommendations.Visible = true;
            rptRecommended.DataSource = dtRec;
            rptRecommended.DataBind();
        }

        private static DataTable CreateRecommendationResultSchema()
        {
            var dtRec = new DataTable();
            dtRec.Columns.Add("id", typeof(int));
            dtRec.Columns.Add("title", typeof(string));
            dtRec.Columns.Add("CompanyName", typeof(string));
            dtRec.Columns.Add("CategoryTitle", typeof(string));
            dtRec.Columns.Add("location", typeof(string));
            dtRec.Columns.Add("salary", typeof(string));
            dtRec.Columns.Add("job_type", typeof(string));
            dtRec.Columns.Add("company_logo", typeof(string));
            dtRec.Columns.Add("created_at", typeof(DateTime));
            dtRec.Columns.Add("MatchPct", typeof(int));
            dtRec.Columns.Add("IsProfileMatch", typeof(bool));
            return dtRec;
        }

        private static DataTable BuildProfileRecommendationTable(List<Tuple<int, double>> ranked, DataTable allJobs, double minCosine)
        {
            var dtRec = CreateRecommendationResultSchema();

            foreach (var pair in ranked)
            {
                if (pair.Item2 < minCosine)
                    break;
                DataRow[] found = allJobs.Select("id = " + pair.Item1);
                if (found.Length == 0)
                    continue;
                var src = found[0];
                var nr = dtRec.NewRow();
                nr["id"] = src["id"];
                nr["title"] = src["title"];
                nr["CompanyName"] = src["CompanyName"];
                nr["CategoryTitle"] = src["CategoryTitle"];
                nr["location"] = src["location"];
                nr["salary"] = src["salary"];
                nr["job_type"] = src["job_type"];
                nr["company_logo"] = src["company_logo"];
                nr["created_at"] = src["created_at"];
                nr["MatchPct"] = (int)Math.Min(99, Math.Round(pair.Item2 * 100.0));
                nr["IsProfileMatch"] = true;
                dtRec.Rows.Add(nr);
            }

            return dtRec;
        }

        private static DataTable BuildRecentFallbackRecommendations(DataTable allJobs, HashSet<int> applied, int take)
        {
            var dtRec = CreateRecommendationResultSchema();
            var rows = new List<DataRow>();
            foreach (DataRow r in allJobs.Rows)
            {
                int id = Convert.ToInt32(r["id"]);
                if (applied != null && applied.Contains(id))
                    continue;
                rows.Add(r);
            }

            rows.Sort((a, b) =>
                DateTime.Compare(
                    SafeJobDate(b["created_at"]),
                    SafeJobDate(a["created_at"])));

            int n = Math.Min(take, rows.Count);
            for (int i = 0; i < n; i++)
            {
                var src = rows[i];
                var nr = dtRec.NewRow();
                nr["id"] = src["id"];
                nr["title"] = src["title"];
                nr["CompanyName"] = src["CompanyName"];
                nr["CategoryTitle"] = src["CategoryTitle"];
                nr["location"] = src["location"];
                nr["salary"] = src["salary"];
                nr["job_type"] = src["job_type"];
                nr["company_logo"] = src["company_logo"];
                nr["created_at"] = src["created_at"];
                nr["MatchPct"] = 0;
                nr["IsProfileMatch"] = false;
                dtRec.Rows.Add(nr);
            }

            return dtRec;
        }

        private static DateTime SafeJobDate(object o)
        {
            if (o == null || o == DBNull.Value)
                return DateTime.MinValue;
            try
            {
                return Convert.ToDateTime(o);
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

    }
}
