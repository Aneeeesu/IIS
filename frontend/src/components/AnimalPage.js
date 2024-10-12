import React, { useState, useEffect } from 'react';
import axios from 'axios';

const AnimalPage = () => {
  const [animals, setAnimals] = useState([]);
  const [animalId, setAnimalId] = useState('');
  const [animalDetails, setAnimalDetails] = useState(null);
  const [newAnimal, setNewAnimal] = useState({ name: '', age: '', sex: '' });
  const [editingAnimal, setEditingAnimal] = useState(null);

  const apiBaseUrl = 'http://localhost:8001/Animal';

  const fetchAnimals = async () => {
    try {
      const response = await axios.get(`${apiBaseUrl}`);
      setAnimals(response.data);
      console.log(response.data);
    } catch (error) {
      console.error('Error fetching animals:', error);
    }
  };

  const fetchAnimalById = async () => {
    if (!animalId) return;
    try {
      const response = await axios.get(`${apiBaseUrl}/${animalId}`);
      setAnimalDetails(response.data);
    } catch (error) {
      console.error('Error fetching animal by ID:', error);
    }
  };

  const handleUpsertAnimal = async (animal) => {
    try {
      const response = await axios.post(`${apiBaseUrl}`, {
        ...animal,
        age: parseInt(animal.age),
        sex: parseInt(animal.sex),
      });
      console.log('Animal saved with ID:', response.data);
      fetchAnimals();
    } catch (error) {
      console.error('Error saving animal:', error);
    }
  };

  const deleteAnimal = async (id) => {
    try {
      await axios.delete(`${apiBaseUrl}/${id}`);
      fetchAnimals();
    } catch (error) {
      console.error('Error deleting animal:', error);
    }
  };

  useEffect(() => {
    fetchAnimals();
  }, []);

  return (
    <div className="container">
      <h1>Animal management</h1>

      <div className="form">
        <h2>{editingAnimal ? 'Edit animal' : 'Add new animal'}</h2>
        <input
            type="text"
            placeholder="Name"
            className="input"
            value={newAnimal.name}
            onChange={(e) => setNewAnimal({ ...newAnimal, name: e.target.value })}
        />
        <input
            type="number"
            placeholder="Age"
            className="input"
            value={newAnimal.age}
            onChange={(e) => setNewAnimal({ ...newAnimal, age: e.target.value })}
        />
        <select
            className="select"
            value={newAnimal.sex}
            onChange={(e) => setNewAnimal({ ...newAnimal, sex: e.target.value })}
        >
            <option value="">Select sex</option>
            <option value="0">Male</option>
            <option value="1">Female</option>
        </select>
        <button className="button" onClick={() => handleUpsertAnimal(newAnimal)}>
            {editingAnimal ? 'Update animal' : 'Add animal'}
        </button>
    </div>

      <div>
        <h2>All animals</h2>
        <ul className="animalList">
          {animals.map((animal) => (
            <div className="animalItem" key={animal.id}>
                Name: {animal.name}, ID: {animal.id}
                <div className="buttonContainer">
                    <button
                    className="button"
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
      </div>

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

export default AnimalPage;
