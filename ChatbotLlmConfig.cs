using System;
using System.Configuration;

namespace CareerConnect.USER
{
    /// <summary>Reads optional LLM settings from Web.config / appSettings (OpenAI-compatible chat completions).</summary>
    public sealed class ChatbotLlmConfig
    {
        public bool Enabled { get; private set; }
        public string ApiUrl { get; private set; }
        public string ApiKey { get; private set; }
        public string Model { get; private set; }
        public int TimeoutSeconds { get; private set; }
        public int MaxTokens { get; private set; }

        public static ChatbotLlmConfig FromAppSettings()
        {
            bool on = ParseBool(ConfigurationManager.AppSettings["Chatbot_LlmEnabled"], false);
            var url = (ConfigurationManager.AppSettings["Chatbot_LlmApiUrl"] ?? "").Trim();
            var key = (ConfigurationManager.AppSettings["Chatbot_LlmApiKey"] ?? "").Trim();
            var model = (ConfigurationManager.AppSettings["Chatbot_LlmModel"] ?? "").Trim();
            if (string.IsNullOrEmpty(model))
                model = "gpt-4o-mini";

            int timeout = ParseInt(ConfigurationManager.AppSettings["Chatbot_LlmTimeoutSeconds"], 90);
            int maxTok = ParseInt(ConfigurationManager.AppSettings["Chatbot_LlmMaxTokens"], 400);

            if (!on || string.IsNullOrEmpty(url))
                on = false;

            return new ChatbotLlmConfig
            {
                Enabled = on,
                ApiUrl = url,
                ApiKey = key,
                Model = model,
                TimeoutSeconds = Math.Max(15, timeout),
                MaxTokens = Clamp(maxTok, 100, 4096)
            };
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

        private static int Clamp(int v, int lo, int hi)
        {
            if (v < lo) return lo;
            if (v > hi) return hi;
            return v;
        }
    }
}
