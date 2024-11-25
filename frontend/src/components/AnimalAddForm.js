import React from 'react';
import "../App.css";

const AnimalAddForm = ({ animal, onChange, onSave, onCancel, onImageChange }) => {
  return (
    <div>
      <h2>Add animal</h2>
      <input
        type="text"
        placeholder="Name"
        className="input"
        value={animal.name}
        onChange={(e) => onChange({ ...animal, name: e.target.value })}
      />
      <input
        type="number"
        placeholder="Age"
        className="input"
        value={animal.age}
        onChange={(e) => onChange({ ...animal, age: e.target.value })}
      />
      <select
        className="input"
        value={animal.sex}
        onChange={(e) => onChange({ ...animal, sex: e.target.value })}
      >
        <option value="">Select sex</option>
        <option value="M">M</option>
        <option value="F">F</option>
      </select>
      <input
        type="date"
        className="input"
        value={animal.dateOfBirth}
        onChange={(e) => onChange({ ...animal, dateOfBirth: e.target.value })}
      />
      <input
        type="file"
        className="input"
        onChange={onImageChange}
      />
      <button className="button" onClick={onSave}>
        Add
      </button>
      <button className="button" onClick={onCancel}>
        Cancel
      </button>
    </div>
  );
};

export default AnimalAddForm;