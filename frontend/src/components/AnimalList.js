import React from 'react';
import AnimalItem from './AnimalItem';

const AnimalList = ({ animals, onEdit, onDelete }) => {
  return (
    <ul className="animalList">
      {animals.map(animal => (
        <AnimalItem 
          key={animal.id}
          animal={animal}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </ul>
  );
};

export default AnimalList;