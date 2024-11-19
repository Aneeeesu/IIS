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
    password: '',
    roles: []
  });
  const availableRoles = ['Caregiver', 'Vet'];

  const [editingUser, setEditingUser] = useState(null);
  const [editingRoles, setEditingRoles] = useState([]);

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

      const filteredUsers = detailedUsers.filter(user => !user.roles.includes('Admin'));
      
      setUsers(filteredUsers);
    } catch (error) {
      console.error('Error fetching users:', error);
    }
  };

  const handleCreateUser = async (e) => {
    e.preventDefault();
    try {
      await axios.post(`${API_BASE_URL}/users/${newUser.roles[0]}`, newUser);
      fetchUsers();
      setNewUser({ userName: '', email: '', password: '', roles: [] });
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

  const handleEditRoles = async (userId) => {
    try {
      const payload = {
        id: userId,
        email: editingUser.email,
        roles: editingRoles
      };
      await axios.put(`${API_BASE_URL}/users`, payload);
      fetchUsers();
      setEditingUser(null);
      setEditingRoles([]);
    } catch (error) {
      console.error('Error editing roles:', error);
    }
  };

  const handleRoleChange = (role) => {
    setEditingRoles([role]);
  };

  const handleNewUserRoleChange = (role) => {
    setNewUser((prevUser) => ({
      ...prevUser,
      roles: [role]
    }));
  };

  if (!user || !user.roles.includes('Admin')) {
    return <h1>Unauthorized</h1>;
  }
  
  return (
    <div className="container">
      <h1>User Management</h1>
      
      <div className="create-user-section">
        <h2>Create New User</h2>
        <form onSubmit={handleCreateUser} className="form">
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

          <div className="roles-section">
            {availableRoles.map((role) => (
              <div key={role} className="role-checkbox">
                <input
                  type="radio"
                  checked={newUser.roles.includes(role)}
                  onChange={() => handleNewUserRoleChange(role)}
                />
                <label>{role}</label>
              </div>
            ))}
          </div>
          
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
              className="button"
              onClick={() => {
                setEditingUser(user);
                setEditingRoles(user.roles);
              }}
            >
              Edit Roles
            </button>
            <button 
              className="deleteButton"
              onClick={() => handleDeleteUser(user.id)}
            >
              Delete
            </button>
          </div>
        ))} 
      </div>

      {editingUser && (
        <div className="edit-roles-section">
          <h2>Edit roles for {editingUser.userName}</h2>
          {availableRoles.map((role) => (
            <div key={role} className="role-checkbox">
              <input
                type="radio"
                checked={editingRoles.includes(role)}
                onChange={() => handleRoleChange(role)}
              />
              <label>{role}</label>
            </div>
          ))}
          <button
            className="button"
            onClick={() => handleEditRoles(editingUser.id)}
          >
            Save Roles
          </button>
        </div>
      )}
    </div>
  );
};

export default UserManagement;