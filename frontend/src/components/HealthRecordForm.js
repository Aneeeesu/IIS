import React, { useState } from 'react';
import axios from 'axios';
import { API_BASE_URL } from '../config';

const HealthRecordForm = ({ user, animalId }) => {
  const [date, setDate] = useState(new Date().toISOString().split('T')[0]);
  const [content, setContent] = useState('');
  const [type, setType] = useState('vaccine');

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post(`${API_BASE_URL}/HealthRecords`, {
        time: date,
        content,
        type,
        animalId,
        vetId: user.id
      });
      setContent('');
    } catch (error) {
      console.error('Error adding health record:', error);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Add health record</h2>
      <label>
        Date:
        <input type="date" value={date} onChange={(e) => setDate(e.target.value)} />
      </label>
      <label>
        Content:
        <textarea maxLength={100} value={content} onChange={(e) => setContent(e.target.value)} />
      </label>
      <label>
        Type:
        <select value={type} onChange={(e) => setType(e.target.value)}>
          <option value="vaccine">Vaccine</option>
          <option value="visit">Visit</option>
        </select>
      </label>
      <button type="submit">Add record</button>
    </form>
  );
};

export default HealthRecordForm;