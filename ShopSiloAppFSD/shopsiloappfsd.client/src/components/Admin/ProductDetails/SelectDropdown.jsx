// src/components/Categories.js
import React from 'react';
//how to create generic control.
const SelectDropdown = ({ data, onSelect, title }) => {
    return (
        console.log(data),
        <select
            className="border rounded p-2"
            onChange={(e) => onSelect(e.target.value)}
        >
            <option value="">{title}</option>
            {data?.map((item) => (
                <option key={item.id} value={item.id}>
                    {item.name}
                </option>
            ))}
        </select>
    );
};

export default SelectDropdown;