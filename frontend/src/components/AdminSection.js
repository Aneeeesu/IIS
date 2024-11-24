import React from 'react';

const AdminSection = ({ navigate }) => (
  <button className="button" onClick={() => navigate('/admin')}>
    Manage users
  </button>
);

export default AdminSection;