# Career Connect — project handoff notes

Last updated from assistant session. Use this when continuing work.

## Paths

- **Development:** `d:\final year project\career-connect`
- **XAMPP mirror:** `C:\xampp\htdocs\career-connect`
- **Sync:** run `tools\sync_to_xampp.ps1` after changes so htdocs matches the dev folder.

## Group A / core portal- Job listing: search, filters, pagination.
- Job details: richer fields, duplicate-apply handling, company public link.
- Employer: application status updates; job add/edit includes `work_arrangement`, `how_to_apply`, `application_deadline`.
- PHP admin: `Admin/add_job.php` and `Admin/edit_job.php` — `edit_job.php` `bind_param` type string must be **`siissssssssssi`** (14 placeholders).
- **DB migration** for new job columns: `database_migration_group_a_core_portal.sql` (adds `work_arrangement`, `how_to_apply`; `application_deadline` assumed in base schema).

## UI / contrast

- **`assets/css/site-theme.css`:** footer text, forms, tables, badges, mobile (Slicknav) hovers, coloured **card headers** (headings forced light on `bg-primary` / success / info / warning / danger).
- **`USER/Profile.aspx`:** `.cv-skills-header` — light text/icons on dark gradient for the Skills block.
- Master pages cache-bust CSS: **`?v=cc11`** on stylesheet links in `usermaster.Master` and `EmployerMaster.Master`.
- **`Admin/includes/header.php`:** extra contrast rules for main content (tables, forms, cards).

## Database (no single full dump in repo)

**Fresh install (order):**

1. `database_setup.sql` (drops/recreates core tables + sample data — only for empty/fresh DB).
2. `database_migration_combined_1_and_2.sql` (profile tables + email verification).
3. `database_migration_group_a_core_portal.sql`.
4. Optional: `database/contact.sql` (contact messages table), `database_seed_categories.sql`.

**Existing DB with data:** do not re-run `database_setup.sql`; apply only missing migrations (same files as above, skip what’s already applied).

## Quick reminders

- If PHP `bind_param` errors: type string length must equal the number of `?` placeholders.
- After theme/CSS changes, bump `?v=` on masters or hard-refresh the browser.
