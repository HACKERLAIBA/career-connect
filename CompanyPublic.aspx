<%@ Page Title="Company" Language="C#" MasterPageFile="~/USER/usermaster.Master" AutoEventWireup="true" CodeBehind="CompanyPublic.aspx.cs" Inherits="CareerConnect.USER.CompanyPublic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="slider-area">
        <div class="single-slider section-overly slider-height2 d-flex align-items-center" data-background="../assets/img/hero/about.jpg">
            <div class="container">
                <div class="hero-cap text-center">
                    <h2><asp:Literal ID="litTitle" runat="server" /> <asp:Literal ID="litVerified" runat="server" /></h2>
                </div>
            </div>
        </div>
    </div>
    <section class="contact-section pt-100 pb-100">
        <div class="container">
            <asp:Panel ID="pnlMissing" runat="server" Visible="false" CssClass="alert alert-warning">Company not found.</asp:Panel>
            <asp:Panel ID="pnlBody" runat="server" Visible="false">
                <div class="card border-0 shadow-sm">
                    <div class="card-body">
                        <p class="text-muted mb-2"><asp:Literal ID="litIndustry" runat="server" /></p>
                        <p class="mb-3"><asp:Literal ID="litDesc" runat="server" /></p>
                        <ul class="list-unstyled small mb-0">
                            <li runat="server" id="liSite" visible="false"><i class="fas fa-link me-2"></i><asp:HyperLink ID="hypWebsite" runat="server" Target="_blank" /></li>
                            <li runat="server" id="liEmail" visible="false"><i class="fas fa-envelope me-2"></i><asp:Literal ID="litEmail" runat="server" /></li>
                            <li runat="server" id="liLoc" visible="false"><i class="fas fa-map-marker-alt me-2"></i><asp:Literal ID="litLoc" runat="server" /></li>
                        </ul>
                        <div class="mt-3">
                            <asp:HyperLink ID="hypMsgCompany" runat="server" CssClass="btn btn-primary" Visible="false" />
                        </div>
                    </div>
                </div>
                <p class="mt-3 small text-muted mb-0">This is a public company profile. To manage jobs, employers sign in to the employer portal.</p>
            </asp:Panel>
        </div>
    </section>
    <script>window.ccMessaging = { companyId: <%= CcMsgCompanyId %>, jobId: 0 };</script>
</asp:Content>
