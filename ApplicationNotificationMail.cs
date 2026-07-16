using System;
using System.Configuration;
using System.Text;
using MySql.Data.MySqlClient;

namespace CareerConnect.USER
{
    /// <summary>
    /// Transactional emails for job applications (apply + optional status updates from ASP.NET).
    /// </summary>
    public static class ApplicationNotificationMail
    {
        public static void TryNotifyApplicationSubmitted(string connStr, int applicantUserId, int jobId)
        {
            if (!MailSender.IsSmtpConfigured())
                return;

            string applicantEmail = null, firstName = null, lastName = null;
            string jobTitle = null, companyName = null, companyEmail = null, posterEmail = null;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                    SELECT u.email, u.first_name, u.last_name,
                           j.title, c.name AS company_name, c.email AS company_email,
                           eu.email AS poster_email
                    FROM users u
                    INNER JOIN jobs j ON j.id = @jid
                    INNER JOIN companies c ON j.company_id = c.id
                    LEFT JOIN users eu ON j.created_by = eu.id
                    WHERE u.id = @uid
                    LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@uid", applicantUserId);
                    cmd.Parameters.AddWithValue("@jid", jobId);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                            return;
                        applicantEmail = r["email"]?.ToString();
                        firstName = r["first_name"]?.ToString() ?? "";
                        lastName = r["last_name"]?.ToString() ?? "";
                        jobTitle = r["title"]?.ToString() ?? "Job";
                        companyName = r["company_name"]?.ToString() ?? "";
                        companyEmail = r["company_email"]?.ToString();
                        posterEmail = r["poster_email"]?.ToString();
                    }
                }
            }

            var displayName = (firstName + " " + lastName).Trim();
            if (string.IsNullOrWhiteSpace(displayName))
                displayName = "there";

            var site = (ConfigurationManager.AppSettings["SitePublicUrl"] ?? "Career Connect").Trim();

            // Applicant confirmation
            var bodyApplicant = new StringBuilder();
            bodyApplicant.AppendLine("Hello " + displayName + ",");
            bodyApplicant.AppendLine();
            bodyApplicant.AppendLine("We have received your application for:");
            bodyApplicant.AppendLine("  • " + jobTitle);
            if (!string.IsNullOrWhiteSpace(companyName))
                bodyApplicant.AppendLine("  • Company: " + companyName);
            bodyApplicant.AppendLine();
            bodyApplicant.AppendLine("You can sign in to Career Connect to track your applications.");
            bodyApplicant.AppendLine();
            bodyApplicant.AppendLine("— " + site);

            MailSender.TrySend(
                applicantEmail,
                "Application received: " + jobTitle,
                bodyApplicant.ToString(),
                isHtml: false);

            // Employer / company inbox (best-effort)
            var notifyEmployer = !string.IsNullOrWhiteSpace(companyEmail)
                ? companyEmail.Trim()
                : (posterEmail ?? "").Trim();

            if (string.IsNullOrWhiteSpace(notifyEmployer))
                return;

            var bodyEmployer = new StringBuilder();
            bodyEmployer.AppendLine("A new application was submitted on Career Connect.");
            bodyEmployer.AppendLine();
            bodyEmployer.AppendLine("Job: " + jobTitle);
            bodyEmployer.AppendLine("Applicant: " + displayName);
            bodyEmployer.AppendLine("Email: " + (applicantEmail ?? ""));
            bodyEmployer.AppendLine();
            bodyEmployer.AppendLine("Review applications in your employer portal or the admin panel.");

            MailSender.TrySend(
                notifyEmployer,
                "New application: " + jobTitle,
                bodyEmployer.ToString(),
                isHtml: false);
        }

        public static void TryNotifyStatusChanged(string connStr, int applicationId, string newStatus)
        {
            if (!MailSender.IsSmtpConfigured() || string.IsNullOrWhiteSpace(newStatus))
                return;

            string applicantEmail = null, displayName = null, jobTitle = null, companyName = null;

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                    SELECT u.email, u.first_name, u.last_name, j.title, c.name AS company_name
                    FROM applications a
                    INNER JOIN users u ON a.user_id = u.id
                    INNER JOIN jobs j ON a.job_id = j.id
                    INNER JOIN companies c ON j.company_id = c.id
                    WHERE a.id = @aid
                    LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@aid", applicationId);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read())
                            return;
                        applicantEmail = r["email"]?.ToString();
                        var fn = r["first_name"]?.ToString() ?? "";
                        var ln = r["last_name"]?.ToString() ?? "";
                        displayName = (fn + " " + ln).Trim();
                        if (string.IsNullOrWhiteSpace(displayName))
                            displayName = "there";
                        jobTitle = r["title"]?.ToString() ?? "your application";
                        companyName = r["company_name"]?.ToString() ?? "";
                    }
                }
            }

            var human = StatusHumanLabel(newStatus);
            var body = new StringBuilder();
            body.AppendLine("Hello " + displayName + ",");
            body.AppendLine();
            body.AppendLine("Your application status has been updated.");
            body.AppendLine();
            body.AppendLine("Job: " + jobTitle);
            if (!string.IsNullOrWhiteSpace(companyName))
                body.AppendLine("Company: " + companyName);
            body.AppendLine("New status: " + human);
            body.AppendLine();
            body.AppendLine("Sign in to Career Connect for more details.");
            body.AppendLine();
            body.AppendLine("— Career Connect");

            MailSender.TrySend(
                applicantEmail,
                "Application update: " + jobTitle + " — " + human,
                body.ToString(),
                isHtml: false);
        }

        private static string StatusHumanLabel(string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return "Updated";
            switch (status.Trim().ToLowerInvariant())
            {
                case "pending": return "Pending";
                case "reviewed": return "Under review";
                case "shortlisted": return "Shortlisted";
                case "interviewed": return "Interview";
                case "hired": return "Hired";
                case "rejected": return "Not selected";
                default: return status;
            }
        }
    }
}
