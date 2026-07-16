# Career Connect - Complete File Transfer List

This document lists every file you need to transfer, organized by phase.

## ðŸ“¦ Phase 1: Foundation Files (Transfer First)

### Root Configuration Files
```
Web.config
Web.Debug.config
Web.Release.config
packages.config
CareerConnect.csproj
CareerConnect.csproj.user
Global.asax
Global.asax.cs
```

### Properties Folder
```
Properties/AssemblyInfo.cs
```

### Database
```
database_setup.sql
```

### Models Folder (if exists)
```
Models/
  (all files in this folder)
```

---

## ðŸŽ¨ Phase 2: Static Assets (Transfer Second)

### Assets Folder Structure
```
assets/
  css/
    animate.min.css
    bootstrap.min.css
    flaticon.css
    fontawesome-all.min.css
    magnific-popup.css
    nice-select.css
    owl.carousel.min.css
    price_rangs.css
    responsive.css
    slick.css
    slicknav.css
    style.css
    themify-icons.css
  
  js/
    animated.headline.js
    bootstrap.min.js
    contact.js
    jquery.ajaxchimp.min.js
    jquery.form.js
    jquery.magnific-popup.js
    jquery.nice-select.min.js
    jquery.paroller.min.js
    jquery.scrollUp.min.js
    jquery.slicknav.min.js
    jquery.sticky.js
    jquery.validate.min.js
    mail-script.js
    main.js
    one-page-nav-min.js
    owl.carousel.min.js
    plugins.js
    popper.min.js
    price_rangs.js
    slick.min.js
    vendor/
      jquery-1.12.4.min.js
      modernizr-3.5.0.min.js
    wow.min.js
  
  img/
    (all image files - maintain folder structure)
    favicon.png
    favicon.ico
    logo/
    about/
    adapt_icon/
    banner/
    blog/
    comment/
    elements/
    gallery/
    hero/
    icon/
    offers/
    our_blog/
    post/
    prising/
    service/
    team/
    testmonial/
  
  fonts/
    (all font files)
    fa-brands-400.*
    fa-regular-400.*
    fa-solid-900.*
    Flaticon.*
    themify.*
    _flaticon.scss
  
  scss/
    (all SCSS files - optional, for source)
    style.scss
    _*.scss (all partial files)
```

---

## ðŸ—ï¸ Phase 3: Core Infrastructure (Transfer Third)

### Master Page Files
```
USER/
  usermaster.Master
  usermaster.Master.cs
  usermaster.Master.designer.cs
```

---

## ðŸ  Phase 4: Root Page (Transfer Fourth)

### Root Default Page
```
default.aspx
default.aspx.cs
default.aspx.designer.cs
```

---

## ðŸ‘¤ Phase 5: User Pages (Transfer One by One)

### About Page
```
USER/
  about.aspx
  about.aspx.cs
  about.aspx.designer.cs
```

### Contact Page
```
USER/
  Contact.aspx
  Contact.aspx.cs
  Contact.aspx.designer.cs
```

### Register Page
```
USER/
  Register.aspx
  Register.aspx.cs
  Register.aspx.designer.cs
```

### Login Page
```
USER/
  Login.aspx
  Login.aspx.cs
  Login.aspx.designer.cs
```

### User Default Page
```
USER/
  default.aspx
  default.aspx.cs
  default.aspx.designer.cs
```

### Job Listing Page
```
USER/
  job listing.aspx
  job listing.aspx.cs
  job listing.aspx.designer.cs
```

### Job Details Page
```
USER/
  job details.aspx
  job details.aspx.cs
  job details.aspx.designer.cs
```

### Profile Page
```
USER/
  Profile.aspx
  Profile.aspx.cs
  Profile.aspx.designer.cs
```

---

## ðŸ”§ Phase 6: Admin Dashboard (Transfer Last)

### Admin Includes
```
Admin/
  includes/
    config.php
    auth.php
    header.php
    footer.php
```

### Admin Pages
```
Admin/
  index.php
  dashboard.php
  users.php
  jobs.php
  companies.php
  categories.php
  applications.php
  resumes.php
  logout.php
  add_job.php (if exists)
  edit_job.php (if exists)
  test_db.php (if exists)
  test_session.php (if exists)
  setup_sample_data.php (if exists)
```

### Admin Uploads
```
Admin/
  uploads/
    (all uploaded files, if any)
```

### Admin Subfolder (if exists)
```
Admin/
  Admin/
    (all files in this subfolder)
```

---

## ðŸ“ Complete Folder Structure

After complete transfer, your project should have this structure:

