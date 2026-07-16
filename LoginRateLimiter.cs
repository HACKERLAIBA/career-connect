using System;
using System.Configuration;
using System.Web;
using System.Web.Caching;

namespace CareerConnect
{
    public static class LoginRateLimiter
    {
        private static string CacheKeyFails(string user, string ip)
        {
            return "cc_login_fails_" + (user ?? "").ToLowerInvariant() + "_" + (ip ?? "");
        }

        private static string CacheKeyLock(string user, string ip)
        {
            return "cc_login_lock_" + (user ?? "").ToLowerInvariant() + "_" + (ip ?? "");
        }

        public static bool IsLocked(string username, out int minutesLeft)
        {
            minutesLeft = 0;
            string ip = GetClientIp();
            var o = HttpRuntime.Cache.Get(CacheKeyLock(username, ip));
            if (o is DateTime until && until > DateTime.UtcNow)
            {
                minutesLeft = Math.Max(1, (int)Math.Ceiling((until - DateTime.UtcNow).TotalMinutes));
                return true;
            }
            return false;
        }

        public static void RecordFailure(string username)
        {
            int max = 5;
            int lockMinutes = 15;
            try
            {
                max = int.Parse(ConfigurationManager.AppSettings["LoginMaxAttempts"] ?? "5");
                lockMinutes = int.Parse(ConfigurationManager.AppSettings["LoginLockoutMinutes"] ?? "15");
            }
            catch { /* defaults */ }

            string ip = GetClientIp();
            string key = CacheKeyFails(username, ip);
            int n = 0;
            var cur = HttpRuntime.Cache.Get(key);
            if (cur is int i) n = i;
            n++;
            HttpRuntime.Cache.Insert(key, n, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(lockMinutes), CacheItemPriority.Low, null);

            if (n >= max)
            {
                var until = DateTime.UtcNow.AddMinutes(lockMinutes);
                HttpRuntime.Cache.Insert(CacheKeyLock(username, ip), until, null, until, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                HttpRuntime.Cache.Remove(key);
            }
        }

        public static void ClearFailures(string username)
        {
            string ip = GetClientIp();
            HttpRuntime.Cache.Remove(CacheKeyFails(username, ip));
            HttpRuntime.Cache.Remove(CacheKeyLock(username, ip));
        }

        private static string GetClientIp()
        {
            try
            {
                return HttpContext.Current?.Request?.UserHostAddress ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}
