<%@ Page Title="" Language="C#" MasterPageFile="~/USER/usermaster.Master" AutoEventWireup="true" CodeBehind="job details.aspx.cs" Inherits="CareerConnect.USER.job_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<main>

    <!-- Hero Area Start -->
    <div class="slider-area">
        <div class="single-slider section-overly slider-height2 d-flex align-items-center" data-background="../assets/img/hero/about.jpg">
            <div class="container">
                <div class="row">
                    <div class="col-xl-12">
                        <div class="hero-cap text-center">
                            <h2><asp:Label ID="lblTitle" runat="server" Text="Job Title" /></h2>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- Job Details Start -->
    <div class="job-post-company pt-120 pb-120">
        <div class="container">
            <div class="row justify-content-between">

                <!-- Left Content -->
                <div class="col-xl-7 col-lg-8">
                    <div class="single-job-items mb-50">
                        <div class="job-items">
                            <div class="job-tittle w-100">
                                <h4><asp:Label ID="lblTitleInner" runat="server" /></h4>
                                <ul>
                                    <li><asp:Label ID="lblCompany" runat="server" /></li>
                                    <li><asp:Label ID="lblVerified" runat="server" /></li>
                                    <li><i class="fas fa-map-marker-alt"></i> <asp:Label ID="lblLocation" runat="server" /></li>
                                    <li><asp:Label ID="lblSalary" runat="server" /></li>
                                </ul>
                            </div>
                        </div>
                    </div>

                    <div class="job-post-details">
                        <asp:Panel ID="pnlSyndicated" runat="server" Visible="false" CssClass="alert alert-info mb-4">
                            This listing was imported from an external source. Open <strong>Apply on official site</strong> on the right (or use the link under How to apply) to complete your application on the employer’s site.
                        </asp:Panel>
                        <asp:Panel ID="pnlAlreadyApplied" runat="server" Visible="false" CssClass="alert alert-success mb-4">
                            You have already applied for this position. Check <a href="Profile.aspx">My Profile</a> for application status.
                        </asp:Panel>

                        <div class="post-details1 mb-50">
                            <div class="small-section-tittle">
                                <h4>Job Description</h4>
                            </div>
                            <div class="text-muted cc-job-desc-wrap"><asp:Literal ID="litDescription" runat="server" Mode="PassThrough" /></div>
                        </div>

                        <div class="post-details1 mb-50">
                            <div class="small-section-tittle">
                                <h4>Requirements</h4>
                            </div>
                            <div class="text-muted"><asp:Label ID="lblRequirements" runat="server" /></div>
                        </div>

                        <div class="post-details1 mb-50">
                            <div class="small-section-tittle">
                                <h4>How to apply</h4>
                            </div>
                            <div class="text-muted"><asp:Label ID="lblHowToApply" runat="server" /></div>
                        </div>

                        <!-- Apply Section -->
                        <asp:Panel ID="pnlApply" runat="server" Visible="false">
                            <div class="post-details1 mb-50">
                                <div class="small-section-tittle">
                                    <h4>Apply Now</h4>
                                </div>
                                <asp:TextBox ID="txtMessage" runat="server" CssClass="form-control" placeholder="Your message" TextMode="MultiLine" Rows="5" />
                                <asp:Button ID="btnApply" runat="server" Text="Submit Application" CssClass="btn btn-primary mt-3" OnClick="btnApply_Click" />
                                <asp:Label ID="lblApplyResult" runat="server" CssClass="text-success mt-2" />
                            </div>
                        </asp:Panel>

                    </div>
                </div>

                <!-- Right Content -->
                <div class="col-xl-4 col-lg-4">
                                       <div class="post-details3 mb-50">
                        <div class="small-section-tittle"><h4>Job Overview</h4></div>
                        <ul>
                            <li>Category : <span><asp:Label ID="lblCategory" runat="server" /></span></li>
                            <li>Posted : <span><asp:Label ID="lblCreatedAt" runat="server" /></span></li>
                            <li>Type : <span><asp:Label ID="lblJobType" runat="server" /></span></li>
                            <li>Work mode : <span><asp:Label ID="lblWorkArr" runat="server" /></span></li>
                            <li>Apply by : <span><asp:Label ID="lblDeadline" runat="server" /></span></li>
                        </ul>
                        <div class="apply-btn2 mb-3">
                            <asp:HyperLink ID="hypExternalApply" runat="server" CssClass="btn btn-success w-100 mb-2 cc-job-details-apply" Visible="false" />
                            <asp:Button ID="btnShowApply" runat="server" Text="Apply Now" CssClass="btn btn-primary w-100 mb-2 cc-job-details-apply" OnClick="btnShowApply_Click" />
                        </div>
                        <asp:Panel ID="pnlSaveJob" runat="server" CssClass="mb-3">
                            <asp:LinkButton ID="btnToggleSave" runat="server" CssClass="btn btn-outline-primary w-100" OnClick="btnToggleSave_Click" CausesValidation="false">
                                <i class="fas fa-bookmark"></i> <asp:Literal ID="litSaveLabel" runat="server" Text="Save job" />
                            </asp:LinkButton>
                            <asp:Label ID="lblSaveHint" runat="server" CssClass="small text-muted d-block mt-1" />
                        </asp:Panel>
                        <asp:HyperLink ID="hypMsgEmployer" runat="server" CssClass="btn btn-outline-secondary w-100 mb-2" Text="Message employer" Visible="false" />
                        <asp:HyperLink ID="hypCompanyProfile" runat="server" CssClass="btn btn-outline-primary w-100" Text="View company profile" Visible="false" />
                    </div>
                    <div class="post-details4 mb-50">
                        <div class="small-section-tittle"><h4>Company Information</h4></div>
                        <asp:Label ID="lblCompanyInfo" runat="server" Text="Company info coming soon..." />
                    </div>

                    <asp:Panel ID="pnlReport" runat="server" CssClass="post-details4 mb-50" Visible="false">
                        <div class="small-section-tittle"><h4>Report this job</h4></div>
                        <div class="mb-2">
                            <label class="form-label small fw-bold">Reason</label>
                            <asp:DropDownList ID="ddlReportReason" runat="server" CssClass="form-select form-select-sm">
                                <asp:ListItem Value="spam" Text="Spam / scam" />
                                <asp:ListItem Value="misleading" Text="Misleading information" />
                                <asp:ListItem Value="duplicate" Text="Duplicate posting" />
                                <asp:ListItem Value="inappropriate" Text="Inappropriate content" />
                                <asp:ListItem Value="other" Text="Other" />
                            </asp:DropDownList>
                        </div>
                        <div class="mb-2">
                            <label class="form-label small fw-bold">Details (optional)</label>
                            <asp:TextBox ID="txtReportDetails" runat="server" CssClass="form-control form-control-sm" TextMode="MultiLine" Rows="3" />
                        </div>
                        <asp:Button ID="btnReport" runat="server" Text="Submit report" CssClass="btn btn-outline-danger btn-sm w-100" OnClick="btnReport_Click" />
                        <asp:Label ID="lblReportResult" runat="server" CssClass="small text-muted d-block mt-2" />
                    </asp:Panel>
                </div>

            </div>
        </div>
    </div>

</main>
    <script>window.ccMessaging = { companyId: <%= CcMsgCompanyId %>, jobId: <%= CcMsgJobId %> };</script>
</asp:Content>

