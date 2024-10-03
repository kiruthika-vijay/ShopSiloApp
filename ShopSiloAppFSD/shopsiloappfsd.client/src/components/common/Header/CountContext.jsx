// components/CountContext.jsx

import React, { createContext, useEffect, useState } from 'react';
import { apiClient, getToken } from '../Axios/auth'; // Adjust the import path as needed

export const CountContext = createContext();

export const CountProvider = ({ children }) => {
    const [cartCount, setCartCount] = useState(0);
    const [wishlistCount, setWishlistCount] = useState(0);

    useEffect(() => {
        const token = getToken();
        if (token) {
            fetchCounts(token);
        }
    }, []);

    const fetchCounts = async (token) => {
        try {
            const [cartResponse, wishlistResponse] = await Promise.all([
                apiClient.get('/ShoppingCart/cart/count', { headers: { Authorization: `Bearer ${token}` } }),
                apiClient.get('/ShoppingCart/wishlist/count', { headers: { Authorization: `Bearer ${token}` } }),
            ]);

            setCartCount(cartResponse.data.count || 0);
            setWishlistCount(wishlistResponse.data.count || 0);
        } catch (error) {
            console.error('Error fetching counts:', error);
        }
    };

    return (
        <CountContext.Provider value={{ cartCount, wishlistCount, setCartCount, setWishlistCount }}>
            {children}
        </CountContext.Provider>
    );
};
