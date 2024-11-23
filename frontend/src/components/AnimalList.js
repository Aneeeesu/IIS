import React from 'react';

const AnimalList = ({ animals, navigate }) => (
  <div className="animalList">
    <h2>Available Animals</h2>
    {animals.map(animal => (
      <div
        key={animal.id}
        className="animalCard"
        onClick={() => navigate(`/animals/${animal.id}`)}
        style={{ cursor: 'pointer' }}
      >
        <h3>{animal.name}</h3>
      </div>
    ))}
  </div>
);

export default AnimalList;