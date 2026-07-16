using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace CareerConnect.USER
{
    /// <summary>Minimal OpenAI-compatible chat-completions client (OpenAI, many proxies, Ollama /v1/chat/completions).</summary>
    public static class ChatbotOpenAiCompatibleClient
    {
        public static bool TryComplete(ChatbotLlmConfig cfg, string systemPrompt, string userMessage, out string reply, out string errorDetail)
        {
            reply = null;
            errorDetail = null;
            if (cfg == null || !cfg.Enabled || string.IsNullOrEmpty(cfg.ApiUrl))
            {
                errorDetail = "llm_not_configured";
                return false;
            }

            try
            {
                ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;

                var ser = new JavaScriptSerializer { MaxJsonLength = 500000 };
                var messages = new object[]
                {
                    new Dictionary<string, object> { { "role", "system" }, { "content", systemPrompt ?? "" } },
                    new Dictionary<string, object> { { "role", "user" }, { "content", userMessage ?? "" } }
                };
                var bodyObj = new Dictionary<string, object>
                {
                    { "model", cfg.Model },
                    { "messages", messages },
                    { "temperature", 0.35 },
                    { "max_tokens", cfg.MaxTokens }
                };
                string json = ser.Serialize(bodyObj);
                byte[] raw = Encoding.UTF8.GetBytes(json);

                var req = (HttpWebRequest)WebRequest.Create(cfg.ApiUrl);
                req.Method = "POST";
                req.ContentType = "application/json; charset=utf-8";
                req.Accept = "application/json";
                req.Timeout = cfg.TimeoutSeconds * 1000;
                req.ReadWriteTimeout = cfg.TimeoutSeconds * 1000;
                if (!string.IsNullOrEmpty(cfg.ApiKey))
                    req.Headers["Authorization"] = "Bearer " + cfg.ApiKey;

                using (var rs = req.GetRequestStream())
                    rs.Write(raw, 0, raw.Length);

                string responseText;
                try
                {
                    using (var resp = (HttpWebResponse)req.GetResponse())
                    using (var sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8))
                        responseText = sr.ReadToEnd();
                }
                catch (WebException wex)
                {
                    using (var er = wex.Response as HttpWebResponse)
                    {
                        if (er != null)
                        {
                            using (var sr = new StreamReader(er.GetResponseStream(), Encoding.UTF8))
                                responseText = sr.ReadToEnd();
                            errorDetail = "http_" + (int)er.StatusCode + ": " + Truncate(responseText, 500);
                        }
                        else
                            errorDetail = wex.Message;
                    }
                    return false;
                }

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    errorDetail = "empty_response";
                    return false;
                }

                var top = ser.DeserializeObject(responseText) as Dictionary<string, object>;
                if (top != null && top.TryGetValue("error", out var errObj))
                {
                    errorDetail = "api_error: " + Truncate(errObj?.ToString() ?? "?", 400);
                    return false;
                }

                string content = ExtractAssistantContent(top);
                if (string.IsNullOrWhiteSpace(content))
                {
                    errorDetail = "no_content: " + Truncate(responseText, 300);
                    return false;
                }

                reply = content.Trim();
                return true;
            }
            catch (Exception ex)
            {
                errorDetail = ex.Message;
                return false;
            }
        }

        private static string ExtractAssistantContent(Dictionary<string, object> top)
        {
            if (top == null || !top.TryGetValue("choices", out var chRaw))
                return null;
            // JavaScriptSerializer often uses object[] for JSON arrays; OpenAI client may use ArrayList.
            object firstObj = null;
            if (chRaw is ArrayList al && al.Count > 0)
                firstObj = al[0];
            else if (chRaw is object[] arr && arr.Length > 0)
                firstObj = arr[0];
            else if (chRaw is IList list && list.Count > 0)
                firstObj = list[0];
            if (firstObj == null)
                return null;
            var first = firstObj as Dictionary<string, object>;
            if (first == null || !first.TryGetValue("message", out var msgRaw))
                return null;
            var msg = msgRaw as Dictionary<string, object>;
            if (msg == null || !msg.TryGetValue("content", out var contentRaw))
                return null;
            if (contentRaw is string s)
                return s;
            return contentRaw?.ToString();
        }

        private static string Truncate(string s, int max)
        {
            if (string.IsNullOrEmpty(s)) return "";
            if (s.Length <= max) return s;
            return s.Substring(0, max) + "…";
        }
    }
}
