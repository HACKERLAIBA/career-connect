-- Group A: core job portal fields (run once in phpMyAdmin on `careerconnect`)
-- Safe: if a column already exists, skip that line or comment it out after the first error.

USE careerconnect;

-- Work arrangement: onsite | remote | hybrid
ALTER TABLE jobs ADD COLUMN work_arrangement VARCHAR(20) NOT NULL DEFAULT 'onsite';

-- Optional instructions shown on job details (e.g. "Apply via email with CV")
ALTER TABLE jobs ADD COLUMN how_to_apply TEXT NULL;

-- Note: application_deadline already exists on standard schema; employers can set it from portal after this migration.
