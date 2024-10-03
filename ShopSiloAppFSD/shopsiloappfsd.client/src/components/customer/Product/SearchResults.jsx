import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { apiClient, getToken } from '../../common/Axios/Auth';
import { FaHeart, FaEye, FaCartPlus } from 'react-icons/fa'; // Importing icons
import useWishlist from '../Home/HomeUtils/useWishlist';
import useCart from '../Home/HomeUtils/useCart';

const SearchResults = () => {
    const { search } = useLocation();
    const query = new URLSearchParams(search).get('query');
    const [results, setResults] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const { wishlistId, addToWishlist } = useWishlist();
    const { addToCart } = useCart();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchSearchResults = async () => {
            setLoading(true);
            setError(null); // Reset error state before fetching
            try {
                const response = await apiClient.get(`/Product/Search/${encodeURIComponent(query)}`, {
                    headers: {
                        Authorization: `Bearer ${getToken()}` // Adjust based on your authentication mechanism
                    }
                });
                console.log(response.data.$values); // Debugging line to check the response structure
                setResults(response.data.$values || []); // Ensure it's an array
            } catch (err) {
                setError('Error fetching search results');
                console.error('Error searching products:', err);
            } finally {
                setLoading(false);
            }
        };

        if (query) {
            fetchSearchResults();
        }
    }, [query]);

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
        await addToCart(productID); // Add to cart functionality
    };

    if (loading) return <p>Loading search results...</p>;
    if (error) return <p>Error fetching search results: {error}</p>;

    return (
        <div className="p-5">
            <h1 className="text-2xl font-bold mb-4">Search Results for: "{query}"</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                {results.length > 0 ? (
                    results.map((product) => (
                        <div key={product.productID} className="border rounded-lg shadow-md overflow-hidden cursor-pointer">
                            <img
                                src={product.imageURL}
                                alt={product.productName}
                                className="w-full h-48 object-cover"
                                onClick={() => handleProductClick(product.productID)} // Click handler for product card
                            />
                            <div className="p-4">
                                <h3 className="text-lg font-semibold">{product.productName}</h3>
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
                                    onClick={(e) => { e.stopPropagation(); handleAddToWishlist(product.productID); }} // Stop propagation for wishlist button
                                >
                                    <FaHeart size={24} />
                                </button>
                                <button
                                    className="text-blue-500 hover:text-blue-600"
                                    onClick={() => handleProductClick(product.productID)} // View product button
                                >
                                    <FaEye size={24} />
                                </button>
                                <button
                                    className="text-green-500 hover:text-green-600"
                                    onClick={(e) => { e.stopPropagation(); handleAddToCart(product.productID); }} // Add to cart button
                                >
                                    <FaCartPlus size={24} />
                                </button>
                            </div>
                        </div>
                    ))
                ) : (
                    <p>No products found.</p>
                )}
            </div>
        </div>
    );
};

export default SearchResults;
