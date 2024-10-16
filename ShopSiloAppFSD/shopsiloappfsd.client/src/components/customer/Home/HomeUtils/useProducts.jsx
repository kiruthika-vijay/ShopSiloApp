// src/Home/HomeUtils/useProducts.js
import { useState, useEffect } from 'react';
import { apiClient, getToken } from '../../../common/Axios/auth'; // Adjust the import path as necessary

const useProducts = () => {
    const [products, setProducts] = useState([]); // Ensure this is an array
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const fetchProducts = async () => {
        const token = getToken(); // Get the JWT token from local storage

        try {
            const response = await apiClient.get(`/Product`, {
                headers: { Authorization: `Bearer ${token}` },
            });
            setProducts(response.data.$values); // Ensure this returns an array
        } catch (error) {
            setError(error.response?.data || error.message);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchProducts();
    }, []);

    return { products, loading, error };
};

export default useProducts;
