import React, { useState, useEffect } from 'react';
import axios from 'axios';
import AnimalList from '../components/AnimalList';
import AnimalEditForm from '../components/AnimalEditForm';
import AnimalDetailSearch from '../components/AnimalDetailSearch';
import { API_BASE_URL } from '../config';
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
        const response = await axios.get(`${API_BASE_URL}/Animal`);
        setAnimals(response.data);
      } catch (error) {
        console.error('Error fetching animals:', error);
      }
    };

    fetchAnimals();
  }, []);

  const fetchAnimalById = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/Animal/${animalId}`);
      setAnimalDetails(response.data);
    } catch (error) {
      console.error('Error fetching animal by ID:', error);
    }
  };

  const deleteAnimal = async (id) => {
    try {
      await axios.delete(`${API_BASE_URL}/Animal/${id}`);
      setAnimals(animals.filter(animal => animal.id !== id));
    } catch (error) {
      console.error('Error deleting animal:', error);
    }
  };

  const handleEditAnimal = async () => {
    try {
      await axios.put(`${API_BASE_URL}/Animal/${editingAnimal.id}`, newAnimal);
      setAnimals(animals.map(animal => (animal.id === editingAnimal.id ? newAnimal : animal)));
      setEditingAnimal(null);
      setNewAnimal({ name: '', age: '', sex: '' });
    } catch (error) {
      console.error('Error editing animal:', error);
    }
  };

  const handleEditClick = (animal) => {
    setEditingAnimal(animal);
    setNewAnimal(animal);
  };

  return (
    <div className="container">
      <h1>Animal Management</h1>
      <AnimalList 
        animals={animals}
        onEdit={handleEditClick}
        onDelete={deleteAnimal}
      />

      {editingAnimal && (
        <AnimalEditForm
          animal={newAnimal}
          onChange={setNewAnimal}
          onSave={handleEditAnimal}
        />
      )}

      <AnimalDetailSearch
        animalId={animalId}
        onIdChange={setAnimalId}
        onSearch={fetchAnimalById}
        animalDetails={animalDetails}
      />
    </div>
  );
};

export default Animal;