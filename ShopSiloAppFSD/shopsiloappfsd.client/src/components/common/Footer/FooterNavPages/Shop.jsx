import React, { useEffect, useState } from 'react';
import { apiClient } from '../../../common/Axios/Auth';
import { FaHeart, FaCartPlus } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import useWishlist from '../../../customer/Home/HomeUtils/useWishlist';
import useCart from '../../../customer/Home/HomeUtils/useCart';

const Shop = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const { wishlistId, addToWishlist } = useWishlist();
    const { addToCart } = useCart();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProducts = async () => {
            setLoading(true);
            setError(null);
            try {
                const response = await apiClient.get('/Product'); // Endpoint to get all products
                setProducts(response.data.$values || []);
            } catch (err) {
                setError('Error fetching products');
                console.error('Error fetching products:', err);
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
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
        await addToCart(productID); // Add to cart functionality
    };

    if (loading) return <p>Loading products...</p>;
    if (error) return <p>Error fetching products: {error}</p>;

    return (
        <div className="p-5">
            <h1 className="text-2xl font-bold mb-4">Shop Our Products</h1>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {products.length > 0 ? (
                    products.map(product => (
                        <div key={product.productID} className="border rounded-lg shadow-md overflow-hidden cursor-pointer">
                            <img
                                src={product.imageURL}
                                alt={product.productName}
                                className="w-full h-48 object-cover"
                                onClick={() => handleProductClick(product.productID)} // Click handler for product card
                            />
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

export default Shop;
