import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { apiClient } from '../../../common/Axios/Auth';
import { FaHeart, FaEye, FaCartPlus } from 'react-icons/fa';
import useWishlist from '../HomeUtils/useWishlist';
import useCart from '../HomeUtils/useCart';

const CategoryProducts = () => {
    const { categoryId } = useParams();
    const [products, setProducts] = useState([]);
    const [subcategories, setSubcategories] = useState([]);
    const [selectedSubcategory, setSelectedSubcategory] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    const { wishlistId, addToWishlist } = useWishlist();
    const { addToCart } = useCart();
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProductsByCategory = async () => {
            try {
                // Fetch all products under the selected category
                const response = await apiClient.get(`/Product/Category/${categoryId}/m`);
                console.log('Products Response:', response.data); // Log response for debugging
                if (!response.data || !response.data.$values) {
                    throw new Error('Failed to fetch products');
                }
                setProducts(response.data.$values);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        const fetchSubcategories = async () => {
            try {
                const response = await apiClient.get(`/Categories/Subcategories/${categoryId}`);
                console.log('Subcategories Response:', response.data); // Log response for debugging
                if (!response.data || !response.data.$values) {
                    throw new Error('Failed to fetch subcategories');
                }
                setSubcategories(response.data.$values);
            } catch (err) {
                setError(err.message);
            }
        };

        fetchProductsByCategory();
        fetchSubcategories();
    }, [categoryId]);

    if (loading) return <p>Loading products...</p>;
    if (error) return <p>Error fetching products: {error}</p>;

    const handleProductClick = (productId) => {
        navigate(`/customer/product/${productId}`);
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

    // Filter products based on selected subcategory
    const filteredProducts = selectedSubcategory
        ? products.filter(product => product.categoryID.toString() === selectedSubcategory)
        : products;

    return (
        <div className="p-5">
            <h1 className="text-2xl font-bold mb-4">Products in Sub Category</h1>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {filteredProducts.length > 0 ? (
                    filteredProducts.map(product => (
                        <div key={product.productID} className={`border rounded-lg shadow-md overflow-hidden ${product.stockQuantity === 0 ? 'opacity-50' : ''} cursor-pointer`}>
                            <img
                                src={product.imageURL}
                                alt={product.productName}
                                className="w-full h-48 object-cover"
                                onClick={() => handleProductClick(product.productID)}
                                style={{ cursor: product.stockQuantity === 0 ? 'not-allowed' : 'pointer' }}
                            />
                            <div className="p-4">
                                <h2 className="text-lg font-semibold">{product.productName}</h2>
                                {product.discountedPrice ? (
                                    <>
                                        <p className="text-xl font-bold text-green-600">&#8377;{product.discountedPrice}</p>
                                        <p className="text-sm text-gray-500 line-through">&#8377;{product.price}</p>
                                    </>
                                ) : (
                                    <p className="text-xl font-bold text-green-600">&#8377;{product.price}</p>
                                )}
                                <p className="text-sm text-gray-500">{product.description}</p>
                                {product.stockQuantity === 0 ? (
                                    <p className="text-red-500 font-bold">Not Available</p>
                                ) : null}
                            </div>
                            <div className="flex justify-between items-center p-4 border-t">
                                <button
                                    className="text-red-500 hover:text-red-600"
                                    onClick={(e) => { e.stopPropagation(); handleAddToWishlist(product.productID); }}
                                    disabled={product.stockQuantity === 0}
                                >
                                    <FaHeart size={24} />
                                </button>
                                <button
                                    className="text-blue-500 hover:text-blue-600"
                                    onClick={() => handleProductClick(product.productID)}
                                    disabled={product.stockQuantity === 0}
                                >
                                    <FaEye size={24} />
                                </button>
                                <button
                                    className="text-green-500 hover:text-green-600"
                                    onClick={(e) => { e.stopPropagation(); handleAddToCart(product.productID); }}
                                    disabled={product.stockQuantity === 0}
                                >
                                    <FaCartPlus size={24} />
                                </button>
                            </div>
                        </div>
                    ))
                ) : (
                    <p>No products available in this category.</p>
                )}
            </div>
        </div>
    );
};

export default CategoryProducts;
