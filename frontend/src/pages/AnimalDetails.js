import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { API_BASE_URL } from '../config';

axios.defaults.withCredentials = true;

const AnimalDetails = () => {
  const [animal, setAnimal] = useState(null);
  const [selectedHours, setSelectedHours] = useState([]);
  const [reservedHours, setReservedHours] = useState([]);
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
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

  useEffect(() => {
    const fetchReservedHours = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/Schedule/Animal/${id}`);
        const scheduleEntries = response.data;

        // Filter entries for the selected date
        const dateEntries = scheduleEntries.filter(entry => {
          const entryDate = new Date(entry.time).toISOString().split('T')[0];
          return entryDate === date;
        });

        // Extract hours from the entries
        const hours = dateEntries.map(entry => new Date(entry.time).getHours());
        setReservedHours(hours);
      } catch (error) {
        console.error('Error fetching reserved hours:', error);
      }
    };

    fetchReservedHours();
  }, [date, id]);

  if (!animal) return <div>Loading...</div>;

  const handleHourClick = (hour) => {
    if (reservedHours.includes(hour)) return;
    if (selectedHours.includes(hour)) {
      setSelectedHours(selectedHours.filter(h => h !== hour));
    } else {
      setSelectedHours([...selectedHours, hour]);
    }
  };

  const handleSubmit = async () => {
    try {
      const requests = selectedHours.map(hour => {
        const time = new Date(date);
        time.setHours(hour, 0, 0, 0);
        return axios.post(`${API_BASE_URL}/Schedule`, {
          time: time.toISOString(),
          type: 'availableForWalk',
          userId: null,
          animalId: id
        });
      });
      await Promise.all(requests);
      alert('Schedule entries created successfully');
      setSelectedHours([]);

      // Refetch reserved hours after submission
      const fetchReservedHours = async () => {
        const response = await axios.get(`${API_BASE_URL}/Schedule/Animal/${id}`);
        const scheduleEntries = response.data;
        const dateEntries = scheduleEntries.filter(entry => {
          const entryDate = new Date(entry.time).toISOString().split('T')[0];
          return entryDate === date;
        });
        const hours = dateEntries.map(entry => new Date(entry.time).getHours());
        setReservedHours(hours);
      };
      fetchReservedHours();
    } catch (error) {
      console.error('Error creating schedule entries:', error);
      alert('Failed to create schedule entries');
    }
  };

  // Hours from 8 to 20
  const hours = Array.from({ length: 13 }, (_, i) => i + 8);

  return (
    <div className="container">
      <h1>Animal Details</h1>
      <div className="animalDetail">
        <h2>{animal.name}</h2>
        <p>Age: {animal.age}</p>
        <p>Sex: {animal.sex}</p>
      </div>
      <div className="schedulePanel">
        <h2>Schedule Availability for Walks</h2>
        <label>
          Select Date:
          <input
            type="date"
            value={date}
            onChange={(e) => setDate(e.target.value)}
          />
        </label>
        <div className="hourButtons">
          {hours.map(hour => (
            <button
              key={hour}
              className={`hourButton ${selectedHours.includes(hour) ? 'selected' : ''} ${reservedHours.includes(hour) ? 'reserved' : ''}`}
              onClick={() => handleHourClick(hour)}
              disabled={reservedHours.includes(hour)}
            >
              {hour}:00
            </button>
          ))}
        </div>
        <button className="submitButton" onClick={handleSubmit}>Send</button>
      </div>
      <button className="backButton" onClick={() => navigate(-1)}>Back</button>
    </div>
  );
};

export default AnimalDetails;