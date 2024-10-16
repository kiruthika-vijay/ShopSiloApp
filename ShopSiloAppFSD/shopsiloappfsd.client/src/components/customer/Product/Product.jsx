import React from 'react';
import { useNavigate } from 'react-router-dom';
import { FaHeart, FaEye, FaCartPlus } from 'react-icons/fa'; // Importing icons
import useProducts from '../Home/HomeUtils/useProducts';
import useWishlist from '../Home/HomeUtils/useWishlist';
import useCart from '../Home/HomeUtils/useCart';

const AllProducts = () => {
    const { products, loading, error } = useProducts(); // Use the custom hook to fetch products

    const { wishlistId, addToWishlist, removeFromWishlist, wishlistLoading, wishlistError } = useWishlist();
    const { addToCart } = useCart();
    const navigate = useNavigate();

    const handleBack = () => {
        navigate(-1); // Go back to the previous page
    };

    const handleProductClick = (productId) => {
        navigate(`/customer/product/${productId}`); // Navigate to the product description page
    };

    // Handle adding to the wishlist
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
        await addToCart(productID); // Pass the price to the addToCart function
    };

    if (loading) return <p>Loading products...</p>;
    if (error) return <p>Error fetching products: {error}</p>;

    return (
        <div className="p-5">
            <button onClick={handleBack} className="mb-4 bg-gray-300 text-black px-4 py-2 rounded-md hover:bg-gray-400 transition-colors">
                Back
            </button>
            <h1 className="text-2xl font-bold mb-4">All Products</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {Array.isArray(products) && products.length > 0 ? (
                    products.map(product => (
                        <div
                            key={product.productID}
                            className="border rounded-lg shadow-md overflow-hidden cursor-pointer"
                            onClick={() => handleProductClick(product.productID)} // Add click handler for product card
                        >
                            <img src={product.imageURL} alt={product.productName} className="w-full h-48 object-cover" />
                            <div className="p-4">
                                <h2 className="text-lg font-semibold">{product.productName}</h2>
                                <p className="text-xl font-bold text-green-600">&#8377;{product.price}</p>
                                {product.discountedPrice && (
                                    <p className="text-sm text-gray-500 line-through">&#8377;{product.discountedPrice}</p>
                                )}
                                <p className="text-sm text-gray-500">{product.description}</p>
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
                                    onClick={(e) => { e.stopPropagation(); handleProductClick(product.productID); }} // View product button
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
                    <p>No products available.</p>
                )}
            </div>
        </div>
    );
};

export default AllProducts;
