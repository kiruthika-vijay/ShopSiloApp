import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { FaHeart, FaEye, FaCartPlus } from 'react-icons/fa'; // Icons
import axios from 'axios'; // Axios for API calls
import useWishlist from '../HomeUtils/useWishlist';
import useCart from '../HomeUtils/useCart';
import { apiClient } from '../../../common/Axios/auth';

const Suggestions = () => {
    const [suggestions, setSuggestions] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const { wishlistId, addToWishlist, removeFromWishlist } = useWishlist();
    const { addToCart } = useCart();
    const navigate = useNavigate();

    useEffect(() => {
        // Fetch random product suggestions from the backend
        const fetchSuggestions = async () => {
            try {
                const response = await apiClient.get('/Product/suggestions'); // Adjust endpoint as needed
                setSuggestions(response.data.$values); // Assuming the response structure from your example
                setLoading(false);
            } catch (err) {
                setError(err.message);
                setLoading(false);
            }
        };
        fetchSuggestions();
    }, []);

    const handleProductClick = (productId) => {
        navigate(`/customer/product/${productId}`); // Navigate to the product description page
    };

    const handleAddToWishlist = async (productId) => {
        if (!wishlistId) {
            console.error("Wishlist ID is not available.");
            return;
        }
        try {
            await addToWishlist(productId);
        } catch (error) {
            console.error(`Error adding to Wishlist: ${error.message}`);
        }
    };

    const handleAddToCart = async (productID) => {
        await addToCart(productID);
    };

    if (loading) return <p>Loading suggestions...</p>;
    if (error) return <p>Error fetching suggestions: {error}</p>;

    return (
        <div className="p-5">
            <h1 className="text-2xl font-bold mb-4">Product Suggestions</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {Array.isArray(suggestions) && suggestions.length > 0 ? (
                    suggestions.map(product => (
                        <div
                            key={product.productID}
                            className="border rounded-lg shadow-md overflow-hidden cursor-pointer"
                            onClick={() => handleProductClick(product.productID)}
                        >
                            <img src={product.imageURL} alt={product.productName} className="w-full h-48 object-cover" />
                            <div className="p-4">
                                <h2 className="text-lg font-semibold">{product.productName}</h2>
                                <p className="text-xl font-bold text-green-600">&#8377;{product.price}</p>
                                {product.discountedPrice && (
                                    <p className="text-sm text-gray-500 line-through">&#8377;{product.discountedPrice}</p>
                                )}
                                <p className="text-sm text-gray-500">{product.description}</p>
                                <p className="text-sm text-gray-500">Stock: {product.stockQuantity}</p>
                            </div>
                            <div className="flex justify-between items-center p-4 border-t">
                                <button
                                    className="text-red-500 hover:text-red-600"
                                    onClick={(e) => { e.stopPropagation(); handleAddToWishlist(product.productID); }}
                                >
                                    <FaHeart size={24} />
                                </button>
                                <button
                                    className="text-blue-500 hover:text-blue-600"
                                    onClick={(e) => { e.stopPropagation(); handleProductClick(product.productID); }}
                                >
                                    <FaEye size={24} />
                                </button>
                                <button
                                    className="text-green-500 hover:text-green-600"
                                    onClick={(e) => { e.stopPropagation(); handleAddToCart(product.productID); }}
                                >
                                    <FaCartPlus size={24} />
                                </button>
                            </div>
                        </div>
                    ))
                ) : (
                    <p>No suggestions available.</p>
                )}
            </div>
        </div>
    );
};

export default Suggestions;
