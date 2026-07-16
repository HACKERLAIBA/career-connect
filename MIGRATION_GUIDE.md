# Career Connect - Step-by-Step Migration Guide

This guide will help you transfer your Career Connect project to another PC in parts, ensuring everything works at each step.

## ðŸ“‹ Pre-Migration Checklist

### On the NEW PC, install these first:
- [ ] **XAMPP** (or WAMP) - for MySQL and Apache
- [ ] **Visual Studio 2019/2022** - for ASP.NET development
- [ ] **.NET Framework 4.7.2** or higher
- [ ] **MySQL Connector** (usually comes with XAMPP)
- [ ] **IIS Express** (comes with Visual Studio)

---

## ðŸ—‚ï¸ Transfer Order (Critical - Follow This Order!)

### PHASE 1: Foundation Files (Transfer First)

These files are essential for the project to build and run.

#### Step 1.1: Core Configuration Files
**Files to transfer:**
- `Web.config`
- `Web.Debug.config`
- `Web.Release.config`
- `packages.config`
- `CareerConnect.csproj` (project file name can be renamed later if you want)
- `CareerConnect.csproj.user`
- `Global.asax`
- `Global.asax.cs`
- `Properties/AssemblyInfo.cs`

**Action on new PC:**
1. Create project folder: `C:\xampp\htdocs\career-connect\`
2. Copy all files above to the new PC
3. Open `CareerConnect.csproj` in Visual Studio
4. Let Visual Studio restore NuGet packages (right-click solution â†’ Restore NuGet Packages)

**Verify:** Project should open without errors (may show missing file warnings - that's OK)

---

#### Step 1.2: Database Setup
**Files to transfer:**
- `database_setup.sql`

**Action on new PC:**
1. Start XAMPP MySQL service
2. Open phpMyAdmin (http://localhost/phpmyadmin)
3. Import `database_setup.sql`
4. Verify database `careerconnect` is created with all tables

**Verify:** Check phpMyAdmin - you should see tables: users, companies, categories, jobs, applications, resumes

---

#### Step 1.3: Update Connection Strings
**File to modify:** `Web.config`

**Action on new PC:**
1. Open `Web.config`
2. Update connection string if MySQL password is different:
   ```xml
   <connectionStrings>
       <add name="MySqlConn" 
            connectionString="server=localhost;user id=root;password=YOUR_PASSWORD;database=careerconnect;" 
            providerName="MySql.Data.MySqlClient" />
   </connectionStrings>
   ```

**Verify:** Save file and check for syntax errors

---

### PHASE 2: Static Assets (Transfer Second)

These files don't depend on code but are needed for pages to display correctly.

#### Step 2.1: Assets Folder
**Folders to transfer:**
- `assets/css/` (all CSS files)
- `assets/js/` (all JavaScript files)
- `assets/img/` (all images)
- `assets/fonts/` (all font files)
- `assets/scss/` (SCSS source files - optional)

**Action on new PC:**
1. Create `assets` folder in project root
2. Copy entire `assets` folder structure
3. Maintain folder structure exactly

**Verify:** Check that all folders exist: `assets/css/`, `assets/js/`, `assets/img/`, `assets/fonts/`

---

### PHASE 3: Core Infrastructure (Transfer Third)

#### Step 3.1: Master Page
**Files to transfer:**
- `USER/usermaster.Master`
- `USER/usermaster.Master.cs`
- `USER/usermaster.Master.designer.cs`

**Action on new PC:**
1. Create `USER` folder
2. Copy master page files
3. Build project (should compile master page)

**Verify:** Build should succeed (may have warnings about missing pages - that's OK)

---

#### Step 3.2: Models Folder (if exists)
**Files to transfer:**
- `Models/` folder (all files)

**Action on new PC:**
1. Create `Models` folder
2. Copy all model files
3. Build project

**Verify:** Build should succeed

---

### PHASE 4: Root Pages (Transfer Fourth)

#### Step 4.1: Default.aspx (Root)
**Files to transfer:**
- `default.aspx`
- `default.aspx.cs`
- `default.aspx.designer.cs`

**Action on new PC:**
1. Copy files to project root
2. Build project
3. Test: Run project and verify homepage loads

**Verify:** 
- Build succeeds
- Homepage displays correctly
- No database errors (if page uses database)

---

### PHASE 5: User Pages (Transfer One by One)

Transfer pages in this order to minimize dependencies:

#### Step 5.1: About Page (Simplest)
**Files to transfer:**
- `USER/about.aspx`
- `USER/about.aspx.cs`
- `USER/about.aspx.designer.cs`

**Action on new PC:**
1. Copy files to `USER` folder
2. Build project
3. Test: Navigate to `http://localhost:PORT/USER/about.aspx`

