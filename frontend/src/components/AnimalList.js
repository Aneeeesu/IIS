import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_BASE_URL } from '../config';
import '../App.css';

const AnimalList = ({ animals, onDelete, user }) => {
  const [animalList, setAnimalList] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    setAnimalList(animals);
  }, [animals]);

  const handleChangeStatus = async (animal) => {
    const newStatus = animal.status === 'Available' ? 'OnWalk' : 'Available';
    try {
      const response = await axios.post(`${API_BASE_URL}/Animal/Status`, {
        animalId: animal.id,
        status: newStatus,
        associatedUserId: user.id
      });
      console.log('Status change response:', response.data);
      const updatedAnimal = await axios.get(`${API_BASE_URL}/Animal/${animal.id}`);
      setAnimalList(animalList.map(a => a.id === animal.id ? { ...a, status: updatedAnimal.data.status } : a));
    } catch (error) {
      console.error('Error changing animal status:', error);
    }
  };

  return (
    <div className="animalList">
      <h2>Available animals</h2>
      {animalList.map(animal => (
        <div
          key={animal.id}
          className={`animalCard ${animal.status === 'Available' ? 'available' : 'onWalk'}`}
          onClick={() => navigate(`/animals/${animal.id}`)}
          style={{ cursor: 'pointer' }}
        >
          <img src={animal.image?.url} alt={animal.name} />
          <h3>{animal.name}</h3>
          {user.roles.includes('Caregiver') && <button
            className="statusButton"
            onClick={(e) => {
              e.stopPropagation();
              handleChangeStatus(animal);
            }}
          >
            {animal.status === 'Available' ? 'Mark as On Walk' : 'Mark as Available'}
          </button>}
          {onDelete && (
            <button
              className="deleteButton"
              onClick={(e) => {
                e.stopPropagation();
                onDelete(animal.id);
              }}
            >
              Delete
            </button>
          )}
        </div>
      ))}
    </div>
  );
};

export default AnimalList;