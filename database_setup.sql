-- Career Connect Database Setup
-- Run this script in phpMyAdmin or MySQL command line

-- Create database if not exists
CREATE DATABASE IF NOT EXISTS careerconnect CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE careerconnect;

-- Drop existing tables if they exist
DROP TABLE IF EXISTS applications;
DROP TABLE IF EXISTS resumes;
DROP TABLE IF EXISTS jobs;
DROP TABLE IF EXISTS categories;
DROP TABLE IF EXISTS companies;
DROP TABLE IF EXISTS users;

-- Create users table
CREATE TABLE users (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password VARCHAR(255) NOT NULL,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    phone VARCHAR(20),
    address TEXT,
    city VARCHAR(50),
    state VARCHAR(50),
    country VARCHAR(50),
    zip_code VARCHAR(20),
    role ENUM('user', 'admin', 'employer') DEFAULT 'user',
    company_id INT NULL DEFAULT NULL,
    status ENUM('active', 'inactive', 'suspended') DEFAULT 'active',
    profile_picture VARCHAR(255),
    bio TEXT,
    skills TEXT,
    experience_years INT DEFAULT 0,
    education_level VARCHAR(50),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Create companies table
CREATE TABLE companies (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    industry VARCHAR(100),
    website VARCHAR(255),
    email VARCHAR(100),
    phone VARCHAR(20),
    address TEXT,
    city VARCHAR(50),
    state VARCHAR(50),
    country VARCHAR(50),
    zip_code VARCHAR(20),
    logo VARCHAR(255),
    founded_year INT,
    company_size VARCHAR(50),
    status ENUM('active', 'inactive') DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

ALTER TABLE users
  ADD CONSTRAINT fk_users_company FOREIGN KEY (company_id) REFERENCES companies(id) ON DELETE SET NULL;

-- Create categories table
CREATE TABLE categories (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT,
    icon VARCHAR(100),
    color VARCHAR(20) DEFAULT '#007bff',
    status ENUM('active', 'inactive') DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- Create jobs table
CREATE TABLE jobs (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    description TEXT NOT NULL,
    requirements TEXT,
    responsibilities TEXT,
    company_id INT NOT NULL,
    category_id INT NOT NULL,
    location VARCHAR(100),
    job_type ENUM('full-time', 'part-time', 'contract', 'internship', 'freelance') DEFAULT 'full-time',
    experience_level ENUM('entry', 'mid', 'senior', 'executive') DEFAULT 'mid',
    salary_min DECIMAL(10,2),
    salary_max DECIMAL(10,2),
    salary_currency VARCHAR(3) DEFAULT 'USD',
    benefits TEXT,
    skills_required TEXT,
    education_required VARCHAR(100),
    application_deadline DATE,
    status ENUM('active', 'inactive', 'closed') DEFAULT 'active',
    views_count INT DEFAULT 0,
    applications_count INT DEFAULT 0,
    created_by INT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (company_id) REFERENCES companies(id) ON DELETE CASCADE,
    FOREIGN KEY (category_id) REFERENCES categories(id) ON DELETE CASCADE,
    FOREIGN KEY (created_by) REFERENCES users(id) ON DELETE CASCADE
);

-- Create resumes table
CREATE TABLE resumes (
    id INT AUTO_INCREMENT PRIMARY KEY,
    user_id INT NOT NULL,
    title VARCHAR(100) NOT NULL,
    file_path VARCHAR(255),
    content TEXT,
    skills TEXT,
    experience TEXT,
    education TEXT,
    certifications TEXT,
    languages TEXT,
    is_default BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE
);

-- Create applications table
CREATE TABLE applications (
    id INT AUTO_INCREMENT PRIMARY KEY,
    job_id INT NOT NULL,
    user_id INT NOT NULL,
    resume_id INT,
    cover_letter TEXT,
    status ENUM('pending', 'reviewed', 'shortlisted', 'interviewed', 'hired', 'rejected') DEFAULT 'pending',
    applied_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    reviewed_at TIMESTAMP NULL,
    reviewed_by INT,
    notes TEXT,
    FOREIGN KEY (job_id) REFERENCES jobs(id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (resume_id) REFERENCES resumes(id) ON DELETE SET NULL,
    FOREIGN KEY (reviewed_by) REFERENCES users(id) ON DELETE SET NULL
);

-- Insert sample data

-- Sample users
INSERT INTO users (username, email, password, first_name, last_name, phone, role, status) VALUES
('admin', 'admin@careerconnect.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Admin', 'User', '+1234567890', 'admin', 'active'),
('john_doe', 'john@example.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'John', 'Doe', '+1234567891', 'user', 'active'),
('jane_smith', 'jane@example.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Jane', 'Smith', '+1234567892', 'user', 'active'),
('employer1', 'employer@techcorp.com', '$2y$10$92IXUNpkjO0rOQ5byMi.Ye4oKoEa3Ro9llC/.og/at2.uheWG/igi', 'Tech', 'Corp', '+1234567893', 'employer', 'active');

-- Sample companies
INSERT INTO companies (name, description, industry, website, email, phone, address, city, state, country, company_size) VALUES
('TechCorp Solutions', 'Leading technology solutions provider specializing in software development and digital transformation.', 'Technology', 'https://techcorp.com', 'contact@techcorp.com', '+1234567890', '123 Tech Street', 'San Francisco', 'CA', 'USA', '100-500'),
('Digital Innovations Inc', 'Innovative digital agency creating cutting-edge web and mobile applications.', 'Technology', 'https://digitalinnovations.com', 'hello@digitalinnovations.com', '+1234567891', '456 Innovation Ave', 'New York', 'NY', 'USA', '50-100'),
('Global Marketing Group', 'Full-service marketing agency helping businesses grow their digital presence.', 'Marketing', 'https://globalmarketing.com', 'info@globalmarketing.com', '+1234567892', '789 Marketing Blvd', 'Los Angeles', 'CA', 'USA', '25-50'),
('Healthcare Systems Ltd', 'Healthcare technology company focused on improving patient care through innovative solutions.', 'Healthcare', 'https://healthcaresystems.com', 'contact@healthcaresystems.com', '+1234567893', '321 Health Way', 'Boston', 'MA', 'USA', '500+');

UPDATE users SET company_id = 1 WHERE username = 'employer1' AND role = 'employer';

-- Sample categories
INSERT INTO categories (name, description, icon, color) VALUES
('Software Development', 'Jobs related to software development, programming, and coding', 'fas fa-code', '#007bff'),
('Web Development', 'Frontend and backend web development positions', 'fas fa-globe', '#28a745'),
('Data Science', 'Data analysis, machine learning, and AI positions', 'fas fa-chart-bar', '#ffc107'),
('Marketing', 'Digital marketing, content creation, and SEO positions', 'fas fa-bullhorn', '#dc3545'),
('Design', 'UI/UX design, graphic design, and creative positions', 'fas fa-palette', '#6f42c1'),
('Sales', 'Sales, business development, and customer success positions', 'fas fa-handshake', '#fd7e14'),
('Healthcare', 'Medical, nursing, and healthcare administration positions', 'fas fa-heartbeat', '#e83e8c'),
('Finance', 'Accounting, banking, and financial services positions', 'fas fa-dollar-sign', '#20c997');

-- Sample jobs
INSERT INTO jobs (title, description, requirements, responsibilities, company_id, category_id, location, job_type, experience_level, salary_min, salary_max, skills_required, status, created_by) VALUES
('Senior Software Engineer', 'We are looking for an experienced software engineer to join our development team.', '5+ years of experience in software development, proficiency in Java, Python, or JavaScript', 'Develop and maintain software applications, collaborate with cross-functional teams', 1, 1, 'San Francisco, CA', 'full-time', 'senior', 80000, 120000, 'Java, Python, JavaScript, SQL', 'active', 4),
('Frontend Developer', 'Join our team as a frontend developer to create amazing user experiences.', '3+ years of frontend development experience, strong knowledge of HTML, CSS, JavaScript', 'Build responsive web applications, optimize for performance', 2, 2, 'New York, NY', 'full-time', 'mid', 60000, 90000, 'HTML, CSS, JavaScript, React, Vue.js', 'active', 4),
('Data Scientist', 'Help us turn data into insights as a data scientist.', 'Masters degree in Statistics, Computer Science, or related field, experience with Python/R', 'Analyze large datasets, build predictive models', 1, 3, 'San Francisco, CA', 'full-time', 'mid', 70000, 110000, 'Python, R, SQL, Machine Learning', 'active', 4),
('Digital Marketing Specialist', 'Drive our digital marketing efforts and grow our online presence.', '2+ years of digital marketing experience, knowledge of SEO and social media', 'Manage social media accounts, create marketing campaigns', 3, 4, 'Los Angeles, CA', 'full-time', 'entry', 45000, 65000, 'SEO, Social Media, Google Analytics', 'active', 4),
('UI/UX Designer', 'Create beautiful and intuitive user interfaces for our products.', '3+ years of design experience, proficiency in Figma, Sketch, or Adobe Creative Suite', 'Design user interfaces, conduct user research', 2, 5, 'New York, NY', 'full-time', 'mid', 55000, 85000, 'Figma, Sketch, Adobe Creative Suite', 'active', 4),
('Sales Representative', 'Join our sales team and help grow our customer base.', '1+ years of sales experience, excellent communication skills', 'Generate leads, close sales, maintain customer relationships', 3, 6, 'Los Angeles, CA', 'full-time', 'entry', 40000, 60000, 'Sales, Communication, CRM', 'active', 4),
('Registered Nurse', 'Provide high-quality patient care in our healthcare facility.', 'Valid RN license, 2+ years of nursing experience', 'Provide patient care, administer medications, maintain patient records', 4, 7, 'Boston, MA', 'full-time', 'mid', 50000, 75000, 'Nursing, Patient Care, Medical Software', 'active', 4),
('Financial Analyst', 'Analyze financial data and provide insights to support business decisions.', 'Bachelors degree in Finance or related field, 2+ years of financial analysis experience', 'Analyze financial statements, prepare reports, support budgeting', 4, 8, 'Boston, MA', 'full-time', 'mid', 55000, 80000, 'Financial Analysis, Excel, Accounting', 'active', 4);

-- Sample resumes
INSERT INTO resumes (user_id, title, content, skills, experience, education, is_default) VALUES
(2, 'John Doe - Software Developer', 'Experienced software developer with 5+ years in web development', 'JavaScript, React, Node.js, Python, SQL', 'Senior Developer at TechCorp (2020-2023), Junior Developer at StartupXYZ (2018-2020)', 'B.S. Computer Science, University of Technology', TRUE),
(3, 'Jane Smith - Marketing Specialist', 'Creative marketing professional with expertise in digital marketing', 'SEO, Social Media Marketing, Google Analytics, Content Creation', 'Marketing Specialist at Digital Agency (2021-2023), Marketing Intern at Corp Inc (2020-2021)', 'B.A. Marketing, Business University', TRUE);

-- Sample applications
INSERT INTO applications (job_id, user_id, resume_id, cover_letter, status) VALUES
(1, 2, 1, 'I am excited to apply for the Senior Software Engineer position. With my 5+ years of experience in software development, I believe I would be a great fit for your team.', 'pending'),
(2, 3, 2, 'I am interested in the Frontend Developer position and would love to contribute to your team with my design and development skills.', 'pending');

-- Create indexes for better performance
CREATE INDEX idx_jobs_company ON jobs(company_id);
CREATE INDEX idx_jobs_category ON jobs(category_id);
CREATE INDEX idx_jobs_status ON jobs(status);
CREATE INDEX idx_applications_job ON applications(job_id);
CREATE INDEX idx_applications_user ON applications(user_id);
CREATE INDEX idx_applications_status ON applications(status);
CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_username ON users(username);

-- Create views for common queries
CREATE VIEW job_summary AS
SELECT 
    j.id,
    j.title,
    j.location,
    j.job_type,
    j.salary_min,
    j.salary_max,
    j.status,
    j.created_at,
    c.name as company_name,
    cat.name as category_name,
    COUNT(a.id) as application_count
FROM jobs j
LEFT JOIN companies c ON j.company_id = c.id
LEFT JOIN categories cat ON j.category_id = cat.id
LEFT JOIN applications a ON j.id = a.job_id
GROUP BY j.id;

CREATE VIEW application_summary AS
SELECT 
    a.id,
    a.status,
    a.applied_at,
    j.title as job_title,
    c.name as company_name,
    u.first_name,
    u.last_name,
    u.email
FROM applications a
JOIN jobs j ON a.job_id = j.id
JOIN companies c ON j.company_id = c.id
JOIN users u ON a.user_id = u.id;

-- Grant permissions (adjust as needed for your setup)
-- GRANT ALL PRIVILEGES ON careerconnect.* TO 'your_username'@'localhost';
-- FLUSH PRIVILEGES; 
