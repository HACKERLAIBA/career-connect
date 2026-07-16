<%@ Page Title="" Language="C#" MasterPageFile="~/USER/usermaster.Master" AutoEventWireup="true" CodeBehind="job listing.aspx.cs" Inherits="CareerConnect.USER.job_listing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<main>

    <!-- Hero Area Start -->
    <div class="slider-area ">
        <div class="single-slider section-overly slider-height2 d-flex align-items-center" data-background="../assets/img/hero/about.jpg">
            <div class="container">
                <div class="row">
                    <div class="col-xl-12">
                        <div class="hero-cap text-center">
                            <h2>Get your job</h2>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!-- Hero Area End -->

    <!-- Job List Area Start -->
    <div class="job-listing-area pt-120 pb-120">
        <div class="container">
            <div class="row">
                <!-- Left content (Filters) -->
                <div class="col-xl-3 col-lg-3 col-md-4">
                    <div class="job-category-listing mb-50">
                        <div class="small-section-tittle2 mb-3">
                            <h4>Search &amp; filters</h4>
                        </div>
                        <div class="single-listing mb-3">
                            <label class="form-label small fw-bold">Keyword</label>
                            <asp:TextBox ID="txtKeyword" runat="server" CssClass="form-control form-control-sm" placeholder="Title, skills, company..." />
                        </div>
                        <div class="single-listing mb-3">
                            <label class="form-label small fw-bold">Category</label>
                            <asp:DropDownList ID="ddlCategoryFilter" runat="server" CssClass="form-select form-select-sm" />
                        </div>
                        <div class="single-listing mb-3">
                            <label class="form-label small fw-bold">Location</label>
                            <asp:TextBox ID="txtLocation" runat="server" CssClass="form-control form-control-sm" placeholder="City, country..." />
                        </div>
                        <div class="single-listing mb-3">
                            <label class="form-label small fw-bold">Job type</label>
                            <asp:DropDownList ID="ddlJobTypeFilter" runat="server" CssClass="form-select form-select-sm">
                                <asp:ListItem Value="" Text="All types" />
                                <asp:ListItem Value="full-time" Text="Full-time" />
                                <asp:ListItem Value="part-time" Text="Part-time" />
                                <asp:ListItem Value="contract" Text="Contract" />
                                <asp:ListItem Value="internship" Text="Internship" />
                                <asp:ListItem Value="freelance" Text="Freelance" />
                            </asp:DropDownList>
                        </div>
                        <div class="single-listing mb-3">
                            <label class="form-label small fw-bold">Salary min / max</label>
                            <div class="row g-1">
                                <div class="col-6"><asp:TextBox ID="txtSalMin" runat="server" CssClass="form-control form-control-sm" placeholder="Min" TextMode="Number" /></div>
                                <div class="col-6"><asp:TextBox ID="txtSalMax" runat="server" CssClass="form-control form-control-sm" placeholder="Max" TextMode="Number" /></div>
                            </div>
                        </div>
                        <div class="single-listing mb-3">
                            <label class="form-label small fw-bold">Posted</label>
                            <asp:DropDownList ID="ddlPosted" runat="server" CssClass="form-select form-select-sm">
                                <asp:ListItem Value="0" Text="Any time" />
                                <asp:ListItem Value="7" Text="Last 7 days" />
                                <asp:ListItem Value="14" Text="Last 14 days" />
                                <asp:ListItem Value="30" Text="Last 30 days" />
                            </asp:DropDownList>
                        </div>
                        <asp:Button ID="btnSearch" runat="server" CssClass="btn btn-primary btn-sm w-100" Text="Apply filters" OnClick="btnSearch_Click" />
                    </div>
                </div>

                <!-- Right content (Job Listings) -->
                <div class="col-xl-9 col-lg-9 col-md-8">
                    <section class="featured-job-area">
                        <div class="container">
                            <asp:Panel ID="pnlRecommendations" runat="server" Visible="false" CssClass="mb-40">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="count-job mb-20">
                                            <span>Recommended for you</span>
                                            <small class="d-block text-muted mt-1" style="font-weight:400;">Based on your profile and skills (TF–IDF similarity).</small>
                                        </div>
                                    </div>
                                </div>
                            <asp:Repeater ID="rptRecommended" runat="server" OnItemCommand="rptRecommended_ItemCommand" OnItemDataBound="rptRecommended_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="single-job-items mb-30 border rounded px-2" style="border-color:#415a77!important;">
                                            <div class="job-items">
                                                <div class="company-img">
                                                    <a href="job details.aspx?jobId=<%# Eval("id") %>"><img src='<%# Eval("company_logo") %>' alt="" /></a>
                                                </div>
                                                <div class="job-tittle job-tittle2">
                                                    <a href="job details.aspx?jobId=<%# Eval("id") %>">
                                                        <h4><%# Eval("title") %> <asp:Literal ID="litRecMatch" runat="server" /></h4>
                                                    </a>
                                                    <ul>
                                                        <li><%# Eval("CompanyName") %> <asp:Literal ID="litVerifiedBadge" runat="server" /></li>
                                                        <li><i class="fas fa-map-marker-alt"></i> <%# Eval("location") %></li>
                                                        <li><%# Eval("salary") %></li>
                                                        <li><%# Eval("CategoryTitle") %></li>
                                                    </ul>
                                                </div>
                                            </div>
                                            <div class="items-link items-link2 f-right">
                                                <a href="job details.aspx?jobId=<%# Eval("id") %>"><%# Eval("job_type") %></a>
                                                <span><%# Eval("created_at", "{0:dd MMM yyyy}") %></span>
                                                <div class="mt-2 text-end">
                                                    <asp:LinkButton ID="btnSave" runat="server" CommandName="toggle_save" CommandArgument='<%# Eval("id") %>' CssClass="btn btn-outline-primary btn-sm" CausesValidation="false">
                                                        <i class="fas fa-bookmark"></i> <asp:Literal ID="litSaveText" runat="server" Text="Save" />
                                                    </asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </asp:Panel>

                            <div class="row align-items-center">
                                <div class="col-lg-8">
                                    <div class="count-job mb-20">
                                        <span>Available Jobs</span>
                                    </div>
                                    <p class="text-muted small mb-3"><asp:Literal ID="litResultSummary" runat="server" /></p>
                                </div>
                                <div class="col-lg-4 text-lg-end mb-3">
                                    <asp:Panel ID="pnlPager" runat="server" Visible="false" CssClass="d-inline-flex align-items-center gap-2 flex-wrap justify-content-lg-end">
                                        <asp:Button ID="btnPrev" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="Prev" OnClick="btnPrev_Click" />
                                        <asp:Literal ID="litPageLabel" runat="server" />
                                        <asp:Button ID="btnNext" runat="server" CssClass="btn btn-outline-secondary btn-sm" Text="Next" OnClick="btnNext_Click" />
                                    </asp:Panel>
                                </div>
                            </div>

                            <!-- Repeater Start -->
                            <asp:Repeater ID="rptJobs" runat="server" OnItemCommand="rptJobs_ItemCommand" OnItemDataBound="rptJobs_ItemDataBound">
                                <ItemTemplate>
                                    <div class="single-job-items mb-30">
                                        <div class="job-items">
                                            <div class="company-img">
                                                <a href='job details.aspx?jobId=<%# Eval("id") %>'><img src='<%# Eval("company_logo") %>' alt="" /></a>
                                            </div>
                                            <div class="job-tittle job-tittle2">
                                                <a href='job details.aspx?jobId=<%# Eval("id") %>'>
                                                    <h4><%# Eval("title") %> <asp:Literal ID="litExternalSrc" runat="server" /></h4>
                                                </a>
                                                <ul>
                                                    <li><%# Eval("CompanyName") %> <asp:Literal ID="litVerifiedBadge" runat="server" /></li>
                                                    <li><i class="fas fa-map-marker-alt"></i> <%# Eval("location") %></li>
                                                    <li><%# Eval("salary") %></li>
                                                    <li><%# Eval("CategoryTitle") %></li>
                                                </ul>
                                            </div>
                                        </div>
                                            <div class="items-link items-link2 f-right">
                                            <a href='job details.aspx?jobId=<%# Eval("id") %>'><%# Eval("job_type") %></a>
                                            <span><%# Eval("created_at", "{0:dd MMM yyyy}") %></span>
                                            <div class="mt-2 text-end">
                                                <asp:LinkButton ID="btnSave" runat="server" CommandName="toggle_save" CommandArgument='<%# Eval("id") %>' CssClass="btn btn-outline-primary btn-sm" CausesValidation="false">
                                                    <i class="fas fa-bookmark"></i> <asp:Literal ID="litSaveText" runat="server" Text="Save" />
                                                </asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                            <!-- Repeater End -->

                            <!-- No Jobs Found Message -->
                            <asp:Panel ID="pnlNoJobs" runat="server" Visible="false" CssClass="text-center py-5">
                                <div class="alert alert-info">
                                    <h4>No Jobs Found</h4>
                                    <p>There are currently no jobs available. Please check back later or contact us for more information.</p>
                                    <a href="default.aspx" class="btn btn-primary">Go Back Home</a>
                                </div>
                            </asp:Panel>

                        </div>
                    </section>
                </div>
            </div>
        </div>
    </div>
    <!-- Job List Area End -->

</main>
</asp:Content>

