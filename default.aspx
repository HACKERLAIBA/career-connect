<%@ Page Title="" Language="C#" MasterPageFile="~/USER/usermaster.Master" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="CareerConnect.USER._default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<main>
    <!-- Hero Section -->
<div class="slider-area">
    <div class="single-slider slider-height d-flex align-items-center" data-background="../assets/img/hero/h1_hero.jpg">
        <div class="container">
            <div class="row">
                <div class="col-xl-8">
                    <div class="hero__caption">
                        <h1 class="text-blue">Find the most exciting startup jobs</h1>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>

    <!-- Our Services Start (Dynamic Categories) -->
    <div class="our-services section-pad-t30">
        <div class="container">
            <!-- Section Tittle -->
            <div class="row">
                <div class="col-lg-12">
                    <div class="section-tittle text-center">
                        <span>FEATURED TOURS Packages</span>
                        <h2>Browse Top Categories</h2>
                    </div>
                </div>
            </div>

            <!-- Dynamic Category Repeater -->
            <div class="row d-flex justify-contnet-center">
                <asp:Repeater ID="rptCategories" runat="server">
                    <ItemTemplate>
                        <div class="col-xl-3 col-lg-3 col-md-4 col-sm-6">
                            <div class="single-services text-center mb-30">
                                <div class="services-ion">
                                    <span class='<%# Eval("IconClass") %>'></span>
                                </div>
                                <div class="services-cap">
                                    <h5>
                                        <a href='job listing.aspx?category=<%# Eval("id") %>'><%# Eval("Title") %></a>
                                    </h5>
                                    <span>(<%# Eval("JobCount") %>)</span>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <!-- Browse All Button -->
            <div class="row">
                <div class="col-lg-12">
                    <div class="browse-btn2 text-center mt-50">
                        <a href="job listing.aspx" class="border-btn2">Browse All Sectors</a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <!-- How Apply Process Section (Static) -->
    <div class="apply-process-area apply-bg pt-150 pb-150" data-background="assets/img/gallery/how-applybg.png">
        <div class="container">
            <!-- Section Tittle -->
            <div class="row">
                <div class="col-lg-12">
                    <div class="section-tittle white-text text-center">
                        <span>Apply process</span>
                        <h2>How it works</h2>
                    </div>
                </div>
            </div>

            <!-- 3-Step Apply Process -->
            <div class="row">
                <div class="col-lg-4 col-md-6">
                    <div class="single-process text-center mb-30">
                        <div class="process-ion">
                            <span class="flaticon-search"></span>
                        </div>
                        <div class="process-cap">
                            <h5>1. Search a job</h5>
                            <p>Discover jobs that match your passion and location.</p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6">
                    <div class="single-process text-center mb-30">
                        <div class="process-ion">
                            <span class="flaticon-curriculum-vitae"></span>
                        </div>
                        <div class="process-cap">
                            <h5>2. Apply for job</h5>
                            <p>Submit your profile and message—stand out from the crowd.</p>
                        </div>
                    </div>
                </div>
                <div class="col-lg-4 col-md-6">
                    <div class="single-process text-center mb-30">
                        <div class="process-ion">
                            <span class="flaticon-tour"></span>
                        </div>
                        <div class="process-cap">
                            <h5>3. Get your job</h5>
                            <p>Get selected and start your new journey with top employers.</p>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</main>
</asp:Content>

