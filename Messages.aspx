<%@ Page Title="Messages" Language="C#" MasterPageFile="~/USER/Employer/EmployerMaster.Master" AutoEventWireup="true" CodeBehind="Messages.aspx.cs" Inherits="CareerConnect.USER.Employer.EmployerMessages" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container pt-50 pb-80">
        <h2 class="contact-title mb-4">Messages</h2>
        <div class="table-responsive">
            <table class="table table-striped align-middle">
                <thead>
                    <tr>
                        <th>Job seeker</th>
                        <th>Subject</th>
                        <th>Updated</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <asp:Repeater ID="rptThreads" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("seeker_name") %></td>
                                <td><%# Eval("subject") %></td>
                                <td><%# Eval("updated_at", "{0:dd MMM yyyy HH:mm}") %></td>
                                <td><a class="btn btn-sm btn-primary" href='MessageThread.aspx?t=<%# Eval("id") %>'>Open</a></td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </tbody>
            </table>
        </div>
        <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="alert alert-info">
            No conversations yet. Job seekers can message you from a job posting.
        </asp:Panel>
    </main>
</asp:Content>
