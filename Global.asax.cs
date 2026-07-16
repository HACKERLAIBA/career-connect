using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace CareerConnect
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            try
            {
                CareerConnect.USER.WeeklyDigestRunner.StartBackgroundSchedule();
            }
            catch
            {
                /* optional: cc_email_cron table not created yet */
            }

            try
            {
                CareerConnect.USER.JobFeedImportScheduler.StartIfConfigured();
            }
            catch
            {
                /* optional: DB or feed config not ready */
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex != null)
                ErrorLogger.Log(ex);
        }
    }
}