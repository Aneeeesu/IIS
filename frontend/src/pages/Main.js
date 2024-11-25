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
  const [vetRequests, setVetRequests] = useState([]);
  const [futureVetVisits, setFutureVetVisits] = useState([]);
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

  useEffect(() => {
    if (!user) return;

    const checkVerificationRequest = async () => {
      try {
        const response = await axios.get(`${API_BASE_URL}/VerificationRequests`);
        const userRequest = response.data.find(request => request.requestee.id === user.id);
        setVerificationRequestSent(!!userRequest);
      } catch (error) {
        console.error('Error checking verification request:', error);
      }
    };

    checkVerificationRequest();

  }, [user]);

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
          endTime: new Date(new Date(schedule.time).getTime() + 3600000),
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

  useEffect(() => {
    if (user && user.roles.includes('Vet')) {
      const fetchVetRequests = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/ReservationRequests`);
          const requests = response.data.filter(request => request.type === 'vetVisit' && !request.resolved && request.targetUser.id === user.id);
          setVetRequests(requests);
        } catch (error) {
          console.error('Error fetching vet requests:', error);
        }
      };

      const fetchFutureVetVisits = async () => {
        try {
          const response = await axios.get(`${API_BASE_URL}/Schedule/user/${user.id}`);
          const futureVisits = response.data.filter(schedule => new Date(schedule.time) > new Date());

          const animalIds = [...new Set(futureVisits.map(visit => visit.animalId))];
          const animalRequests = animalIds.map(id => axios.get(`${API_BASE_URL}/Animal/${id}`));
          const animalResponses = await Promise.all(animalRequests);

          const animalMap = {};
          animalResponses.forEach(animalRes => {
            const animal = animalRes.data;
            animalMap[animal.id] = animal.name;
          });

          const visitsWithAnimalNames = futureVisits.map(visit => ({
            ...visit,
            animalName: animalMap[visit.animalId] || visit.animalId,
          }));

          setFutureVetVisits(visitsWithAnimalNames);
        } catch (error) {
          console.error('Error fetching future vet visits:', error);
        }
      };

      fetchVetRequests();
      fetchFutureVetVisits();
    }
  }, [user]);

  const handleSendVerificationRequest = async () => {
    try {
      await axios.post(`${API_BASE_URL}/VerificationRequests`, {
        requesteeID: user.id,
      });
      setVerificationRequestSent(true);
      localStorage.setItem('verificationRequestSent', 'true');
    } catch (error) {
      console.error('Error sending verification request:', error);
    }
  };

  const handleResolveRequest = async (requestId, approved) => {
    try {
      await axios.post(`${API_BASE_URL}/ReservationRequests/Resolve/${requestId}?Approved=${approved}`);
      setVetRequests(vetRequests.filter(request => request.id !== requestId));
    } catch (error) {
      console.error('Error resolving request:', error);
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
      <div className="working-hours">
        <p>We work from 7:00 to 18:00</p>
      </div>

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

      {user && user.roles.includes('Vet') && (
        <div className="vetRequests">
          <div className="schedulePanel">
          <h2>Vet visit requests</h2>
            {vetRequests.length > 0 ? (
              vetRequests.map(request => (
                <div key={request.id} className="requestItem">
                  <p><strong>Animal:</strong> {request.animal.name}</p>
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
              <p>No pending vet visit requests.</p>
            )}
          </div>
          <div className="schedulePanel">
            <h2>Future vet visits</h2>
            <div className="grid-container">
              {futureVetVisits.length > 0 ? (
                futureVetVisits.map(visit => (
                  <div key={visit.id} className="visitItem">
                    <p><strong>Animal:</strong> {visit.animalName}</p>
                    <p><strong>Time:</strong> {new Date(visit.time).toLocaleString()}</p>
                  </div>
                ))
              ) : (
                <p>No future vet visits.</p>
              )}
            </div>
          </div>
        </div>
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

      {user && !user.roles.includes('Admin') && user.roles.includes('Volunteer') && !verificationRequestSent && (
        <button className="button" onClick={handleSendVerificationRequest}>
          Request to be a verified volunteer
        </button>
      )}

      {user && verificationRequestSent && user.roles.includes('Volunteer') && (
        <p>Your verification request has been sent.</p>
      )}

      <AnimalList animals={animals} user={user} />
    </div>
  );
};

export default Main;