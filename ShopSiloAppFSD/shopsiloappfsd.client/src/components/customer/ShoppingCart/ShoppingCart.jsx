import React, { useState, useEffect, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { apiClient, getToken } from '../../common/Axios/auth';
import { AuthContext } from '../Auth/AuthContext';
import { CountContext } from '../../common/Header/CountContext';

const ShoppingCart = () => {
    const [addresses, setAddresses] = useState({ billing: [], shipping: [] });
    const [cartItemSellerId, setCartItemSellerId] = useState([]);
    const [shippingAddressId, setShippingAddressId] = useState(0);
    const [billingAddressId, setBillingAddressId] = useState(0);
    const [shippingAddress, setShippingAddress] = useState('');
    const [billingAddress, setBillingAddress] = useState('');
    const { setCartCount, setWishlistCount } = useContext(CountContext);
    const [cartItems, setCartItems] = useState([]);
    const [orderItems, setOrderItems] = useState([]);
    const [subtotalAmount, setSubTotalAmount] = useState(0);
    const [totalAmount, setTotalAmount] = useState(0);
    const [shippingCharges, setShippingCharges] = useState(0);
    const [couponCode, setCouponCode] = useState('');
    const [discountAmount, setDiscountAmount] = useState(0);
    const [savingsMessage, setSavingsMessage] = useState('');
    const [paymentStatus, setPaymentStatus] = useState(null);
    const [showSuccessMessage, setShowSuccessMessage] = useState(false);
    const { isLoggedIn, userId } = useContext(AuthContext);
    const navigate = useNavigate();

    useEffect(() => {
        const token = getToken();
        if (token && isLoggedIn) {
            fetchCartItems(token);
            fetchAddresses(token);
            fetchCartItemsSellerId(token);
        }
    }, [isLoggedIn]);

    const fetchCartItems = async (token) => {
        try {
            const response = await apiClient.get(`/ShoppingCart/GetCart`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setCartItems(response.data.$values || []);
            calculateTotal(response.data.$values || []);
            setCartCount(response.data.$values.length); // Update cart count based on fetched items
        } catch (error) {
            console.error('Error fetching cart items:', error);
        }
    };

    const fetchCartItemsSellerId = async (token) => {
        try {
            const response = await apiClient.get(`/ShoppingCart/items`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setCartItemSellerId(response.data.$values[0].sellerId || []);
            console.log(response.data.$values[0].sellerId || []);
        } catch (error) {
            console.error('Error fetching cart items:', error);
        }
    };

    const fetchAddresses = async (token) => {
        try {
            const response = await apiClient.get(`/ShippingAddress/user`, {
                headers: { Authorization: `Bearer ${token}` },
            });

            const addresses = response.data.$values || [];

            // Assuming there's a field like addressType indicating if it's a billing or shipping address
            const billingAddresses = addresses.filter(
                (address) => address.isBillingAddress === true
            );
            const shippingAddresses = addresses.filter(
                (address) => address.isShippingAddress === true
            );

            setAddresses({ billing: billingAddresses, shipping: shippingAddresses });

            console.log('Billing Addresses:', billingAddresses);
            console.log('Shipping Addresses:', shippingAddresses);
        } catch (error) {
            console.error('Error fetching addresses:', error);
        }
    };


    const calculateTotal = (items, discountValue = 0) => {
        const subtotal = items.reduce(
            (acc, item) => acc + ((item.discountedPrice || item.price) * item.quantity),
            0
        );
        setSubTotalAmount(subtotal);
        const shipping = subtotal >= 1500 ? 0 : 80; // Shipping charges logic
        setShippingCharges(shipping);
        setTotalAmount(subtotal + shipping - discountValue);
    };

    const applyCoupon = async (event) => {
        event.preventDefault();
        const token = getToken();

        if (!couponCode) {
            setSavingsMessage("Please enter a coupon code.");
            return;
        }

        try {
            const response = await apiClient.post(
                `/Discount/Apply?discountCode=${couponCode}`,
                {},
                { headers: { Authorization: `Bearer ${token}` } }
            );

            if (response.status === 200) {
                const discountPercent = response.data.discountPercentage;
                const discountValue = (subtotalAmount * discountPercent) / 100;

                setDiscountAmount(discountValue);
                setSavingsMessage(`Coupon applied successfully! You saved ₹ ${discountValue.toFixed(2)}.`);

                // Recalculate total with the new discount amount
                calculateTotal(cartItems, discountValue);
            }
        } catch (error) {
            if (error.response && error.response.status === 404) {
                setSavingsMessage("Coupon not found.");
            } else {
                setSavingsMessage("Invalid or expired coupon.");
            }
            setDiscountAmount(0);
            calculateTotal(cartItems); // Recalculate without discount
        }
    };


    const updateCartItem = async (itemId, quantity) => {
        if (!isLoggedIn) return;

        try {
            const token = getToken();
            await apiClient.put(`/CartItem/updateQuantity/${itemId}`, { quantity: quantity }, {
                headers: { Authorization: `Bearer ${token}` },
            });
            fetchCartItems(token);
        } catch (error) {
            console.error('Error updating cart item:', error);
        }
    };

    const removeCartItem = async (itemId) => {
        if (!isLoggedIn) return;

        try {
            const token = getToken();
            await apiClient.delete(`/CartItem/${itemId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            // Update the local cartItems state to remove the deleted item
            setCartItems(prevItems => prevItems.filter(item => item.id !== itemId));
            setCartCount(prevCount => prevCount - 1);
            fetchCartItems(token);
        } catch (error) {
            console.error('Error removing cart item:', error);
        }
    };

    const incrementQuantity = (itemId, currentQuantity) => {
        const newQuantity = currentQuantity + 1;
        updateCartItem(itemId, newQuantity);
        setCartCount(prevCount => prevCount + 1);

    };

    const decrementQuantity = (itemId, currentQuantity) => {
        if (currentQuantity > 1) {
            const newQuantity = currentQuantity - 1;
            updateCartItem(itemId, newQuantity);
            setCartCount(prevCount => prevCount - 1);
        }
    };

    const clearCart = async () => {
        const token = getToken();
        try {
            // Make API call to clear the user's cart
            const response = await apiClient.delete('/ShoppingCart/clear-cart', {
                headers: { Authorization: `Bearer ${token}` },
            });

            if (response.data) {
                console.log(response.data.message); // Log success message
                // Clear cart state
                setCartItems([]); // Set cart items to an empty array
                setCartCount(0);  // Set cart count to 0
                fetchCartItems(getToken());
            }
        } catch (error) {
            console.error("Error clearing cart:", error.response ? error.response.data : error.message);
        }
    };

    const handlePayment = async () => {
        console.log(shippingAddressId, billingAddressId);
        const registrationData = {
            name: 'John Doe',
            email: 'john@example.com',
            mobile: '9876543210',
            amount: totalAmount.toFixed(2),
            discount: discountAmount.toFixed(2) // Include discount amount if applicable
        };
       
        try {
            console.log(registrationData);
            const orderResponse = await apiClient.post('/Payment/create-order', registrationData);
            const { data: razorpayOptions } = orderResponse;

            const options = {
                key: razorpayOptions.key,
                amount: razorpayOptions.amountInSubUnits,
                currency: razorpayOptions.currency,
                name: razorpayOptions.name,
                description: razorpayOptions.description,
                order_id: razorpayOptions.orderId,
                handler: async function (response) {
                    // Wait for the verification to complete
                    await verifyPayment(response);
                },
                prefill: {
                    name: razorpayOptions.profileName,
                    email: razorpayOptions.profileEmail,
                    contact: razorpayOptions.profileContact,
                },
            };

            const razorpay = new window.Razorpay(options);
            razorpay.open();
        } catch (err) {
            console.error('Payment initiation failed', err);
        }
    };

    const verifyPayment = async (response) => {
        const paymentVerification = {
            orderId: response.razorpay_order_id,
            paymentId: response.razorpay_payment_id,
            signature: response.razorpay_signature,
        };

        try {
            const verifyResponse = await apiClient.post('/Payment/verify-payment', paymentVerification);
           
            if (verifyResponse.data && verifyResponse.data.message === "Payment successful") {
                // Prepare the complete payment transaction data
                const paymentTransactionData = {
                    razorpayPaymentId: response.razorpay_payment_id,
                    razorpayOrderId: response.razorpay_order_id,
                    amount: totalAmount.toFixed(2) || 0, // Replace with your method to get the cart total
                    paymentStatus: 'Success', // Set appropriate status
                    shippingAddressID: shippingAddressId, // Get shipping address ID
                    billingAddressID: billingAddressId || shippingAddressId, // Get billing address ID
                    sellerID: cartItemSellerId, // Get seller ID if applicable
                    orderItems: cartItems, // Include order items here
                };
                console.log(paymentTransactionData);
                // Complete the payment and create the order
                const completePaymentResponse = await apiClient.post('/Payment/complete-payment', paymentTransactionData);
                console.log(completePaymentResponse);
                setPaymentStatus(completePaymentResponse.data.message);
                clearCart();

                // Assuming completePaymentResponse is the response you're working with
                const orderDetails = {
                    ...paymentTransactionData,
                    orderId: completePaymentResponse.data.orderId || "N/A", // Using orderId
                    trackingNumber: completePaymentResponse.data.trackingNumber || "N/A", // Ensure trackingNumber is available
                    sellerName: completePaymentResponse.data.seller.sellerName || "N/A", // Access sellerName directly
                    companyName: completePaymentResponse.data.seller.companyName || "N/A", // Access companyName directly
                    contactNumber: completePaymentResponse.data.seller.contactNumber || "N/A", // Access contactNumber directly
                    shopAddress: completePaymentResponse.data.seller.address || "N/A", // Access shopAddress directly
                    totalAmount: completePaymentResponse.data.totalAmount || 0, // Ensure totalAmount is correct
                    discount: completePaymentResponse.data.discount || 0, // Ensure discount is correct
                    shippingAddress: completePaymentResponse.data.shippingAddress || "N/A", // Check if shippingAddress is present
                    billingAddress: completePaymentResponse.data.billingAddress || "N/A", // Check if billingAddress is present
                    orderItems: cartItems, // Pass the order items
                };

                // Navigation to Order Summary
                navigate('/customer/order-summary', {
                    state: {
                        orderDetails: {
                            orderDetails: orderDetails,
                            shippingAddress: {
                                id: shippingAddress.id,
                                addressLine1: shippingAddress.addressLine1,
                                addressLine2: shippingAddress.addressLine2,
                                city: shippingAddress.city,
                                state: shippingAddress.state,
                                postalCode: shippingAddress.postalCode,
                                country: shippingAddress.country,
                            },
                            billingAddress: {
                                id: billingAddress.id,
                                addressLine1: billingAddress.addressLine1,
                                addressLine2: billingAddress.addressLine2,
                                city: billingAddress.city,
                                state: billingAddress.state,
                                postalCode: billingAddress.postalCode,
                                country: billingAddress.country,
                            }
                        }
                    }
                });
            }
        } catch (err) {
            setPaymentStatus('Payment verification or processing failed');
            console.error('Error:', err);
        }
    };



    return (
        <div className="container mx-auto p-5">
            <div className="flex justify-between items-center mb-5">
                <h1 className="text-2xl font-bold">Shopping Cart ({cartItems.length})</h1>
            </div>

            <div className="mb-4 mt-5">
                <h2 className="text-xl font-semibold mb-2 text-orange-600">
                    Select Shipping Address:
                </h2>
                <select
                    onChange={(e) => {
                        const selectedValue = parseInt(e.target.value, 10);
                        const selectedAddress = addresses.shipping.find(
                            (address) => address.addressID === selectedValue
                        );
                        console.log("Selected Shipping Address ID:", selectedValue);
                        console.log("Selected Shipping Address:", selectedAddress);

                        // Set both the shippingAddressId and the full shippingAddress
                        setShippingAddressId(selectedValue);
                        setShippingAddress(selectedAddress);
                    }}
                    value={shippingAddressId} // Binds to the selected shipping address ID
                    className="border rounded p-2 w-full border-orange-600 focus:outline-none focus:ring-2 focus:ring-orange-600"
                >
                    <option value="">Select Address</option>
                    {addresses.shipping.map((address) => (
                        <option key={address.addressID} value={address.addressID}>
                            {`${address.addressLine1}, ${address.addressLine2}, ${address.city}, ${address.state}, ${address.postalCode}`}
                        </option>
                    ))}
                </select>
            </div>

            <div className="mb-4 mt-5">
                <h2 className="text-xl font-semibold mb-2 text-orange-600">
                    Select Billing Address: (If Applicable)
                </h2>
                <select
                    onChange={(e) => {
                        const selectedValue = parseInt(e.target.value, 10);
                        const selectedAddress = addresses.billing.find(
                            (address) => address.addressID === selectedValue
                        );
                        console.log("Selected Billing Address ID:", selectedValue);
                        console.log("Selected Billing Address:", selectedAddress);

                        // Set both the billingAddressId and the full billingAddress
                        setBillingAddressId(selectedValue);
                        setBillingAddress(selectedAddress);
                    }}
                    value={billingAddressId} // Binds to the selected billing address ID
                    className="border rounded p-2 w-full border-orange-600 focus:outline-none focus:ring-2 focus:ring-orange-600"
                >
                    <option value="">Select Address</option>
                    {addresses.billing.map((address) => (
                        <option key={address.addressID} value={address.addressID}>
                            {`${address.addressLine1}, ${address.addressLine2}, ${address.city}, ${address.state}, ${address.postalCode}`}
                        </option>
                    ))}
                </select>
            </div>

            <table className="w-full table-auto text-left">
                <thead>
                    <tr>
                        <th className="px-4 py-2">Product</th>
                        <th className="px-4 py-2 text-right">Price</th>
                        <th className="px-4 py-2 text-center">Quantity</th>
                        <th className="px-4 py-2 text-right">Total</th>
                        <th className="px-4 py-2 text-center">Action</th>
                    </tr>
                </thead>
                <tbody>
                    {cartItems.length > 0 ? (
                        cartItems.map(item => (
                            <tr key={item.cartItemID} className="border-t">
                                <td className="px-4 py-2 flex items-center">
                                    <img src={item.imageUrl} alt={item.productName} className="w-16 h-16 object-cover rounded mr-4" />
                                    {item.productName}
                                </td>
                                <td className="px-4 py-2 text-right">
                                    &#8377;{(item.discountedPrice || item.price).toFixed(2)}
                                </td>
                                <td className="px-4 py-2 text-center">
                                    <div className="flex justify-center items-center">
                                        <button
                                            onClick={() => decrementQuantity(item.cartItemID, item.quantity)}
                                            className="px-2 py-1 bg-red-500 font-bold text-white rounded-l hover:bg-red-600"
                                        >
                                            -
                                        </button>
                                        <input
                                            type="number"
                                            value={item.quantity}
                                            readOnly
                                            className="w-16 h-8 pl-3 text-center border border-gray-300"
                                        />
                                        <button
                                            onClick={() => incrementQuantity(item.cartItemID, item.quantity)}
                                            className="px-2 py-1 bg-green-500 font-bold text-white rounded-r hover:bg-green-600"
                                        >
                                            +
                                        </button>
                                    </div>
                                </td>
                                <td className="px-4 py-2 text-right">
                                    &#8377;{((item.discountedPrice || item.price) * item.quantity).toFixed(2)}
                                </td>
                                <td className="px-4 py-2 text-center">
                                    <button
                                        onClick={() => removeCartItem(item.cartItemID)}
                                        className="font-bold text-red-500 hover:text-red-700"
                                    >
                                        X
                                    </button>
                                </td>
                            </tr>
                        ))
                    ) : (
                        <p>No items in the cart.</p>
                    )}
                </tbody>
            </table>

            <div className="mt-4 flex justify-between">
                <div className="flex items-center">
                    <form onSubmit={applyCoupon}>
                        <input
                            type="text"
                            value={couponCode}
                            onChange={(e) => setCouponCode(e.target.value)}
                            placeholder="Coupon Code"
                            className="border border-gray-300 p-2 rounded-l"
                        />
                        <button
                            type="submit"
                            className="bg-blue-500 text-white p-2 rounded-r"
                        >
                            Apply
                        </button>
                    </form>
                </div>
                {/* Coupon status message */}
                {savingsMessage && (
                    <div className={`mt-2 border px-4 py-3 rounded text-center ${savingsMessage.includes("successfully")
                            ? 'bg-green-100 border-green-400 text-green-700'
                            : savingsMessage.includes("not found") || savingsMessage.includes("Invalid")
                                ? 'bg-red-100 border-red-400 text-red-700'
                                : ''
                        }`}>
                        <span className="block">{savingsMessage}</span>
                    </div>
                )}
            </div>

            <div className="mt-4">
                <div className="flex justify-between border-t border-b py-2">
                    <span className="font-bold">Subtotal:</span>
                    <span>&#8377;{subtotalAmount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between py-2">
                    <span className="font-bold">Shipping:</span>
                    <span>&#8377;{shippingCharges.toFixed(2)}</span>
                </div>
                <div className="flex justify-between border-b py-2">
                    <span className="font-bold">Discount:</span>
                    <span>&#8377;{discountAmount.toFixed(2)}</span>
                </div>
                <div className="flex justify-between font-bold py-2">
                    <span>Total:</span>
                    <span>&#8377;{totalAmount.toFixed(2)}</span>
                </div>
            </div>

            <div className="mt-5 flex justify-end">
                <button
                    onClick={handlePayment}
                    className="bg-green-500 text-white py-2 px-6 rounded hover:bg-green-600">
                    Proceed to Checkout
                </button>
            </div>
            {paymentStatus && (
                <div className={`border px-4 py-3 rounded relative mt-4 text-center ${paymentStatus.includes('failed')
                        ? 'bg-red-100 border-red-400 text-red-700'
                        : 'bg-green-100 border-green-400 text-green-700'
                    }`} role="alert">
                    <strong className="font-bold">{paymentStatus}!</strong>
                    <br />
                    <span className="block sm:inline">You can now view your order details in Your Account Orders...</span>
                </div>
            )}
        </div>
    );
};

export default ShoppingCart;
