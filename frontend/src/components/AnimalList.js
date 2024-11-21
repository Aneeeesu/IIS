import React from 'react';
import AnimalItem from './AnimalItem';

const AnimalList = ({ animals, onDelete }) => {
  return (
    <ul className="animalList">
      {animals.map(animal => (
        <AnimalItem 
          key={animal.id}
          animal={animal}
          onDelete={onDelete}
        />
      ))}
    </ul>
  );
};

export default AnimalList;