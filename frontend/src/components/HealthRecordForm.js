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
      await axios.post(`${API_BASE_URL}/HealthRecords`, {
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
    <form onSubmit={handleSubmit} className="health-record-form">
      <h2>Add health record</h2>

      <label>
        Content:
        <textarea
          maxLength={100}
          value={content}
          onChange={(e) => setContent(e.target.value)}
          className="textarea"
        />
      </label>

      <div className="date-type-container">
        <label>
          Date:
          <input
            type="date"
            value={date}
            onChange={(e) => setDate(e.target.value)}
            className="input-small"
          />
        </label>

        <label>
          Type:
          <select
            value={type}
            onChange={(e) => setType(e.target.value)}
            className="select-small"
          >
            <option value="vaccine">Vaccine</option>
            <option value="visit">Visit</option>
          </select>
        </label>
      </div>

      <button type="submit" className="button">Add record</button>
    </form>
  );
};

export default HealthRecordForm;