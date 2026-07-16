# Career Connect - Migration Checklist

## âœ… Quick Reference Checklist

### Phase 1: Foundation (Do First!)
- [ ] Install XAMPP on new PC
- [ ] Install Visual Studio on new PC
- [ ] Install .NET Framework 4.7.2+
- [ ] Copy `Web.config`
- [ ] Copy `packages.config`
- [ ] Copy `CareerConnect.csproj`
- [ ] Copy `Global.asax` and `Global.asax.cs`
- [ ] Copy `database_setup.sql`
- [ ] Import database in phpMyAdmin
- [ ] Update connection strings in `Web.config`
- [ ] Open project in Visual Studio
- [ ] Restore NuGet packages
- [ ] **VERIFY:** Project opens without errors

### Phase 2: Assets (Do Second)
- [ ] Copy `assets/css/` folder
- [ ] Copy `assets/js/` folder
- [ ] Copy `assets/img/` folder
- [ ] Copy `assets/fonts/` folder
- [ ] **VERIFY:** All asset folders exist

### Phase 3: Core Infrastructure (Do Third)
- [ ] Copy `USER/usermaster.Master` and related files
- [ ] Copy `Models/` folder (if exists)
- [ ] Build project
- [ ] **VERIFY:** Build succeeds

### Phase 4: Root Page
- [ ] Copy `default.aspx` and related files
- [ ] Build project
- [ ] Test homepage
- [ ] **VERIFY:** Homepage loads correctly

### Phase 5: User Pages (One by One)

#### About Page
- [ ] Copy `USER/about.aspx` and related files
- [ ] Build and test
- [ ] **VERIFY:** About page loads

#### Contact Page
- [ ] Copy `USER/Contact.aspx` and related files
- [ ] Build and test
- [ ] **VERIFY:** Contact page loads

#### Register Page
- [ ] Copy `USER/Register.aspx` and related files
- [ ] Build and test
- [ ] Test user registration
- [ ] **VERIFY:** User can register

#### Login Page
- [ ] Copy `USER/Login.aspx` and related files
- [ ] Build and test
- [ ] Test user login
- [ ] **VERIFY:** User can login

#### User Default Page
- [ ] Copy `USER/default.aspx` and related files
- [ ] Build and test
- [ ] **VERIFY:** User home page loads

#### Job Listing Page
- [ ] Copy `USER/job listing.aspx` and related files
- [ ] Build and test
- [ ] Test job search
- [ ] **VERIFY:** Jobs display correctly

#### Job Details Page
- [ ] Copy `USER/job details.aspx` and related files
- [ ] Build and test
- [ ] Test job details view
- [ ] **VERIFY:** Job details display

#### Profile Page
- [ ] Copy `USER/Profile.aspx` and related files
- [ ] Build and test
- [ ] Test profile view/edit
- [ ] **VERIFY:** Profile works

### Phase 6: Admin Dashboard

#### Admin Config
- [ ] Copy `Admin/includes/config.php`
- [ ] Copy `Admin/includes/auth.php`
- [ ] Copy `Admin/includes/header.php`
- [ ] Copy `Admin/includes/footer.php`
- [ ] Update database credentials in `config.php`
- [ ] **VERIFY:** No PHP errors

#### Admin Pages
- [ ] Copy `Admin/index.php`
- [ ] Copy `Admin/dashboard.php`
- [ ] Copy `Admin/users.php`
- [ ] Copy `Admin/jobs.php`
- [ ] Copy `Admin/companies.php`
- [ ] Copy `Admin/categories.php`
- [ ] Copy `Admin/applications.php`
- [ ] Copy `Admin/resumes.php`
- [ ] Copy `Admin/logout.php`
- [ ] Test admin login
- [ ] Test each admin page
- [ ] **VERIFY:** Admin panel works

#### Admin Uploads
- [ ] Create `Admin/uploads/` folder
- [ ] Copy any uploaded files
- [ ] Set folder permissions
- [ ] **VERIFY:** File uploads work

### Phase 7: Final Steps
- [ ] Clean solution
- [ ] Rebuild solution
- [ ] Fix any errors
- [ ] Test complete application
- [ ] **VERIFY:** Everything works!

---

## ðŸ§ª Testing Checklist

After migration, test these features:

### User Side
- [ ] Homepage loads
- [ ] User can register
- [ ] User can login
- [ ] User can view job listings
- [ ] User can view job details
- [ ] User can apply for jobs
- [ ] User can view/edit profile
- [ ] Contact form works
- [ ] About page loads

### Admin Side
- [ ] Admin can login
- [ ] Admin dashboard loads
- [ ] Admin can view users
- [ ] Admin can manage users
- [ ] Admin can view jobs
- [ ] Admin can manage jobs
- [ ] Admin can manage companies
- [ ] Admin can manage categories
- [ ] Admin can view applications
- [ ] Admin can view resumes
- [ ] File uploads work

### Database
- [ ] Database connection works
- [ ] Data can be read
- [ ] Data can be written
- [ ] Data can be updated
- [ ] Data can be deleted

---

## ðŸ› Common Issues Checklist

If something doesn't work, check:

- [ ] MySQL service is running
- [ ] Apache service is running (for admin)
- [ ] Database connection string is correct
- [ ] Database exists and has tables
- [ ] NuGet packages are restored
- [ ] All file paths are correct
- [ ] File permissions are set correctly
- [ ] Visual Studio can build the project
- [ ] No compilation errors
- [ ] Browser cache is cleared

---

## ðŸ“‹ File Transfer Order Summary

1. **Foundation** â†’ Config files, project files, database
2. **Assets** â†’ CSS, JS, images, fonts
3. **Core** â†’ Master page, models
4. **Root** â†’ default.aspx
5. **User Pages** â†’ About, Contact, Register, Login, etc.
6. **Admin** â†’ Config, pages, uploads
7. **Final** â†’ Build, test, fix

---

## ðŸ’¡ Pro Tips

1. **Test after each phase** - Don't wait until the end
2. **Keep backups** - Always backup before changes
3. **Document issues** - Write down any errors
4. **One page at a time** - Transfer and test each page
5. **Verify database** - Check database after each step

---

**Print this checklist and check off items as you complete them!** âœ…





