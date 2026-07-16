-- Group C: messaging between job seeker and employer + admin moderation
-- Run on MySQL database careerconnect (same as Web.config).

SET NAMES utf8mb4;

CREATE TABLE IF NOT EXISTS message_threads (
  id INT AUTO_INCREMENT PRIMARY KEY,
  user_id INT NOT NULL COMMENT 'job seeker (users.id, role=user)',
  company_id INT NOT NULL,
  job_id INT NULL,
  subject VARCHAR(255) NOT NULL DEFAULT '',
  status ENUM('open','closed','flagged') NOT NULL DEFAULT 'open',
  admin_reviewed TINYINT(1) NOT NULL DEFAULT 0,
  admin_notes VARCHAR(2000) NULL,
  reviewed_by INT NULL,
  reviewed_at DATETIME NULL,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  updated_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  UNIQUE KEY uq_seeker_company (user_id, company_id),
  KEY idx_company (company_id),
  KEY idx_status (status),
  CONSTRAINT fk_msgt_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
  CONSTRAINT fk_msgt_company FOREIGN KEY (company_id) REFERENCES companies(id) ON DELETE CASCADE,
  CONSTRAINT fk_msgt_job FOREIGN KEY (job_id) REFERENCES jobs(id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS message_posts (
  id INT AUTO_INCREMENT PRIMARY KEY,
  thread_id INT NOT NULL,
  sender_role ENUM('seeker','employer','admin') NOT NULL,
  sender_user_id INT NULL,
  body TEXT NOT NULL,
  is_hidden TINYINT(1) NOT NULL DEFAULT 0,
  admin_flagged TINYINT(1) NOT NULL DEFAULT 0,
  created_at DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  KEY idx_thread (thread_id),
  KEY idx_created (created_at),
  CONSTRAINT fk_msgp_thread FOREIGN KEY (thread_id) REFERENCES message_threads(id) ON DELETE CASCADE,
  CONSTRAINT fk_msgp_user FOREIGN KEY (sender_user_id) REFERENCES users(id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
