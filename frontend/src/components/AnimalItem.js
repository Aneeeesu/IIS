import React from 'react';
import { useNavigate } from 'react-router-dom';

const AnimalItem = ({ animal, onEdit, onDelete }) => {
  const navigate = useNavigate();

  const handleClick = (e) => {
    if (e.target.tagName === 'BUTTON') return;
    navigate(`/animals/${animal.id}`);
  };

  return (
    <div className="animalItem" onClick={handleClick} style={{ cursor: 'pointer' }}>
      <p>{animal.name}</p>
      <div className="animalActions" onClick={e => e.stopPropagation()}>
        <button
          className="button"
          onClick={() => onEdit(animal)}
        >
          Edit
        </button>
        <button 
          className="deleteButton" 
          onClick={() => onDelete(animal.id)}
        >
          Delete
        </button>
      </div>
    </div>
  );
};

export default AnimalItem;