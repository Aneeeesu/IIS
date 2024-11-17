import React, { useState, useEffect } from 'react';
import axios from 'axios';
import '../App.css';

const Animal = () => {
  const [animals, setAnimals] = useState([]);
  const [animalId, setAnimalId] = useState('');
  const [animalDetails, setAnimalDetails] = useState(null);
  const [editingAnimal, setEditingAnimal] = useState(null);
  const [newAnimal, setNewAnimal] = useState({ name: '', age: '', sex: '' });

  useEffect(() => {
    const fetchAnimals = async () => {
      try {
        const response = await axios.get('http://localhost:8001/Animal');
        setAnimals(response.data);
      } catch (error) {
        console.error('Error fetching animals:', error);
      }
    };

    fetchAnimals();
  }, []);

  const fetchAnimalById = async () => {
    try {
      const response = await axios.get(`http://localhost:8001/Animal/${animalId}`);
      setAnimalDetails(response.data);
    } catch (error) {
      console.error('Error fetching animal by ID:', error);
    }
  };

  const deleteAnimal = async (id) => {
    try {
      await axios.delete(`http://localhost:8001/Animal/${id}`);
      setAnimals(animals.filter(animal => animal.id !== id));
    } catch (error) {
      console.error('Error deleting animal:', error);
    }
  };

  const handleEditAnimal = async () => {
    try {
      await axios.put(`http://localhost:8001/Animal/${editingAnimal.id}`, newAnimal);
      setAnimals(animals.map(animal => (animal.id === editingAnimal.id ? newAnimal : animal)));
      setEditingAnimal(null);
      setNewAnimal({ name: '', age: '', sex: '' });
    } catch (error) {
      console.error('Error editing animal:', error);
    }
  };

  return (
    <div className="container">
      <h1>Animal Management</h1>
      <ul className="animalList">
        {animals.map(animal => (
          <div key={animal.id} className="animalItem">
            <p>Name: {animal.name}</p>
            <p>Age: {animal.age}</p>
            <p>Sex: {animal.sex}</p>
            <div className="animalActions">
              <button
                className="editButton"
                onClick={() => {
                  setEditingAnimal(animal);
                  setNewAnimal(animal);
                }}
              >
                Edit
              </button>
              <button className="deleteButton" onClick={() => deleteAnimal(animal.id)}>
                Delete
              </button>
            </div>
          </div>
        ))}
      </ul>

      {editingAnimal && (
        <div>
          <h2>Edit Animal</h2>
          <input
            type="text"
            placeholder="Name"
            className="input"
            value={newAnimal.name}
            onChange={(e) => setNewAnimal({ ...newAnimal, name: e.target.value })}
          />
          <input
            type="text"
            placeholder="Age"
            className="input"
            value={newAnimal.age}
            onChange={(e) => setNewAnimal({ ...newAnimal, age: e.target.value })}
          />
          <input
            type="text"
            placeholder="Sex"
            className="input"
            value={newAnimal.sex}
            onChange={(e) => setNewAnimal({ ...newAnimal, sex: e.target.value })}
          />
          <button className="button" onClick={handleEditAnimal}>
            Save
          </button>
        </div>
      )}

      <div>
        <h2>Get animal by ID</h2>
        <input
          type="text"
          placeholder="Animal ID"
          className="input"
          value={animalId}
          onChange={(e) => setAnimalId(e.target.value)}
        />
        <button className="button" onClick={fetchAnimalById}>
          Fetch animal
        </button>
        {animalDetails && (
          <div className="animalDetail">
            <h3>Animal details</h3>
            <p>Name: {animalDetails.name}</p>
            <p>Age: {animalDetails.age}</p>
            <p>Sex: {animalDetails.sex}</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default Animal;