import React from 'react';
import VerificationRequestItem from '../components/VerificationRequestItem';

const CaregiverSection = ({ verificationRequests, handleResolveVerificationRequest, navigate }) => (
  <>
    <button className="button" onClick={() => navigate('/animals')}>
      Manage animals
    </button>
    <div className="verificationRequests">
      <h2>Pending verification requests</h2>
      {verificationRequests.length > 0 ? (
        verificationRequests.map(request => (
          <VerificationRequestItem key={request.id} request={request} handleResolveVerificationRequest={handleResolveVerificationRequest} />
        ))
      ) : (
        <p>No pending verification requests.</p>
      )}
    </div>
  </>
);

export default CaregiverSection;