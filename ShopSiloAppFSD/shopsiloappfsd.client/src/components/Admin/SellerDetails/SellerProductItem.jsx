import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';

const SellerProductItem = ({ productID }) => {
    const [product, setProduct] = useState(null);

    useEffect(() => {
        fetchProductDetails();
    }, [productID]);

    const fetchProductDetails = async () => {
        try {
            const response = await apiClient.get(`/Product/${productID}`); // Adjust the endpoint as necessary
            setProduct(response.data);
        } catch (error) {
            console.error('Error fetching product details:', error);
        }
    };

    if (!product) return <div>Loading...</div>;

    return (
        <div className="border p-4 mt-4">
            <h3 className="text-2xl font-bold">{product.productName}</h3>
            <p><strong>Price:</strong> ${product.price.toFixed(2)}</p>
            <p><strong>Description:</strong> {product.description}</p>
            <p><strong>Stock Quantity :</strong> {product.stockQuantity}</p>
            {/* Add more product details as necessary */}
        </div>
    );
};

export default SellerProductItem;