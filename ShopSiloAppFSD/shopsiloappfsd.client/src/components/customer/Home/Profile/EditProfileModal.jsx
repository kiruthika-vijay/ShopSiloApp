import React, { useState, useEffect } from 'react';
import axios from 'axios'; // Make sure axios is installed
import { apiClient } from '../../../common/Axios/auth';

const EditProfileModal = ({ onClose, title, data }) => {
    // State to hold form data
    const [formData, setFormData] = useState({});

    // Initialize the form data when the modal opens
    useEffect(() => {
        if (data) {
            setFormData(data);
        }
    }, [data]);

    // Handle input changes
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    // Handle form submission
    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            // Assuming you have an API endpoint to update the customer details
            const response = await apiClient.put(`/CustomerDetail`, formData); // Ensure the correct endpoint
            console.log("Profile updated:", response.data); // Optional: log the response
            onClose(); // Close the modal after updating
        } catch (error) {
            console.error("Error updating profile:", error);
            // Optionally handle error response (e.g., show error message)
        }
    };

    return (
        <div className="fixed inset-0 flex items-center justify-center z-50">
            <div className="bg-black opacity-50 absolute inset-0" onClick={onClose}></div>
            <div className="bg-white rounded-lg p-6 relative z-10 w-96">
                <h2 className="text-lg font-semibold mb-4">{title}</h2>
                <form onSubmit={handleSubmit}>
                    {/* Non-editable Customer ID */}
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">Customer ID</label>
                        <input
                            type="text"
                            name="id" // Name for identification
                            value={formData.customerID || ''} // Controlled component
                            readOnly // Make it non-editable
                            className="border rounded w-full p-2 bg-gray-200" // Add bg color to indicate non-editable
                        />
                    </div>
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">First Name</label>
                        <input
                            type="text"
                            name="firstName" // Add name attribute for identification
                            value={formData.firstName || ''} // Controlled component
                            onChange={handleChange} // Handle changes
                            className="border rounded w-full p-2"
                        />
                    </div>
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">Last Name</label>
                        <input
                            type="text"
                            name="lastName" // Add name attribute for identification
                            value={formData.lastName || ''} // Controlled component
                            onChange={handleChange} // Handle changes
                            className="border rounded w-full p-2"
                        />
                    </div>
                    <div className="mb-4">
                        <label className="block text-sm font-medium mb-1">Phone Number</label>
                        <input
                            type="tel"
                            name="phoneNumber" // Add name attribute for identification
                            value={formData.phoneNumber || ''} // Controlled component
                            onChange={handleChange} // Handle changes
                            className="border rounded w-full p-2"
                        />
                    </div>
                    {/* Add other fields as needed */}
                    <div className="flex justify-end mt-4">
                        <button
                            type="button"
                            className="bg-gray-300 text-gray-700 rounded px-4 py-2 mr-2"
                            onClick={onClose}
                        >
                            Cancel
                        </button>
                        <button
                            type="submit"
                            className="bg-blue-500 text-white rounded px-4 py-2"
                        >
                            Save
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default EditProfileModal;
