using System;
using System.Text.RegularExpressions;

namespace CareerConnect.USER
{
    /// <summary>Strips implementation and admin-only references from chatbot text shown to visitors.</summary>
    public static class ChatbotPublicReplySanitizer
    {
        private static readonly Regex[] RedactPatterns =
        {
            new Regex(@"Web\.config", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"database_migration[\w]*\.sql", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"phpMyAdmin", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"MySqlConn", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"EmailWorkerSecret", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"JobFeedImport_[A-Za-z0-9_]+", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"Chatbot_Llm[A-Za-z0-9_]*", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\bErrorLogger\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"Global\.asax", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"JobNotifyWorker\.ashx", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"JobFeedImportWorker\.ashx", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"MessagingApi\.ashx", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"ChatbotApi\.ashx", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\bApp_Data[/\\][^\s\)]+", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"CareerConnectKnowledgeBase\.txt", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"TF.C?IDF[\w\s\-–—,\+]*", RegexOptions.IgnoreCase | RegexOptions.Compiled),
            new Regex(@"\bcosine similarity\b", RegexOptions.IgnoreCase | RegexOptions.Compiled),
        };

        private static readonly Regex MultiBlank = new Regex(@"\n{3,}", RegexOptions.Compiled);

        public static string SanitizeReply(string reply)
        {
            if (string.IsNullOrWhiteSpace(reply))
                return (reply ?? "").Trim();

            var s = reply.Replace("\r\n", "\n").Replace("`", "");
            foreach (var rx in RedactPatterns)
                s = rx.Replace(s, "");

            s = Regex.Replace(s, @"\(\s*for AI context\s*\)", "", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, @"\bfor AI context\b", "", RegexOptions.IgnoreCase);
            s = MultiBlank.Replace(s.Trim(), "\n\n");
            return s.Trim();
        }
    }
}
