-- Aggregator company for RSS/Atom syndicated jobs (run once on careerconnect).
-- Job import resolves company id by this name when JobFeedImport_CompanyId is 0.
USE careerconnect;

INSERT INTO companies (name, description, industry, website, email, phone, address, city, state, country, company_size, status)
SELECT 'External job feeds (RSS)',
       'Syndicated listings from configured RSS/Atom feeds. Candidates apply on each source site.',
       'Aggregator',
       '',
       'feeds@localhost',
       '',
       NULL,
       '',
       '',
       '',
       '1',
       'active'
FROM DUAL
WHERE NOT EXISTS (SELECT 1 FROM companies WHERE name = 'External job feeds (RSS)' LIMIT 1);
