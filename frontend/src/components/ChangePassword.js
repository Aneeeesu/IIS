import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config';

const ChangePassword = ({ userId }) => {
  const [oldPassword, setOldPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [errorMessage, setErrorMessage] = useState('');

  const handleChangePassword = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        userId: userId,
        oldPassword: oldPassword,
        newPassword: newPassword
      };
      await axios.put(`${API_BASE_URL}/Users/ChangePassword`, payload);
      setSuccessMessage('Password changed successfully.');
      setOldPassword('');
      setNewPassword('');
    } catch (error) {
      console.error('Error changing password:', error);
      setErrorMessage('Failed to change password.');
    }
  };

  useEffect(() => {
    const timer = setTimeout(() => {
      setSuccessMessage('');
      setErrorMessage('');
    }, 3000);
    return () => clearTimeout(timer);
  }, [successMessage, errorMessage]);

  return (
    <div className="change-password-section">
      {successMessage && (
        <div className="success-message">
          <p>{successMessage}</p>
        </div>
      )}
      {errorMessage && (
        <div className="error-message">
          <p>{errorMessage}</p>
        </div>
      )}
      <form onSubmit={handleChangePassword} className="form">
        <label>Old Password:</label>
        <input
          type="password"
          placeholder="Old Password"
          className="input"
          value={oldPassword}
          onChange={(e) => setOldPassword(e.target.value)}
        />

        <label>New Password:</label>
        <input
          type="password"
          placeholder="New Password"
          className="input"
          value={newPassword}
          onChange={(e) => setNewPassword(e.target.value)}
        />

        <button type="submit" className="button">Change Password</button>
      </form>
    </div>
  );
};

export default ChangePassword;