================================================================================
  CAREER CONNECT â€” PORTABLE PACKAGE (another PC)
================================================================================

WHAT IS IN THIS ZIP
  Full Visual Studio project source, SQL scripts, assets, tools, and docs.
  bin/obj/packages are NOT included (smaller zip). You rebuild on the new PC.

ON THE OTHER PC (order)
  1) Install Visual Studio 2022 (or 2019) with workload:
     - ASP.NET and web development
     - .NET desktop development (if prompted for .NET Framework 4.7.2 targeting)

  2) Install MySQL Server (or XAMPP with MySQL) and phpMyAdmin.

  3) Extract this ZIP anywhere (e.g. C:\Projects\career-connect).

  4) Open CareerConnect.sln or CareerConnect.csproj in Visual Studio.

  5) Right-click solution -> Restore NuGet Packages (if needed).

  6) Build -> Build Solution (Release or Debug).

  7) Database: create database careerconnect (or any name), then in phpMyAdmin
     import database_setup.sql first (WARNING: drops tables if re-run on same DB).
     Then import any migration_*.sql files you use, in the same order as before.

  8) Edit Web.config connectionStrings -> MySqlConn for your local MySQL.

  9) Run (F5) with IIS Express, or publish to local IIS / XAMPP per QUICK_START.md.

OPTIONAL
  tools\sync_to_xampp.ps1 â€” copy to C:\xampp\htdocs\career-connect (edit path if needed)
  tools\LIVE_PUBLISH_STEPS.txt â€” when you deploy to the internet

================================================================================
