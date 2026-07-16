# Career Connect - Configuration Guide

This guide provides detailed instructions for configuring and setting up the Career Connect system.

## ðŸ“‹ Table of Contents

1. [Database Configuration](#database-configuration)
2. [ASP.NET Configuration](#aspnet-configuration)
3. [PHP Admin Configuration](#php-admin-configuration)
4. [Web Server Setup](#web-server-setup)
5. [Security Configuration](#security-configuration)
6. [Testing Configuration](#testing-configuration)

## ðŸ—„ï¸ Database Configuration

### 1. MySQL Setup

1. **Install MySQL** (if not already installed)
   - Download from: https://dev.mysql.com/downloads/mysql/
   - Install with default settings
   - Remember the root password

2. **Create Database**
   ```sql
   CREATE DATABASE careerconnect CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```

3. **Import Schema**
   - Open phpMyAdmin
   - Select the `careerconnect` database
   - Click "Import"
   - Choose `database_setup.sql`
   - Click "Go"

### 2. Database Connection Settings

**For ASP.NET (Web.config):**
```xml
<connectionStrings>
    <add name="MySqlConn" 
         connectionString="server=localhost;user id=root;password=YOUR_PASSWORD;database=careerconnect;charset=utf8;" 
         providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

**For PHP Admin (Admin/includes/config.php):**
```php
$host = 'localhost';
$dbname = 'careerconnect';
$username = 'root';
$password = 'YOUR_PASSWORD';
```

## ðŸ–¥ï¸ ASP.NET Configuration

### 1. Visual Studio Setup

1. **Open Project**
   - Open `CareerConnect.csproj` in Visual Studio
   - Allow Visual Studio to restore NuGet packages

2. **Verify Dependencies**
   - Right-click solution â†’ "Manage NuGet Packages"
   - Ensure `MySql.Data` package is installed
   - Version should be 9.3.0 or compatible

3. **Build Configuration**
   - Set configuration to "Debug" for development
   - Target framework: .NET Framework 4.7.2

### 2. Web.config Settings

**Connection String:**
```xml
<connectionStrings>
    <add name="MySqlConn" 
         connectionString="server=localhost;user id=root;password=;database=careerconnect;" 
         providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

**App Settings:**
```xml
<appSettings>
    <add key="username" value="Admin" />
    <add key="password" value="123" />
    <add key="ValidationSettings:UnobtrusiveValidationMode" value="None" />
</appSettings>
```

**Compilation Settings:**
```xml
<system.web>
    <compilation debug="true" targetFramework="4.7.2" />
    <httpRuntime targetFramework="4.7.2" />
</system.web>
```

### 3. IIS Express Configuration

1. **Port Configuration**
   - Default port: 55072
   - Can be changed in project properties

2. **SSL Configuration**
   - Enable SSL if needed for production
   - Configure certificates appropriately

## ðŸ˜ PHP Admin Configuration

### 1. Web Server Setup

**XAMPP:**
1. Install XAMPP
2. Copy `Admin` folder to `htdocs` directory
3. Start Apache and MySQL services

**WAMP:**
1. Install WAMP
2. Copy `Admin` folder to `www` directory
3. Start WAMP services

**Custom Server:**
1. Configure Apache/Nginx
2. Enable PHP module
3. Set document root appropriately

### 2. PHP Configuration

**Admin/includes/config.php:**
```php
<?php
// Database configuration
$host = 'localhost';
$dbname = 'careerconnect';
$username = 'root';
$password = '';

// Create connection
try {
    $pdo = new PDO("mysql:host=$host;dbname=$dbname;charset=utf8", $username, $password);
    $pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
    $pdo->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_ASSOC);
} catch(PDOException $e) {
    die("Connection failed: " . $e->getMessage());
}

// Site configuration
define('SITE_NAME', 'Career Connect Admin');
define('SITE_URL', 'http://localhost/job%20pilot/Admin/');
define('UPLOAD_PATH', '../uploads/');

// Session configuration
session_start();

// Error reporting
error_reporting(E_ALL);
ini_set('display_errors', 1);
?>
```

### 3. Authentication Configuration

**Admin/includes/auth.php:**
```php
<?php
require_once 'config.php';

// Check if user is logged in
function isLoggedIn() {
    return isset($_SESSION['admin_logged_in']) && $_SESSION['admin_logged_in'] === true;
}

// Login function
function login($username, $password) {
    // For demo purposes, using hardcoded admin credentials
    // In production, you should use database authentication
    if ($username === 'admin' && $password === 'admin123') {
        $_SESSION['admin_logged_in'] = true;
        $_SESSION['admin_username'] = $username;
        return true;
    }
    return false;
}
?>
```

## ðŸŒ Web Server Setup

### 1. Apache Configuration

**Virtual Host (Optional):**
```apache
<VirtualHost *:80>
    ServerName careerconnect.local
    DocumentRoot "C:/xampp/htdocs/CareerConnect"
    
    <Directory "C:/xampp/htdocs/CareerConnect">
        Options Indexes FollowSymLinks MultiViews
        AllowOverride All
        Require all granted
    </Directory>
    
    ErrorLog "logs/careerconnect_error.log"
    CustomLog "logs/careerconnect_access.log" combined
</VirtualHost>
```

### 2. File Permissions

**Windows:**
- Ensure IIS_IUSRS or IUSR has read access
- For uploads: Full control for web server user

**Linux:**
```bash
chmod 755 /var/www/html/job\ pilot/
chmod 755 /var/www/html/job\ pilot/Admin/
chmod 777 /var/www/html/job\ pilot/Admin/uploads/
```

### 3. .htaccess Configuration

**Admin/.htaccess:**
```apache
RewriteEngine On
RewriteCond %{REQUEST_FILENAME} !-f
RewriteCond %{REQUEST_FILENAME} !-d
RewriteRule ^(.*)$ index.php [QSA,L]

# Security headers
Header always set X-Content-Type-Options nosniff
Header always set X-Frame-Options DENY
Header always set X-XSS-Protection "1; mode=block"
```

## ðŸ”’ Security Configuration

### 1. Database Security

1. **Change Default Passwords**
   ```sql
   ALTER USER 'root'@'localhost' IDENTIFIED BY 'strong_password';
   FLUSH PRIVILEGES;
   ```

2. **Create Application User**
   ```sql
   CREATE USER 'careerconnect'@'localhost' IDENTIFIED BY 'app_password';
   GRANT SELECT, INSERT, UPDATE, DELETE ON careerconnect.* TO 'careerconnect'@'localhost';
   FLUSH PRIVILEGES;
   ```

### 2. File Upload Security

**Admin/uploads/.htaccess:**
```apache
Options -Indexes
<FilesMatch "\.(php|php3|php4|php5|phtml|pl|py|jsp|asp|sh|cgi)$">
    Order Deny,Allow
    Deny from all
</FilesMatch>

<FilesMatch "\.(pdf|doc|docx|txt|rtf)$">
    Order Allow,Deny
    Allow from all
</FilesMatch>
```

### 3. Session Security

**PHP Session Configuration:**
```php
// In config.php
ini_set('session.cookie_httponly', 1);
ini_set('session.use_only_cookies', 1);
ini_set('session.cookie_secure', 1); // Enable for HTTPS
session_start();
```

## ðŸ§ª Testing Configuration

### 1. Database Connection Test

**PHP Test Script:**
```php
<?php
require_once 'Admin/includes/config.php';
try {
    $stmt = $pdo->query("SELECT COUNT(*) as count FROM users");
    $result = $stmt->fetch();
    echo "Database connection successful. Users count: " . $result['count'];
} catch(PDOException $e) {
    echo "Database connection failed: " . $e->getMessage();
}
?>
```

**ASP.NET Test:**
```csharp
using (MySqlConnection conn = new MySqlConnection(connStr))
{
    conn.Open();
    Console.WriteLine("Database connection successful");
}
```

### 2. Admin Panel Test

1. **Access URL:** `http://localhost/job%20pilot/Admin/`
2. **Login Credentials:**
   - Username: `admin`
   - Password: `admin123`
3. **Verify Dashboard loads**
4. **Test CRUD operations**

### 3. User Side Test

1. **Access URL:** `http://localhost:55072/USER/default.aspx`
2. **Verify home page loads**
3. **Test job search functionality**
4. **Test user registration/login**

## ðŸ”§ Troubleshooting

### Common Issues

1. **Database Connection Failed**
   - Verify MySQL service is running
   - Check connection string parameters
   - Ensure database exists

2. **PHP Errors**
   - Check error logs in web server
   - Enable error reporting in config.php
   - Verify PHP extensions (PDO, MySQL)

3. **ASP.NET Build Errors**
   - Restore NuGet packages
   - Check .NET Framework version
   - Verify MySQL.Data package

4. **File Upload Issues**
   - Check folder permissions
   - Verify upload_max_filesize in PHP
   - Check .htaccess configuration

### Error Logs

**Apache Error Log:**
- Windows: `C:\xampp\apache\logs\error.log`
- Linux: `/var/log/apache2/error.log`

**PHP Error Log:**
- Check php.ini error_log setting
- Or use `error_log()` function

**ASP.NET Error Log:**
- Visual Studio Output window
- Event Viewer (Windows)

## ðŸ“ž Support

For additional support:
1. Check the main README.md file
2. Review error logs
3. Verify configuration settings
4. Test with minimal configuration first

---

**Configuration Complete!** ðŸŽ‰

Your Career Connect system should now be fully configured and ready to use. 
