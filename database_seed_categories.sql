-- Career Connect: default job categories for employer/admin dropdowns
-- Run in phpMyAdmin or: mysql -u root -p careerconnect < database_seed_categories.sql
-- Safe to run multiple times: skips duplicates when `name` is UNIQUE.

USE careerconnect;

INSERT IGNORE INTO categories (name, description, icon, color) VALUES
('Software Development', 'Programming and software engineering', 'fas fa-code', '#007bff'),
('Web Development', 'Web frontend and backend', 'fas fa-globe', '#28a745'),
('Data Science', 'Data analysis and machine learning', 'fas fa-chart-bar', '#ffc107'),
('Marketing', 'Digital marketing and growth', 'fas fa-bullhorn', '#dc3545'),
('Design', 'UI/UX and creative', 'fas fa-palette', '#6f42c1'),
('Sales', 'Sales and business development', 'fas fa-handshake', '#fd7e14'),
('Healthcare', 'Medical and health', 'fas fa-heartbeat', '#e83e8c'),
('Finance', 'Accounting and finance', 'fas fa-dollar-sign', '#20c997'),
('Human Resources', 'HR and recruitment', 'fas fa-users', '#6610f2'),
('Customer Support', 'Support and customer success', 'fas fa-headset', '#0dcaf0'),
('Education', 'Teaching and training', 'fas fa-graduation-cap', '#198754'),
('Operations', 'Operations and logistics', 'fas fa-cogs', '#6c757d');
