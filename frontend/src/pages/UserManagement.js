import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';
import { API_BASE_URL } from '../config';
import '../App.css';

const UserManagement = () => {
  const [users, setUsers] = useState([]);
  const { user } = useAuth();
  const [newUser, setNewUser] = useState({
    userName: '',
    email: '',
    password: ''
  });
  const [userType, setUserType] = useState('caretaker');

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/users`);
      
      const detailedUsers = await Promise.all(
        response.data.map(async (user) => {
          const userDetailResponse = await axios.get(`${API_BASE_URL}/users/${user.id}`);
          return userDetailResponse.data;
        })
      );
      
      setUsers(detailedUsers);
    } catch (error) {
      console.error('Error fetching users:', error);
    }
  };

  const handleCreateUser = async (e) => {
    e.preventDefault();
    try {
      const endpoint = userType === 'vet' ? 'users/vets' : 'users/caretakers';
      await axios.post(`${API_BASE_URL}/${endpoint}`, newUser);
      fetchUsers();
      setNewUser({ userName: '', email: '', password: '' });
    } catch (error) {
      console.error('Error creating user:', error);
    }
  };

  const handleDeleteUser = async (userId) => {
    try {
      await axios.delete(`${API_BASE_URL}/users/${userId}`);
      fetchUsers();
    } catch (error) {
      console.error('Error deleting user:', error);
    }
  };

  return (
    <div className="container">
      <h1>User Management</h1>
      
      <div className="create-user-section">
        <h2>Create New User</h2>
        <form onSubmit={handleCreateUser} className="form">
          <select 
            className="select"
            value={userType}
            onChange={(e) => setUserType(e.target.value)}
          >
            <option value="caretaker">Caretaker</option>
            <option value="vet">Veterinarian</option>
          </select>
          
          <input
            type="text"
            placeholder="Username"
            className="input"
            value={newUser.userName}
            onChange={(e) => setNewUser({...newUser, userName: e.target.value})}
          />
          
          <input
            type="email"
            placeholder="Email"
            className="input"
            value={newUser.email}
            onChange={(e) => setNewUser({...newUser, email: e.target.value})}
          />
          
          <input
            type="password"
            placeholder="Password"
            className="input"
            value={newUser.password}
            onChange={(e) => setNewUser({...newUser, password: e.target.value})}
          />
          
          <button type="submit" className="button">Create User</button>
        </form>
      </div>

      <div className="users-list">
        <h2>Users</h2>
            {users.map(user => (
                <div key={user.id} className="userItem">
                    <div>
                    <p><strong>{user.userName}</strong></p>
                    <p>{user.email}</p>
                    <p>Roles: {user.roles.join(', ')}</p>
                    </div>
                    <button 
                    className="deleteButton"
                    onClick={() => handleDeleteUser(user.id)}
                    >
                    Delete
                    </button>
                </div>
                ))}
      </div>
    </div>
  );
};

export default UserManagement;