using System;
using System.Text.RegularExpressions;

namespace CareerConnect.USER
{
    /// <summary>Validates off-site apply URLs for syndicated listings (http/https only).</summary>
    public static class ExternalJobLinkHelper
    {
        private static readonly Regex FirstUrl = new Regex(@"https?://[^\s<>""']+", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>Returns a safe absolute http(s) URL, or null.</summary>
        public static string SanitizeHttpUrl(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return null;
            var t = raw.Trim();
            if (!Uri.TryCreate(t, UriKind.Absolute, out var u))
                return null;
            if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
                return null;
            return u.AbsoluteUri;
        }

        /// <summary>First http(s) URL found in arbitrary text (e.g. how_to_apply).</summary>
        public static string ExtractFirstHttpUrl(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;
            var m = FirstUrl.Match(text);
            if (!m.Success)
                return null;
            return SanitizeHttpUrl(m.Value);
        }
    }
}
