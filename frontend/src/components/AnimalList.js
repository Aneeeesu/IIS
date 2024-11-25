import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_BASE_URL } from '../config';
import '../App.css';

const AnimalList = ({ animals, onDelete, user }) => {
  console.log('animals:', animals);
  const [animalList, setAnimalList] = useState([]);
  const navigate = useNavigate();

  useEffect(() => {
    setAnimalList(animals);
  }, [animals]);

  const handleChangeStatus = async (animal) => {
    if (!user) return; // Prevent status change if no user

    const newStatus = animal.status === 'Available' ? 'OnWalk' : 'Available';
    try {
      await axios.post(`${API_BASE_URL}/Animal/Status`, {
        animalId: animal.id,
        status: newStatus,
        associatedUserId: user.id
      });
      const updatedAnimal = await axios.get(`${API_BASE_URL}/Animal/${animal.id}`);
      setAnimalList(animalList.map(a => a.id === animal.id ? { ...a, status: updatedAnimal.data.status } : a));
    } catch (error) {
      console.error('Error changing animal status:', error);
    }
  };

  return (
    <>
      <h2>Available animals</h2>
      <div className="animalList">
        {animalList.map(animal => (
          <div
            key={animal.id}
            className={`animalCard ${animal.status === 'Available' ? 'available' : 'onWalk'}`}
            onClick={() => navigate(`/animals/${animal.id}`)}
          >
            <img src={animal.image?.url} alt={animal.name} />
            <h3>{animal.name}</h3>
            <button
              className={`statusButton ${!user ? 'disabled' : ''}`}
              onClick={(e) => {
                e.stopPropagation();
                handleChangeStatus(animal);
              }}
              disabled={!user}
            >
              {animal.status === 'Available' ? 'Mark as on walk' : 'Mark as available'}
            </button>
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
    </>
  );
};

export default AnimalList;