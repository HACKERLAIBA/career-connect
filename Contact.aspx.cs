using System;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Web.UI;
using System.Data.SqlClient;

namespace CareerConnect.USER
{
    public partial class Contact : System.Web.UI.Page
    {
        // MySQL connection using Web.config key
        MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySqlConn"].ConnectionString);
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnsend_Click(object sender, EventArgs e)
        {
            try
            {
                conn.Open();

                // Create contact table if it doesn't exist
                string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS contact (
                        id INT AUTO_INCREMENT PRIMARY KEY,
                        name VARCHAR(100) NOT NULL,
                        email VARCHAR(100) NOT NULL,
                        subject VARCHAR(200) NOT NULL,
                        message TEXT NOT NULL,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                    )";
                MySqlCommand createCmd = new MySqlCommand(createTableQuery, conn);
                createCmd.ExecuteNonQuery();

                string query = @"INSERT INTO contact (name, email, subject, message) 
                                 VALUES (@Name, @Email, @Subject, @Message)";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Name", name.Value.Trim());
                cmd.Parameters.AddWithValue("@Email", email.Value.Trim());
                cmd.Parameters.AddWithValue("@Subject", subject.Value.Trim());
                cmd.Parameters.AddWithValue("@Message", message.Value.Trim());

                int result = cmd.ExecuteNonQuery();

                if (result > 0)
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Thanks for reaching out! We'll get back to you soon.";
                    lblMsg.CssClass = "alert alert-success";
                    Clear();
                }
                else
                {
                    lblMsg.Visible = true;
                    lblMsg.Text = "Could not submit your message. Please try again.";
                    lblMsg.CssClass = "alert alert-danger";
                }
            }
            catch (Exception ex)
            {
                lblMsg.Visible = true;
                lblMsg.Text = "Error: " + ex.Message;
                lblMsg.CssClass = "alert alert-danger";
            }
            finally
            {
                conn.Close();
            }
        }

        private void Clear()
        {
            name.Value = "";
            email.Value = "";
            subject.Value = "";
            message.Value = "";
        }
    }
}

