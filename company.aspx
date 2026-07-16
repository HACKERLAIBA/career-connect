<%@ Page Title="Company profile" Language="C#" MasterPageFile="~/USER/Employer/EmployerMaster.Master" AutoEventWireup="true" CodeBehind="company.aspx.cs" Inherits="CareerConnect.USER.Employer.EmployerCompany" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="container pt-50 pb-80">
        <h2 class="contact-title mb-4">Company profile</h2>
        <asp:Label ID="lblMsg" runat="server" Visible="false" CssClass="alert d-block" />

        <div class="row">
            <div class="col-lg-8">
                <div class="form-group mb-3">
                    <label>Company name</label>
                    <asp:TextBox ID="txtName" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Industry</label>
                    <asp:TextBox ID="txtIndustry" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Website</label>
                    <asp:TextBox ID="txtWebsite" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Email</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                </div>
                <div class="form-group mb-3">
                    <label>Phone</label>
                    <asp:TextBox ID="txtPhone" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>City</label>
                    <asp:TextBox ID="txtCity" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Country</label>
                    <asp:TextBox ID="txtCountry" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group mb-3">
                    <label>Description</label>
                    <asp:TextBox ID="txtDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" />
                </div>
                <asp:Button ID="btnSave" runat="server" CssClass="button boxed-btn" Text="Save" OnClick="btnSave_Click" />
            </div>
        </div>
    </main>
</asp:Content>
