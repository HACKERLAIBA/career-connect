-- Marketing email prefs, OTP verification, weekly digest bookkeeping
-- Run once in MySQL for database `careerconnect`.

ALTER TABLE users ADD COLUMN notify_job_suggestions TINYINT(1) NOT NULL DEFAULT 1;
ALTER TABLE users ADD COLUMN notify_weekly_digest TINYINT(1) NOT NULL DEFAULT 1;
ALTER TABLE users ADD COLUMN email_prefs_token VARCHAR(64) NULL DEFAULT NULL;
ALTER TABLE users ADD COLUMN email_verify_otp_hash VARCHAR(128) NULL DEFAULT NULL;
ALTER TABLE users ADD COLUMN email_verify_otp_expires DATETIME NULL DEFAULT NULL;

UPDATE users SET email_prefs_token = REPLACE(UUID(), '-', '') WHERE email_prefs_token IS NULL OR email_prefs_token = '';

CREATE TABLE IF NOT EXISTS cc_email_cron (
  id INT NOT NULL PRIMARY KEY,
  last_weekly_digest_at DATETIME NULL
);

INSERT IGNORE INTO cc_email_cron (id, last_weekly_digest_at) VALUES (1, NULL);

CREATE TABLE IF NOT EXISTS job_suggestion_email_log (
  user_id INT NOT NULL,
  job_id INT NOT NULL,
  sent_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (user_id, job_id)
);
