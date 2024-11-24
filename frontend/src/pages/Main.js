import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import axios from 'axios';
import { API_BASE_URL } from '../config';
import VolunteerSection from '../components/VolunteerSection';
import CaregiverSection from '../components/CaregiverSection';
import AdminSection from '../components/AdminSection';
import AnimalList from '../components/AnimalList';

axios.defaults.withCredentials = true;

const Main = () => {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [animals, setAnimals] = useState([]);
  const [pendingRequests, setPendingRequests] = useState([]);
  const [approvedSchedules, setApprovedSchedules] = useState([]);
  const [verificationRequestSent, setVerificationRequestSent] = useState(false);
  const [verificationRequests, setVerificationRequests] = useState([]);
  const [showPastWalks, setShowPastWalks] = useState(false);

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

  useEffect(() => {
    if (user && user.roles.includes('Caregiver')) {
      const fetchVerificationRequests = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/VerificationRequests`);
          setVerificationRequests(response.data);
        } catch (error) {
          console.error('Error fetching verification requests:', error);
        }
      };

      fetchVerificationRequests();
    }
  }, [user]);

  const handleSendVerificationRequest = async () => {
    try {
      await axios.post(`${API_BASE_URL}/VerificationRequests`, {
        requesteeID: user.id,
        content: 'Request to be a verified volunteer'
      });
      setVerificationRequestSent(true);
    } catch (error) {
      console.error('Error sending verification request:', error);
    }
  };

  const handleResolveVerificationRequest = async (requestId, approved) => {
    try {
      await axios.post(`${API_BASE_URL}/VerificationRequests/Resolve/${requestId}`, null, {
        params: { Approved: approved }
      });
      setVerificationRequests(verificationRequests.filter(request => request.id !== requestId));
    } catch (error) {
      console.error('Error resolving verification request:', error);
    }
  };

  const toggleShowPastWalks = () => {
    setShowPastWalks(!showPastWalks);
  };

  const currentDateTime = new Date();

  return (
    <div className="container">
      <h1>Welcome to IIS</h1>

      {user && user.roles.includes('Verified volunteer') && (
        <VolunteerSection
          pendingRequests={pendingRequests}
          approvedSchedules={approvedSchedules}
          showPastWalks={showPastWalks}
          toggleShowPastWalks={toggleShowPastWalks}
          currentDateTime={currentDateTime}
          formatTimeRange={formatTimeRange}
        />
      )}

      {user && user.roles.includes('Caregiver') && (
        <CaregiverSection
          verificationRequests={verificationRequests}
          handleResolveVerificationRequest={handleResolveVerificationRequest}
          navigate={navigate}
        />
      )}

      {user && (
        <AdminSection navigate={navigate} />
      )}

      {user && !user.roles.includes('Verified volunteer') && !verificationRequestSent && (
        <button className="button" onClick={handleSendVerificationRequest}>
          Request to be a Verified Volunteer
        </button>
      )}

      {verificationRequestSent && (
        <p>Your verification request has been sent.</p>
      )}

      <AnimalList animals={animals} />
    </div>
  );
};

export default Main;