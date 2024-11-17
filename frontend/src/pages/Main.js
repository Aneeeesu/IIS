import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { useAuth } from '../contexts/AuthContext';
import { API_BASE_URL } from '../config';
import '../App.css';

const Main = () => {
  const [animals, setAnimals] = useState([]);
  const { user } = useAuth();
  const [editingAnimal, setEditingAnimal] = useState(null);
  const [editForm, setEditForm] = useState({
    name: '',
    age: '',
    sex: ''
  });

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

  const handleEdit = (animal) => {
    setEditingAnimal(animal);
    setEditForm({
      name: animal.name,
      age: animal.age,
      sex: animal.sex
    });
  };

  const handleDelete = async (id) => {
    try {
      await axios.delete(`${API_BASE_URL}/Animal/${id}`, {
        withCredentials: true
      });
      setAnimals(animals.filter(animal => animal.id !== id));
    } catch (error) {
      console.error('Error deleting animal:', error);
    }
  };

  const handleEditSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.put(`${API_BASE_URL}/Animal/${editingAnimal.id}`, editForm, {
        withCredentials: true
      });
      
      setAnimals(animals.map(animal => 
        animal.id === editingAnimal.id ? { ...animal, ...editForm } : animal
      ));
      
      setEditingAnimal(null);
      setEditForm({
        name: '',
        age: '',
        sex: ''
      });
    } catch (error) {
      console.error('Error updating animal:', error);
    }
  };

  return (
    <div className="container">
      <h1>Welcome to Animal Shelter</h1>
      {!user && (
        <div className="welcome-section">
          <h2>About Us</h2>
          <p>Welcome to our animal shelter. Here you can view our available animals.</p>
          <p>Please login or register to interact with animals.</p>
        </div>
      )}
      
      <div className="animalList">
        <h2>Available Animals</h2>
        {animals.map(animal => (
          <div key={animal.id} className="animalCard">
            {editingAnimal?.id === animal.id ? (
              <form onSubmit={handleEditSubmit}>
                <input
                  type="text"
                  value={editForm.name}
                  onChange={(e) => setEditForm({...editForm, name: e.target.value})}
                />
                <input
                  type="number"
                  value={editForm.age}
                  onChange={(e) => setEditForm({...editForm, age: e.target.value})}
                />
                <select
                  value={editForm.sex}
                  onChange={(e) => setEditForm({...editForm, sex: e.target.value})}
                >
                  <option value="M">Male</option>
                  <option value="F">Female</option>
                </select>
                <button type="submit">Save</button>
                <button type="button" onClick={() => setEditingAnimal(null)}>Cancel</button>
              </form>
            ) : (
              <>
                <h3>{animal.name}</h3>
                <p>Age: {animal.age}</p>
                <p>Sex: {animal.sex}</p>
                {user && user.roles.includes('Caregiver') && (
                  <div className="admin-controls">
                    <button onClick={() => handleEdit(animal)}>Edit</button>
                    <button onClick={() => handleDelete(animal.id)}>Delete</button>
                  </div>
                )}
              </>
            )}
          </div>
        ))}
      </div>
    </div>
  );
};

export default Main;