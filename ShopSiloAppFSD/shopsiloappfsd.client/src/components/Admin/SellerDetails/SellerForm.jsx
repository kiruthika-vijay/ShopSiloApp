import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';

const SellerForm = ({ sellerToEdit, onAddOrUpdate }) => {
    const [seller, setSeller] = useState({
        sellerID: '',
        companyName: '',
        contactPerson: '',
        contactNumber: '',
        address: '',
        storeDescription: '',
        isActive: true,
    });

    useEffect(() => {
        if (sellerToEdit) {
            setSeller(sellerToEdit);
        }
    }, [sellerToEdit]);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setSeller({ ...seller, [name]: value });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const sellerPayload = {
                companyName: seller.companyName,
                contactPerson: seller.contactPerson,
                contactNumber: seller.contactNumber,
                address: seller.address,
                storeDescription: seller.storeDescription,
                isActive: seller.isActive,
            };

            if (seller.sellerID) {
                // Update existing seller
                await apiClient.put(`/Seller/${ seller.sellerID }`, { ...sellerPayload, sellerID: seller.sellerID });
            } else {
                // Add new seller, don't include sellerID
                await apiClient.post(`/Seller`, sellerPayload);
            }
            onAddOrUpdate(); // Notify parent component to refresh list
        } catch (error) {
            console.error('Error saving seller details:', error);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="bg-white shadow-md rounded px-8 pt-6 pb-8 mb-4 max-w-lg mx-auto">
            <h2 className="text-2xl font-bold mb-6 text-center">
                {seller.sellerID ? 'Edit Seller' : 'Add Seller'}
            </h2>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="companyName">
                    Company Name
                </label>
                <input
                    type="text"
                    name="companyName"
                    value={seller.companyName}
                    onChange={handleChange}
                    placeholder="Company Name"
                    required
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                />
            </div>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="contactPerson">
                    Contact Person
                </label>
                <input
                    type="text"
                    name="contactPerson"
                    value={seller.contactPerson}
                    onChange={handleChange}
                    placeholder="Contact Person"
                    required
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                />
            </div>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="contactNumber">
                    Contact Number
                </label>
                <input
                    type="text"
                    name="contactNumber"
                    value={seller.contactNumber}
                    onChange={handleChange}
                    placeholder="Contact Number"
                    required
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                />
            </div>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="address">
                    Address
                </label>
                <input
                    type="text"
                    name="address"
                    value={seller.address}
                    onChange={handleChange}
                    placeholder="Address"
                    required
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                />
            </div>
            <div className="mb-4">
                <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="storeDescription">
                    Store Description
                </label>
                <input
                    type="text"
                    name="storeDescription"
                    value={seller.storeDescription}
                    onChange={handleChange}
                    placeholder="Store Description"
                    required
                    className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
                />
            </div>
            <div className="flex items-center justify-between">
                <button
                    type="submit"
                    className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline"
                >
                    {seller.sellerID ? 'Update' : 'Add'}
                </button>
            </div>
        </form>
    );
};

export default SellerForm;