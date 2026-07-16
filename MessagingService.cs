using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace CareerConnect
{
    public static class MessagingService
    {
        public static string Conn => ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString;

        public static bool TryLoadThread(int threadId, out int seekerUserId, out int companyId)
        {
            seekerUserId = 0;
            companyId = 0;
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "SELECT user_id, company_id FROM message_threads WHERE id=@id LIMIT 1", conn))
                {
                    cmd.Parameters.AddWithValue("@id", threadId);
                    using (var r = cmd.ExecuteReader())
                    {
                        if (!r.Read()) return false;
                        seekerUserId = Convert.ToInt32(r["user_id"]);
                        companyId = Convert.ToInt32(r["company_id"]);
                        return true;
                    }
                }
            }
        }

        public static void InsertPost(int threadId, string role, int? senderUserId, string body)
        {
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var ins = new MySqlCommand(
                    @"INSERT INTO message_posts (thread_id, sender_role, sender_user_id, body, is_hidden, admin_flagged)
                      VALUES (@t, @r, @u, @b, 0, 0)", conn))
                {
                    ins.Parameters.AddWithValue("@t", threadId);
                    ins.Parameters.AddWithValue("@r", role);
                    ins.Parameters.AddWithValue("@u", senderUserId.HasValue ? (object)senderUserId.Value : DBNull.Value);
                    ins.Parameters.AddWithValue("@b", body);
                    ins.ExecuteNonQuery();
                }
                using (var up = new MySqlCommand("UPDATE message_threads SET updated_at=NOW() WHERE id=@t", conn))
                {
                    up.Parameters.AddWithValue("@t", threadId);
                    up.ExecuteNonQuery();
                }
            }
        }

        public static int EnsureThreadSeeker(int userId, int companyId, int? jobId)
        {
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var q = new MySqlCommand("SELECT id FROM message_threads WHERE user_id=@u AND company_id=@c LIMIT 1", conn))
                {
                    q.Parameters.AddWithValue("@u", userId);
                    q.Parameters.AddWithValue("@c", companyId);
                    var o = q.ExecuteScalar();
                    if (o != null && o != DBNull.Value)
                        return Convert.ToInt32(o);
                }

                string subj = "Conversation";
                if (jobId.HasValue)
                {
                    using (var jq = new MySqlCommand("SELECT title FROM jobs WHERE id=@j LIMIT 1", conn))
                    {
                        jq.Parameters.AddWithValue("@j", jobId.Value);
                        var t = jq.ExecuteScalar();
                        if (t != null) subj = "Re: " + t;
                    }
                }

                using (var ins = new MySqlCommand(
                    @"INSERT INTO message_threads (user_id, company_id, job_id, subject, status, admin_reviewed)
                      VALUES (@u,@c,@j,@s,'open',0)", conn))
                {
                    ins.Parameters.AddWithValue("@u", userId);
                    ins.Parameters.AddWithValue("@c", companyId);
                    ins.Parameters.AddWithValue("@j", jobId.HasValue ? (object)jobId.Value : DBNull.Value);
                    ins.Parameters.AddWithValue("@s", subj);
                    ins.ExecuteNonQuery();
                    return (int)ins.LastInsertedId;
                }
            }
        }

        public static void MarkThreadReadSeeker(int threadId, int seekerUserId)
        {
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "UPDATE message_threads SET seeker_read_at=NOW() WHERE id=@t AND user_id=@u", conn))
                {
                    cmd.Parameters.AddWithValue("@t", threadId);
                    cmd.Parameters.AddWithValue("@u", seekerUserId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void MarkThreadReadEmployer(int threadId, int companyId)
        {
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    "UPDATE message_threads SET employer_read_at=NOW() WHERE id=@t AND company_id=@c", conn))
                {
                    cmd.Parameters.AddWithValue("@t", threadId);
                    cmd.Parameters.AddWithValue("@c", companyId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int GetUnreadTotalSeeker(int userId)
        {
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    @"SELECT COUNT(*) FROM message_posts mp
                      INNER JOIN message_threads mt ON mp.thread_id = mt.id
                      WHERE mt.user_id = @u AND mp.is_hidden = 0
                        AND mp.sender_role IN ('employer','admin')
                        AND (mt.seeker_read_at IS NULL OR mp.created_at > mt.seeker_read_at)", conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static int GetUnreadTotalEmployer(int companyId)
        {
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    @"SELECT COUNT(*) FROM message_posts mp
                      INNER JOIN message_threads mt ON mp.thread_id = mt.id
                      WHERE mt.company_id = @c AND mp.is_hidden = 0
                        AND mp.sender_role IN ('seeker','admin')
                        AND (mt.employer_read_at IS NULL OR mp.created_at > mt.employer_read_at)", conn))
                {
                    cmd.Parameters.AddWithValue("@c", companyId);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static List<Dictionary<string, object>> ListThreadsSeekerJson(int userId)
        {
            var list = new List<Dictionary<string, object>>();
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    @"SELECT mt.id, mt.subject, mt.updated_at,
                             COALESCE(c.name,'Company') AS company_name,
                             (SELECT COUNT(*) FROM message_posts mp
                              WHERE mp.thread_id = mt.id AND mp.is_hidden = 0
                                AND mp.sender_role IN ('employer','admin')
                                AND (mt.seeker_read_at IS NULL OR mp.created_at > mt.seeker_read_at)) AS unread_count
                      FROM message_threads mt
                      INNER JOIN companies c ON c.id = mt.company_id
                      WHERE mt.user_id = @u
                      ORDER BY mt.updated_at DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@u", userId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new Dictionary<string, object>
                            {
                                ["id"] = Convert.ToInt32(r["id"]),
                                ["subject"] = r["subject"].ToString(),
                                ["updatedAt"] = Convert.ToDateTime(r["updated_at"]).ToString("o"),
                                ["peerLabel"] = r["company_name"].ToString(),
                                ["unread"] = Convert.ToInt32(r["unread_count"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static List<Dictionary<string, object>> ListThreadsEmployerJson(int companyId)
        {
            var list = new List<Dictionary<string, object>>();
            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(
                    @"SELECT mt.id, mt.subject, mt.updated_at,
                             COALESCE(TRIM(CONCAT(u.first_name,' ',u.last_name)), u.username) AS seeker_name,
                             (SELECT COUNT(*) FROM message_posts mp
                              WHERE mp.thread_id = mt.id AND mp.is_hidden = 0
                                AND mp.sender_role IN ('seeker','admin')
                                AND (mt.employer_read_at IS NULL OR mp.created_at > mt.employer_read_at)) AS unread_count
                      FROM message_threads mt
                      INNER JOIN users u ON u.id = mt.user_id
                      WHERE mt.company_id = @c
                      ORDER BY mt.updated_at DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@c", companyId);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            list.Add(new Dictionary<string, object>
                            {
                                ["id"] = Convert.ToInt32(r["id"]),
                                ["subject"] = r["subject"].ToString(),
                                ["updatedAt"] = Convert.ToDateTime(r["updated_at"]).ToString("o"),
                                ["peerLabel"] = r["seeker_name"].ToString(),
                                ["unread"] = Convert.ToInt32(r["unread_count"])
                            });
                        }
                    }
                }
            }
            return list;
        }

        public static DataTable LoadPostsForParticipant(int threadId, bool viewerIsSeeker)
        {
            string sql = viewerIsSeeker
                ? @"SELECT body, created_at, sender_role,
                    CASE sender_role WHEN 'seeker' THEN 'You' WHEN 'employer' THEN 'Employer' ELSE 'Admin' END AS who
                    FROM message_posts WHERE thread_id=@t AND is_hidden=0 ORDER BY created_at ASC"
                : @"SELECT body, created_at, sender_role,
                    CASE sender_role WHEN 'seeker' THEN 'Job seeker' WHEN 'employer' THEN 'You' ELSE 'Admin' END AS who
                    FROM message_posts WHERE thread_id=@t AND is_hidden=0 ORDER BY created_at ASC";

            using (var conn = new MySqlConnection(Conn))
            {
                conn.Open();
                using (var da = new MySqlDataAdapter(sql, conn))
                {
                    da.SelectCommand.Parameters.AddWithValue("@t", threadId);
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
