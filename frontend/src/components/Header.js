import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';
import { API_BASE_URL } from '../config';
import '../App.css';

const Header = () => {
    const navigate = useNavigate();
    const { user, logout: authLogout } = useAuth();

    const handleLogout = async () => {
        try {
            await axios.post(`${API_BASE_URL}/Account/Logout`, {}, {
                withCredentials: true
            });
            
            authLogout();
            navigate('/login');
        } catch (error) {
            console.error('Error logging out:', error);
        }
    };

    return (
        <header className="header">
            <nav className="nav">
                <Link to="/" className="nav-logo">
                    Animal Shelter
                </Link>
                {user && (
                    <span className="user-info">
                        Logged in as {user.userName} ({user.roles.join(', ')})
                    </span>
                )}

                <div className="nav-links">
                    <Link to="/" className="nav-link">Home</Link>
                    <Link to="/animals" className="nav-link">Animals</Link>
                    {!user ? (
                        <>
                            <Link to="/login" className="nav-link">Login</Link>
                            <Link to="/signup" className="nav-link">Sign Up</Link>
                        </>
                    ) : (
                        <>
                            <button onClick={handleLogout} className="logout-btn">
                                Logout
                            </button>
                        </>
                    )}
                </div>
            </nav>
        </header>
    );
};

export default Header;