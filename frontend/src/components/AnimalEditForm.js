import React from 'react';
import "../App.css";

const AnimalEditForm = ({ animal, onChange, onSave }) => {
  return (
    <div>
      <h2>Edit Animal</h2>
      <input
        type="text"
        placeholder="Name"
        className="input"
        value={animal.name}
        onChange={(e) => onChange({ ...animal, name: e.target.value })}
      />
      <input
        type="text"
        placeholder="Age"
        className="input"
        value={animal.age}
        onChange={(e) => onChange({ ...animal, age: e.target.value })}
      />
      <input
        type="text"
        placeholder="Sex"
        className="input"
        value={animal.sex}
        onChange={(e) => onChange({ ...animal, sex: e.target.value })}
      />
      <button className="button" onClick={onSave}>
        Save
      </button>
    </div>
  );
};

export default AnimalEditForm;