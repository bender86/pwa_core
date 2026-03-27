DROP EVENT update_match_statuses;

CREATE EVENT update_match_statuses
ON SCHEDULE EVERY 1 MINUTE
DO
  UPDATE Matches SET Status = CASE
    WHEN Status = 'Scheduled' AND ScheduledDate <= NOW() AND ScheduledDate >= NOW() - INTERVAL 3 HOUR THEN 'Live'
    WHEN Status = 'Live' AND ScheduledDate < NOW() - INTERVAL 3 HOUR THEN 'Finished'
    ELSE Status
  END
  WHERE Status IN ('Scheduled', 'Live');