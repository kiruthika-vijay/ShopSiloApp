import { useState, useEffect, useContext } from 'react';
import { apiClient, getToken, getEmailFromToken } from '../../../common/Axios/auth';
import useWishlist from './useWishlist';
import { CountContext } from '../../../common/Header/CountContext';

const useCart = () => {
    const [cartId, setCartId] = useState(null);
    const { setCartCount, setWishlistCount } = useContext(CountContext);
    const [cartItems, setCartItems] = useState([]); // To store cart items
    const [cartLoading, setCartLoading] = useState(true);
    const [cartError, setCartError] = useState(null);
    const { removeFromWishlist } = useWishlist();
    const [userID, setUserID] = useState(null);

    useEffect(() => {
        const token = getToken();
        if (!token) return;

        const emailFromToken = getEmailFromToken(token);
        fetchUserId(emailFromToken); // Fetch user ID using email
    }, []);

    const fetchUserId = async (email) => {
        const token = getToken();
        try {
            const response = await apiClient.get(`/Users/email/${email}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            const user = response.data.$values[0];
            if (user) {
                setUserID(user.userID); // Set userID
                fetchCartId(user.userID); // Fetch cart ID using userID
            } else {
                setCartError("User not found");
            }
        } catch (error) {
            setCartError(error);
            setCartLoading(false);
        }
    };

    const fetchCartId = async (userId) => {
        try {
            const response = await apiClient.get(`/ShoppingCart/user/${userId}`, {
                headers: { Authorization: `Bearer ${getToken()}` },
            });
            if (response.data && response.data.cartID) {
                setCartId(response.data.cartID); // Set existing cart ID
                setCartItems(response.data.cartItems.$values || []); // Set cart items
            } else {
                const newCartResponse = await apiClient.post('/ShoppingCart', {}, {
                    headers: { Authorization: `Bearer ${getToken()}` },
                });
                setCartId(newCartResponse.data.cartID);
            }
        } catch (error) {
            setCartError(error);
        } finally {
            setCartLoading(false);
        }
    };

    const addToCart = async (productId, quantity = 1) => {
        try {
            const response = await apiClient.post(`/ShoppingCart/add`, { productId, quantity }, {
                headers: { Authorization: `Bearer ${getToken()}` },
            });
            // Check if the item is already in the cart
            if (!cartItems.some(item => item.productID === productId)) {
                setCartItems(prevItems => [...prevItems, { productID: productId, quantity }]); // Update cart items state
            }
            console.log(`Added to Cart: ${response.data}`);
            setCartCount(prevCount => prevCount + 1);
        } catch (error) {
            setCartError(error);
        }
    };

    const isInCart = (productId) => {
        return cartItems.some(item => item.productID === productId);
    };

    return { cartId, cartItems, addToCart, cartLoading, cartError, userID, isInCart };
};

export default useCart;
