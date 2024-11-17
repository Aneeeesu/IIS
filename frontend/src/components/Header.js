import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_BASE_URL } from '../config';

const Header = ({ authToken, setAuthToken }) => {
    const navigate = useNavigate();

    const handleLogout = async () => {
        try {
            await axios.post(`${API_BASE_URL}/Account/Logout`, {}, {
                withCredentials: true
            });
            
            setAuthToken(false);
            navigate('/login');
        } catch (error) {
            console.error('Error logging out:', error);
        }
    };

    return (
        <header>
            <nav>
                <Link to="/">Home</Link>
                {authToken ? (
                    <button onClick={handleLogout}>Logout</button>
                ) : (
                    <Link to="/login">Login</Link>
                )}
            </nav>
        </header>
    );
};

export default Header;