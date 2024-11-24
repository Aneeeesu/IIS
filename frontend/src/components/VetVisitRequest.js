import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config';

const VetVisitRequest = ({ animalId, userId }) => {
  const [selectedHours, setSelectedHours] = useState([]);
  const [vetSchedules, setVetSchedules] = useState([]);
  const [animalSchedules, setAnimalSchedules] = useState([]);
  const [availableHours, setAvailableHours] = useState([]);
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const hours = Array.from({ length: 11 }, (_, i) => i + 8);

  useEffect(() => {
    const fetchVetSchedules = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/Schedule/user/${userId}`);
        setVetSchedules(response.data);
      } catch (error) {
        console.error('Error fetching vet schedules:', error);
      }
    };

    const fetchAnimalSchedules = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/Schedule/Animal/${animalId}`);
        setAnimalSchedules(response.data);
      } catch (error) {
        console.error('Error fetching animal schedules:', error);
      }
    };

    fetchVetSchedules();
    fetchAnimalSchedules();
  }, [animalId, userId, date]);

  useEffect(() => {
    const vetBusyHours = vetSchedules
      .filter(entry => new Date(entry.time).toISOString().split('T')[0] === date)
      .map(entry => new Date(entry.time).getHours());

    const animalBusyHours = animalSchedules
      .filter(entry => new Date(entry.time).toISOString().split('T')[0] === date)
      .map(entry => new Date(entry.time).getHours());

    const busyHours = new Set([...vetBusyHours, ...animalBusyHours]);
    const available = hours.filter(hour => !busyHours.has(hour));
    setAvailableHours(available);
  }, [vetSchedules, animalSchedules, date, hours]);

  const handleHourClick = (hour) => {
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
        time.setUTCHours(hour, 0, 0, 0);
        return axios.post(`${API_BASE_URL}/ReservationRequests`, {
          time: time.toISOString(),
          type: 'vetVisit',
          targetUserId: userId,
          animalId: animalId
        });
      });
      await Promise.all(requests);
      setSelectedHours([]);
      alert('Vet visit request sent successfully');
    } catch (error) {
      console.error('Error creating vet visit requests:', error);
    }
  };

  return (
    <div className="vetVisitRequest">
      <h2>Request a Vet Visit</h2>
      <label>
        Select date:
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
            className={`hourButton ${selectedHours.includes(hour) ? 'selected' : ''} ${!availableHours.includes(hour) ? 'unavailable' : ''}`}
            onClick={() => handleHourClick(hour)}
            disabled={!availableHours.includes(hour)}
          >
            {hour}:00
          </button>
        ))}
      </div>
      <button className="submitButton" onClick={handleSubmit}>Send request</button>
    </div>
  );
};

export default VetVisitRequest;