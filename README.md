# Career Connect - Complete Job Portal System

A comprehensive job portal system with both user-facing ASP.NET application and PHP-based admin dashboard.

## ðŸš€ Features

### User Side (ASP.NET)
- **Job Search & Browse**: Search and filter jobs by category, location, and type
- **User Registration & Login**: Secure user authentication system
- **Job Applications**: Apply to jobs with cover letters and resume uploads
- **Profile Management**: User profile creation and management
- **Responsive Design**: Modern, mobile-friendly interface

### Admin Dashboard (PHP)
- **Dashboard Overview**: Statistics and analytics
- **User Management**: Manage user accounts and permissions
- **Job Management**: Create, edit, and manage job postings
- **Company Management**: Manage company profiles
- **Category Management**: Organize job categories
- **Application Management**: Review and manage job applications
- **Resume Management**: View and manage user resumes

## ðŸ“‹ Prerequisites

- **Web Server**: XAMPP, WAMP, or similar (for PHP admin)
- **ASP.NET Framework**: 4.7.2 or higher
- **MySQL**: 5.7 or higher
- **Visual Studio**: 2019 or higher (for ASP.NET development)
- **PHP**: 7.4 or higher (for admin dashboard)

## ðŸ› ï¸ Installation & Setup

### 1. Database Setup

1. **Open phpMyAdmin** or MySQL command line
2. **Import the database**:
   - Open `database_setup.sql` in phpMyAdmin
   - Click "Go" to execute the script
   - Or run: `mysql -u root -p < database_setup.sql`

### 2. ASP.NET User Side Setup

1. **Open the project** in Visual Studio
2. **Update Web.config** connection string if needed:
   ```xml
   <connectionStrings>
       <add name="MySqlConn" 
            connectionString="server=localhost;user id=root;password=;database=careerconnect;" 
            providerName="MySql.Data.MySqlClient" />
   </connectionStrings>
   ```
3. **Restore NuGet packages**:
   - Right-click solution â†’ "Restore NuGet Packages"
   - Or run: `nuget restore`
4. **Build the project** (Ctrl+Shift+B)
5. **Run the application** (F5)

### 3. PHP Admin Dashboard Setup

1. **Copy Admin folder** to your web server directory (e.g., `htdocs` for XAMPP)
2. **Update database configuration** in `Admin/includes/config.php`:
   ```php
   $host = 'localhost';
   $dbname = 'careerconnect';
   $username = 'root';
   $password = '';
   ```
3. **Set permissions** for uploads folder (if needed)
4. **Access admin panel**: `http://localhost/job%20pilot/Admin/`

## ðŸ” Default Credentials

### Admin Dashboard
- **Username**: `admin`
- **Password**: `admin123`

### Sample Users
- **John Doe**: `john@example.com` / `password`
- **Jane Smith**: `jane@example.com` / `password`

## ðŸ“ Project Structure

```
CareerConnect/
â”œâ”€â”€ Admin/                     # PHP Admin Dashboard
â”‚   â”œâ”€â”€ includes/             # PHP includes and config
â”‚   â”œâ”€â”€ dashboard.php         # Admin dashboard
â”‚   â”œâ”€â”€ users.php            # User management
â”‚   â”œâ”€â”€ jobs.php             # Job management
â”‚   â”œâ”€â”€ companies.php        # Company management
â”‚   â”œâ”€â”€ categories.php       # Category management
â”‚   â”œâ”€â”€ applications.php     # Application management
â”‚   â””â”€â”€ resumes.php          # Resume management
â”œâ”€â”€ USER/                     # ASP.NET User Side
â”‚   â”œâ”€â”€ default.aspx         # Home page
â”‚   â”œâ”€â”€ job listing.aspx     # Job search
â”‚   â”œâ”€â”€ job details.aspx     # Job details
â”‚   â”œâ”€â”€ Login.aspx           # User login
â”‚   â”œâ”€â”€ Register.aspx        # User registration
â”‚   â””â”€â”€ usermaster.Master    # Master page
â”œâ”€â”€ assets/                   # Static assets
â”‚   â”œâ”€â”€ css/                 # Stylesheets
â”‚   â”œâ”€â”€ js/                  # JavaScript files
â”‚   â”œâ”€â”€ img/                 # Images
â”‚   â””â”€â”€ fonts/               # Font files
â”œâ”€â”€ Models/                   # Data models
â”œâ”€â”€ Web.config               # ASP.NET configuration
â”œâ”€â”€ packages.config          # NuGet packages
â”œâ”€â”€ database_setup.sql       # Database schema
â””â”€â”€ README.md               # This file
```

## ðŸ—„ï¸ Database Schema

### Core Tables
- **users**: User accounts and profiles
- **companies**: Company information
- **categories**: Job categories
- **jobs**: Job postings
- **applications**: Job applications
- **resumes**: User resumes

### Key Relationships
- Jobs belong to companies and categories
- Applications link users to jobs
- Resumes belong to users
- All tables include timestamps and status fields

## ðŸŽ¨ Customization

