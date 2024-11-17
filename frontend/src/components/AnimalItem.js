import React from 'react';

const AnimalItem = ({ animal, onEdit, onDelete }) => {
  return (
    <div className="animalItem">
      <p>Name: {animal.name}</p>
      <p>Age: {animal.age}</p>
      <p>Sex: {animal.sex}</p>
      <div className="animalActions">
        <button
          className="editButton"
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