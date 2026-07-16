using System;
using System.Configuration;
using System.Timers;

namespace CareerConnect.USER
{
    /// <summary>
    /// Periodically runs <see cref="JobFeedImportService"/> when enabled (in addition to JobFeedImportWorker.ashx).
    /// </summary>
    public static class JobFeedImportScheduler
    {
        private static readonly object Gate = new object();
        private static bool _started;

        public static void StartIfConfigured()
        {
            if (_started)
                return;

            if (!ParseBool(ConfigurationManager.AppSettings["JobFeedImport_Enabled"], false))
                return;
            if (!ParseBool(ConfigurationManager.AppSettings["JobFeedImport_AutoSchedule"], false))
                return;

            _started = true;
            int minutes = ParseInt(ConfigurationManager.AppSettings["JobFeedImport_AutoIntervalMinutes"], 120);
            if (minutes < 15)
                minutes = 15;

            var timer = new Timer(minutes * 60.0 * 1000.0) { AutoReset = true };
            timer.Elapsed += (_, __) =>
            {
                lock (Gate)
                {
                    try { JobFeedImportService.Run(); } catch { /* ignore */ }
                }
            };
            timer.Start();

            System.Threading.ThreadPool.QueueUserWorkItem(_ =>
            {
                try
                {
                    System.Threading.Thread.Sleep(20000);
                    lock (Gate)
                    {
                        JobFeedImportService.Run();
                    }
                }
                catch { /* ignore */ }
            });
        }

        private static bool ParseBool(string s, bool def)
        {
            if (string.IsNullOrWhiteSpace(s)) return def;
            s = s.Trim().ToLowerInvariant();
            return s == "1" || s == "true" || s == "yes" || s == "on";
        }

        private static int ParseInt(string s, int def)
        {
            if (int.TryParse(s, out var n)) return n;
            return def;
        }
    }
}
