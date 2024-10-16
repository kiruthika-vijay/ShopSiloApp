import React, { useRef } from 'react';
import { FaHeart, FaEye, FaShoppingCart, FaArrowLeft, FaArrowRight, FaStar, FaStarHalfAlt, FaRegStar } from 'react-icons/fa';
import PropTypes from 'prop-types';
import useWishlist from '../HomeUtils/useWishlist';
import useCart from '../HomeUtils/useCart';
import { useNavigate } from 'react-router-dom'; // Import useNavigate for navigation
import WishlistButton from '../common/WishlistButton';
import ViewDetailsButton from '../common/ViewDetailsButton';
import AddToCartButton from '../common/AddToCart';

const BestSellingProducts = ({ products = [] }) => {
    const scrollContainerRef = useRef(null);
    const { wishlistId, addToWishlist, removeFromWishlist, wishlistLoading, wishlistError } = useWishlist();
    const { addToCart } = useCart();
    //const { productDetails, fetchProductDetails, productLoading, productError } = useProductDetails();
    const navigate = useNavigate();


    const scrollLeft = () => {
        if (scrollContainerRef.current) {
            scrollContainerRef.current.scrollBy({
                top: 0,
                left: -300,
                behavior: 'smooth',
            });
        }
    };

    const scrollRight = () => {
        if (scrollContainerRef.current) {
            scrollContainerRef.current.scrollBy({
                top: 0,
                left: 300,
                behavior: 'smooth',
            });
        }
    };

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

    const handleAddToCart = async (productID, price) => {
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
                    This Month
                </h3>
            </div>
           <div className="flex items-center justify-between mb-6">
                <h2 className="text-2xl font-bold">Best Selling Products</h2>
                <button
                    onClick={handleViewAllProducts}
                    className="bg-utorange text-white px-6 py-2 rounded-full hover:bg-orange-600 transition-colors"
                >
                    View All Products
                </button>
            </div>

            <div className="relative">
                {/* Scrollable Container */}
                <div
                    ref={scrollContainerRef}
                    className="flex overflow-x-scroll scrollbar-hide space-x-6 px-8 py-4"
                >
                    {Array.isArray(products) && products.length > 0 ? (
                        products.slice(0, 4).map(({ product }) => {
                            const reviews = product.productReviews?.$values || [];
                            const averageRating = reviews.length > 0
                                ? reviews.reduce((sum, review) => sum + review.rating, 0) / reviews.length
                                : 0;

                            return (
                                <div
                                    key={product.productID}
                                    className="relative group bg-white border rounded-lg shadow-md overflow-hidden w-72 h-96 flex-shrink-0"
                                >
                                    {/* Overlay Icons */}
                                    <div className="absolute top-2 right-2 flex space-x-2">
                                        <WishlistButton productId={product.productID} isWishlisted={product.isWishlisted} onWishlistToggle={handleAddToWishlist} />
                                        <ViewDetailsButton productId={product.productID} onViewDetails={handleViewDetails} />
                                    </div>

                                    {/* Product Image */}
                                    <img
                                        src={product.imageURL}
                                        alt={product.productName}
                                        className="w-full h-48 object-cover"
                                    />

                                    {/* Product Details */}
                                    <div className="p-4 flex flex-col justify-between h-36">
                                        <div>
                                            <h3 className="text-lg font-semibold mb-1">{product.productName}</h3>
                                            <div className="flex items-center space-x-2">
                                                {product.price !== undefined ? (
                                                    <span className="text-xl font-bold text-green-600">
                                                        &#8377;{product.price.toFixed(2)}
                                                    </span>
                                                ) : (
                                                    <span className="text-xl font-bold text-red-600">
                                                        Price Unavailable
                                                    </span>
                                                )}
                                            </div>
                                        </div>

                                        {/* Ratings */}
                                        <div className="flex justify-between items-center">
                                            <div className="flex items-center">
                                                {renderStars(averageRating)}
                                                <span className="text-sm text-gray-600 ml-2">
                                                    ({reviews.length})
                                                </span>
                                            </div>
                                        </div>
                                    </div>

                                    {/* Add to Cart Button - Hidden by Default */}
                                    <div className="absolute bottom-0 left-0 w-full bg-gradient-to-t from-black to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                                        <AddToCartButton productId={product.productID} onAddToCart={() => handleAddToCart(product.productId, product.price)} />
                                    </div>
                                </div>
                            );
                        })
                    ) : (
                        <p className="text-center">No best-selling products available at the moment.</p>
                    )}
                </div>
            </div>
        </div>
    );
};

// Helper Functions
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

BestSellingProducts.propTypes = {
    products: PropTypes.arrayOf(
        PropTypes.shape({
            productID: PropTypes.number.isRequired,
            product: PropTypes.shape({
                imageURL: PropTypes.string.isRequired,
                productName: PropTypes.string.isRequired,
                price: PropTypes.number,
                productReviews: PropTypes.shape({
                    $values: PropTypes.arrayOf(
                        PropTypes.shape({
                            rating: PropTypes.number.isRequired,
                        })
                    ),
                }),
            }),
        })
    ),
};

export default BestSellingProducts;
