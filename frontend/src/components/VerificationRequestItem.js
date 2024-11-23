import React from 'react';

const VerificationRequestItem = ({ request, handleResolveVerificationRequest }) => (
  <div className="verificationRequestItem">
    <p><strong>Name:</strong> {request.requestee.userName}</p>
    <button className="button" onClick={() => handleResolveVerificationRequest(request.id, true)}>
      Approve
    </button>
    <button className="button" onClick={() => handleResolveVerificationRequest(request.id, false)}>
      Reject
    </button>
  </div>
);

export default VerificationRequestItem;