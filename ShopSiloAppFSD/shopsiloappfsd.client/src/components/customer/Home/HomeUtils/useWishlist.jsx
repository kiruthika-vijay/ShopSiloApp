// useWishlist.js
import { useState, useEffect, useContext } from 'react';
import { apiClient, getToken } from '../../../common/Axios/auth';
import { CountContext } from '../../../common/Header/CountContext';

const useWishlist = () => {
    const [wishlistId, setWishlistId] = useState(null);
    const [userId, setUserId] = useState(null);
    const { setCartCount, setWishlistCount } = useContext(CountContext);
    const [token, setToken] = useState(null);
    const [wishlistLoading, setWishlistLoading] = useState(true);
    const [wishlistError, setWishlistError] = useState(null);

    useEffect(() => {
        const token = getToken();
        if (token) {
            setToken(token);
            fetchWishlistId();
        }

        if (!token) return;
    }, []);

    const fetchWishlistId = async () => {
        try {
            const response = await apiClient.get('/Wishlists', {
                headers: { Authorization: `Bearer ${token}` },
            });
            setWishlistId(response.data.$values[0].wishListID);
        }
        catch (error) {
            console.log(error);
        }
    }

    const addToWishlist = async (productId) => {
        try {            
            const response = await apiClient.post(`/Wishlists/add`, { productId });
            setWishlistCount(prevCount => prevCount + 1);
            console.log(`Added to Wishlist: ${response.data}`);
        } catch (error) {
            setWishlistError(error);
        }
    };

    const removeFromWishlist = async (productId) => {
        const token = getToken(); // Fetch the JWT token for authorization

        try {
            const response = await apiClient.delete(`/Wishlists/${wishlistId}/items/${productId}`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            if (response.status === 204) {
                console.log(`Removed from Wishlist: ${response.data}`);
                setWishlistCount(prevCount => prevCount - 1);
            }
            return response.data; // Return response if needed
        } catch (error) {
            console.error(`Error removing from Wishlist: ${error.response?.data || error.message}`);
            throw error; // Re-throw the error for further handling if necessary
        }
    };

    return { wishlistId, addToWishlist, removeFromWishlist,  wishlistLoading, wishlistError };
};

export default useWishlist;
