import React, { useState, useEffect } from 'react';
import { apiClient, getUserId } from '../../../common/Axios/auth'; // Assuming Axios setup here

const OrderView = () => {
    const [orders, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [expandedOrderId, setExpandedOrderId] = useState(null);
    const [orderItems, setOrderItems] = useState({});
    const [orderTotals, setOrderTotals] = useState({}); // New state for order totals

    useEffect(() => {
        const sellerId = getUserId(); // Assuming getUserId fetches seller ID from token/session
        fetchSellerOrders(sellerId);
    }, []);

    const fetchSellerOrders = async (sellerId) => {
        try {
            const response = await apiClient.get(`/Order/SellerOrder/${sellerId}`); // API call to backend
            setOrders(response.data.$values);
            setLoading(false);
        } catch (error) {
            console.error('Error fetching seller orders:', error);
            setError('Error fetching orders');
            setLoading(false);
        }
    };

    const fetchOrderItems = async (orderId) => {
        try {
            const response = await apiClient.get(`/OrderItem/Order/${orderId}`); // API call to backend for order items
            setOrderItems(prev => ({ ...prev, [orderId]: response.data.$values }));
            calculateTotalAmount(orderId, response.data.$values); // Calculate total for the fetched order items
        } catch (error) {
            console.error('Error fetching order items:', error);
        }
    };

    const calculateTotalAmount = (orderId, items) => {
        const total = items.reduce((sum, item) => sum + (item.product?.price * item.quantity), 0);
        setOrderTotals(prev => ({ ...prev, [orderId]: total }));
    };

    const handleViewDetails = (orderId) => {
        if (expandedOrderId === orderId) {
            setExpandedOrderId(null); // Collapse if already expanded
        } else {
            setExpandedOrderId(orderId);
            fetchOrderItems(orderId);
        }
    };

    return (
        <div className="container mx-auto px-4 py-6">
            <h2 className="text-2xl font-bold mb-4">Seller Orders</h2>
            {loading ? (
                <p>Loading...</p>
            ) : error ? (
                <p>{error}</p>
            ) : orders.length === 0 ? (
                <p>No orders found.</p>
            ) : (
                <div className="overflow-x-auto">
                    <table className="table-auto w-full bg-white shadow-md rounded-lg">
                        <thead>
                            <tr className="bg-gray-200">
                                <th className="px-4 py-2 text-center">Order ID</th>
                                <th className="px-4 py-2 text-center">Order Date</th>
                                <th className="px-4 py-2 text-center">Total Amount</th>
                                <th className="px-4 py-2 text-center">Status</th>
                                <th className="px-4 py-2 text-center">Tracking Number</th>
                                <th className="px-4 py-2 text-center">Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {orders.map((order) => (
                                <React.Fragment key={order.orderID}>
                                    <tr className="border-t">
                                        <td className="px-4 py-2 text-center">{order.orderID}</td>
                                        <td className="px-4 py-2 text-center">{new Date(order.orderDate).toLocaleDateString()}</td>
                                        <td className="px-4 py-2 text-center">₹ {orderTotals[order.orderID]?.toFixed(2) || order.totalAmount?.toFixed(2)}</td>
                                        <td className="px-4 py-2 text-center">{order.orderStatus}</td>
                                        <td className="px-4 py-2 text-center">{order.trackingNumber}</td>
                                        <td className="px-4 py-2 text-center">
                                            <button
                                                onClick={() => handleViewDetails(order.orderID)}
                                                className="bg-blue-500 hover:bg-blue-600 text-white px-4 py-2 rounded-lg"
                                            >
                                                {expandedOrderId === order.orderID ? 'Hide Details' : 'View Details'}
                                            </button>
                                        </td>
                                    </tr>

                                    {expandedOrderId === order.orderID && (
                                        <tr>
                                            <td colSpan="6" className="p-4">
                                                <table className="table-auto w-full bg-gray-100 shadow-inner rounded-lg">
                                                    <thead>
                                                        <tr className="bg-gray-300">
                                                            <th className="px-4 py-2 text-center">Product Name</th>
                                                            <th className="px-4 py-2 text-center">Quantity</th>
                                                            <th className="px-4 py-2 text-center">Price</th>
                                                        </tr>
                                                    </thead>
                                                    <tbody>
                                                        {orderItems[order.orderID]?.map((item) => (
                                                            <tr key={item.orderItemID}>
                                                                <td className="px-4 py-2 text-center">{item.product?.productName}</td>
                                                                <td className="px-4 py-2 text-center">{item.quantity}</td>
                                                                <td className="px-4 py-2 text-center">₹ {(item.product?.price * item.quantity).toFixed(2)}</td>
                                                            </tr>
                                                        ))}
                                                        <tr>
                                                            <td className="px-4 py-2 text-right font-bold" colSpan="2">Total:</td>
                                                            <td className="px-4 py-2 text-center">
                                                                ₹ {orderTotals[order.orderID]?.toFixed(2) || order.totalAmount?.toFixed(2)}
                                                            </td>
                                                        </tr>
                                                    </tbody>
                                                </table>
                                            </td>
                                        </tr>
                                    )}
                                </React.Fragment>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
};

export default OrderView;