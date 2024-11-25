import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import axios from 'axios';
import { API_BASE_URL } from '../config';
import VetVisitRequest from '../components/VetVisitRequest';
import HealthRecordForm from '../components/HealthRecordForm';
import HealthRecordList from '../components/HealthRecordList';

axios.defaults.withCredentials = true;

const AnimalDetails = () => {
  const [animal, setAnimal] = useState(null);
  const [selectedHours, setSelectedHours] = useState([]);
  const [reservedHours, setReservedHours] = useState([]);
  const [availableHours, setAvailableHours] = useState([]);
  const [selectedVolunteerHours, setSelectedVolunteerHours] = useState([]);
  const [pendingRequests, setPendingRequests] = useState([]);
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const [errorMessage, setErrorMessage] = useState('');
  const { id } = useParams();
  const { user } = useAuth();
  const navigate = useNavigate();
  const hours = Array.from({ length: 10 }, (_, i) => i + 8);

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
          time.setUTCHours(hour, 0, 0, 0);
          
          return axios.post(`${API_BASE_URL}/Schedule`, {
            time: time.toISOString(),
            type: 'availableForWalk',
            userId: null,
            animalId: id
          });
        });
        await Promise.all(requests);
        setSelectedHours([]);
  
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

  const handleStatusChange = async () => {
    if (!user || !user.roles.includes('Caregiver')) {
      return;
    }
  
    const newStatus = animal.lastStatus === 'Available' ? 'OnWalk' : 'Available';
    try {
      await axios.post(`${API_BASE_URL}/Animal/Status`, {
        animalId: animal.id,
        status: newStatus,
        associatedUserId: user.id
      });
      const updatedAnimal = await axios.get(`${API_BASE_URL}/Animal/${animal.id}`);
      setAnimal(updatedAnimal.data);
    } catch (error) {
      console.error('Error changing animal status:', error);
    }
  };

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
          time.setUTCHours(hour, 0, 0, 0);
          return axios.post(`${API_BASE_URL}/ReservationRequests`, {
            time: time.toISOString(),
            type: 'walk',
            targetUserId: user.id,
            animalId: id
          });
        });
        await Promise.all(requests);
        setSelectedVolunteerHours([]);
        setErrorMessage('');

        const response = await axios.get(`${API_BASE_URL}/Schedule/Animal/${id}`);
        const scheduleEntries = response.data;
        const dateEntries = scheduleEntries.filter(entry => {
          const entryDate = new Date(entry.time).toISOString().split('T')[0];
          return entryDate === date && entry.type === 'availableForWalk';
        });
        const hours = dateEntries.map(entry => new Date(entry.time).getHours());
        setAvailableHours(hours);
      } catch (error) {
        setErrorMessage('Error. Date must be in the future.');
      }
    }
  };

  const handleResolveRequest = async (requestId, approved) => {
    try {
      await axios.post(`${API_BASE_URL}/ReservationRequests/Resolve/${requestId}?Approved=${approved}`);
      const response = await axios.get(`${API_BASE_URL}/ReservationRequests`);
      const requests = response.data.filter(request => request.type === 'walk' && !request.resolved && request.animal.id === id);
      setPendingRequests(requests);
    } catch (error) {
      console.error('Error resolving request:', error);
    }
  };

  return (
    <div className="container">
      <h1>Animal details</h1>
      <div className="animalDetail">
        <img src={animal.image.url} alt={animal.name} />
        <h2>{animal.name}</h2>
        <p>Birth: {new Date(animal.dateOfBirth).toLocaleDateString()}</p>
        <p>Sex: {animal.sex === 'M' ? 'Male' : 'Female'}</p>
        <h2 
          onClick={user?.roles.includes('Caregiver') ? handleStatusChange : undefined}
          className={`status ${animal.lastStatus === 'Available' ? 'available' : 'onWalk'} ${!user?.roles.includes('Caregiver') ? 'non-clickable' : ''}`}
        >
          {animal.lastStatus === 'Available' ? 'Available' : 'On walk'}
        </h2>
      </div>

      {user && user.roles.includes('Caregiver') && (
        <>
          <div className="schedulePanel">
            <h2>Schedule availability for walks</h2>
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

          <div className="schedulePanel">
            <h2>Pending walk requests</h2>
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
          {user.roles.includes('Caregiver') && <VetVisitRequest animalId={id} userId={user.id} />}
        </>
      )}

      {user && user.roles.includes('Verified volunteer') && (
        <div className="schedulePanel">
          <h2>Request to walk</h2>
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
                className={`hourButton ${selectedVolunteerHours.includes(hour) ? 'selected' : ''} ${!availableHours.includes(hour) ? 'unavailable' : ''}`}
                onClick={() => handleVolunteerHourClick(hour)}
                disabled={!availableHours.includes(hour)}
              >
                {hour}:00
              </button>
            ))}
          </div>
          <button className="submitButton" onClick={handleVolunteerSubmit}>Send request</button>
          {errorMessage && (
            <div className="error-message">
              {errorMessage}
            </div>
          )}
        </div>
      )}

      {user && user.roles.includes('Vet') && (
        <HealthRecordForm user={user} animalId={id} />
      )}

      <HealthRecordList animalId={id} />

      <button className="backButton" onClick={() => navigate(-1)}>Back</button>
    </div>
  );
};

export default AnimalDetails;