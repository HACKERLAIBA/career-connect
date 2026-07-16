# Career Connect - Quick Migration Guide

## ðŸš€ Quick Start (5 Minutes)

### Step 1: Prepare New PC
1. Install **XAMPP** (MySQL + Apache)
2. Install **Visual Studio** (2019/2022)
3. Install **.NET Framework 4.7.2+**

### Step 2: Transfer Foundation Files
Copy these files first:
- `Web.config`
- `packages.config`
- `CareerConnect.csproj`
- `Global.asax` + `Global.asax.cs`
- `database_setup.sql`

### Step 3: Setup Database
1. Start XAMPP MySQL
2. Open phpMyAdmin
3. Import `database_setup.sql`
4. Update connection string in `Web.config`

### Step 4: Transfer Assets
Copy entire `assets/` folder

### Step 5: Transfer Core Files
Copy:
- `USER/usermaster.Master` + related files
- `Properties/AssemblyInfo.cs`

### Step 6: Open in Visual Studio
1. Open `CareerConnect.csproj`
2. Restore NuGet packages
3. Build project

### Step 7: Transfer Pages (One by One)
Transfer and test each page:
1. Root `default.aspx`
2. `USER/about.aspx`
3. `USER/Contact.aspx`
4. `USER/Register.aspx`
5. `USER/Login.aspx`
6. `USER/default.aspx`
7. `USER/job listing.aspx`
8. `USER/job details.aspx`
9. `USER/Profile.aspx`

### Step 8: Transfer Admin
1. Copy `Admin/includes/` folder
2. Update `config.php` with database credentials
3. Copy admin pages one by one
4. Test admin login

### Step 9: Final Test
- Build solution
- Test all pages
- Test database operations
- Test admin panel

---

## ðŸ“‹ Transfer Order Summary

```
1. Foundation Files (config, project files, database)
   â†“
2. Assets (CSS, JS, images, fonts)
   â†“
3. Core Infrastructure (master page, models)
   â†“
4. Root Page (default.aspx)
   â†“
5. User Pages (one by one)
   â†“
6. Admin Dashboard
   â†“
7. Build & Test
```

---

## ðŸ”§ Essential Configuration

### Web.config Connection String
```xml
<connectionStrings>
    <add name="MySqlConn" 
         connectionString="server=localhost;user id=root;password=;database=careerconnect;" 
         providerName="MySql.Data.MySqlClient" />
</connectionStrings>
```

### Admin config.php
```php
$host = 'localhost';
$dbname = 'careerconnect';
$username = 'root';
$password = '';
```

---

## âœ… Verification Checklist

- [ ] Project opens in Visual Studio
- [ ] NuGet packages restored
- [ ] Project builds successfully
- [ ] Database connection works
- [ ] Homepage loads
- [ ] User registration works
- [ ] User login works
- [ ] Job listings display
- [ ] Admin login works
- [ ] Admin panel functions

---

## ðŸ› Quick Troubleshooting

### Build Errors
â†’ Restore NuGet packages
â†’ Clean and rebuild solution

### Database Errors
â†’ Check MySQL is running
â†’ Verify connection string
â†’ Check database exists

### Missing Files
â†’ Run `verify_migration.bat` or `verify_migration.ps1`
â†’ Check FILE_TRANSFER_LIST.md

### PHP Admin Errors
â†’ Check Apache is running
â†’ Verify config.php credentials
â†’ Check file permissions

---

## ðŸ“š Detailed Guides

For more details, see:
- **MIGRATION_GUIDE.md** - Complete step-by-step guide
- **MIGRATION_CHECKLIST.md** - Printable checklist
- **FILE_TRANSFER_LIST.md** - Complete file list

---

## ðŸ’¡ Pro Tips

1. **Test after each phase** - Don't wait until the end
2. **Keep backups** - Always backup your work
3. **One page at a time** - Transfer and test incrementally
4. **Document issues** - Write down any errors
5. **Verify database** - Check database after each step

---

**Good luck! Follow the detailed guides for step-by-step instructions.** ðŸŽ‰





