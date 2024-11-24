import React from 'react';
import { useNavigate } from 'react-router-dom';
import '../App.css';

const AnimalList = ({ animals, onDelete }) => {
  const navigate = useNavigate();
  return (
  <div className="animalList">
    <h2>Available Animals</h2>
    {animals.map(animal => (
      
      <div
        key={animal.id}
        className="animalCard"
        onClick={() => navigate(`/animals/${animal.id}`)}
        style={{ cursor: 'pointer' }}
      >
        {console.log(animal)}
        <img src={animal.image?.url} />
        <h3>{animal.name}</h3>
        {onDelete && <button className="deleteButton" onClick={() => onDelete(animal.id)}>Delete</button>}
      </div>
    ))}
  </div>
)};

export default AnimalList;