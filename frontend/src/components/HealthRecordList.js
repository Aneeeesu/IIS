import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config';
import '../App.css';

const HealthRecordList = ({ animalId }) => {
  const [records, setRecords] = useState([]);

  const fetchRecords = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/HealthRecords/Animal/${animalId}`);
      setRecords(response.data);
    } catch (error) {
      console.error('Error fetching health records:', error);
    }
  };

  useEffect(() => {
    fetchRecords();
  }, [animalId]);

  return (
    <div>
      <h2>Health records</h2>
      <button onClick={() => fetchRecords()}>Refresh</button>
      {records.length > 0 ? (
        records.map(record => (
          <div key={record.id} className="recordItem">
            <p><strong>Date:</strong> {new Date(record.time).toLocaleDateString()}</p>
            <p><strong>Type:</strong> {record.type}</p>
            <p><strong>Content:</strong> {record.content}</p>
          </div>
        ))
      ) : (
        <p>No health records available.</p>
      )}
    </div>
  );
};

export default HealthRecordList;