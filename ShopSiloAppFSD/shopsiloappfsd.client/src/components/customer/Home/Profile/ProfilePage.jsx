import React, { useState, useEffect } from 'react';
import { MdDelete } from "react-icons/md";
import { BiSolidCopy } from "react-icons/bi";
import {
    FaUserCircle,
    FaEdit,
    FaAddressBook,
    FaHistory,
    FaCreditCard,
    FaHeart,
    FaCog,
    FaDownload,
    FaTimes
} from 'react-icons/fa';
import './ProfilePage.css';
import { apiClient, fetchUserId, getToken, getUserId } from '../../../common/Axios/auth';
import EditProfileModal from './EditProfileModal';
import AddressModal from './AddressModal';
import Toggle from 'react-toggle';  // For the toggle switches
import 'react-toggle/style.css';    // For toggle styling
import ChangePasswordModal from './ChangePasswordModal';
import CustomerDetailsForm from './CustomerDetailsForm';

const ProfilePage = () => {
    const [token, setToken] = useState(null);
    const [activeTab, setActiveTab] = useState('personal');
    const [user, setUser] = useState({});
    const [addresses, setAddresses] = useState([]);
    const [orders, setOrders] = useState([]);
    const [payments, setPayments] = useState([]);
    const [wishlist, setWishlist] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [modalOpen, setModalOpen] = useState(false);
    const [addressModalOpen, setAddressModalOpen] = useState(false);
    const [changePasswordModalOpen, setChangePasswordModalOpen] = useState(false);
    const [editData, setEditData] = useState(null);
    const [showCustomerForm, setShowCustomerForm] = useState(false);
    const [userId, setUserId] = useState(null);

    useEffect(() => {
        const token = getToken();  // Assuming getToken() is a function that retrieves the token
        setToken(token);

        const loadUserIdAndData = async () => {
            const userId = await fetchUserId();  // Wait for the user ID to be fetched
            setUserId(userId);  // Set the userId state
            if (userId) {
                await fetchAllData(token);  // Fetch data once userId is available
            }
        };

        loadUserIdAndData();  // Call the async function

    }, []);  // Empty dependency array to run on component mount


    const fetchAllData = async (token) => {
        setLoading(true);
        setError(null);
        try {
            const [profileResponse, ordersResponse, paymentsResponse, wishlistResponse] = await Promise.all([
                apiClient.get('/CustomerDetail/details', { headers: { Authorization: `Bearer ${token}` } }),
                apiClient.get('/Order/user/orders', { headers: { Authorization: `Bearer ${token}` } }),
                apiClient.get('/Payment/userpayments', { headers: { Authorization: `Bearer ${token}` } }),
                apiClient.get('/Wishlists', { headers: { Authorization: `Bearer ${token}` } }),
            ]);
            const userData = profileResponse.data.$values[0];

            if (!userData) {
                setShowCustomerForm(true); // Show form if no details exist
            } else {
                setUser(userData);
                setAddresses(profileResponse.data.$values[0].addresses.$values || []);
                setOrders(ordersResponse.data.$values || []);
                setPayments(paymentsResponse.data.$values);
                setWishlist(wishlistResponse.data.$values || []);
            }
        } catch (err) {
            console.error('Error fetching data:', err);
            setError('Failed to fetch data');
        } finally {
            setLoading(false);
        }
    };

    // Callback for closing modal and refetching details
    const handleAddForm = () => {
        setIsModalOpen(false);
    };

    const handleEditProfile = (data) => {
        setEditData(data);
        setModalOpen(true);
    };

    const handleChangePassword = () => {
        setChangePasswordModalOpen(true);
    };

    // Function to open the modal for adding a new address
    const handleAddAddress = () => {
        setAddressModalOpen(true);
    };

    const handleSaveUpdatedProfile = (updatedData) => {
        setUser(prevUser => ({
            ...prevUser,
            ...updatedData // Merge updated data into the user state
        }));
    };

    const handleSaveAddress = (newAddress) => {
        setAddresses(prevAddress => ({
            ...prevAddress,
            ...newAddress // Merge updated data into the user state
        }));
    };

    const handleDeleteAddress = async (addressId) => {
        if (window.confirm("Are you sure you want to delete this address?")) {
            try {
                await apiClient.delete(`/ShippingAddress/${addressId}`, {
                    headers: { Authorization: `Bearer ${token}` }
                });

                // Update the state to remove the deleted address
                setAddresses(prevAddresses => prevAddresses.filter(address => address.addressID !== addressId));
                alert('Address deleted successfully!');
            } catch (error) {
                console.error('Error deleting address:', error);
                alert('Failed to delete address. Please try again.');
            }
        }
    };

    const handleToggleAddress = async (addressId) => {
        // Optimistically update the address's active status in the UI
        setAddresses(prevAddresses => {
            const updatedAddresses = prevAddresses.map(address =>
                address.addressID === addressId
                    ? { ...address, isActive: !address.isActive } // Toggle the isActive state
                    : address
            );
            return updatedAddresses; // Return the new addresses array
        });

        try {
            // Make the API call to toggle the address
            await apiClient.put(`/ShippingAddress/toggle/${addressId}`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });

            // Optionally display a success message or update further state here
            console.log('Address toggled successfully!');
        } catch (error) {
            console.error('Error toggling address:', error);

            // Rollback the change if the API call fails
            setAddresses(prevAddresses =>
                prevAddresses.map(address =>
                    address.addressID === addressId
                        ? { ...address, isActive: !address.isActive } // Revert the active status
                        : address
                )
            );

            alert('Failed to toggle address. Please try again.');
        }
    };

    const handleToggleBillingAddress = async (addressId) => {
        // Optimistically update the address's active status in the UI
        setAddresses(prevAddresses => {
            const updatedAddresses = prevAddresses.map(address =>
                address.addressID === addressId
                    ? { ...address, isBillingAddress: !address.isBillingAddress } // Toggle the isActive state
                    : address
            );
            return updatedAddresses; // Return the new addresses array
        });

        try {
            // Make the API call to toggle the address
            await apiClient.put(`/ShippingAddress/toggleBillingAddress/${addressId}`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });

            // Optionally display a success message or update further state here
            console.log('Address toggled successfully!');
        } catch (error) {
            console.error('Error toggling address:', error);

            // Rollback the change if the API call fails
            setAddresses(prevAddresses =>
                prevAddresses.map(address =>
                    address.addressID === addressId
                        ? { ...address, isBillingAddress: !address.isBillingAddress } // Revert the active status
                        : address
                )
            );

            alert('Failed to toggle address. Please try again.');
        }
    };

    const handleToggleShippingAddress = async (addressId) => {
        // Optimistically update the address's active status in the UI
        setAddresses(prevAddresses => {
            const updatedAddresses = prevAddresses.map(address =>
                address.addressID === addressId
                    ? { ...address, isShippingAddress: !address.isShippingAddress } // Toggle the isActive state
                    : address
            );
            return updatedAddresses; // Return the new addresses array
        });

        try {
            // Make the API call to toggle the address
            await apiClient.put(`/ShippingAddress/toggleShippingAddress/${addressId}`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });

            // Optionally display a success message or update further state here
            console.log('Address toggled successfully!');
        } catch (error) {
            console.error('Error toggling address:', error);

            // Rollback the change if the API call fails
            setAddresses(prevAddresses =>
                prevAddresses.map(address =>
                    address.addressID === addressId
                        ? { ...address, isShippingAddress: !address.isShippingAddress } // Revert the active status
                        : address
                )
            );

            alert('Failed to toggle address. Please try again.');
        }
    };


    const handleCancelOrder = (orderId) => {
        if (window.confirm('Are you sure you want to cancel this order?')) {
            // Call your API to cancel the order
            apiClient.put(`/Order/Cancel/${orderId}`)
                .then(response => {
                    alert('Order cancelled successfully!');
                    // Optionally, refresh the order list
                })
                .catch(error => {
                    console.error('Failed to cancel order:', error);
                    alert('Failed to cancel order. Please try again.');
                });
        }
    };

    const handleDownloadInvoice = async (orderId) => {
        try {
            // Make the API call to get the invoice
            const response = await apiClient.get(`/Invoice/${orderId}`, {
                headers: {
                    Authorization: `Bearer ${token}`,
                    responseType: 'blob' // Specify the expected response type
                },
                responseType: 'blob' // This tells Axios to expect a Blob response
            });

            // Check if the response is OK
            if (response.status !== 200) {
                throw new Error('Failed to download invoice');
            }

            // Create a Blob from the response data
            const blob = new Blob([response.data], { type: 'application/pdf' });
            const url = window.URL.createObjectURL(blob); // Create a URL for the Blob
            const link = document.createElement('a'); // Create a temporary anchor element
            link.href = url; // Set the href to the Blob URL
            link.setAttribute('download', `Invoice_${orderId}.pdf`); // Set the download attribute with filename
            document.body.appendChild(link); // Append link to the body
            link.click(); // Programmatically click the link to trigger the download
            document.body.removeChild(link); // Remove the link from the DOM
            window.URL.revokeObjectURL(url); // Release the Blob URL
        } catch (error) {
            console.error('Error downloading invoice:', error);
        }
    };

    const copyToClipboard = (text) => {
        navigator.clipboard.writeText(text).then(() => {
            alert('Transaction ID copied to clipboard!');
        }).catch(err => {
            console.error('Failed to copy transaction ID: ', err);
        });
    };

    const formatDateTime = (dateString) => {
        const date = new Date(dateString);
        return `${date.toLocaleDateString()} ${date.toLocaleTimeString()}`;
    };

    const renderOrderStatusIndicator = (status) => {
          switch (status) {
            case 'Pending':
              return 'bg-yellow-400 text-black';  // Yellow background for pending orders
            case 'Delivered':
              return 'bg-green-500 text-white';  // Green background for delivered orders
            case 'Cancelled':
              return 'bg-red-500 text-white';    // Red background for cancelled orders
            case 'Shipped':
              return 'bg-blue-500 text-white';   // Blue background for shipped orders
            default:
              return 'bg-gray-400 text-white';   // Default background color
          }
    };

    const renderPaymentStatusIndicator = (paymentStatus) => {
        switch (paymentStatus) {
            case 'Pending':
              return 'bg-yellow-400 text-black';  // Yellow background for pending payments
            case 'Success':
              return 'bg-green-500 text-white';  // Green background for success payments
            case 'Failed':
              return 'bg-red-500 text-white';    // Red background for failed payments
            case 'Refunded':
              return 'bg-blue-500 text-white';   // Blue background for refunded payments
            default:
              return 'bg-gray-400 text-white';   // Default background color
          }
    };

    const renderTabContent = () => {
        switch (activeTab) {
            case 'personal':
                return (
                    <div className="tab-content address-box">
                        <h2 className="mb-4 text-orange-500 font-semibold text-l">Personal Details</h2>
                        <p><strong>Name:</strong> {user.firstName} {user.lastName}</p>
                        <p><strong>Phone:</strong> {user.phoneNumber || 'Not Provided'}</p>
                        <p><strong>Username:</strong> {user.user.username || 'Not Provided'}</p>
                        <button className="edit-btn mt-4" onClick={() => handleEditProfile(user)}>
                            <FaEdit className="mr-3" /> Edit Details
                        </button>
                    </div>
                );
            case 'addresses':
                return (
                    <div className="tab-content">
                        <h2 className="mb-4 text-orange-500 font-semibold text-l">Addresses</h2>
                        {addresses.length > 0 ? (
                            addresses.map((address) => (
                                <div className="address-box relative border p-4 mb-2" key={address.addressID}>
                                    <FaTimes
                                        className="absolute top-2 right-2 text-red-500 cursor-pointer hover:text-red-700"
                                        onClick={() => handleDeleteAddress(address.addressID)}
                                    />                                  

                                    <p>{address.addressLine1}</p>
                                    <p>{address.addressLine2}, {address.city}</p>
                                    <p>{address.state}, {address.postalCode}</p>
                                    <p>{address.country}</p>
                                    
                                    <div className="flex justify-between items-center mt-2">
                                        <div className="flex items-center mr-4">
                                            <label className={`mr-2 ${address.isActive ? 'text-green-500 font-semibold' : 'text-red-500 font-semibold'}`}>
                                                {address.isActive ? 'Active' : 'Inactive'}
                                            </label>
                                            <Toggle
                                                onChange={() => handleToggleAddress(address.addressID)}
                                                checked={address.isActive}
                                            />
                                        </div>
                                        <div className="flex items-center mr-4">
                                            <label className={`mr-2 ${address.isBillingAddress ? 'text-green-500 font-semibold' : 'text-red-500 font-semibold'}`}>
                                                {address.isBillingAddress ? 'BillingAddress' : 'BillingAddress'}
                                            </label>
                                            <Toggle
                                                onChange={() => handleToggleBillingAddress(address.addressID)}
                                                checked={address.isBillingAddress}
                                            />
                                        </div>
                                        <div className="flex items-center">
                                            <label className={`mr-2 ${address.isShippingAddress ? 'text-green-500 font-semibold' : 'text-red-500 font-semibold'}`}>
                                                {address.isShippingAddress ? 'ShippingAddress' : 'ShippingAddress'}
                                            </label>
                                            <Toggle
                                                onChange={() => handleToggleShippingAddress(address.addressID)}
                                                checked={address.isShippingAddress}
                                            />
                                        </div>
                                        {/* This div will take up all available space and push the Download Invoice button to the right */}
                                        <div className="flex-grow" />
                                    </div>
                                </div>
                            ))
                        ) : (
                            <p>No addresses found</p>
                        )}

                        <button className="edit-btn" onClick={handleAddAddress}>
                            <FaEdit className="mr-3" /> Add New Address
                        </button>
                    </div>
                );
            case 'orders':
                return (
                    <div className="tab-content">
                        <h2 className="mb-4 text-orange-500 font-semibold text-l">Order History</h2>
                        {orders.length > 0 ? (
                            orders.map((order) => (
                                <div className="order-item" key={order.orderID}>

                                    <div className={`order-status-indicator ${renderOrderStatusIndicator(order.orderStatus)} flex justify-center items-center absolute top-5 right-5 h-10 w-auto p-5 text-white font-bold rounded-full`}>
                                        {order.orderStatus}
                                    </div>
                                    <p><strong>Order ID:</strong> {order.orderID}</p>
                                    <p><strong>Date:</strong> {formatDateTime(order.orderDate)}</p>
                                    <p><strong>Total:</strong> &#8377;{order.totalAmount}</p>
                                    <p><strong>Status:</strong> {order.orderStatus}</p>
                                    <div className="flex justify-between items-center mt-2">
                                        {/* This div will take up all available space and push the Download Invoice button to the right */}
                                        <div className="flex-grow" />

                                        {order.orderStatus !== 'Delivered' && order.orderStatus !== 'Cancelled' && order.orderStatus !== 'Shipped' && (
                                            <button className="cancel-order-btn bg-red-500 text-white hover:bg-red-700" onClick={() => handleCancelOrder(order.orderID)}>
                                                <MdDelete className="mr-3" /> Cancel Order
                                            </button>
                                        )}

                                        {order.orderStatus !== 'Pending' && order.orderStatus !== 'Cancelled' && order.orderStatus !== 'Shipped' && order.orderStatus !== 'Processing' && (
                                            <button className="download-invoice-btn" onClick={() => handleDownloadInvoice(order.orderID)}>
                                                <FaDownload className="mr-3" /> Download Invoice
                                            </button>
                                        )}
                                    </div>
                                </div>
                            ))
                        ) : (
                            <p>No orders found</p>
                        )}
                    </div>
                );
            case 'payments':
                return (
                    <div className="tab-content">
                        <h2 className="mb-4 text-orange-500 font-semibold text-l">Payment History</h2>
                        {payments.length > 0 ? (
                            payments.map((payment) => (
                                <div className="payment-item" key={payment.transactionId}>
                                    <div className={`payment-status-indicator ${renderPaymentStatusIndicator(payment.paymentStatus)} flex justify-center items-center absolute top-5 right-5 h-10 w-auto p-5 text-white font-bold rounded-full`}>
                                        {payment.paymentStatus}
                                    </div>
                                    <p><strong>Payment ID:</strong> {payment.transactionId}</p>
                                    <p><strong>Order ID:</strong> {payment.orderID}</p>
                                    <p><strong>Date:</strong> {new Date(payment.date).toLocaleDateString()}</p>
                                    <p><strong>Amount:</strong> &#8377;{payment.amount}</p>
                                    <p><strong>Method:</strong> Razorpay Transaction</p>
                                    <div className="flex items-center">
                                        <p><strong>Razorpayment ID:</strong> {payment.razorpayPaymentId}</p>
                                        <BiSolidCopy className="copy-btn ml-2 cursor-pointer text-red-500" onClick={() => copyToClipboard(payment.razorpayPaymentId)} />
                                    </div>
                                </div>
                            ))
                        ) : (
                            <p>No payment records found</p>
                        )}
                    </div>
                );
            case 'wishlist':
                return (
                    <div className="tab-content">
                        <h2 className="mb-4 text-orange-500 font-semibold text-l">Wishlist</h2>
                        {wishlist.length > 0 ? (
                            <ul className="wishlist">
                                {wishlist.map((item) => (
                                    <li key={item.id}>
                                        <img className="m-5 w-20 h-20" src={item.imageUrl} alt={item.productName} />
                                        <span className="ml-10 font-semibold text-orange-500 text-xl">{item.productName}</span>
                                    </li>
                                ))}
                            </ul>
                        ) : (
                            <p>Your wishlist is empty</p>
                        )}
                    </div>
                );
            case 'settings':
                return (
                    <div className="tab-content">
                        <h2 className="mb-4 text-orange-500 font-semibold text-l">Settings</h2>
                        <button className="edit-btn" onClick={handleChangePassword}>
                            <FaEdit className="mr-3" /> Change Password
                        </button>
                    </div>
                );
            default:
                return null;
        }
    };

    if (loading) return <div className="loading">Loading...</div>;
    if (error) return <div className="error">{error}</div>;

    return (
        <div className="profile-page">
            <div className="profile-header">
                <span className="rounded-full">
                    <img src="https://avatar.iran.liara.run/public" alt="User" className="profile-icon" size={120} />
                </span>
                <div className="profile-details">
                    <h1 className="font-bold text-orange-500 text-xl">{user.firstName} {user.lastName}</h1>
                    <p><strong>Email: </strong>{user.user.email}</p>
                    <p><strong>Phone: </strong>{user.phoneNumber || 'Not Provided'}</p>
                </div>
                <button className="edit-btn" onClick={() => handleEditProfile(user)}>
                    <FaEdit className="mr-3" /> Edit Profile
                </button>
            </div>

            <div className="tabs">
                <button className={`tab-btn text-black ${activeTab === 'personal' ? 'active' : ''}`} onClick={() => setActiveTab('personal')}>
                    <FaUserCircle /> Personal
                </button>
                <button className={`tab-btn text-black ${activeTab === 'addresses' ? 'active' : ''}`} onClick={() => setActiveTab('addresses')}>
                    <FaAddressBook /> Addresses
                </button>
                <button className={`tab-btn text-black ${activeTab === 'orders' ? 'active' : ''}`} onClick={() => setActiveTab('orders')}>
                    <FaHistory /> Orders
                </button>
                <button className={`tab-btn text-black ${activeTab === 'payments' ? 'active' : ''}`} onClick={() => setActiveTab('payments')}>
                    <FaCreditCard /> Payments
                </button>
                <button className={`tab-btn text-black ${activeTab === 'wishlist' ? 'active' : ''}`} onClick={() => setActiveTab('wishlist')}>
                    <FaHeart /> Wishlist
                </button>
                <button className={`tab-btn text-black ${activeTab === 'settings' ? 'active' : ''}`} onClick={() => setActiveTab('settings')}>
                    <FaCog /> Settings
                </button>
            </div>

            <div className="tab-content-container">
                {renderTabContent()}
            </div>

            {modalOpen && (
                <EditProfileModal onClose={() => setModalOpen(false)} title="Edit Profile" data={editData} onSave={handleSaveUpdatedProfile}/>
            )}

            {addressModalOpen && (
                <AddressModal onClose={() => setAddressModalOpen(false)} title="Add Address" addressData={addresses} onSave={handleSaveAddress} customerID={user.customerID} />
            )}

            {changePasswordModalOpen && (
                <ChangePasswordModal onClose={() => setChangePasswordModalOpen(false)} title="Change Password" oldPassword={user.user.password} />
            )}

            {showCustomerForm && userId && (
                <CustomerDetailsForm
                    token={token}
                    userId={userId}  // Pass the fetched userId here
                    onSubmit={handleAddForm}
                    open={true}  // Control modal visibility
                    handleClose={() => setShowCustomerForm(false)}
                />
            )}

        </div>
    );
};

export default ProfilePage;
