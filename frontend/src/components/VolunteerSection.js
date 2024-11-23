import React from 'react';
import PendingRequestItem from './PendingRequestItem';
import ScheduledWalkItem from './ScheduleWalkItem';

const VolunteerSection = ({ pendingRequests, approvedSchedules, showPastWalks, toggleShowPastWalks, currentDateTime, formatTimeRange }) => (
  <div className="volunteerSection">
    <h2>Your pending walk requests</h2>
    {pendingRequests.length > 0 ? (
      pendingRequests.map(request => (
        <PendingRequestItem key={request.id} request={request} />
      ))
    ) : (
      <p>You have no pending reservation requests.</p>
    )}

    <h2>Your scheduled walks</h2>
    <button className="button" onClick={toggleShowPastWalks}>
      {showPastWalks ? 'Hide past walks' : 'Show past walks'}
    </button>
    {approvedSchedules.length > 0 ? (
      approvedSchedules
        .filter(schedule => showPastWalks || new Date(schedule.endTime) >= currentDateTime)
        .map(schedule => (
          <ScheduledWalkItem key={schedule.id} schedule={schedule} formatTimeRange={formatTimeRange} />
        ))
    ) : (
      <p>You have no scheduled walks.</p>
    )}
  </div>
);

export default VolunteerSection;