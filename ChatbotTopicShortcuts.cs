using System;
using System.Collections.Generic;
using System.Linq;

namespace CareerConnect.USER
{
    /// <summary>
    /// High-precision canned answers for visitor questions. Rules are evaluated in descending <see cref="ShortcutRule.Priority"/>.
    /// </summary>
    internal static class ChatbotTopicShortcuts
    {
        private sealed class ShortcutRule
        {
            public int Priority;
            public string[] AnyOneOf;
            public string[] AllOf;
            public string[] NoneOf;
            public string Answer;
        }

        private static readonly ShortcutRule[] Rules = BuildRules();

        private static ShortcutRule[] BuildRules()
        {
            var list = new List<ShortcutRule>
            {
                new ShortcutRule
                {
                    Priority = 980,
                    AnyOneOf = new[] { "resend" },
                    AllOf = new[] { "code", "verification" },
                    NoneOf = null,
                    Answer = "Open Resend verification, enter your email, then check your inbox/spam for a new code. Still nothing? Use Contact — this chat cannot resend mail for you."
                },
                new ShortcutRule
                {
                    Priority = 970,
                    AnyOneOf = new[] { "unsubscribe" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Use the unsubscribe link on marketing emails, or the site Unsubscribe page. Login or password problems go through Contact."
                },
                new ShortcutRule
                {
                    Priority = 960,
                    AnyOneOf = new[] { "forgot password", "reset password", "lost password", "recover password" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "This chat cannot reset passwords. Use Contact / support for login recovery."
                },
                new ShortcutRule
                {
                    Priority = 950,
                    AnyOneOf = new[] { "verified employer", "employer verified", "verification badge" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "A “Verified employer” badge means staff marked that company as verified. It is shown on job and company pages when present."
                },
                new ShortcutRule
                {
                    Priority = 940,
                    AnyOneOf = new[] { "about page", "about us", "what is this site" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Open About from the menu for a general overview of Career Connect."
                },
                new ShortcutRule
                {
                    Priority = 930,
                    AnyOneOf = new[] { "upload resume", "resume upload", "add resume", "attach resume", "pdf resume" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Sign in → My Profile → Resume section → choose your file (PDF/DOC/DOCX) → Upload. You can remove old files there if the page allows."
                },
                new ShortcutRule
                {
                    Priority = 920,
                    AnyOneOf = new[] { "track my application", "application status", "my applications", "where is my application" },
                    AllOf = null,
                    NoneOf = new[] { "employer", "review applicants", "shortlist" },
                    Answer = "Sign in → My Profile (applications area). You can see jobs you applied to and their latest status there."
                },
                new ShortcutRule
                {
                    Priority = 910,
                    AnyOneOf = new[] { "duplicate apply", "apply twice", "already applied" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "You usually cannot apply twice to the same job. If you already applied, open My Profile to see that application."
                },
                new ShortcutRule
                {
                    Priority = 900,
                    AnyOneOf = new[] { "cover letter" },
                    AllOf = null,
                    NoneOf = new[] { "employer template" },
                    Answer = "On Apply, use the message/box on the form like a short cover letter. Keep it relevant to that job."
                },
                new ShortcutRule
                {
                    Priority = 890,
                    AnyOneOf = new[] { "remote", "hybrid", "work from home", "onsite" },
                    AllOf = new[] { "job", "filter" },
                    NoneOf = null,
                    Answer = "On the job listing, use filters and open each job — work mode (on-site / remote / hybrid) is shown on the job details when the employer provided it."
                },
                new ShortcutRule
                {
                    Priority = 880,
                    AnyOneOf = new[] { "salary filter", "filter salary", "pay range" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "On Find Jobs, set the salary min/max filters, then refresh results. Not every job lists salary."
                },
                new ShortcutRule
                {
                    Priority = 870,
                    AnyOneOf = new[] { "job type", "full time", "part time", "internship", "contract job" },
                    AllOf = new[] { "filter" },
                    NoneOf = null,
                    Answer = "Use the job type filter on the listing, then open a job to read full details."
                },
                new ShortcutRule
                {
                    Priority = 860,
                    AnyOneOf = new[] { "employer" },
                    AllOf = new[] { "edit job", "change job", "update job" },
                    NoneOf = null,
                    Answer = "Employers: sign in → Jobs → open the job → Edit, change fields, save. If edits require approval on your site, wait until it is active again."
                },
                new ShortcutRule
                {
                    Priority = 850,
                    AnyOneOf = new[] { "employer" },
                    AllOf = new[] { "close job", "delete job", "remove job" },
                    NoneOf = null,
                    Answer = "Employers: use your jobs list / edit job screen to close or stop showing a role (wording may say close/inactive depending on setup). If unsure, use Contact."
                },
                new ShortcutRule
                {
                    Priority = 840,
                    AnyOneOf = new[] { "employer" },
                    AllOf = new[] { "company profile", "company page", "edit company" },
                    NoneOf = null,
                    Answer = "Employers: sign in → Company / profile area in the employer portal to update public company text and details."
                },
                new ShortcutRule
                {
                    Priority = 830,
                    AnyOneOf = new[] { "employer" },
                    AllOf = new[] { "applicant", "candidate", "application" },
                    NoneOf = null,
                    Answer = "Employers: open Applications in the dashboard, pick a candidate, read their note, and update status (pending/reviewed/shortlisted/rejected/hired) as you use internally."
                },
                new ShortcutRule
                {
                    Priority = 820,
                    AnyOneOf = new[] { "employer" },
                    AllOf = new[] { "message", "chat", "inbox", "reply" },
                    NoneOf = null,
                    Answer = "Employers: open Messages in the employer portal to read and reply to job seekers."
                },
                new ShortcutRule
                {
                    Priority = 810,
                    AnyOneOf = new[] { "employer" },
                    AllOf = new[] { "pending", "not approved", "waiting" },
                    NoneOf = null,
                    Answer = "New employer companies may need staff approval first. The pending screen explains next steps; use Contact if it never clears."
                },
                new ShortcutRule
                {
                    Priority = 800,
                    AnyOneOf = new[] { "employer" },
                    AllOf = new[] { "post", "add", "create" },
                    NoneOf = null,
                    Answer = "Employers: sign in → Add job → fill title, category, location, type, salary, how to apply → submit. Approval may be required before it goes live."
                },
                new ShortcutRule
                {
                    Priority = 780,
                    AnyOneOf = new[] { "register as employer", "employer register", "sign up as employer" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Use Register → Employer, complete company details, then sign in. Wait for company approval if your site requires it."
                },
                new ShortcutRule
                {
                    Priority = 770,
                    AnyOneOf = new[] { "register", "sign up", "create account" },
                    AllOf = null,
                    NoneOf = new[] { "employer" },
                    Answer = "Use Register → Job seeker, fill the form, then check email for the verification code after sign-up."
                },
                new ShortcutRule
                {
                    Priority = 760,
                    AnyOneOf = new[] { "login", "sign in", "cannot login", "can't login", "log in" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Use Login with your username and password. If you are locked out or forgot credentials, use Contact — this chat cannot reset passwords."
                },
                new ShortcutRule
                {
                    Priority = 750,
                    AnyOneOf = new[] { "otp", "verification code", "email code", "6 digit", "6-digit" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "After registering, check email for the code, sign in, enter it on the verification screen. Use Resend if offered. Use Contact for ongoing mail problems."
                },
                new ShortcutRule
                {
                    Priority = 740,
                    AnyOneOf = new[] { "privacy", "gdpr", "terms of", "cookie policy" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Open Privacy and Terms in the footer. They describe data use and essential login cookies."
                },
                new ShortcutRule
                {
                    Priority = 730,
                    AnyOneOf = new[] { "cookie" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "The site uses essential cookies for login/sessions. Details are in the Privacy page (footer)."
                },
                new ShortcutRule
                {
                    Priority = 720,
                    AnyOneOf = new[] { "report" },
                    AllOf = new[] { "job", "spam", "scam", "fake", "misleading" },
                    NoneOf = null,
                    Answer = "On the job page, use Report and pick a reason. Use Contact for urgent personal account issues."
                },
                new ShortcutRule
                {
                    Priority = 710,
                    AnyOneOf = new[] { "syndicat", "syndicate", "external job", "imported job", "partner feed" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Some listings come from outside feeds. When shown, apply on the original employer site using the instructions on that job page."
                },
                new ShortcutRule
                {
                    Priority = 700,
                    AnyOneOf = new[] { "what is career connect", "what does career connect", "what can i do", "features" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Seekers: search/apply/save jobs, My Profile, messages. Employers: post jobs and manage applicants after company approval. Footer: Privacy, Terms, Contact."
                },
                new ShortcutRule
                {
                    Priority = 690,
                    AnyOneOf = new[] { "message employer", "contact employer", "chat with employer" },
                    AllOf = null,
                    NoneOf = new[] { "applicant", "candidate", "review" },
                    Answer = "Sign in as a seeker → open the messages icon or “Message employer” on a job."
                },
                new ShortcutRule
                {
                    Priority = 680,
                    AnyOneOf = new[] { "floating", "envelope", "message icon" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Signed-in seekers: use the floating messages (envelope) icon to open inbox and chats with employers."
                },
                new ShortcutRule
                {
                    Priority = 670,
                    AnyOneOf = new[] { "saved job", "bookmark", "save job" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Sign in → Save on a job → open Saved Jobs from your profile area."
                },
                new ShortcutRule
                {
                    Priority = 660,
                    AnyOneOf = new[] { "recommend", "suggested", "for you" },
                    AllOf = new[] { "job" },
                    NoneOf = null,
                    Answer = "Complete My Profile while logged in; the listing can show better suggested jobs for you."
                },
                new ShortcutRule
                {
                    Priority = 650,
                    AnyOneOf = new[] { "find job", "search job", "job search", "browse job", "job listing" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Open Find Jobs. Filter by keyword, location, category, type, salary, and date. Open a result for full details."
                },
                new ShortcutRule
                {
                    Priority = 640,
                    AnyOneOf = new[] { "company page", "view company", "employer page" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Open the company link from a job to read the public company page. Messaging may be available when signed in."
                },
                new ShortcutRule
                {
                    Priority = 630,
                    AnyOneOf = new[] { "how to apply", "where to apply", "can i apply", "apply for this job" },
                    AllOf = null,
                    NoneOf = new[] { "employer", "applicant", "review", "shortlist" },
                    Answer = "Sign in as a seeker → open the job → Apply → add your message/cover note → submit. Track it in My Profile."
                },
                new ShortcutRule
                {
                    Priority = 620,
                    AnyOneOf = new[] { "how", "where", "can" },
                    AllOf = new[] { "apply" },
                    NoneOf = new[] { "employer", "applicant", "review", "shortlist" },
                    Answer = "Sign in → open the job → Apply → submit. Track applications in My Profile."
                },
                new ShortcutRule
                {
                    Priority = 619,
                    AnyOneOf = new[] { "how", "where", "can" },
                    AllOf = new[] { "application" },
                    NoneOf = new[] { "employer", "applicant", "review", "shortlist" },
                    Answer = "Sign in → open the job → Apply → submit. Track applications in My Profile."
                },
                new ShortcutRule
                {
                    Priority = 600,
                    AnyOneOf = new[] { "contact", "support", "help desk", "customer service" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Use the Contact page in the footer for help and account issues."
                },
                new ShortcutRule
                {
                    Priority = 590,
                    AnyOneOf = new[] { "delete account", "close account", "remove my account" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Account deletion (if available) is handled by support. Use Contact and request account removal."
                },
                new ShortcutRule
                {
                    Priority = 580,
                    AnyOneOf = new[] { "change email", "wrong email", "update email" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Email changes usually need support. Use Contact and explain what you need."
                },
                new ShortcutRule
                {
                    Priority = 570,
                    AnyOneOf = new[] { "too many login", "locked out", "rate limit", "attempts" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "If login is temporarily blocked after many attempts, wait the lockout time or use Contact if it does not clear."
                },
                new ShortcutRule
                {
                    Priority = 565,
                    AnyOneOf = new[] { "weekly digest", "job match email", "job suggestion email", "email notifications", "turn off emails", "stop emails", "marketing email" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Use the Unsubscribe page (footer or link in emails) to turn off weekly digest and job-match suggestion mail. Login or OTP problems: Contact."
                },
                new ShortcutRule
                {
                    Priority = 560,
                    AnyOneOf = new[] { "site down", "website down", "not loading", "page error", "something broke", "broken page" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Try refreshing or another browser. If it keeps failing, use Contact and describe what you clicked and any error text."
                },
                new ShortcutRule
                {
                    Priority = 555,
                    AnyOneOf = new[] { "who runs", "who owns", "company behind", "operator" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Open About from the menu for general information. Specific legal or business questions: Contact."
                },
                new ShortcutRule
                {
                    Priority = 550,
                    AnyOneOf = new[] { "withdraw application", "cancel application", "undo apply", "remove application" },
                    AllOf = null,
                    NoneOf = new[] { "employer", "reject candidate" },
                    Answer = "Check My Profile / applications for a withdraw option if shown. If not, use Contact and ask to remove an application."
                },
                new ShortcutRule
                {
                    Priority = 545,
                    AnyOneOf = new[] { "change application", "edit application", "update application", "fix my application" },
                    AllOf = null,
                    NoneOf = new[] { "employer", "applicant", "review", "status", "shortlist" },
                    Answer = "You usually cannot edit an application after it is sent. Use Messages to reach the employer if available, or Contact for help."
                },
                new ShortcutRule
                {
                    Priority = 540,
                    AnyOneOf = new[] { "phone number", "call support", "whatsapp support", "live chat human" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Use the Contact page in the footer. This chat is automated and may not replace phone support."
                },
                new ShortcutRule
                {
                    Priority = 535,
                    AnyOneOf = new[] { "internship", "graduate scheme", "entry level", "trainee" },
                    AllOf = new[] { "find", "search", "get" },
                    NoneOf = new[] { "employer", "post", "hire" },
                    Answer = "Open Find Jobs, search those keywords, and use filters. Open each job for full requirements and how to apply."
                },
                new ShortcutRule
                {
                    Priority = 530,
                    AnyOneOf = new[] { "visa", "sponsorship", "work permit", "relocation" },
                    AllOf = new[] { "job" },
                    NoneOf = null,
                    Answer = "Visa or sponsorship depends on each employer. Read the job description and ask via Message employer if the listing allows it."
                },
                new ShortcutRule
                {
                    Priority = 525,
                    AnyOneOf = new[] { "discriminat", "harassment", "illegal" },
                    AllOf = new[] { "job", "employer", "listing" },
                    NoneOf = null,
                    Answer = "Use Report on the job if the form fits, and Contact for serious concerns. Keep any evidence (screenshots, links)."
                },
                new ShortcutRule
                {
                    Priority = 520,
                    AnyOneOf = new[] { "dark mode", "mobile app", "android app", "ios app" },
                    AllOf = null,
                    NoneOf = null,
                    Answer = "Use the website in your phone browser. There may be no separate app; dark mode depends on your browser or OS settings."
                },
            };

            return list.OrderByDescending(r => r.Priority).ToArray();
        }

        public static string TryMatch(string normalizedQuestion)
        {
            var q = (normalizedQuestion ?? "").Trim().ToLowerInvariant();
            if (q.Length == 0)
                return null;

            foreach (var r in Rules)
            {
                if (r.NoneOf != null)
                {
                    bool blocked = false;
                    foreach (var n in r.NoneOf)
                    {
                        if (q.Contains(n))
                        {
                            blocked = true;
                            break;
                        }
                    }
                    if (blocked)
                        continue;
                }

                if (r.AllOf != null)
                {
                    bool all = true;
                    foreach (var a in r.AllOf)
                    {
                        if (!q.Contains(a))
                        {
                            all = false;
                            break;
                        }
                    }
                    if (!all)
                        continue;
                }

                if (r.AnyOneOf != null && r.AnyOneOf.Length > 0)
                {
                    bool any = false;
                    foreach (var a in r.AnyOneOf)
                    {
                        if (q.Contains(a))
                        {
                            any = true;
                            break;
                        }
                    }
                    if (!any)
                        continue;
                }

                return r.Answer;
            }

            return null;
        }
    }
}
