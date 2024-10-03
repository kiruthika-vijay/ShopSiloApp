import React, { useEffect, useRef, useState } from 'react';
import { FaArrowLeft, FaArrowRight } from 'react-icons/fa';
import PropTypes from 'prop-types';
import useWishlist from '../HomeUtils/useWishlist';
import { useNavigate } from 'react-router-dom'; // Import useNavigate for navigation
import { apiClient, getToken } from '../../../common/Axios/auth';
import useProducts from '../HomeUtils/useProducts';
import useCart from '../HomeUtils/useCart';
import CountdownTimer from '../common/CountdownTimer';
import WishlistButton from '../common/WishlistButton';
import ViewDetailsButton from '../common/ViewDetailsButton';
import StarRating from '../common/StarRating';
import AddToCartButton from '../common/AddToCart';

const TodayDeals = ({ deals = [] }) => {
    const scrollContainerRef = useRef(null);
    const { wishlistId, addToWishlist, removeFromWishlist, wishlistLoading, wishlistError } = useWishlist();
    const { products, loading, error } = useProducts();
    const { addToCart } = useCart();
    const navigate = useNavigate();

    const scrollLeft = () => {
        if (scrollContainerRef.current) {
            scrollContainerRef.current.scrollBy({ top: 0, left: -300, behavior: 'smooth' });
        }
    };

    const scrollRight = () => {
        if (scrollContainerRef.current) {
            scrollContainerRef.current.scrollBy({ top: 0, left: 300, behavior: 'smooth' });
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

    const handleAddToCart = async (product) => {
        const { productID, price } = product;
        await addToCart(productID, price); // Pass the price to the addToCart function
    };

    const handleViewAllProducts = () => {
        navigate('/customer/products');
    };

    const endTime = deals.length > 0 ? Math.min(...deals.map(deal => new Date(deal.flashSaleEnd))) : null;


    return (
        <div className="mt-10 p-5">
            <div className="flex items-center mb-6">
                <h3 className="flex items-center text-lg font-bold text-utorange">
                    <span className="block h-10 w-4 bg-orange-500 rounded-md mr-2"></span>
                    Today's
                </h3>
            </div>
            <div className="flex items-center justify-between mb-6">
                <h2 className="text-2xl font-bold">Flash Sales</h2>
                {endTime && <CountdownTimer endTime={endTime} />}
                <div className="flex space-x-2">
                    <button onClick={scrollLeft} className="bg-white bg-opacity-75 hover:bg-opacity-100 text-gray-700 rounded-full p-2 shadow-md" aria-label="Scroll Left">
                        <FaArrowLeft />
                    </button>
                    <button onClick={scrollRight} className="bg-white bg-opacity-75 hover:bg-opacity-100 text-gray-700 rounded-full p-2 shadow-md" aria-label="Scroll Right">
                        <FaArrowRight />
                    </button>
                </div>
            </div>

            <div className="relative">
                <div ref={scrollContainerRef} className="flex overflow-x-scroll scrollbar-hide space-x-6 px-8 py-4">
                    {deals.length > 0 ? (
                        deals.map((deal) => (
                            <div key={deal.productID} className="relative group bg-white border rounded-lg shadow-md overflow-hidden w-72 h-96 flex-shrink-0">
                                <div className="absolute top-2 right-2 bg-green-500 text-white text-xs font-bold px-2 py-1 rounded-full">
                                    - {calculateDiscountPercentage(deal.price, deal.discountedPrice)}%
                                </div>
                                <div className="absolute top-2 left-2 flex space-x-2">
                                    <WishlistButton productId={deal.productID} isWishlisted={deal.isWishlisted} onWishlistToggle={handleAddToWishlist} />
                                    <ViewDetailsButton productId={deal.productID} onViewDetails={handleViewDetails} />
                                </div>
                                <img src={deal.imageURL} alt={deal.productName} className="w-full h-48 object-cover" />
                                <div className="p-4 flex flex-col justify-between h-36">
                                    <h3 className="text-lg font-semibold mb-1">{deal.productName}</h3>
                                    <div className="flex items-center space-x-2">
                                        <span className="text-xl font-bold text-green-600">&#8377;{deal.discountedPrice}</span>
                                        <span className="text-sm text-gray-500 line-through">&#8377;{deal.price}</span>
                                    </div>
                                    <StarRating rating={deal.averageRating} reviewCount={deal.reviewCount} />
                                </div>
                                <div className="absolute bottom-0 left-0 w-full bg-gradient-to-t from-black to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                                    <AddToCartButton productId={deal.productID} onAddToCart={() => handleAddToCart(products)} />
                                </div>
                            </div>
                        ))
                    ) : (
                        <p className="text-center">No deals available at the moment.</p>
                    )}
                </div>
            </div>

            <div className="flex justify-center mt-6">
                <button onClick={handleViewAllProducts} className="bg-utorange text-white font-semibold py-2 px-4 rounded">
                    View All Products
                </button>
            </div>
        </div>
    );
};

const calculateDiscountPercentage = (originalPrice, discountedPrice) => {
    if (!originalPrice || !discountedPrice) return 0;
    return Math.round(((originalPrice - discountedPrice) / originalPrice) * 100);
};

TodayDeals.propTypes = {
    deals: PropTypes.arrayOf(
        PropTypes.shape({
            productID: PropTypes.number.isRequired,
            imageURL: PropTypes.string.isRequired,
            price: PropTypes.number.isRequired,
            discountedPrice: PropTypes.number.isRequired,
            flashSaleEnd: PropTypes.string.isRequired,
            productName: PropTypes.string.isRequired,
            averageRating: PropTypes.number.isRequired,
            reviewCount: PropTypes.number.isRequired,
        })
    ),
};

export default TodayDeals;
