import React, { useState } from 'react';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import '../App.css';

const Signup = () => {
  const [userName, setUserName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const history = useNavigate();

  const handleSignup = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('http://localhost:8001/Account/Register', { userName, email, password });
      localStorage.setItem('authToken', response.data.token);
      history.push('/animals');
    } catch (error) {
      console.error('Error signing up:', error);
    }
  };

  return (
    <div className="container">
      <h2>Sign Up</h2>
      <form className="form" onSubmit={handleSignup}>
        <label>
          User Name:
          <input type="text" className="input" value={userName} onChange={(e) => setUserName(e.target.value)} />
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