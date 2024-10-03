import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';

const CustomerOrderItem = ({ orderID }) => {
    const [customerOrderItems, setCustomerOrderItems] = useState([]);

    useEffect(() => {
        fetchCustomerOrderItems();
    }, [orderID]);

    const fetchCustomerOrderItems = async () => {
        try {
            const response = await apiClient.get(`/OrderItem/Order/${orderID}`);
            setCustomerOrderItems(response.data.$values);
        } catch (error) {
            console.error('Error fetching order items:', error);
        }
    };

    return (
        <div className="container mx-auto p-6">
            <h2 className="text-2xl font-bold text-gray-800 mb-4">Customer Order Items</h2>
            <table className="min-w-full bg-white border border-gray-300 rounded-lg overflow-hidden">
                <thead className="bg-gray-100">
                    <tr>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Order Item ID</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Order ID</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Product ID</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Product Name</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Product description</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Quantity</th>
                        <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Price</th>

                    </tr>
                </thead>
                <tbody>
                    {customerOrderItems.map((item) => (
                        <tr key={item.orderItemID} className="hover:bg-gray-50">
                            <td className="px-6 py-4">{item.orderItemID}</td>
                            <td className="px-6 py-4">{item.orderID}</td>
                            <td className="px-6 py-4">{item.productID}</td>
                            <td className="px-6 py-4">{item.product.productName}</td>
                            <td className="px-6 py-4">{item.product.description}</td>
                            <td className="px-6 py-4">{item.quantity}</td>
                            {/* Display price with ₹ symbol */}
                            <td className="px-6 py-4">₹{item.price}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
};

export default CustomerOrderItem;