**Verify:** 
- Page loads without errors
- Layout displays correctly
- No missing resources

---

#### Step 5.2: Contact Page
**Files to transfer:**
- `USER/Contact.aspx`
- `USER/Contact.aspx.cs`
- `USER/Contact.aspx.designer.cs`

**Action on new PC:**
1. Copy files
2. Build project
3. Test: Navigate to Contact page

**Verify:** Page loads and form displays

---

#### Step 5.3: Register Page
**Files to transfer:**
- `USER/Register.aspx`
- `USER/Register.aspx.cs`
- `USER/Register.aspx.designer.cs`

**Action on new PC:**
1. Copy files
2. Build project
3. Test: 
   - Page loads
   - Form validation works
   - Can register a test user (check database)

**Verify:** 
- Registration form works
- User is created in database
- No database connection errors

---

#### Step 5.4: Login Page
**Files to transfer:**
- `USER/Login.aspx`
- `USER/Login.aspx.cs`
- `USER/Login.aspx.designer.cs`

**Action on new PC:**
1. Copy files
2. Build project
3. Test:
   - Page loads
   - Can login with registered user
   - Session is created

**Verify:** 
- Login works
- Redirects correctly after login
- Session persists

---

#### Step 5.5: User Default Page
**Files to transfer:**
- `USER/default.aspx`
- `USER/default.aspx.cs`
- `USER/default.aspx.designer.cs`

**Action on new PC:**
1. Copy files
2. Build project
3. Test: Navigate to user home page

**Verify:** Page loads with job listings

---

#### Step 5.6: Job Listing Page
**Files to transfer:**
- `USER/job listing.aspx`
- `USER/job listing.aspx.cs`
- `USER/job listing.aspx.designer.cs`

**Action on new PC:**
1. Copy files
2. Build project
3. Test:
   - Page loads
   - Job listings display
   - Search/filter works

**Verify:** 
- Jobs display from database
- Filters work
- Pagination works (if implemented)

---

#### Step 5.7: Job Details Page
**Files to transfer:**
- `USER/job details.aspx`
- `USER/job details.aspx.cs`
- `USER/job details.aspx.designer.cs`

**Action on new PC:**
1. Copy files
2. Build project
3. Test:
   - Click job from listing
   - Details page loads
   - Apply button works (if logged in)

**Verify:** 
- Job details display correctly
- Related jobs show
- Apply functionality works

---

#### Step 5.8: Profile Page
**Files to transfer:**
- `USER/Profile.aspx`
- `USER/Profile.aspx.cs`
- `USER/Profile.aspx.designer.cs`

**Action on new PC:**
1. Copy files
2. Build project
3. Test:
   - Login as user
   - Navigate to profile
   - View/edit profile

**Verify:** 
- Profile displays user data
- Can update profile
- Changes save to database

---

### PHASE 6: Admin Dashboard (Transfer Last)

#### Step 6.1: Admin Configuration
**Files to transfer:**
- `Admin/includes/config.php`
- `Admin/includes/auth.php`
- `Admin/includes/header.php`
- `Admin/includes/footer.php`

**Action on new PC:**
1. Create `Admin/includes` folder
2. Copy all include files
3. Update `config.php` with correct database credentials:
   ```php
   $host = 'localhost';
   $dbname = 'careerconnect';
   $username = 'root';
   $password = 'YOUR_PASSWORD';
   ```

**Verify:** No PHP syntax errors

---

#### Step 6.2: Admin Pages (One by One)
**Transfer order:**
1. `Admin/index.php` (login page)
2. `Admin/dashboard.php`
3. `Admin/users.php`
4. `Admin/jobs.php`
5. `Admin/companies.php`
6. `Admin/categories.php`
7. `Admin/applications.php`
8. `Admin/resumes.php`
9. `Admin/logout.php`

**Action on new PC:**
1. Copy each file one at a time
2. Test each page:
   - Login to admin panel
   - Verify each page loads
   - Test CRUD operations

**Verify:** 
- Admin login works
- Each page loads correctly
- Database operations work
- No PHP errors

---

#### Step 6.3: Admin Uploads Folder
**Files to transfer:**
- `Admin/uploads/` folder (if contains files)

