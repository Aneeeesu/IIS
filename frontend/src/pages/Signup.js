import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import { API_BASE_URL } from '../config';
import '../App.css';

const Signup = () => {
  const [userName, setUserName] = useState('');
  const [lastName, setLastName] = useState('');
  const [firstName, setFirstName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const history = useNavigate();

  const handleSignup = async (e) => {
    e.preventDefault();
    try {
      await axios.post(`${API_BASE_URL}/Account/Register`, { userName, email, password, firstName, lastName });
      history('/login');
    } catch (error) {
      console.error('Error signing up:', error);
      setErrorMessage(error.response?.data || 'Error signing up. Please try again.');
    }
  };

  return (
    <div className="container">
      <h2>Sign up</h2>
      <form className="form" onSubmit={handleSignup}>
        {errorMessage && <p className="error">{errorMessage}</p>}

        <label>
          Username:
          <input type="text" className="input" value={userName} onChange={(e) => setUserName(e.target.value)} />
        </label>
        <label>
          First name:
          <input type="text" className="input" value={firstName} onChange={(e) => setFirstName(e.target.value)} />
        </label>
        <label>
          Last name:
          <input type="text" className="input" value={lastName} onChange={(e) => setLastName(e.target.value)} />
        </label>
        <label>
          Email:
          <input type="email" className="input" value={email} onChange={(e) => setEmail(e.target.value)} />
        </label>
        <label>
          Password:
          <input type="password" className="input" value={password} onChange={(e) => setPassword(e.target.value)} />
        </label>
        <button type="submit" className="button">Sign Up</button>
      </form>
    </div>
  );
};

export default Signup;