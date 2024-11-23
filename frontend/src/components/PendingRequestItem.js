import React from 'react';

const PendingRequestItem = ({ request }) => (
  <div className="requestItem">
    <p><strong>Animal:</strong> {request.animalName}</p>
    <p><strong>Time:</strong> {new Date(request.time).toLocaleString()}</p>
  </div>
);

export default PendingRequestItem;