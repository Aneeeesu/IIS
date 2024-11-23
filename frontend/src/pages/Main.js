import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import axios from 'axios';
import { API_BASE_URL } from '../config';

axios.defaults.withCredentials = true;

const Main = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [animals, setAnimals] = useState([]);
  const [pendingRequests, setPendingRequests] = useState([]);
  const [approvedSchedules, setApprovedSchedules] = useState([]);

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

  const combineConsecutiveSchedules = (schedules) => {
    const sortedSchedules = [...schedules].sort((a, b) => 
      new Date(a.time).getTime() - new Date(b.time).getTime()
    );

    const combinedSchedules = [];
    let currentGroup = null;

    sortedSchedules.forEach((schedule) => {
      if (!currentGroup) {
        currentGroup = {
          ...schedule,
          startTime: new Date(schedule.time),
          endTime: new Date(new Date(schedule.time).getTime() + 3600000), // +1 hour
        };
      } else {
        const currentEnd = currentGroup.endTime;
        const nextStart = new Date(schedule.time);

        if (currentGroup.animalId === schedule.animalId && 
            nextStart.getTime() === currentEnd.getTime()) {
          currentGroup.endTime = new Date(nextStart.getTime() + 3600000);
        } else {
          combinedSchedules.push(currentGroup);
          currentGroup = {
            ...schedule,
            startTime: new Date(schedule.time),
            endTime: new Date(new Date(schedule.time).getTime() + 3600000),
          };
        }
      }
    });

    if (currentGroup) {
      combinedSchedules.push(currentGroup);
    }

    return combinedSchedules;
  };

  const formatTimeRange = (startTime, endTime) => {
    const formatTime = (date) => date.toLocaleTimeString([], { 
      hour: '2-digit', 
      minute: '2-digit'
    });
    
    if (startTime.toDateString() === new Date().toDateString()) {
      return `Today, ${formatTime(startTime)} - ${formatTime(endTime)}`;
    }
    return `${startTime.toLocaleDateString()}, ${formatTime(startTime)} - ${formatTime(endTime)}`;
  };

  useEffect(() => {
    if (user && user.roles.includes('Verified volunteer')) {
      const fetchPendingRequests = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/ReservationRequests`);
          const requests = response.data.filter(
            request => request.creator.id === user.id && !request.resolved
          );

          // Fetch animal details
          const animalIds = [...new Set(requests.map(request => request.animal.id))];
          const animalRequests = animalIds.map(id => axios.get(`${API_BASE_URL}/Animal/${id}`));
          const animalResponses = await Promise.all(animalRequests);

          const animalMap = {};
          animalResponses.forEach(animalRes => {
            const animal = animalRes.data;
            animalMap[animal.id] = animal.name;
          });

          const requestsWithAnimalNames = requests.map(request => ({
            ...request,
            animalName: animalMap[request.animal.id] || request.animal.id,
          }));

          setPendingRequests(requestsWithAnimalNames);
        } catch (error) {
          console.error('Error fetching pending requests:', error);
        }
      };

      const fetchApprovedSchedules = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/Schedule`);
          const schedules = response.data.filter(schedule => schedule.userId === user.id);

          const animalIds = [...new Set(schedules.map(schedule => schedule.animalId))];
          const animalRequests = animalIds.map(id => axios.get(`${API_BASE_URL}/Animal/${id}`));
          const animalResponses = await Promise.all(animalRequests);

          const animalMap = {};
          animalResponses.forEach(animalRes => {
            const animal = animalRes.data;
            animalMap[animal.id] = animal.name;
          });

          const schedulesWithAnimalNames = schedules.map(schedule => ({
            ...schedule,
            animalName: animalMap[schedule.animalId] || schedule.animalId,
          }));

          const combinedSchedules = combineConsecutiveSchedules(schedulesWithAnimalNames);
          setApprovedSchedules(combinedSchedules);
        } catch (error) {
          console.error('Error fetching approved schedules:', error);
        }
      };

      fetchPendingRequests();
      fetchApprovedSchedules();
    }
  }, [user]);

  return (
    <div className="container">
      <h1>Welcome to IIS</h1>

      {user && user.roles.includes('Verified volunteer') && (
        <div className="volunteerSection">
          <h2>Your pending walk requests</h2>
          {pendingRequests.length > 0 ? (
            pendingRequests.map(request => (
              <div key={request.id} className="requestItem">
                <p><strong>Animal:</strong> {request.animalName}</p>
                <p><strong>Time:</strong> {new Date(request.time).toLocaleString()}</p>
              </div>
            ))
          ) : (
            <p>You have no pending reservation requests.</p>
          )}

          <h2>Your scheduled walks</h2>
          {approvedSchedules.length > 0 ? (
            approvedSchedules.map(schedule => (
              <div key={schedule.id} className="scheduleItem">
                <p><strong>Animal:</strong> {schedule.animalName}</p>
                <p><strong>Time:</strong> {formatTimeRange(schedule.startTime, schedule.endTime)}</p>
              </div>
            ))
          ) : (
            <p>You have no scheduled walks.</p>
          )}
        </div>
      )}

      {user && user.roles.includes('Caregiver') && (
        <button
          className="button"
          onClick={() => navigate('/animals')}
        >
          Manage Animals
        </button>
      )}
      {user && user.roles.includes('Admin') && (
        <button
          className="button"
          onClick={() => navigate('/admin')}
        >
          Manage Users
        </button>
      )}

      <div className="animalList">
        <h2>Available Animals</h2>
        {animals.map(animal => (
          <div
            key={animal.id}
            className="animalCard"
            onClick={() => navigate(`/animals/${animal.id}`)}
            style={{ cursor: 'pointer' }}
          >
            <h3>{animal.name}</h3>
          </div>
        ))}
      </div>
    </div>
  );
};

export default Main;