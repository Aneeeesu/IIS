import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { API_BASE_URL } from './config';
import Animal from './pages/Animal';
import AnimalDetails from './pages/AnimalDetails';
import UserManagement from './pages/UserManagement';
import Signup from './pages/Signup';
import Main from './pages/Main';
import Login from './pages/Login';
import Header from './components/Header';
import axios from 'axios';
import './App.css';

axios.defaults.withCredentials = true;
axios.defaults.baseURL = API_BASE_URL;

const App = () => {
  return (
    <AuthProvider>
      <Router>
        <Header />
        <Routes>
          <Route path="/" element={<Main />} />
          <Route path="/signup" element={<Signup />} />
          <Route path="/login" element={<Login />} />
          <Route path="/animals" element={<Animal />} />
          <Route path="/animals/:id" element={<AnimalDetails />} />
          <Route path="/admin/users" element={<UserManagement />} />
        </Routes>
      </Router>
    </AuthProvider>
  );
};

export default App;