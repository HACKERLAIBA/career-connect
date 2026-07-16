<%@ Page Title="My jobs" Language="C#" MasterPageFile="~/USER/Employer/EmployerMaster.Master" AutoEventWireup="true" CodeBehind="jobs.aspx.cs" Inherits="CareerConnect.USER.Employer.EmployerJobs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container pt-50 pb-80">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="contact-title mb-0">My jobs</h2>
            <a href="job_add.aspx" class="btn btn-primary">Post job</a>
        </div>
        <div class="table-responsive">
            <table class="table table-striped">
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Category</th>
                        <th>Location</th>
                        <th>Type</th>
                        <th>Status</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptJobs" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("title") %></td>
                                <td><%# Eval("category_name") %></td>
                                <td><%# Eval("location") %></td>
                                <td><%# Eval("job_type") %></td>
                                <td><%# Eval("status") %></td>
                                <td><a class="btn btn-sm btn-outline-primary" href='job_edit.aspx?id=<%# Eval("id") %>'>Edit</a></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
            No jobs posted yet. <a href="job_add.aspx">Post your first job</a>.
        </asp:Panel>
    </main>
</asp:Content>
