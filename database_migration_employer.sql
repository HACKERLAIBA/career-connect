-- Run once on existing careerconnect database (phpMyAdmin → SQL)
USE careerconnect;

-- Link employers to their company
ALTER TABLE users
  ADD COLUMN company_id INT NULL DEFAULT NULL AFTER role,
  ADD CONSTRAINT fk_users_company FOREIGN KEY (company_id) REFERENCES companies(id) ON DELETE SET NULL;

-- Sample employer account → TechCorp (id 1)
UPDATE users SET company_id = 1 WHERE username = 'employer1' AND role = 'employer';
