-- Unread counts: last-read timestamps per participant on each thread.
-- Run on MySQL (careerconnect) after group C messaging migration.

SET NAMES utf8mb4;

ALTER TABLE message_threads
  ADD COLUMN seeker_read_at DATETIME NULL DEFAULT NULL AFTER updated_at,
  ADD COLUMN employer_read_at DATETIME NULL DEFAULT NULL AFTER seeker_read_at;

-- Treat existing threads as "caught up" so users don't see huge historical unread counts.
UPDATE message_threads
SET seeker_read_at = COALESCE(updated_at, created_at),
    employer_read_at = COALESCE(updated_at, created_at)
WHERE seeker_read_at IS NULL AND employer_read_at IS NULL;
