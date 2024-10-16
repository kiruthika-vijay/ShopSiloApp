import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';
import SellerProductItem from './SellerProductItem.jsx'; // Assuming you will create a component to display product details

const SellerProductDetails = ({ sellerID }) => {
    const [sellerProducts, setSellerProducts] = useState([]);
    const [productID, setProductID] = useState(null);

    useEffect(() => {
        fetchSellerProducts();
    }, [sellerID]);

    const fetchSellerProducts = async () => {
        try {
            const response = await apiClient.get(`/Seller/products/${sellerID}`);
            setSellerProducts(response.data.$values);
        } catch (error) {
            console.error('Error fetching seller products:', error);
        }
    };

    return (
        <div className="container mx-auto p-4">
            <h2 className="text-3xl font-bold mb-6 text-center">Seller Product List</h2>
            <table className="min-w-full bg-white border border-gray-200">
                <thead>
                    <tr className="bg-gray-100 border-b">
                        <th className="py-2 px-4 text-left">Product ID</th>
                        <th className="py-2 px-4 text-left">Product Name</th>
                        <th className="py-2 px-4 text-left">Price</th>
                        {/* <th className="py-2 px-4 text-left">Quantity Sold</th> */}
                        <th className="py-2 px-4 text-left">Action</th>
                    </tr>
                </thead>
                <tbody>
                    {sellerProducts.map((product) => (
                        <tr key={product.productID} className="border-b hover:bg-gray-50">
                            <td className="py-2 px-4">{product.productID}</td>
                            <td className="py-2 px-4">{product.productName}</td>
                            <td className="py-2 px-4">${product.price.toFixed(2)}</td>
                            {/* <td className="py-2 px-4">{product.quantitySold}</td> */}
                            <td className="py-2 px-4">
                                <button
                                    onClick={() => setProductID(product.productID)}
                                    className="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600"
                                >
                                    View Product Details
                                </button>
                            </td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {productID && (
                <div className="mt-8">
                    <SellerProductItem productID={productID} />
                </div>
            )}
        </div>
    );
};

export default SellerProductDetails;