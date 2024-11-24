import React from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_BASE_URL } from '../config';
import '../App.css';

const AnimalList = ({ animals, onDelete, user }) => {
  const navigate = useNavigate();

  const handleChangeStatus = async (animal) => {
    
    const newStatus = animal.status === 'Available' ? 'OnWalk' : 'Available';
    try {
      await axios.post(`${API_BASE_URL}/Animal/Status`, {
        animalId: animal.id,
        status: newStatus,
        associatedUserId: user.id
      });
      const response = await axios.get(`${API_BASE_URL}/Animal/${animal.id}`);
      animal.status = response.data.status;
    } catch (error) {
      console.error('Error changing animal status:', error);
    }
  };

  return (
    <div className="animalList">
      <h2>Available animals</h2>
      {animals.map(animal => (
        <div
          key={animal.id}
          className={`animalCard ${animal.status === 'Available' ? 'available' : 'onWalk'}`}
          onClick={() => navigate(`/animals/${animal.id}`)}
          style={{ cursor: 'pointer' }}
        >
          <img src={animal.image?.url} />
          <h3>{animal.name}</h3>
          <button
            className="statusButton"
            onClick={(e) => {
              e.stopPropagation();
              handleChangeStatus(animal);
            }}
          >
            {animal.status === 'Available' ? 'Mark as On Walk' : 'Mark as Available'}
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
  );
};

export default AnimalList;