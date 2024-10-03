import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';
import SellerForm from './SellerForm';
import SellerProductDetails from './SellerProductDetails';

const SellerList = () => {
    const [sellers, setSellers] = useState([]);
    const [sellerToEdit, setSellerToEdit] = useState(undefined);
    const [expandedProductID, setExpandedProductID] = useState(null);
    const [isFormVisible, setIsFormVisible] = useState(false);

    useEffect(() => {
        fetchSellers();
    }, []);

    const fetchSellers = async () => {
        try {
            const response = await apiClient.get('/Seller/list');
            setSellers(response.data.$values);
        } catch (error) {
            console.error('Error fetching sellers:', error);
        }
    };

    const onDelete = async (id) => {
        try {
            await apiClient.delete(`/Seller/${id}`);
            fetchSellers();
        } catch (error) {
            console.error('Error deleting seller:', error);
        }
    };

    const onEdit = (seller) => {
        setSellerToEdit(seller);
        setIsFormVisible(true);
    };

    const onAddOrUpdate = () => {
        fetchSellers();
        setSellerToEdit(undefined);
        setIsFormVisible(false);
        setExpandedProductID(null);
    };

    const toggleProductDetails = (productID) => {
        setExpandedProductID(expandedProductID === productID ? null : productID);
    };

    const onAddNewSeller = () => {
        setSellerToEdit(undefined);
        setIsFormVisible(true);
    };

    return (
        <div className="container mx-auto p-4">
            <h2 className="text-3xl font-bold mb-6 text-center">Seller List</h2>

            {/*<button*/}
            {/*    onClick={onAddNewSeller}*/}
            {/*    className="bg-green-600 text-white px-4 py-2 rounded mb-4 transition-all hover:bg-green-700 shadow-md"*/}
            {/*>*/}
            {/*    Add New Seller*/}
            {/*</button>*/}

            <table className="w-full max-w-6xl mx-auto border border-gray-200 my-8">
                <thead>
                    <tr className="bg-gray-100 border-b">
                        <th className="py-4 px-6 text-left">Seller ID</th>
                        <th className="py-4 px-6 text-left">Company Name</th>
                        <th className="py-4 px-6 text-left">Contact Person</th>
                        <th className="py-4 px-6 text-left">Contact Number</th>
                        <th className="py-4 px-6 text-left">Address</th>
                        <th className="py-4 px-6 text-left">Store Description</th>
                        <th className="py-4 px-6 text-center">Actions</th>
                    </tr>
                </thead>
                <tbody>
                    {sellers.map((seller) => (
                        <React.Fragment key={seller.sellerID}>
                            <tr className="border-b hover:bg-gray-50">
                                <td className="py-2 px-4">{seller.sellerID}</td>
                                <td className="py-2 px-4">{seller.companyName}</td>
                                <td className="py-2 px-4">{seller.contactPerson}</td>
                                <td className="py-2 px-4">{seller.contactNumber}</td>
                                <td className="py-2 px-4">{seller.address}</td>
                                <td className="py-2 px-4">{seller.storeDescription}</td>
                                <td className="py-2 px-4 flex space-x-2">
                                    <button
                                        onClick={() => onEdit(seller)}
                                        className="bg-blue-600 text-white px-3 py-1 rounded hover:bg-blue-700 shadow-md transition-all"
                                    >
                                        Edit
                                    </button>
                                    <button
                                        onClick={() => onDelete(seller.sellerID)}
                                        className="bg-red-600 text-white px-3 py-1 rounded hover:bg-red-700 shadow-md transition-all"
                                    >
                                        Delete
                                    </button>
                                    <button
                                        onClick={() => toggleProductDetails(seller.sellerID)}
                                        className="bg-green-600 text-white px-3 py-1 rounded hover:bg-green-700 shadow-md transition-all flex items-center"
                                    >
                                        {expandedProductID === seller.sellerID ? '▲' : '▼'}
                                        <span className="ml-2">Product Details</span>
                                    </button>
                                </td>
                            </tr>
                            {expandedProductID === seller.sellerID && (
                                <tr className="bg-gray-50">
                                    <td colSpan="7">
                                        <SellerProductDetails sellerID={seller.sellerID} />
                                    </td>
                                </tr>
                            )}
                        </React.Fragment>
                    ))}
                </tbody>
            </table>

            {/* Render SellerForm for Add/Edit in Modal */}
            {isFormVisible && (
                <div className="fixed inset-0 flex items-center justify-center z-50 bg-black bg-opacity-50 overflow-y-auto">
                    <div className="bg-white p-6 rounded-lg shadow-lg max-w-md w-full h-auto">
                        <h3 className="text-xl font-bold mb-4">{sellerToEdit ? 'Edit Seller' : 'Add New Seller'}</h3>
                        <div className="overflow-y-auto max-h-85"> {/* Make this scrollable */}
                            <SellerForm onAddOrUpdate={onAddOrUpdate} sellerToEdit={sellerToEdit} />
                        </div>
                        <button
                            onClick={() => setIsFormVisible(false)}
                            className="bg-red-600 text-white px-4 py-2 rounded mt-4 hover:bg-red-700 transition-all"
                        >
                            Cancel
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default SellerList;
