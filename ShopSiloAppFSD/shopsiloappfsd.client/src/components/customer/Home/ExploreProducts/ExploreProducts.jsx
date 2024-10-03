import React from 'react';
import { FaHeart, FaEye, FaShoppingCart, FaStar, FaRegStar, FaStarHalfAlt } from 'react-icons/fa';
import PropTypes from 'prop-types';
import WishlistButton from '../common/WishlistButton';
import { useNavigate } from 'react-router-dom'; // Import useNavigate for navigation
import ViewDetailsButton from '../common/ViewDetailsButton';
import AddToCartButton from '../common/AddToCart';
import useWishlist from '../HomeUtils/useWishlist';
import useCart from '../HomeUtils/useCart';

const ExploreProducts = ({ products = [] }) => {

    const { wishlistId, addToWishlist, removeFromWishlist, wishlistLoading, wishlistError } = useWishlist();
    const { addToCart } = useCart();
    //const { productDetails, fetchProductDetails, productLoading, productError } = useProductDetails();
    const navigate = useNavigate();

    // Handle adding to the wishlist
    const handleAddToWishlist = async (productId, shouldAdd) => {
        if (!wishlistId) {
            console.error("Wishlist ID is not available.");
            return;
        }
        try {
            if (shouldAdd) {
                await addToWishlist(productId);
            }
            else {
                await removeFromWishlist(productId);
            }
        } catch (error) {
            console.error(`Error adding to Wishlist: ${error.message}`);
        }
    };

    const handleViewDetails = (productId) => {
        navigate(`/customer/product/${productId}`);
    };

    const handleAddToCart = async (product) => {
        const { productID, price } = product;
        await addToCart(productID, price); // Pass the price to the addToCart function
    };

    const handleViewAllProducts = () => {
        navigate('/customer/products');
    };

    return (
        <div className="mt-10 p-5">
            <div className="flex items-center mb-6">
                <h3 className="flex items-center text-lg font-bold text-utorange">
                    <span className="block h-10 w-4 bg-orange-500 rounded-md mr-2"></span>
                    Our Products
                </h3>
            </div>
            <div className="flex items-center justify-between mb-6">
                <h2 className="text-2xl font-bold">Explore Our Products</h2>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                {Array.isArray(products) && products.length > 0 ? (
                    products.slice(0, 8).map(({
                        productID,
                        imageURL,
                        productName,
                        discountedPrice,
                        price,
                        averageRating, // Adjusted to match API response
                        reviewCount, // Adjusted to match API response
                        isNewArrival,
                        colorVariations = [] // Adjusted to match API response
                    }) => (
                        <div key={productID} className="relative group bg-white border rounded-lg shadow-md overflow-hidden">
                            {/* New Arrival Badge */}
                            {isNewArrival && (
                                <span className="absolute top-2 left-2 text-xs font-semibold text-white bg-blue-500 px-2 py-1 rounded-full">
                                    New Arrival
                                </span>
                            )}

                            {/* Color Variations */}
                            {colorVariations.length > 0 && (
                                <div className="absolute bottom-2 left-2 flex space-x-1">
                                    {colorVariations.map((colorVariation) => (
                                        <img
                                            key={colorVariation.colorVariationID} // Adjusted to match API response
                                            src={colorVariation.imageURL} // Adjusted to match API response
                                            alt={colorVariation.colorName} // Adjusted to match API response
                                            className="w-4 h-4 rounded-full border-2 border-white"
                                            title={colorVariation.colorName}
                                        />
                                    ))}
                                </div>
                            )}

                            {/* Overlay Icons */}
                            <div className="absolute top-2 right-2 flex space-x-2">
                                <WishlistButton productId={productID} isWishlisted={products.isWishlisted} onWishlistToggle={handleAddToWishlist} />
                                <ViewDetailsButton productId={productID} onViewDetails={handleViewDetails} />
                            </div>

                            {/* Product Image */}
                            <img
                                src={imageURL}
                                alt={productName}
                                className="w-full h-48 object-cover"
                            />

                            {/* Product Details */}
                            <div className="p-4 flex flex-col justify-between h-36">
                                <div>
                                    <h3 className="text-lg font-semibold mb-1">{productName}</h3>
                                    <div className="flex items-center space-x-2">
                                        {discountedPrice ? (
                                            <>
                                                <span className="text-xl font-bold text-green-600">
                                                    &#8377;{discountedPrice}
                                                </span>
                                                <span className="text-sm text-gray-500 line-through">
                                                    &#8377;{price}
                                                </span>
                                            </>
                                        ) : (
                                            <span className="text-xl font-bold text-green-600">
                                                &#8377;{price}
                                            </span>
                                        )}
                                    </div>
                                </div>

                                {/* Ratings */}
                                <div className="flex justify-between items-center">
                                    <div className="flex items-center">
                                        {renderStars(averageRating)} {/* Updated to use averageRating */}
                                        <span className="text-sm text-gray-600 ml-2">
                                            {reviewCount > 0 ? `(${reviewCount})` : '(No Reviews)'}
                                        </span>
                                    </div>
                                </div>
                            </div>

                            {/* Add to Cart Button - Hidden by Default */}
                            <div className="absolute bottom-0 left-0 w-full bg-gradient-to-t from-black to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                                <AddToCartButton productId={productID} onAddToCart={() => handleAddToCart(products)} />
                            </div>
                        </div>
                    ))
                ) : (
                    <p className="text-center">No products available at the moment.</p>
                )}
            </div>

            {/* View All Products Button */}
            <div className="flex justify-center mt-6">
                <button
                    onClick={handleViewAllProducts}
                    className="bg-utorange text-white px-6 py-3 rounded-full hover:bg-orange-600 transition-colors"
                >
                    View All Products
                </button>
            </div>
        </div>
    );
};

// Helper function to render stars
const renderStars = (ratings) => {
    const stars = [];
    const fullStars = Math.floor(ratings);
    const halfStar = ratings % 1 >= 0.5;
    const emptyStars = 5 - fullStars - (halfStar ? 1 : 0);

    for (let i = 0; i < fullStars; i++) {
        stars.push(<FaStar key={`full-${i}`} className="text-yellow-400" />);
    }

    if (halfStar) {
        stars.push(<FaStarHalfAlt key="half" className="text-yellow-400" />);
    }

    for (let i = 0; i < emptyStars; i++) {
        stars.push(<FaRegStar key={`empty-${i}`} className="text-gray-300" />);
    }

    return <div className="flex">{stars}</div>;
};

ExploreProducts.propTypes = {
    products: PropTypes.arrayOf(
        PropTypes.shape({
            productID: PropTypes.number.isRequired,
            imageURL: PropTypes.string.isRequired,
            productName: PropTypes.string.isRequired,
            price: PropTypes.number.isRequired,
            discountedPrice: PropTypes.number, // Optional
            averageRating: PropTypes.number.isRequired, // Updated to match API response
            reviewCount: PropTypes.number.isRequired, // Updated to match API response
            isNewArrival: PropTypes.bool,
            colorVariations: PropTypes.arrayOf(
                PropTypes.shape({
                    colorVariationID: PropTypes.number.isRequired, // Updated to match API response
                    colorName: PropTypes.string.isRequired, // Updated to match API response
                    imageURL: PropTypes.string.isRequired, // Updated to match API response
                })
            ),
        })
    ),
};

export default ExploreProducts;
