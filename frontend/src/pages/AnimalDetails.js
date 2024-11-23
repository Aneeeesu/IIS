import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import axios from 'axios';
import { API_BASE_URL } from '../config';

axios.defaults.withCredentials = true;

const sleep = ms => setTimeout(ms);
const AnimalDetails = () => {
  const [animal, setAnimal] = useState(null);
  const [selectedHours, setSelectedHours] = useState([]);
  const [reservedHours, setReservedHours] = useState([]);
  const [availableHours, setAvailableHours] = useState([]);
  const [selectedVolunteerHours, setSelectedVolunteerHours] = useState([]);
  const [pendingRequests, setPendingRequests] = useState([]);
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const { id } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const hours = Array.from({ length: 6 }, (_, i) => i + 6);

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
    setSelectedHours([]);
    setSelectedVolunteerHours([]);
  }, [date]);

  useEffect(() => {
    if (user && user.roles.includes('Caregiver')) {
      const fetchReservedHours = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/Schedule/Animal/${id}`);
          const scheduleEntries = response.data;

          const dateEntries = scheduleEntries.filter(entry => {
            const entryDate = new Date(entry.time).toISOString().split('T')[0];
            return entryDate === date;
          });

          const hours = dateEntries.map(entry => new Date(entry.time).getHours());
          setReservedHours(hours);
        } catch (error) {
          console.error('Error fetching reserved hours:', error);
        }
      };

      fetchReservedHours();
    }
  }, [date, id, user]);

  useEffect(() => {
    if (user && user.roles.includes('Verified volunteer')) {
      const fetchAvailableHours = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/Schedule/Animal/${id}`);
          const scheduleEntries = response.data;

          const dateEntries = scheduleEntries.filter(entry => {
            const entryDate = new Date(entry.time).toISOString().split('T')[0];
            return entryDate === date && entry.type === 'availableForWalk';
          });

          const hours = dateEntries.map(entry => new Date(entry.time).getHours());
          setAvailableHours(hours);
        } catch (error) {
          console.error('Error fetching available hours:', error);
        }
      };

      fetchAvailableHours();
    }
  }, [date, id, user]);

  // Fetch pending requests for caregivers
  useEffect(() => {
    if (user && user.roles.includes('Caregiver')) {
      const fetchPendingRequests = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/ReservationRequests`);
          const requests = response.data.filter(request => request.type === 'walk' && !request.resolved && request.animal.id === id);
          setPendingRequests(requests);
        } catch (error) {
          console.error('Error fetching pending requests:', error);
        }
      };

      fetchPendingRequests();
    }
  }, [id, user]);

  if (!animal) return <div>Loading...</div>;

  const handleHourClick = (hour) => {
    if (selectedHours.includes(hour)) {
      const newHours = selectedHours.filter(h => h !== hour);
      setSelectedHours(newHours);
    } else {
      const newHours = [...selectedHours, hour];
      setSelectedHours(newHours);
    }
  };

  const handleSubmit = async () => {
    if (user && user.roles.includes('Caregiver')) {
      try {
        const requests = selectedHours.map(hour => {
          const time = new Date(date);
          time.setHours(hour, 0, 0, 0);

          console.log('Sending time:', time.toISOString(), 'for hour:', hour);
          
          return axios.post(`${API_BASE_URL}/Schedule`, {
            time: time.toISOString(),
            type: 'availableForWalk',
            userId: null,
            animalId: id
          });
        });
        await Promise.all(requests);
        setSelectedHours([]);

        // Refresh reserved hours
        const response = await axios.get(`${API_BASE_URL}/Schedule/Animal/${id}`);
        const scheduleEntries = response.data;
        const dateEntries = scheduleEntries.filter(entry => {
          const entryDate = new Date(entry.time).toISOString().split('T')[0];
          return entryDate === date;
        });
        const hours = dateEntries.map(entry => new Date(entry.time).getHours());
        setReservedHours(hours);
      } catch (error) {
        console.error('Error creating schedule entries:', error);
      }
    }
  };

  // Volunteer handlers
  const handleVolunteerHourClick = (hour) => {
    if (!availableHours.includes(hour)) return;
    if (selectedVolunteerHours.includes(hour)) {
      setSelectedVolunteerHours(selectedVolunteerHours.filter(h => h !== hour));
    } else {
      setSelectedVolunteerHours([...selectedVolunteerHours, hour]);
    }
  };

  const handleVolunteerSubmit = async () => {
    if (user && user.roles.includes('Verified volunteer')) {
      try {
        const requests = selectedVolunteerHours.map(hour => {
          const time = new Date(date);
          time.setHours(hour, 0, 0, 0);
          return axios.post(`${API_BASE_URL}/ReservationRequests`, {
            time: time.toISOString(),
            type: 'walk',
            targetUserId: user.id,
            animalId: id
          });
        });
        await Promise.all(requests);
        setSelectedVolunteerHours([]);

        // Refresh available hours
        const response = await axios.get(`${API_BASE_URL}/Schedule/Animal/${id}`);
        const scheduleEntries = response.data;
        const dateEntries = scheduleEntries.filter(entry => {
          const entryDate = new Date(entry.time).toISOString().split('T')[0];
          return entryDate === date && entry.type === 'availableForWalk';
        });
        const hours = dateEntries.map(entry => new Date(entry.time).getHours());
        setAvailableHours(hours);
      } catch (error) {
        console.error('Error creating reservation requests:', error);
      }
    }
  };

  // Handle approval or rejection of requests
  const handleResolveRequest = async (requestId, approved) => {
    try {
      await axios.post(`${API_BASE_URL}/ReservationRequests/Resolve/${requestId}?Approved=${approved}`);
      // Refresh pending requests
      const response = await axios.get(`${API_BASE_URL}/ReservationRequests/Animal/${id}`);
      const requests = response.data.filter(request => !request.resolved);
      setPendingRequests(requests);
    } catch (error) {
      console.error('Error resolving request:', error);
    }
  };

  return (
    <div className="container">
      <h1>Animal Details</h1>
      <div className="animalDetail">
        <h2>{animal.name}</h2>
        <p>Age: {animal.age}</p>
        <p>Sex: {animal.sex}</p>
      </div>

      {user && user.roles.includes('Caregiver') && (
        <>
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

          <div className="pendingRequests">
            <h2>Pending Walk Requests</h2>
            {pendingRequests.length > 0 ? (
              pendingRequests.map(request => (
                <div key={request.id} className="requestItem">
                  <p><strong>Volunteer:</strong> {request.creator.userName}</p>
                  <p><strong>Time:</strong> {new Date(request.time).toLocaleString()}</p>
                  <button
                    className="approveButton"
                    onClick={() => handleResolveRequest(request.id, true)}
                  >
                    Approve
                  </button>
                  <button
                    className="rejectButton"
                    onClick={() => handleResolveRequest(request.id, false)}
                  >
                    Reject
                  </button>
                </div>
              ))
            ) : (
              <p>No pending requests.</p>
            )}
          </div>
        </>
      )}

      {user && user.roles.includes('Verified volunteer') && (
        <div className="requestPanel">
          <h2>Request to Walk</h2>
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
                className={`hourButton ${selectedVolunteerHours.includes(hour) ? 'selected' : ''} ${!availableHours.includes(hour) ? 'unavailable' : ''}`}
                onClick={() => handleVolunteerHourClick(hour)}
                disabled={!availableHours.includes(hour)}
              >
                {hour}:00
              </button>
            ))}
          </div>
          <button className="submitButton" onClick={handleVolunteerSubmit}>Send Request</button>
        </div>
      )}
      <button className="backButton" onClick={() => navigate(-1)}>Back</button>
    </div>
  );
};

export default AnimalDetails;