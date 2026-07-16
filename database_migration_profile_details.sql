-- Career Connect: structured profile skills, education, work experience
-- Run once in phpMyAdmin or MySQL CLI after backup.

USE careerconnect;

CREATE TABLE IF NOT EXISTS user_skills (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    skill_name VARCHAR(200) NOT NULL,
    experience_years DECIMAL(5,1) NOT NULL DEFAULT 0,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_user_skills_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    UNIQUE KEY uq_user_skill_name (user_id, skill_name(100))
);

CREATE TABLE IF NOT EXISTS user_education (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    institution VARCHAR(255) NOT NULL DEFAULT '',
    degree VARCHAR(200) DEFAULT '',
    field_of_study VARCHAR(200) DEFAULT '',
    start_year INT NULL,
    end_year INT NULL,
    grade VARCHAR(80) NULL,
    details TEXT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_user_education_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS user_work_experience (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    company_name VARCHAR(255) NOT NULL DEFAULT '',
    job_title VARCHAR(200) NOT NULL DEFAULT '',
    start_date VARCHAR(32) NULL,
    end_date VARCHAR(32) NULL,
    is_current TINYINT(1) NOT NULL DEFAULT 0,
    description TEXT NULL,
    sort_order INT NOT NULL DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT fk_user_work_user FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

CREATE INDEX idx_user_skills_user ON user_skills(user_id);
CREATE INDEX idx_user_education_user ON user_education(user_id);
CREATE INDEX idx_user_work_user ON user_work_experience(user_id);
