import React from 'react';

const AnimalDetail = ({ animal }) => {
  return (
    <div className="animalDetail">
      <h3>Animal details</h3>
      <p>Name: {animal.name}</p>
      <p>Age: {animal.age}</p>
      <p>Sex: {animal.sex}</p>
    </div>
  );
};

export default AnimalDetail;