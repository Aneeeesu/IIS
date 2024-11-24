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
    firstName: '',
    lastName: '',
    password: '',
    roles: []
  });
  const availableRoles = ['Caregiver', 'Vet'];

  const [editingUser, setEditingUser] = useState(null);
  const [editingRoles, setEditingRoles] = useState([]);
  const [selfEditingUser, setSelfEditingUser] = useState(user);

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
      setNewUser({ userName: '', firstName: '', lastName: '', password: '', roles: []});
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
        roles: editingRoles,
        firstName: editingUser.firstName,
        lastName: editingUser.lastName,
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

  const handleImageUpload = async (file) => {
    const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
    const urlResponse = await axios.post(`${API_BASE_URL}/Files/GenerateUrl`, null, {
      params: { fileName: fileExtension }
    });
    const { id, url } = urlResponse.data;
    const fileData = await file.arrayBuffer();
    await fetch(url, {
      method: 'PUT',
      body: fileData,
    });
    const validationResponse = await axios.post(`${API_BASE_URL}/Files/ValidateFile/${id}`);
    return validationResponse.data.id;
  };

  const handleSelfImageChange = async (e) => {
    const file = e.target.files[0];
    if (file) {
      const imageId = await handleImageUpload(file);
      setSelfEditingUser((prevUser) => ({
        ...prevUser,
        imageId: imageId
      }));
    }
  };

  const handleSelfEdit = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        id: user.id,
        firstName: selfEditingUser.firstName,
        lastName: selfEditingUser.lastName,
        userName: selfEditingUser.userName,
        imageId: selfEditingUser.imageId
      };
      await axios.put(`${API_BASE_URL}/users`, payload);
      setSelfEditingUser(user);
    } catch (error) {
      console.error('Error editing user:', error);
    }
  };

  if (!user) {
    return <h1>Unauthorized</h1>;
  }

  return (
    <div className="container">
      <h1>User management</h1>
      
      {user.roles.includes('Admin') && (
        <>
          <div className="create-user-section">
            <h2>Create new user</h2>
            <form onSubmit={handleCreateUser} className="form">
              <input
                type="text"
                placeholder="Username"
                className="input"
                value={newUser.userName}
                onChange={(e) => setNewUser({...newUser, userName: e.target.value})}
              />

              <input
                type="text"
                placeholder="First name"
                className="input"
                value={newUser.firstName}
                onChange={(e) => setNewUser({...newUser, firstName: e.target.value})}
              />

              <input
                type="text"
                placeholder="Last name"
                className="input"
                value={newUser.lastName}
                onChange={(e) => setNewUser({...newUser, lastName: e.target.value})}
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
                <div className="user-row">
                  <img src={user.image?.url} className="userImage" />
                  <div style={{padding: "5px"}}>
                    <p><strong>{user.userName}</strong></p>
                    <p>{user.firstName} {user.lastName}</p>
                    <p>Roles: {user.roles.join(', ')}</p>
                  </div>
                  
                </div>
                <button 
                  className="button"
                  onClick={() => {
                    setEditingUser(user);
                    setEditingRoles(user.roles);
                  }}
                >
                  Edit roles
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
                Save roles
              </button>
            </div>
          )}
        </>
      )}

      <div className="self-edit-section">
        <h2>Edit your profile</h2>
        <form onSubmit={handleSelfEdit} className="form">
          <input
            type="text"
            placeholder="Username"
            className="input"
            value={selfEditingUser.userName}
            onChange={(e) => setSelfEditingUser({...selfEditingUser, userName: e.target.value})}
          />

          <input
            type="text"
            placeholder="First name"
            className="input"
            value={selfEditingUser.firstName}
            onChange={(e) => setSelfEditingUser({...selfEditingUser, firstName: e.target.value})}
          />

          <input
            type="text"
            placeholder="Last name"
            className="input"
            value={selfEditingUser.lastName}
            onChange={(e) => setSelfEditingUser({...selfEditingUser, lastName: e.target.value})}
          />
          
          <input
            type="file"
            className="input"
            onChange={handleSelfImageChange}
          />
          
          <button type="submit" className="button">Save</button>
        </form>
      </div>
    </div>
  );
};

export default UserManagement;