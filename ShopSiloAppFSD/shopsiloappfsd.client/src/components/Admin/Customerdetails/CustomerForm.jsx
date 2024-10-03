import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';

const CustomerForm = ({ customerToEdit, onAddOrUpdate }) => {
    const [customer, setCustomer] = useState({
        firstName: '',
        lastName: '',
        phoneNumber: '',
        isActive: true,
    });

    useEffect(() => {
        if (customerToEdit) {
            setCustomer(customerToEdit);
        }
    }, [customerToEdit]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setCustomer({ ...customer, [name]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (customer.customerID) {
                // Update existing customer
                await apiClient.put(`/CustomerDetail`, customer);
            } else {
                // Add new customer
                await apiClient.post(`/CustomerDetail`, customer); //Assuming singular API route
            }
            onAddOrUpdate(); // Notify parent component to refresh list
        } catch (error) {
            console.error('Error saving customer details:', error);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4 max-w-lg mx-auto">
            <h2 className="text-2xl font-bold mb-6 text-center">
                {customer.customerID ? 'Edit Customer' : 'Add Customer'}
            </h2>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="firstName">
                    First Name
                </label>
                <input
                    type="text"
                    name="firstName"
                    value={customer.firstName}
                    onChange={handleChange}
                    placeholder="First Name"
                    required
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                />
            </div>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="lastName">
                    Last Name
                </label>
                <input
                    type="text"
                    name="lastName"
                    value={customer.lastName}
                    onChange={handleChange}
                    placeholder="Last Name"
                    required
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                />
            </div>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="phoneNumber">
                    Phone Number
                </label>
                <input
                    type="text"
                    name="phoneNumber"
                    value={customer.phoneNumber}
                    onChange={handleChange}
                    placeholder="Phone Number"
                    required
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                />
            </div>
            <div className="flex items-center justify-between">
                <button
                    type="submit"
                    className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                >
                    {customer.customerID ? 'Update' : 'Add'}
                </button>
            </div>
        </form>
    );
};

export default CustomerForm;