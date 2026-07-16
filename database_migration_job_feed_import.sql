-- Career Connect — syndicated job feeds (RSS / Atom)
-- Run once in phpMyAdmin on `careerconnect`. Backup first.
-- This single script adds all syndication columns + index.
-- If you already ran an older version (only external_key / feed_url / source_label), run only:
--   ALTER TABLE jobs ADD COLUMN external_apply_url VARCHAR(1024) NULL DEFAULT NULL AFTER external_source_label;
-- Then run database_seed_job_feed_aggregator.sql so imports can resolve the aggregator company.
--
-- Legal: only import from feeds you are allowed to republish or link to (site ToS / licensing).
-- Listings are stored as jobs; candidates apply on the original site (external_apply_url).
--
USE careerconnect;

ALTER TABLE jobs
  ADD COLUMN external_key VARCHAR(64) NULL DEFAULT NULL AFTER created_by,
  ADD COLUMN external_feed_url VARCHAR(512) NULL DEFAULT NULL AFTER external_key,
  ADD COLUMN external_source_label VARCHAR(200) NULL DEFAULT NULL AFTER external_feed_url,
  ADD COLUMN external_apply_url VARCHAR(1024) NULL DEFAULT NULL AFTER external_source_label;

CREATE UNIQUE INDEX uq_jobs_external_key ON jobs (external_key);

-- Optional: aggregator company (note the id for Web.config JobFeedImport_CompanyId)
-- INSERT INTO companies (name, description, industry, email, phone, city, country, status)
-- VALUES (
--   'External job feeds',
--   'Syndicated listings from RSS/Atom feeds configured by the administrator. Candidates apply on the source site.',
--   'Aggregator',
--   'feeds@localhost',
--   '',
--   '',
--   '',
--   'active'
-- );
