# Career Connect - Quick Start Guide

Get your Career Connect system up and running in 10 minutes!

## âš¡ Quick Setup (10 minutes)

### 1. Database Setup (2 minutes)

1. **Open phpMyAdmin** in your browser
2. **Create database**: Click "New" â†’ Enter "careerconnect" â†’ Click "Create"
3. **Import schema**: Select "careerconnect" â†’ Click "Import" â†’ Choose `database_setup.sql` â†’ Click "Go"

### 2. ASP.NET Setup (3 minutes)

1. **Open Visual Studio**
2. **Open project**: File â†’ Open â†’ Project/Solution â†’ Select `CareerConnect.csproj`
3. **Restore packages**: Right-click solution â†’ "Restore NuGet Packages"
4. **Build**: Ctrl+Shift+B
5. **Run**: F5

### 3. PHP Admin Setup (3 minutes)

1. **Copy Admin folder** to your web server (e.g., `C:\xampp\htdocs\`)
2. **Start web server** (Apache + MySQL)
3. **Access admin**: `http://localhost/job%20pilot/Admin/`
4. **Login**: admin / admin123

### 4. Test Everything (2 minutes)

1. **User side**: `http://localhost:55072/USER/default.aspx`
2. **Admin side**: `http://localhost/job%20pilot/Admin/`
3. **Verify both work correctly**

## ðŸ” Default Credentials

| System | Username | Password |
|--------|----------|----------|
| Admin Panel | admin | admin123 |
| Sample User | john@example.com | password |

## ðŸ“ Key Files

| File | Purpose |
|------|---------|
| `database_setup.sql` | Complete database schema |
| `Web.config` | ASP.NET configuration |
| `Admin/includes/config.php` | PHP admin configuration |
| `setup.bat` | Windows setup helper |

## ðŸš¨ Common Issues & Quick Fixes

### Database Connection Error
```xml
<!-- Check Web.config -->
<connectionStrings>
    <add name="MySqlConn" 
         connectionString="server=localhost;user id=root;password=;database=careerconnect;" 
         providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

### PHP Admin Not Loading
- Verify Apache is running
- Check file permissions
- Ensure PHP is enabled

### ASP.NET Build Errors
- Restore NuGet packages
- Check .NET Framework 4.7.2
- Verify MySQL.Data package

## ðŸ“ž Need Help?

1. **Check logs**: Look for error messages
2. **Verify configs**: Database connection strings
3. **Test step by step**: Each component separately
4. **Read full docs**: `README.md` and `CONFIGURATION_GUIDE.md`

## âœ… Success Checklist

- [ ] Database created and imported
- [ ] ASP.NET project builds successfully
- [ ] User side loads at `http://localhost:55072/USER/default.aspx`
- [ ] Admin panel loads at `http://localhost/job%20pilot/Admin/`
- [ ] Can login to admin panel
- [ ] Can view jobs on user side
- [ ] Database shows sample data

## ðŸŽ‰ You're Ready!

Your Career Connect system is now running! 

**Next Steps:**
1. Customize the design
2. Add more jobs and users
3. Configure email notifications
4. Set up production deployment

---

**Happy Job Hunting!** ðŸš€ 
