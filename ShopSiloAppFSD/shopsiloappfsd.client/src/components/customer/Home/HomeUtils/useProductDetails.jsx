// src/hooks/useProduct.jsx
import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { apiClient } from '../../../common/Axios/auth';

const useProduct = () => {
    const { productId } = useParams();
    const [product, setProduct] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchProduct = async () => {
            setLoading(true);
            try {
                const response = await apiClient.get(`/Product/${productId}`); // Adjust your API endpoint
                setProduct(response.data);
            } catch (error) {
                console.error("Error fetching product:", error);
                setError(error);
            } finally {
                setLoading(false);
            }
        };

        fetchProduct();
    }, [productId]);

    return { product, loading, error };
};

export default useProduct;
