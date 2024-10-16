import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';
import CustomerForm from './CustomerForm';
import CustomerOrderDetails from './CustomerOrderDetails';

const CustomerList = () => {
    const [customers, setCustomers] = useState([]);
    const [customerToEdit, setCustomerToEdit] = useState(undefined);
    const [customerID, setCustomerID] = useState(undefined);

    useEffect(() => {
        fetchCustomers();
    }, []);

    const fetchCustomers = async () => {
        try {
            const response = await apiClient.get('/CustomerDetail/list');
            setCustomers(response.data.$values);
        } catch (error) {
            console.error('Error fetching customers:', error);
        }
    };

    const onDelete = async (id) => {
        try {
            await apiClient.delete(`/CustomerDetail/${id}`);
            fetchCustomers();
        } catch (error) {
            console.error('Error deleting customer:', error);
        }
    };

    const onEdit = (customer) => {
        setCustomerToEdit(customer);
    };

    const onAddOrUpdate = () => {
        fetchCustomers();  // Refresh the list after add or update
        setCustomerToEdit(undefined);  // Clear the form
    };

    return (
        <div className="container mx-auto p-4">
            <h2 className="text-3xl font-bold mb-6 text-center">Customer List</h2>
            <table className="min-w-full bg-white border border-gray-200">
                <thead>
                    <tr className="bg-gray-100 border-b">
                        <th className="py-2 px-4 text-left">First Name</th>
                        <th className="py-2 px-4 text-left">Last Name</th>
                        <th className="py-2 px-4 text-left">Phone Number</th>
                        <th className="py-2 px-4 text-center">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {customers.map((customer) => (
                        <tr key={customer.customerID} className="border-b hover:bg-gray-50">
                            <td className="py-2 px-4">{customer.firstName}</td>
                            <td className="py-2 px-4">{customer.lastName}</td>
                            <td className="py-2 px-4">{customer.phoneNumber}</td>
                            <td className="py-2 px-4 flex space-x-2 justify-between">
                                <button
                                    onClick={() => onEdit(customer)}
                                    className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600"
                                >
                                    Edit
                                </button>
                                <button
                                    onClick={() => onDelete(customer.customerID)}
                                    className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600"
                                >
                                    Delete
                                </button>
                                <button
                                    onClick={() => setCustomerID(customer.customerID)}
                                    className="bg-green-500 text-white px-3 py-1 rounded hover:bg-green-600"
                                >
                                    Orders
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {/* Render CustomerForm for Add/Edit */}
            {customerToEdit !== undefined && (
                <div className="mt-8">
                    <CustomerForm onAddOrUpdate={onAddOrUpdate} customerToEdit={customerToEdit} />
                </div>
            )}

            {/* Render CustomerOrderDetails if a customer is selected */}
            {customerID !== undefined && (
                <div className="mt-8">
                    <CustomerOrderDetails customerID={customerID} />
                </div>
            )}
        </div>
    );
};

export default CustomerList;