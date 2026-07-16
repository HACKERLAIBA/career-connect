<%@ Page Title="Post job" Language="C#" MasterPageFile="~/USER/Employer/EmployerMaster.Master" AutoEventWireup="true" CodeBehind="job_add.aspx.cs" Inherits="CareerConnect.USER.Employer.EmployerJobAdd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container pt-50 pb-80">
        <h2 class="contact-title mb-4">Post a job</h2>
        <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="alert d-block" />

        <div class="row">
            <div class="col-lg-8">
                <div class="form-group mb-3">
                    <label>Title</label>
                    <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" required />
                </div>
                <div class="form-group mb-3">
                    <label>Category</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Location</label>
                    <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Job type</label>
                    <asp:DropDownList ID="ddlJobType" runat="server" CssClass="form-control">
                        <asp:ListItem Value="full-time" Text="Full-time" />
                        <asp:ListItem Value="part-time" Text="Part-time" />
                        <asp:ListItem Value="contract" Text="Contract" />
                        <asp:ListItem Value="internship" Text="Internship" />
                        <asp:ListItem Value="freelance" Text="Freelance" />
                    </asp:DropDownList>
                </div>
                <div class="form-group mb-3">
                    <label>Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                </div>
                <div class="form-group mb-3">
                    <label>Requirements</label>
                    <asp:TextBox ID="txtRequirements" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                </div>
                <div class="row">
                    <div class="col-md-6">
                        <div class="form-group mb-3">
                            <label>Salary min</label>
                            <asp:TextBox ID="txtSalaryMin" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="form-group mb-3">
                            <label>Salary max</label>
                            <asp:TextBox ID="txtSalaryMax" runat="server" CssClass="form-control" TextMode="Number" />
                        </div>
                    </div>
                </div>
                <div class="form-group mb-3">
                    <label>Skills required (comma separated)</label>
                    <asp:TextBox ID="txtSkills" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Work arrangement</label>
                    <asp:DropDownList ID="ddlWorkArrangement" runat="server" CssClass="form-control">
                        <asp:ListItem Value="onsite" Text="On-site" />
                        <asp:ListItem Value="remote" Text="Remote" />
                        <asp:ListItem Value="hybrid" Text="Hybrid" />
                    </asp:DropDownList>
                </div>
                <div class="form-group mb-3">
                    <label>Application deadline (optional)</label>
                    <asp:TextBox ID="txtAppDeadline" runat="server" CssClass="form-control" TextMode="Date" />
                    <small class="text-muted">Leave blank if there is no fixed deadline.</small>
                </div>
                <div class="form-group mb-3">
                    <label>How to apply (optional)</label>
                    <asp:TextBox ID="txtHowToApply" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="e.g. Email CV to hr@company.com or complete the form below." />
                </div>
                <asp:Button ID="btnSave" runat="server" CssClass="button boxed-btn" Text="Publish job" OnClick="btnSave_Click" />
            </div>
        </div>
    </main>
</asp:Content>
