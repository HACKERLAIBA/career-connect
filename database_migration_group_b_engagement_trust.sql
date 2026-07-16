-- =============================================================================
-- Career Connect — Group B (Engagement & trust)
--
-- Adds:
--  - saved_jobs (bookmarks)
--  - company verification badge (companies.is_verified)
--  - reports/flags (reports table)
--  - job alerts (job_alerts) [optional features can ignore it]
--
-- Run once on existing DB (phpMyAdmin → SQL). Backup first.
-- =============================================================================

USE careerconnect;

-- -----------------------------------------------------------------------------
-- 1) Saved jobs / bookmarks
-- -----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS saved_jobs (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  job_id INT NOT NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_saved_jobs_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  CONSTRAINT fk_saved_jobs_job FOREIGN KEY (job_id) REFERENCES jobs(id) ON DELETE CASCADE,
  UNIQUE KEY uq_saved_jobs_user_job (user_id, job_id)
);

CREATE INDEX idx_saved_jobs_user ON saved_jobs(user_id);
CREATE INDEX idx_saved_jobs_job ON saved_jobs(job_id);

-- -----------------------------------------------------------------------------
-- 2) Verified employer badge (company-level)
-- -----------------------------------------------------------------------------

ALTER TABLE companies
  ADD COLUMN is_verified TINYINT(1) NOT NULL DEFAULT 0;

CREATE INDEX idx_companies_is_verified ON companies(is_verified);

-- -----------------------------------------------------------------------------
-- 3) Report / flag job or employer + admin review
-- -----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS reports (
  id INT AUTO_INCREMENT PRIMARY KEY,
  reporter_user_id INT NULL,
  job_id INT NULL,
  company_id INT NULL,
  reason VARCHAR(50) NOT NULL DEFAULT 'other',
  details TEXT NULL,
  status ENUM('open','reviewed','actioned','dismissed') NOT NULL DEFAULT 'open',
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  reviewed_by INT NULL,
  reviewed_at TIMESTAMP NULL,
  admin_notes TEXT NULL,
  CONSTRAINT fk_reports_reporter FOREIGN KEY (reporter_user_id) REFERENCES users(id) ON DELETE SET NULL,
  CONSTRAINT fk_reports_job FOREIGN KEY (job_id) REFERENCES jobs(id) ON DELETE SET NULL,
  CONSTRAINT fk_reports_company FOREIGN KEY (company_id) REFERENCES companies(id) ON DELETE SET NULL,
  CONSTRAINT fk_reports_reviewed_by FOREIGN KEY (reviewed_by) REFERENCES users(id) ON DELETE SET NULL
);

CREATE INDEX idx_reports_status ON reports(status);
CREATE INDEX idx_reports_created_at ON reports(created_at);
CREATE INDEX idx_reports_job ON reports(job_id);
CREATE INDEX idx_reports_company ON reports(company_id);

-- -----------------------------------------------------------------------------
-- 4) Job alerts (optional email/notifications)
-- -----------------------------------------------------------------------------

CREATE TABLE IF NOT EXISTS job_alerts (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL,
  name VARCHAR(100) NOT NULL DEFAULT 'My alert',
  keyword VARCHAR(200) NULL,
  location VARCHAR(120) NULL,
  category_id INT NULL,
  job_type VARCHAR(30) NULL,
  salary_min DECIMAL(10,2) NULL,
  salary_max DECIMAL(10,2) NULL,
  work_arrangement VARCHAR(20) NULL,
  is_active TINYINT(1) NOT NULL DEFAULT 1,
  frequency ENUM('instant','daily','weekly') NOT NULL DEFAULT 'daily',
  last_sent_at TIMESTAMP NULL,
  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_job_alerts_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  CONSTRAINT fk_job_alerts_category FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE SET NULL
);

CREATE INDEX idx_job_alerts_user ON job_alerts(user_id);
CREATE INDEX idx_job_alerts_active ON job_alerts(is_active);

-- =============================================================================
-- End
-- =============================================================================

