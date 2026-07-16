using System;
using System.IO;
using System.Web;

namespace CareerConnect
{
    public static class ErrorLogger
    {
        public static void Log(Exception ex)
        {
            if (ex == null) return;
            try
            {
                string baseDir = HttpRuntime.AppDomainAppPath;
                string dir = Path.Combine(baseDir, "App_Data", "logs");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                string file = Path.Combine(dir, "app-" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".log");
                string line = DateTime.UtcNow.ToString("o") + " | " + ex.GetType().Name + " | " + ex.Message
                    + Environment.NewLine + ex.StackTrace + Environment.NewLine + "---" + Environment.NewLine;
                File.AppendAllText(file, line);
            }
            catch
            {
                /* avoid throwing from logger */
            }
        }
    }
}
