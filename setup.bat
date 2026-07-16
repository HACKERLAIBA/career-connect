@echo off
echo ========================================
echo Career Connect - Setup Script
echo ========================================
echo.

echo This script will help you set up the Career Connect project.
echo.

echo 1. Checking prerequisites...
echo.

REM Check if MySQL is running
echo Checking MySQL connection...
mysql --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ERROR: MySQL is not installed or not in PATH
    echo Please install MySQL and add it to your system PATH
    pause
    exit /b 1
)

echo MySQL is available.
echo.

echo 2. Database Setup Instructions:
echo.
echo Please follow these steps to set up the database:
echo.
echo a) Open phpMyAdmin in your web browser
echo b) Create a new database named 'careerconnect'
echo c) Import the database_setup.sql file
echo d) Verify all tables are created successfully
echo.

echo 3. Configuration Files:
echo.
echo The following files need to be configured:
echo.
echo - Web.config: Database connection string
echo - Admin/includes/config.php: Database settings
echo - Admin/includes/auth.php: Admin credentials
echo.

echo 4. Web Server Setup:
echo.
echo For the PHP admin panel:
echo - Copy the Admin folder to your web server directory
echo - Ensure PHP is enabled
echo - Set proper file permissions
echo.

echo 5. ASP.NET Setup:
echo.
echo For the ASP.NET user side:
echo - Open the project in Visual Studio
echo - Restore NuGet packages
echo - Build the solution
echo - Run the application
echo.

echo ========================================
echo Setup Instructions Complete
echo ========================================
echo.
echo Default Admin Credentials:
echo Username: admin
echo Password: admin123
echo.
echo Access URLs:
echo - User Side: http://localhost:port/USER/default.aspx
echo - Admin Panel: http://localhost/job%%20pilot/Admin/
echo.
pause 
