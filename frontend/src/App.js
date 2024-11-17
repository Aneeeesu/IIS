import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Animal from './pages/Animal';
import Signup from './pages/Signup';
import Main from './pages/Main';
import Login from './pages/Login';
import Header from './components/Header';
import axios from 'axios';
import './App.css';

const App = () => {
    const [authToken, setAuthToken] = useState(false);

    axios.defaults.withCredentials = true;

    useEffect(() => {
        const checkAuth = async () => {
            try {
                const response = await axios.get('http://localhost:5181/users', {
                    withCredentials: true
                });
                if (response.status === 200) {
                    setAuthToken(true);
                }
            } catch (error) {
                setAuthToken(false);
            }
        };

        checkAuth();
    }, []);

    return (
        <Router>
            <Header authToken={authToken} setAuthToken={setAuthToken} />
            <Routes>
                <Route path="/" element={authToken ? <Main /> : <Navigate to="/login" />} />
                <Route path="/signup" element={<Signup />} />
                <Route path="/login" element={<Login setAuthToken={setAuthToken} />} />
                <Route 
                    path="/animals" 
                    element={authToken ? 
                        <Animal /> : <Navigate to="/" />
                    } 
                />
            </Routes>
        </Router>
    );
};

export default App;