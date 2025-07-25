import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import { API_BASE_URL } from '../config';
import '../App.css';

const Login = () => {
  const [name, setName] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const navigate = useNavigate();
  const { login } = useAuth();

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(`${API_BASE_URL}/Account/Login`, {
        name,
        password
      }, {
        headers: { 'accept': 'application/json' },
        withCredentials: true
      });

      if (response.data.succeeded) {
        const userResponse = await axios.get(`${API_BASE_URL}/Account/GetUserID`);
        const userData = await axios.get(`${API_BASE_URL}/users/${userResponse.data}`);
        await login(userData.data);
        navigate('/');
      } else {
        setErrorMessage('Login failed. Please try again.');
      }
    } catch (error) {
      console.error('Error logging in:', error);
      setErrorMessage('An error occurred. Please try again.');
    }
  };

  return (
    <div className="container">
      <h2>Login</h2>
      <form className="form" onSubmit={handleLogin}>
        <label>
          Name:
          <input type="text" className="input" value={name} onChange={(e) => setName(e.target.value)} />
        </label>
        <label>
          Password:
          <input type="password" className="input" value={password} onChange={(e) => setPassword(e.target.value)} />
        </label>
        <button type="submit" className="button">Login</button>
      </form>
      {errorMessage && <p className="error">{errorMessage}</p>}
    </div>
  );
};

export default Login;