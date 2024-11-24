import React from 'react';

const AdminSection = ({ navigate }) => (
  <button className="button" onClick={() => navigate('/admin')}>
    Profile management
  </button>
);

export default AdminSection;