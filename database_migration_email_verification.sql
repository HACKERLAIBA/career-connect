-- Email verification + block login until verified (job seekers & employers)
USE careerconnect;

ALTER TABLE users
  ADD COLUMN email_verified TINYINT(1) NOT NULL DEFAULT 0 AFTER email,
  ADD COLUMN email_verification_token VARCHAR(128) NULL DEFAULT NULL,
  ADD COLUMN email_verification_expires DATETIME NULL DEFAULT NULL;

-- Mark existing accounts as verified so they are not locked out (run once after deploy)
UPDATE users SET email_verified = 1;
