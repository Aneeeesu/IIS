import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_BASE_URL } from '../config';

const AnimalDetails = () => {
  const [animal, setAnimal] = useState(null);
  const { id } = useParams();
  const navigate = useNavigate();

  useEffect(() => {
    const fetchAnimal = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/Animal/${id}`);
        setAnimal(response.data);
      } catch (error) {
        console.error('Error fetching animal:', error);
        navigate('/animals');
      }
    };

    fetchAnimal();
  }, [id, navigate]);

  if (!animal) return <div>Loading...</div>;

  return (
    <div className="container">
      <h1>Animal Details</h1>
      <div className="animalDetail">
        <h2>{animal.name}</h2>
        <p>Age: {animal.age}</p>
        <p>Sex: {animal.sex}</p>
      </div>
      <button className="button" onClick={() => navigate(-1)}>Back</button>
    </div>
  );
};

export default AnimalDetails;