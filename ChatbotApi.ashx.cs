using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace CareerConnect.USER
{
    /// <summary>
    /// Chatbot API: optional OpenAI-compatible LLM + local TF–IDF RAG context; falls back to offline retriever.
    /// </summary>
    public class ChatbotApi : IHttpHandler
    {
        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json; charset=utf-8";

            string action = (context.Request["action"] ?? "").Trim().ToLowerInvariant();
            if (context.Request.HttpMethod == "GET" && action == "status")
            {
                WriteJson(context, new { ok = true, aiEnabled = true });
                return;
            }

            if (context.Request.HttpMethod != "POST")
            {
                WriteJson(context, new { ok = false, error = "method" });
                return;
            }

            string message = GetPostedMessage(context);
            if (string.IsNullOrWhiteSpace(message))
            {
                WriteJson(context, new { ok = false, error = "empty_message" });
                return;
            }
            if (message.Length > 4000)
            {
                WriteJson(context, new { ok = false, error = "message_too_long" });
                return;
            }

            string kbPath = context.Server.MapPath("~/App_Data/CareerConnectKnowledgeBase.txt");
            string kb = KnowledgeBaseChatbotRetriever.LoadKbFile(kbPath);
            if (string.IsNullOrWhiteSpace(kb))
            {
                WriteJson(context, new { ok = false, mode = "no_kb" });
                return;
            }

            string trimmed = message.Trim();
            var llmCfg = ChatbotLlmConfig.FromAppSettings();

            var quickIntent = KnowledgeBaseChatbotRetriever.TryQuickIntentsAnswer(trimmed);
            if (!string.IsNullOrWhiteSpace(quickIntent.Reply))
            {
                WriteJson(context, new
                {
                    ok = true,
                    mode = "local_intent",
                    reply = quickIntent.Reply,
                    lowConfidence = false,
                    sources = SerializeSources(quickIntent.Sources)
                });
                return;
            }

            var tfidf = KnowledgeBaseChatbotRetriever.TryTfidfKbAnswer(kb, trimmed);
            bool tfidfStrong = !string.IsNullOrWhiteSpace(tfidf.Reply) && !tfidf.LowConfidence;
            if (tfidfStrong)
            {
                WriteJson(context, new
                {
                    ok = true,
                    mode = "local_kb_rag",
                    reply = tfidf.Reply,
                    lowConfidence = false,
                    sources = SerializeSources(tfidf.Sources)
                });
                return;
            }

            if (llmCfg.Enabled)
            {
                string ctx = KnowledgeBaseChatbotRetriever.BuildLlmRagContext(kb, trimmed, out var ragSources);
                string systemPrompt = BuildLlmSystemPrompt(ctx);
                if (ChatbotOpenAiCompatibleClient.TryComplete(llmCfg, systemPrompt, trimmed, out var llmReply, out _))
                {
                    WriteJson(context, new
                    {
                        ok = true,
                        mode = "llm_rag",
                        reply = llmReply,
                        lowConfidence = false,
                        sources = SerializeSources(ragSources)
                    });
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(tfidf.Reply))
            {
                WriteJson(context, new
                {
                    ok = true,
                    mode = "local_kb_rag",
                    reply = tfidf.Reply,
                    lowConfidence = tfidf.LowConfidence,
                    sources = SerializeSources(tfidf.Sources)
                });
                return;
            }

            WriteJson(context, new { ok = false, mode = "no_match" });
        }

        private static List<object> SerializeSources(List<ChatbotKbSource> list)
        {
            var sources = new List<object>();
            if (list == null) return sources;
            foreach (var s in list)
            {
                sources.Add(new
                {
                    title = s.Title,
                    matchPct = (int)Math.Min(99, Math.Round(s.Score * 100.0))
                });
            }
            return sources;
        }

        private static string BuildLlmSystemPrompt(string context)
        {
            var ctx = string.IsNullOrWhiteSpace(context) ? "(No help sections retrieved — give only general Career Connect guidance and suggest the Contact page for account issues.)" : context.Trim();
            var sb = new StringBuilder();
            sb.AppendLine("You are the Career Connect job-portal assistant for the public.");
            sb.AppendLine();
            sb.AppendLine("Rules:");
            sb.AppendLine("- For greetings, thanks, goodbyes, or small talk: reply briefly and warmly. Mention you can help with using Career Connect (finding jobs, applying, employers, messaging, privacy/terms).");
            sb.AppendLine("- For questions about features, roles, or how the site works: use ONLY the facts in CONTEXT below. If CONTEXT does not contain enough information, say you are not sure and direct the user to the site Contact page for account-specific problems.");
            sb.AppendLine("- Do not invent features, integrations, or policies that are not implied by CONTEXT.");
            sb.AppendLine("- Do not give legal, medical, or financial advice. No password or OTP handling.");
            sb.AppendLine("- Answer in at most 5 short sentences unless the user explicitly asks for detail. No long bullet lists. Plain text only.");
            sb.AppendLine("- Never mention configuration files, database scripts, connection strings, API keys, background jobs, internal admin URLs, or implementation/technology stack. Speak like a user guide, not a developer.");
            sb.AppendLine();
            sb.AppendLine("CONTEXT:");
            sb.AppendLine(ctx);
            return sb.ToString().Trim();
        }

        private static string GetPostedMessage(HttpContext context)
        {
            string raw = context.Request.Unvalidated["message"];
            if (!string.IsNullOrEmpty(raw))
                return raw;
            try
            {
                var input = context.Request.InputStream;
                if (input.CanSeek)
                    input.Position = 0;
                string body;
                using (var sr = new StreamReader(input, Encoding.UTF8, detectEncodingFromByteOrderMarks: true, bufferSize: 4096, leaveOpen: true))
                    body = sr.ReadToEnd();
                if (string.IsNullOrWhiteSpace(body))
                    return null;
                var ser = new JavaScriptSerializer();
                var o = ser.DeserializeObject(body) as Dictionary<string, object>;
                if (o != null && o.TryGetValue("message", out var m) && m != null)
                    return m.ToString();
            }
            catch
            {
                /* ignore */
            }
            return null;
        }

        private static void WriteJson(HttpContext context, object o)
        {
            var ser = new JavaScriptSerializer { MaxJsonLength = 500000 };
            context.Response.Write(ser.Serialize(o));
        }
    }
}
