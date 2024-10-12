import React, { useState } from 'react';

const AnimalForm = () => {
  const [animalInfo, setAnimalInfo] = useState({
    name: '',
    age: '',
    sex: 'M',
  });

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setAnimalInfo({
      ...animalInfo,
      [name]: value,
    });
  };

  return (
    <div>
      <form>
        <label>
          Name:
          <input
            type="text"
            name="name"
            value={animalInfo.name}
            onChange={handleInputChange}
          />
        </label>
        <br />
        <label>
          Age:
          <input
            type="number"
            name="age"
            value={animalInfo.age}
            onChange={handleInputChange}
          />
        </label>
        <br />
        <label>
          Sex:
          <select
            name="sex"
            value={animalInfo.sex}
            onChange={handleInputChange}
          >
            <option value="M">Male</option>
            <option value="F">Female</option>
          </select>
        </label>
        <br />
        <button type="submit">Submit</button>
      </form>

      <h3>Animal Info:</h3>
      <p>Name: {animalInfo.name}</p>
      <p>Age: {animalInfo.age}</p>
      <p>Sex: {animalInfo.sex}</p>
    </div>
  );
};

export default AnimalForm;