**Action on new PC:**
1. Create `Admin/uploads` folder
2. Copy any existing uploaded files
3. Set folder permissions (write access for web server)

**Verify:** File uploads work in admin panel

---

### PHASE 7: Final Steps

#### Step 7.1: Build Configuration
**Action on new PC:**
1. Open project in Visual Studio
2. Clean solution (Build â†’ Clean Solution)
3. Rebuild solution (Build â†’ Rebuild Solution)
4. Fix any compilation errors

**Verify:** 
- Build succeeds with no errors
- All references resolved
- NuGet packages restored

---

#### Step 7.2: Test Complete Application
**Test checklist:**
- [ ] Homepage loads
- [ ] User registration works
- [ ] User login works
- [ ] Job listing displays jobs
- [ ] Job details page works
- [ ] User can apply for jobs
- [ ] Profile page works
- [ ] Admin login works
- [ ] Admin can manage users
- [ ] Admin can manage jobs
- [ ] Admin can manage companies
- [ ] Admin can manage categories
- [ ] Admin can view applications
- [ ] Admin can view resumes

---

#### Step 7.3: Clean Up
**Files you can delete (if not needed):**
- `bin/` folder (will be regenerated)
- `obj/` folder (will be regenerated)
- `USER.zip` (if exists)
- Any temporary files

**Note:** `bin/` and `obj/` folders are build outputs and will be regenerated when you build the project.

---

## ðŸ”§ Troubleshooting Common Issues

### Issue 1: NuGet Package Errors
**Solution:**
1. Right-click solution â†’ Restore NuGet Packages
2. If that doesn't work, delete `packages` folder (if exists in parent directory)
3. Rebuild solution

### Issue 2: Database Connection Errors
**Solution:**
1. Verify MySQL service is running in XAMPP
2. Check connection string in `Web.config`
3. Test connection in phpMyAdmin
4. Verify database `careerconnect` exists

### Issue 3: Missing DLL Errors
**Solution:**
1. Clean solution
2. Restore NuGet packages
3. Rebuild solution
4. Check `packages.config` for all required packages

### Issue 4: Page Not Found Errors
**Solution:**
1. Verify file paths are correct
2. Check `Web.config` for URL routing issues
3. Verify IIS Express is running
4. Check project properties for correct start page

### Issue 5: PHP Admin Errors
**Solution:**
1. Verify Apache is running in XAMPP
2. Check PHP error logs
3. Verify `config.php` has correct database credentials
4. Check file permissions on `Admin/uploads` folder

### Issue 6: Session Errors
**Solution:**
1. Verify session state is enabled in `Web.config`
2. Check PHP session configuration
3. Clear browser cookies
4. Restart web server

---

## ðŸ“¦ Quick Transfer Checklist

Use this checklist to track your progress:

### Foundation
- [ ] Web.config and project files
- [ ] Database setup
- [ ] Connection strings updated

### Assets
- [ ] CSS files
- [ ] JavaScript files
- [ ] Images
- [ ] Fonts

### Core
- [ ] Master page
- [ ] Models (if any)
- [ ] Global.asax

### Pages
- [ ] Root default.aspx
- [ ] About page
- [ ] Contact page
- [ ] Register page
- [ ] Login page
- [ ] User default page
- [ ] Job listing page
- [ ] Job details page
- [ ] Profile page

### Admin
- [ ] Admin config files
- [ ] Admin pages (all)
- [ ] Admin uploads folder

### Final
- [ ] Build succeeds
- [ ] All tests pass
- [ ] Application works end-to-end

---

## ðŸš€ Quick Migration Script (Optional)

If you want to transfer everything at once (not recommended for first time):

1. **Copy entire project folder** to new PC
2. **Update connection strings** in `Web.config` and `Admin/includes/config.php`
3. **Import database** using `database_setup.sql`
4. **Open project** in Visual Studio
5. **Restore NuGet packages**
6. **Build and test**

---

## ðŸ“ Notes

1. **Always test after each phase** - Don't wait until the end to test
2. **Keep backups** - Backup your work before making changes
3. **Document issues** - Write down any errors you encounter
4. **Verify database** - Check database after each database-related transfer
5. **Check file paths** - Ensure all file paths are correct for the new PC

---

## ðŸ†˜ Need Help?

If you encounter issues:
1. Check the error message carefully
2. Review the troubleshooting section above
3. Verify all prerequisites are installed
4. Check database connection
5. Review file permissions

---

**Good luck with your migration!** ðŸŽ‰

Follow this guide step by step, and you'll have your project working on the new PC in no time.