### Styling
- **User Side**: Modify `assets/css/style.css`
- **Admin Side**: Bootstrap 5 classes and custom CSS

### Configuration
- **Database**: Update connection strings in `Web.config` and `Admin/includes/config.php`
- **Admin Credentials**: Modify `Admin/includes/auth.php`
- **Upload Paths**: Update `UPLOAD_PATH` in config files

## ðŸ”§ Troubleshooting

### Common Issues

1. **Database Connection Error**
   - Verify MySQL is running
   - Check connection string in Web.config
   - Ensure database 'careerconnect' exists

2. **Admin Login Issues**
   - Verify PHP is enabled in web server
   - Check file permissions
   - Ensure session support is enabled

3. **File Upload Issues**
   - Check folder permissions
   - Verify upload path configuration
   - Check file size limits

4. **ASP.NET Build Errors**
   - Restore NuGet packages
   - Check .NET Framework version
   - Verify MySQL.Data package is installed

### Error Logs
- **PHP Errors**: Check web server error logs
- **ASP.NET Errors**: Check Visual Studio Output window
- **Database Errors**: Check MySQL error logs

## ðŸš€ Deployment

### Local Development
1. Use Visual Studio for ASP.NET development
2. Use XAMPP/WAMP for PHP admin testing
3. Use phpMyAdmin for database management

### Production Deployment
1. **ASP.NET**: Deploy to IIS or Azure
2. **PHP Admin**: Deploy to shared hosting or VPS
3. **Database**: Use production MySQL server
4. **Security**: Update default passwords and enable HTTPS

## ðŸ“¦ Migrating to Another PC

Need to transfer this project to another computer? We've got you covered!

### ðŸŽ¯ Two Migration Methods Available

#### Method 1: ZIP Files (Recommended - Easiest) â­
**Best for:** Incremental transfer and testing

1. Run `create_migration_zips.ps1` to create ZIP files
2. Transfer ZIP files to new PC
3. Extract and test one ZIP at a time
4. Only proceed to next ZIP if current one works

**See ZIP_MIGRATION_GUIDE.md for detailed instructions.**

#### Method 2: Manual File Transfer
**Best for:** Full control over transfer process

1. Follow MIGRATION_GUIDE.md step by step
2. Transfer files manually in phases
3. Test after each phase

### Migration Guides Available

1. **ZIP_MIGRATION_QUICK_START.md** - Quick start for ZIP method â­
2. **ZIP_MIGRATION_GUIDE.md** - Complete ZIP migration guide
3. **QUICK_MIGRATION_GUIDE.md** - Quick 5-minute overview
4. **MIGRATION_GUIDE.md** - Complete step-by-step guide (manual method)
5. **MIGRATION_CHECKLIST.md** - Printable checklist to track progress
6. **FILE_TRANSFER_LIST.md** - Complete file list organized by phase
7. **MIGRATION_README.md** - Overview of all migration documentation

### Quick Start (ZIP Method)

1. Run `create_migration_zips.ps1` to create ZIP files
2. Transfer ZIP files to new PC in order
3. Extract and test one ZIP at a time
4. Follow **ZIP_MIGRATION_GUIDE.md** for detailed instructions

### Quick Start (Manual Method)

1. Read **QUICK_MIGRATION_GUIDE.md** for overview
2. Follow **MIGRATION_GUIDE.md** step by step
3. Use **MIGRATION_CHECKLIST.md** to track progress
4. Run `verify_migration.bat` or `verify_migration.ps1` to verify files

### Migration Process

The migration follows this order:
1. **Foundation Files** - Config files, project files, database
2. **Assets** - CSS, JS, images, fonts
3. **Core Infrastructure** - Master page, models
4. **Root Page** - default.aspx
5. **User Pages** - Transfer and test one by one
6. **Admin Dashboard** - Admin config and pages
7. **Final Steps** - Build, test, and verify

**See ZIP_MIGRATION_GUIDE.md (ZIP method) or MIGRATION_GUIDE.md (manual method) for detailed instructions.**

## ðŸ“ API Endpoints

### Admin API (PHP)
- `GET /Admin/dashboard.php` - Dashboard overview
- `GET /Admin/users.php` - User management
- `GET /Admin/jobs.php` - Job management
- `POST /Admin/actions/` - CRUD operations

### User API (ASP.NET)
- `GET /USER/default.aspx` - Home page
- `GET /USER/job listing.aspx` - Job search
- `POST /USER/Login.aspx` - User authentication

## ðŸ¤ Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## ðŸ“„ License

This project is licensed under the MIT License - see the LICENSE file for details.

## ðŸ†˜ Support

For support and questions:
- Check the troubleshooting section above
- Review the code comments
- Create an issue in the repository

## ðŸ”„ Version History

- **v1.0.0**: Initial release with basic functionality
- **v1.1.0**: Added admin dashboard
- **v1.2.0**: Enhanced user interface and database schema
- **v1.3.0**: Complete system with all features

---

**Career Connect** - Connecting talent with opportunities! ðŸš€ 