```
career-connect/
â”œâ”€â”€ Admin/
â”‚   â”œâ”€â”€ includes/
â”‚   â”‚   â”œâ”€â”€ config.php
â”‚   â”‚   â”œâ”€â”€ auth.php
â”‚   â”‚   â”œâ”€â”€ header.php
â”‚   â”‚   â””â”€â”€ footer.php
â”‚   â”œâ”€â”€ uploads/
â”‚   â”œâ”€â”€ index.php
â”‚   â”œâ”€â”€ dashboard.php
â”‚   â”œâ”€â”€ users.php
â”‚   â”œâ”€â”€ jobs.php
â”‚   â”œâ”€â”€ companies.php
â”‚   â”œâ”€â”€ categories.php
â”‚   â”œâ”€â”€ applications.php
â”‚   â”œâ”€â”€ resumes.php
â”‚   â””â”€â”€ logout.php
â”‚
â”œâ”€â”€ USER/
â”‚   â”œâ”€â”€ usermaster.Master
â”‚   â”œâ”€â”€ usermaster.Master.cs
â”‚   â”œâ”€â”€ usermaster.Master.designer.cs
â”‚   â”œâ”€â”€ about.aspx
â”‚   â”œâ”€â”€ about.aspx.cs
â”‚   â”œâ”€â”€ about.aspx.designer.cs
â”‚   â”œâ”€â”€ Contact.aspx
â”‚   â”œâ”€â”€ Contact.aspx.cs
â”‚   â”œâ”€â”€ Contact.aspx.designer.cs
â”‚   â”œâ”€â”€ default.aspx
â”‚   â”œâ”€â”€ default.aspx.cs
â”‚   â”œâ”€â”€ default.aspx.designer.cs
â”‚   â”œâ”€â”€ job listing.aspx
â”‚   â”œâ”€â”€ job listing.aspx.cs
â”‚   â”œâ”€â”€ job listing.aspx.designer.cs
â”‚   â”œâ”€â”€ job details.aspx
â”‚   â”œâ”€â”€ job details.aspx.cs
â”‚   â”œâ”€â”€ job details.aspx.designer.cs
â”‚   â”œâ”€â”€ Login.aspx
â”‚   â”œâ”€â”€ Login.aspx.cs
â”‚   â”œâ”€â”€ Login.aspx.designer.cs
â”‚   â”œâ”€â”€ Profile.aspx
â”‚   â”œâ”€â”€ Profile.aspx.cs
â”‚   â”œâ”€â”€ Profile.aspx.designer.cs
â”‚   â”œâ”€â”€ Register.aspx
â”‚   â”œâ”€â”€ Register.aspx.cs
â”‚   â””â”€â”€ Register.aspx.designer.cs
â”‚
â”œâ”€â”€ assets/
â”‚   â”œâ”€â”€ css/
â”‚   â”œâ”€â”€ js/
â”‚   â”œâ”€â”€ img/
â”‚   â”œâ”€â”€ fonts/
â”‚   â””â”€â”€ scss/
â”‚
â”œâ”€â”€ Models/
â”‚   â””â”€â”€ (model files, if any)
â”‚
â”œâ”€â”€ Properties/
â”‚   â””â”€â”€ AssemblyInfo.cs
â”‚
â”œâ”€â”€ Web.config
â”œâ”€â”€ Web.Debug.config
â”œâ”€â”€ Web.Release.config
â”œâ”€â”€ packages.config
â”œâ”€â”€ CareerConnect.csproj
â”œâ”€â”€ CareerConnect.csproj.user
â”œâ”€â”€ Global.asax
â”œâ”€â”€ Global.asax.cs
â”œâ”€â”€ default.aspx
â”œâ”€â”€ default.aspx.cs
â”œâ”€â”€ default.aspx.designer.cs
â””â”€â”€ database_setup.sql
```

---

## âš ï¸ Files NOT to Transfer

These files will be regenerated automatically:

```
bin/                    (Build output - will be regenerated)
obj/                    (Build output - will be regenerated)
*.user                  (User-specific settings - optional)
.vs/                    (Visual Studio cache - optional)
packages/               (NuGet packages - will be restored)
```

**Note:** You can transfer these, but they're not necessary and may cause issues.

---

## ðŸ“‹ Transfer Methods

### Method 1: Manual Copy (Recommended for First Time)
- Copy files one phase at a time
- Test after each phase
- Follow the migration guide

### Method 2: Full Copy (Quick but Risky)
- Copy entire project folder
- Update configuration files
- Restore NuGet packages
- Build and test

### Method 3: Version Control (Best Practice)
- Use Git to track changes
- Clone repository on new PC
- Update configuration
- Build and test

---

## ðŸ” Verification

After transferring files, run:
- `verify_migration.bat` (Windows batch script)
- Or check manually using the checklist

---

## ðŸ’¡ Tips

1. **Maintain folder structure** - Keep the exact same folder structure
2. **Preserve file names** - Don't rename files (especially .aspx files with spaces)
3. **Check file paths** - Verify all file paths are correct
4. **Test incrementally** - Test after each phase
5. **Keep backups** - Always keep backups of original files

---

**Use this list as a reference when transferring files!** ðŸ“¦





