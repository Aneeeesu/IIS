import React, { useState, useEffect } from 'react';
import axios from 'axios';
import '../App.css';

const Main = () => {
  const [animals, setAnimals] = useState([]);

  useEffect(() => {
    const fetchAnimals = async () => {
      try {
        const response = await axios.get('http://localhost:8001/Animal');
        setAnimals(response.data);
      } catch (error) {
        console.error('Error fetching animals:', error);
      }
    };

    fetchAnimals();
  }, []);

  return (
    <div className="container">
      <h1>Available Animals</h1>
      <ul className="animalList">
        {animals.map(animal => (
          <li key={animal.id}>{animal.name}</li>
        ))}
      </ul>
    </div>
  );
};

export default Main;