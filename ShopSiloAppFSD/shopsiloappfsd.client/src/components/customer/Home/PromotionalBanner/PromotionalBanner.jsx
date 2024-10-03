import React from 'react';
import PropTypes from 'prop-types';
import './PromotionalBanner.css';

const PromotionalBanner = ({ imageUrl, productId }) => {
    const handleBuyNow = () => {
        // Implement redirection to product description
        console.log(`Redirecting to product ID: ${productId}`);
    };

    return (
        <div className="relative w-full p-4">
            <div className="banner-container">
                <img
                    src={imageUrl}
                    alt="Promotional Banner"
                    className="w-full h-auto object-cover rounded-lg"
                />
                <button
                    onClick={handleBuyNow}
                    className="absolute bottom-4 left-4 bg-mustard text-white px-6 py-2 rounded-full hover:bg-darkmustard transition-colors"
                >
                    Buy Now
                </button>
            </div>
        </div>
    );
};

PromotionalBanner.propTypes = {
    imageUrl: PropTypes.string.isRequired,
    productId: PropTypes.number.isRequired, // Adjust based on your product ID type
};

export default PromotionalBanner;
