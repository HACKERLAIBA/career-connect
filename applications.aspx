<%@ Page Title="Applications" Language="C#" MasterPageFile="~/USER/Employer/EmployerMaster.Master" AutoEventWireup="true" CodeBehind="applications.aspx.cs" Inherits="CareerConnect.USER.Employer.EmployerApplications" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container pt-50 pb-80">
        <h2 class="contact-title mb-4">Applications</h2>
        <p class="text-muted">Candidates who applied to your company’s jobs. Update status to notify applicants by email (if SMTP is configured).</p>

        <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="alert d-block" />

        <div class="table-responsive">
            <table class="table table-striped align-middle">
                <thead>
                    <tr>
                        <th>Applicant</th>
                        <th>Email</th>
                        <th>Job</th>
                        <th>Status</th>
                        <th>Applied</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptApps" runat="server" OnItemCommand="rptApps_ItemCommand" OnItemDataBound="rptApps_ItemDataBound">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("user_name") %></td>
                                <td><%# Eval("user_email") %></td>
                                <td><%# Eval("job_title") %></td>
                                <td><%# Eval("status") %></td>
                                <td><%# Eval("applied_at", "{0:dd MMM yyyy HH:mm}") %></td>
                                <td>
                                    <div class="d-flex flex-wrap gap-1 align-items-center">
                                        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select form-select-sm" style="min-width:9rem;">
                                            <asp:ListItem Value="pending" Text="Pending" />
                                            <asp:ListItem Value="reviewed" Text="Reviewed" />
                                            <asp:ListItem Value="shortlisted" Text="Shortlisted" />
                                            <asp:ListItem Value="interviewed" Text="Interview" />
                                            <asp:ListItem Value="hired" Text="Hired" />
                                            <asp:ListItem Value="rejected" Text="Rejected" />
                                        </asp:DropDownList>
                                        <asp:LinkButton ID="btnUpd" runat="server" CssClass="btn btn-sm btn-primary" CommandName="UpdateStatus" Text="Save" />
                                    </div>
                                    <asp:HiddenField ID="hidAppId" runat="server" Value='<%# Eval("application_id") %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">No applications yet.</asp:Panel>
    </main>
</asp:Content>
