// AddToCartButton.js
import React, { useState, useEffect } from 'react';
import { FaShoppingCart } from 'react-icons/fa';
import PropTypes from 'prop-types';
import useCart from '../HomeUtils/useCart';

const AddToCartButton = ({ productId }) => {
    const { cartItems, addToCart } = useCart();
    const [addedToCart, setAddedToCart] = useState(false);

    useEffect(() => {
        // Check if the product is already in the cart
        const itemExists = cartItems.some(item => item.productID === productId);
        setAddedToCart(itemExists);
    }, [cartItems, productId]);

    const handleAddToCart = () => {
        if (!addedToCart) {
            addToCart(productId);
        }
    };

    return (
        <button
            onClick={handleAddToCart}
            className={`w-full ${addedToCart ? 'bg-gray-500 cursor-not-allowed' : 'bg-orange-500'} cartButton py-3 flex items-center justify-center space-x-2 hover:bg-orange-600 transition-colors`}
            disabled={addedToCart}
        >
            <FaShoppingCart />
            <span>{addedToCart ? 'Already in Cart' : 'Add to Cart'}</span>
        </button>
    );
};

AddToCartButton.propTypes = {
    productId: PropTypes.number.isRequired,
};

export default AddToCartButton;
