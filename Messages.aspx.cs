using System;
using System.Web.UI;

namespace CareerConnect.USER.Employer
{
    public partial class EmployerMessages : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["employerCompanyId"] == null)
            {
                Response.Redirect("~/USER/Login.aspx?role=employer");
                return;
            }

            Response.Redirect("default.aspx?msg=1", false);
        }
    }
}
