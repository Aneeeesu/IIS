import React from 'react';
import AnimalDetail from './AnimalDetail';

const AnimalDetailSearch = ({ animalId, onIdChange, onSearch, animalDetails }) => {
  return (
    <div>
      <h2>Get animal by ID</h2>
      <input
        type="text"
        placeholder="Animal ID"
        className="input"
        value={animalId}
        onChange={(e) => onIdChange(e.target.value)}
      />
      <button className="button" onClick={onSearch}>
        Fetch animal
      </button>
      {animalDetails && <AnimalDetail animal={animalDetails} />}
    </div>
  );
};

export default AnimalDetailSearch;