import React, { useState, useEffect } from 'react';
import axios from 'axios';
import AnimalList from '../components/AnimalList';
import AnimalAddForm from '../components/AnimalAddForm';
import Modal from '../components/Modal';
import { API_BASE_URL } from '../config';
import '../App.css';

const Animal = () => {
  const [animals, setAnimals] = useState([]);
  const [addingAnimal, setAddingAnimal] = useState(false);
  const [newAnimal, setNewAnimal] = useState({ name: '', age: '', sex: '' });
  const [imageFile, setImageFile] = useState(null);

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

  const deleteAnimal = async (id) => {
    try {
      await axios.delete(`${API_BASE_URL}/Animal/${id}`);
      setAnimals(animals.filter(animal => animal.id !== id));
    } catch (error) {
      console.error('Error deleting animal:', error);
    }
  };

  const handleAddAnimal = async () => {
    try {
      const response = await axios.post(`${API_BASE_URL}/Animal`, {
        ...newAnimal,
        imageID: imageFile // This is now the validated file ID, not the File object
      });
      
      setAnimals([...animals, response.data]);
      setAddingAnimal(false);
      setNewAnimal({ name: '', age: '', sex: '' });
      setImageFile(null);
    } catch (error) {
      console.error('Error adding animal:', error);
      alert('Failed to add animal');
    }
  };

  const handleImageChange = async (e) => {
    const file = e.target.files[0];
    console.log('Selected file:', file);
    if (!file) return;
  
    try {
      const fileExtension = '.' + file.name.split('.').pop().toLowerCase();
      const urlResponse = await axios.post(`${API_BASE_URL}/Files/GenerateUrl`, null, {
        params: { fileName: fileExtension }
      });
      
      const { id, url } = urlResponse.data;
  
      const fileData = await file.arrayBuffer();
  
      await fetch(url, {
        method: 'PUT',
        body: fileData,
      });
  
      const validationResponse = await axios.post(
        `${API_BASE_URL}/Files/ValidateFile/${id}`
      );
  
      setImageFile(validationResponse.data.id);
  
    } catch (error) {
      console.error('Error uploading image:', error);
      alert('Failed to upload image');
    }
  };

  return (
    <div className="container">
      <h1>Animal Management</h1>
      <button className='button' onClick={() => setAddingAnimal(true)}>Add Animal</button>
      <AnimalList 
        animals={animals}
        onDelete={deleteAnimal}
      />
  
      <Modal
        isOpen={addingAnimal}
        onClose={() => {
          setAddingAnimal(false);
          setNewAnimal({ name: '', age: '', sex: '' });
          setImageFile(null);
        }}
      >
        <AnimalAddForm
          animal={newAnimal}
          onChange={setNewAnimal}
          onSave={handleAddAnimal}
          onCancel={() => {
            setAddingAnimal(false);
            setNewAnimal({ name: '', age: '', sex: '' });
            setImageFile(null);
          }}
          onImageChange={handleImageChange}
        />
      </Modal>
    </div>
  );
};

export default Animal;