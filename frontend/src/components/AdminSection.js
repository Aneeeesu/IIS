import React from 'react';

const AdminSection = ({ navigate }) => (
  <button className="button" onClick={() => navigate('/admin')}>
    Manage Users
  </button>
);

export default AdminSection;