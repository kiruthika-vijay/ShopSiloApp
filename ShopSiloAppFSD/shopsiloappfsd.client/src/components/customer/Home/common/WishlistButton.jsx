import React, { useEffect, useState } from 'react';
import { FaHeart, FaTrash } from 'react-icons/fa';
import PropTypes from 'prop-types';
import useWishlist from '../HomeUtils/useWishlist';
import { apiClient, getToken } from '../../../common/Axios/auth';

const WishlistButton = ({ productId }) => {
    const { wishlistId, addToWishlist, removeFromWishlist, wishlistLoading } = useWishlist();
    const [inWishlist, setInWishlist] = useState(false);

    useEffect(() => {
        const checkWishlist = async () => {
            if (wishlistId) {
                try {
                    const response = await apiClient.get(`/Wishlists/${wishlistId}`, {
                        headers: { Authorization: `Bearer ${getToken()}` },
                    });
                    // Access wishlistItems correctly
                    const items = response.data.wishlistItems.$values || [];
                    const itemExists = items.some(item => item.productID === productId);
                    setInWishlist(itemExists);
                } catch (error) {
                    console.error(`Error checking wishlist: ${error.message}`);
                }
            }
        };

        checkWishlist(); // Call to check wishlist on component mount and when wishlistId changes
    }, [wishlistId, productId]);

    const handleWishlistClick = async () => {
        if (inWishlist) {
            await removeFromWishlist(productId);
            setInWishlist(false);
        } else {
            await addToWishlist(productId);
            setInWishlist(true);
        }
    };

    return (
        <button
            onClick={handleWishlistClick}
            className={`text-white ${inWishlist ? 'bg-red-600' : 'bg-red-500'} rounded-full p-2 hover:bg-red-700 transition-colors`}
            aria-label={inWishlist ? 'Remove from Wishlist' : 'Add to Wishlist'}
        >
            {inWishlist ? <FaTrash /> : <FaHeart />}
        </button>
    );
};

WishlistButton.propTypes = {
    productId: PropTypes.number.isRequired,
};

export default WishlistButton;
