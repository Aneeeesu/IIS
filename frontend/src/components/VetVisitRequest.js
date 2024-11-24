import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config';

const VetVisitRequest = ({ animalId }) => {
  const [selectedVet, setSelectedVet] = useState(null);
  const [vets, setVets] = useState([]);
  const [selectedHours, setSelectedHours] = useState([]);
  const [vetSchedules, setVetSchedules] = useState([]);
  const [animalSchedules, setAnimalSchedules] = useState([]);
  const [availableHours, setAvailableHours] = useState([]);
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const hours = Array.from({ length: 11 }, (_, i) => i + 8);

  useEffect(() => {
    const fetchVets = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/users`);
        const allUsers = response.data;
        const vetUsers = allUsers.filter(user => user.roles.includes('Vet'));
        setVets(vetUsers);
      } catch (error) {
        console.error('Error fetching users:', error);
      }
    };

    fetchVets();
  }, []);

  useEffect(() => {
    if (selectedVet) {
      const fetchVetSchedules = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/Schedule/user/${selectedVet.id}`);
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
    }
  }, [selectedVet, animalId, date]);

  useEffect(() => {
    if (selectedVet) {
      const vetBusyHours = vetSchedules
        .filter(entry => new Date(entry.time).toISOString().split('T')[0] === date)
        .map(entry => new Date(entry.time).getHours());

      const animalBusyHours = animalSchedules
        .filter(entry => new Date(entry.time).toISOString().split('T')[0] === date)
        .map(entry => new Date(entry.time).getHours());

      const busyHours = new Set([...vetBusyHours, ...animalBusyHours]);
      const available = hours.filter(hour => !busyHours.has(hour));
      setAvailableHours(available);
    }
  }, [vetSchedules, animalSchedules, date, hours, selectedVet]);

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
          targetUserId: selectedVet.id,
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
      <h2>Request a vet visit</h2>
      <label>
        Select vet:
        <select onChange={(e) => setSelectedVet(vets.find(vet => vet.id === e.target.value))}>
          <option value="">Select a vet</option>
          {vets.map(vet => (
            <option key={vet.id} value={vet.id}>{vet.userName}</option>
          ))}
        </select>
      </label>
      {selectedVet && (
        <>
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
        </>
      )}
    </div>
  );
};

export default VetVisitRequest;