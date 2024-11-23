import React from 'react';

const ScheduledWalkItem = ({ schedule, formatTimeRange }) => (
  <div className="scheduleItem">
    <p><strong>Animal:</strong> {schedule.animalName}</p>
    <p><strong>Time:</strong> {formatTimeRange(schedule.startTime, schedule.endTime)}</p>
  </div>
);

export default ScheduledWalkItem;