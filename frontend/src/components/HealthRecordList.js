import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config';
import Modal from './Modal';
import '../App.css';

const HealthRecordList = ({ animalId }) => {
  const [records, setRecords] = useState([]);
  const [selectedRecord, setSelectedRecord] = useState(null);

  const fetchRecords = async () => {
    try {
      const response = await axios.get(`${API_BASE_URL}/HealthRecords/Animal/${animalId}`);
      const sortedRecords = response.data.sort((a, b) => new Date(b.time) - new Date(a.time));
      setRecords(sortedRecords);
    } catch (error) {
      console.error('Error fetching health records:', error);
    }
  };

  useEffect(() => {
    fetchRecords();
  }, [animalId]);

  const handleRecordClick = (record) => {
    setSelectedRecord(record);
  };

  const closeModal = () => {
    setSelectedRecord(null);
  };

  return (
    <div>
      <h2>Health records</h2>
      <button onClick={() => fetchRecords()}>Refresh</button>
      <div className="health-record-grid">
        {records.length > 0 ? (
          records.map(record => (
            <div key={record.id} className="recordItem">
              <div className="recordDetails">
                <p><strong>Date:</strong> {new Date(record.time).toLocaleDateString()}</p>
                <p><strong>Type:</strong> {record.type}</p>
              </div>
              <button onClick={() => handleRecordClick(record)}>View Content</button>
            </div>
          ))
        ) : (
          <p>No health records available.</p>
        )}
      </div>
      {selectedRecord && (
        <Modal isOpen={!!selectedRecord} onClose={closeModal}>
          <p><strong>Date:</strong> {new Date(selectedRecord.time).toLocaleDateString()}</p>
          <p><strong>Type:</strong> {selectedRecord.type}</p>
          <p><strong>Content:</strong> {selectedRecord.content}</p>
        </Modal>
      )}
    </div>
  );
};

export default HealthRecordList;