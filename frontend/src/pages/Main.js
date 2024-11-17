import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';
import { API_BASE_URL } from '../config';
import '../App.css';

const Main = () => {
  const [animals, setAnimals] = useState([]);
  const { user } = useAuth();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchAnimals = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/Animal`);
        setAnimals(response.data);
      } catch (error) {
        console.error('Error fetching animals:', error);
      }
    };

    fetchAnimals();
  }, []);

  return (
    <div className="container">
      <h1>Welcome to Animal Shelter</h1>
      {!user && (
        <div className="welcome-section">
          <h2>About Us</h2>
          <p>Welcome to our animal shelter. Here you can view our available animals.</p>
          <p>Please login or register to interact with animals.</p>
        </div>
      )}
      
      {user && user.roles.includes('Caregiver') && (
        <div className="animal-management-section">
          <button 
            className="button"
            onClick={() => navigate('/animals')}
          >
            Manage Animals
          </button>
        </div>
      )}

      <div className="animalList">
        <h2>Available Animals</h2>
        {animals.map(animal => (
          <div key={animal.id} className="animalCard">
            <h3>{animal.name}</h3>
            <p>Age: {animal.age}</p>
            <p>Sex: {animal.sex}</p>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Main;