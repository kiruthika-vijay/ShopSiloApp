import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';
import CustomerOrderItem from './CustomerOrderItem';

const CustomerOrderDetails = ({ customerID }) => {
    const [customerOrder, setCustomerOrder] = useState([]);
    const [orderID, setOrderID] = useState(null);

    useEffect(() => {
        fetchCustomerOrder();
    }, [customerID]);

    const fetchCustomerOrder = async () => {
        try {
            const response = await apiClient.get(`/Order/User/${customerID}`);
            setCustomerOrder(response.data.$values);
        } catch (error) {
            console.error('Error fetching customer orders:', error);
        }
    };

    return (
        <div className="container mx-auto p-4">
            <h2 className="text-3xl font-bold mb-6 text-center">Customer Order List</h2>
            <table className="min-w-full bg-white border border-gray-200">
                <thead>
                    <tr className="bg-gray-100 border-b">
                        <th className="py-2 px-4 text-left">Order ID</th>
                        <th className="py-2 px-4 text-left">Order Date</th>
                        <th className="py-2 px-4 text-left">Total Amount</th>
                        <th className="py-2 px-4 text-left">Order Status</th>
                        <th className="py-2 px-4 text-left">Tracking Number</th>
                        <th className="py-2 px-4 text-left">Action</th>
                    </tr>
                </thead>
                <tbody>
                    {customerOrder.map((order) => (
                        <tr key={order.orderID} className="border-b hover:bg-gray-50">
                            <td className="py-2 px-4">{order.orderID}</td>
                            <td className="py-2 px-4">{order.orderDate}</td>
                            <td className="py-2 px-4">{order.totalAmount}</td>
                            <td className="py-2 px-4">{order.orderStatus}</td>
                            <td className="py-2 px-4">{order.trackingNumber}</td>
                            <td className="py-2 px-4">
                                <button
                                    onClick={() => setOrderID(order.orderID)}
                                    className="bg-green-500 text-white px-3 py-1 rounded hover:bg-green-600"
                                >
                                    View Order Items
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {orderID && (
                <div className="mt-8">
                    <CustomerOrderItem orderID={orderID} />
                </div>
            )}
        </div>
    );
};

export default CustomerOrderDetails;