// Wishlist.jsx
import React, { useState, useEffect, useContext } from 'react';
import { FaTrash, FaShoppingCart } from 'react-icons/fa'; // Import FaShoppingCart
import { apiClient, getToken } from '../../../common/Axios/auth';
import { AuthContext } from '../../Auth/AuthContext';
import { Link } from 'react-router-dom';
import useWishlist from '../HomeUtils/useWishlist';
import useCart from '../HomeUtils/useCart';
import { CountContext } from '../../../common/Header/CountContext';
import AddToCartButton from '../common/AddToCart';
import { Tab } from 'bootstrap';

const Wishlist = () => {
    const { setCartCount, setWishlistCount } = useContext(CountContext);
    const [wishlistItems, setWishlistItems] = useState([]);
    const [suggestions, setSuggestions] = useState([]);
    const { wishlistId, addToWishlist, removeFromWishlist, wishlistLoading, wishlistError } = useWishlist();
    const { addToCart } = useCart();
    const { isLoggedIn } = useContext(AuthContext); // Get isLoggedIn from context

    useEffect(() => {
        const token = getToken();
        if (token && isLoggedIn) {
            fetchWishlistItems(token);
            fetchSuggestions();
        }
       
    }, [isLoggedIn]); // Only run when isLoggedIn changes

    const fetchWishlistItems = async (token) => {
        try {
            const response = await apiClient.get('/Wishlists', {
                headers: { Authorization: `Bearer ${token}` },
            });
            setWishlistItems(response.data.$values || []);
        } catch (error) {
            console.error('Error fetching wishlist items:', error);
        }
    };

    const fetchSuggestions = async () => {
        try {
            const response = await apiClient.get('/Product/suggestions');
            setSuggestions(response.data.$values || []);
        } catch (error) {
            console.error('Error fetching suggestions:', error);
        }
    };

    const removeItemFromWishlist = async (productID) => {
        if (!isLoggedIn) return; // Do nothing if not logged in

        try {
            const token = getToken();
            removeFromWishlist(productID);
            setWishlistItems(wishlistItems.filter(item => item.productID !== productID));
        } catch (error) {
            console.error('Error removing item from wishlist:', error);
        }
    };

    const addToCartHandler = async (productID) => {
        if (!isLoggedIn) return; // Do nothing if not logged in

        try {
            // Add the item to the cart
            await addToCart(productID);

            // Once the item is added to the cart, remove it from the wishlist
            await removeItemFromWishlist(productID);

            // Immediately update the UI by filtering the item out of the wishlistItems state
            setWishlistItems(wishlistItems.filter(item => item.productID !== productID));
            if (response.ok) { // Check if the addition was successful
                setCartCount(prevCount => prevCount + 1); // Increment cart count
            }
        } catch (error) {
            console.error('Error adding item to cart:', error);
        }
    };

    const moveAllToBag = async () => {
        if (!isLoggedIn) return; // Do nothing if not logged in

        try {
            const token = getToken();
            const addToCartPromises = wishlistItems.map(item =>
                apiClient.post(`/Wishlists/${wishlistId}/move-to-bag`, {
                    headers: { Authorization: `Bearer ${token}` },
                })
            );
            await Promise.all(addToCartPromises);
            setWishlistItems([]); // Clear wishlist after moving to 
            setWishlistCount(0);
            // you can update the cart count based on the number of items moved
            setCartCount(prevCartCount => prevCartCount + wishlistItems.length);
        } catch (error) {
            console.error('Error moving all items to cart:', error);
        }
    };

    // Helper function to calculate discount
    const calculateDiscount = (originalPrice, discountedPrice) => {
        if (originalPrice && discountedPrice) {
            const discount = Math.round(((originalPrice - discountedPrice) / originalPrice) * 100);
            return discount;
        }
        return null;
    };

    return (
        <div className="container mx-auto p-5 wishlistContent">
            {/* Wishlist Header */}
            <header className="flex justify-between items-center mb-5 wishlist">
                <h1 className="text-2xl font-bold ml-10">Wishlist ({wishlistItems.length})</h1>
                {isLoggedIn && wishlistItems.length > 0 && (
                    <button
                        onClick={moveAllToBag}
                        className="bg-orange-500 text-white px-4 py-2 rounded hover:bg-orange-600 transition-colors mr-20"
                    >
                        Move All to Bag
                    </button>
                )}
            </header>

            {/* Wishlist Items */}
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6 wishlistItem">
                {wishlistItems.length > 0 ? (
                    wishlistItems.map(item => {
                        const discountPercentage = calculateDiscount(item.originalPrice, item.discountedPrice);
                        return (
                            <div key={item.id} className="relative border rounded-lg overflow-hidden flex flex-col">
                                {/* Product Image Container */}
                                <div className="relative">
                                    {/* Discount Badge */}
                                    {discountPercentage && (
                                        <span className="absolute top-2 left-2 bg-red-500 text-white text-xs font-semibold px-2 py-1 rounded">
                                            -{discountPercentage}%
                                        </span>
                                    )}

                                    {/* Product Image */}
                                    <img
                                        src={item.imageUrl}
                                        alt={item.productName}
                                        className="w-full h-48 object-cover rounded-lg"
                                    />

                                    {/* Delete Icon */}
                                    <button
                                        onClick={() => removeItemFromWishlist(item.productID)}
                                        className="absolute top-2 right-2 bg-white rounded-full w-8 h-8 flex items-center justify-center shadow-md"
                                        aria-label="Remove from wishlist"
                                    >
                                        <FaTrash className="text-black" />
                                    </button>
                                </div>

                                {/* Product Details */}
                                <div className="p-4 flex flex-col flex-grow">
                                    <h3 className="font-semibold text-lg mb-2">{item.productName}</h3>
                                    <div className="mb-4">
                                        {discountPercentage ? (
                                            <div className="flex items-center">
                                                <span className="text-gray-500 line-through mr-2">&#8377;{item.originalPrice}</span>
                                                <span className="text-utorange font-bold">&#8377;{item.discountedPrice}</span>
                                            </div>
                                        ) : (
                                                <span className="text-utorange font-bold">&#8377;{item.price}</span>
                                        )}
                                    </div>
                                    {/* Add to Cart Button*/}
                                    <div className="mt-auto left-0 bottom-0 right-0 rounded w-full hover:bg-gray-800 transition-colors flex items-center justify-center space-x-2">
                                        <AddToCartButton productId={item.productID} onAddToCart={() => addToCartHandler(item.productId)} />
                                    </div>
                                </div>
                            </div>
                        );
                    })
                ) : (
                    <p>No items in your wishlist.</p>
                )}
            </div>

            {/* Just for You Section */}
            <section className="mt-10">
                <div className="flex justify-between items-center mb-4">
                    <h2 className="text-xl font-bold">Just for You</h2>
                    <Link to="/suggestions">
                        <button className="bg-orange-500 text-white px-4 py-2 rounded hover:bg-orange-600 transition-colors">
                           View All
                        </button>
                    </Link>
                </div>
                <div className="grid grid-cols-2 sm:grid-cols-4 gap-6">
                    {suggestions.slice(0, 4).map(suggestion => {
                        const discountPercentage = calculateDiscount(suggestion.originalPrice, suggestion.discountedPrice);
                        return (
                            <div key={suggestion.id} className="relative border rounded-lg overflow-hidden flex flex-col">
                                {/* Product Image Container */}
                                <div className="relative">
                                    {/* Discount Badge */}
                                    {discountPercentage && (
                                        <span className="absolute top-2 left-2 bg-red-500 text-white text-xs font-semibold px-2 py-1 rounded">
                                            - {discountPercentage}%
                                        </span>
                                    )}

                                    {/* Product Image */}
                                    <img
                                        src={suggestion.imageUrl}
                                        alt={suggestion.productName}
                                        className="w-full h-48 object-cover rounded-lg"
                                    />
                                </div>

                                {/* Product Details */}
                                <div className="p-4 flex flex-col flex-grow">
                                    <h3 className="font-semibold text-lg mb-2">{suggestion.productName}</h3>
                                    <div className="mb-4">
                                        {discountPercentage ? (
                                            <div className="flex items-center">
                                                <span className="text-gray-500 line-through mr-2">&#8377;{suggestion.originalPrice}</span>
                                                <span className="text-utorange font-bold">&#8377;{suggestion.discountedPrice}</span>
                                            </div>
                                        ) : (
                                                <span className="text-utorange font-bold">&#8377;{suggestion.price}</span>
                                        )}
                                    </div>
                                    {/* Add to Cart Button*/}
                                    <div className="mt-auto left-0 bottom-0 right-0 rounded w-full hover:bg-gray-800 transition-colors flex items-center justify-center space-x-2">
                                        <AddToCartButton productId={suggestion.productID} onAddToCart={() => addToCartHandler(suggestion.productId)} />
                                    </div>
                                </div>
                            </div>
                        );
                    })}
                </div>
            </section>
        </div>
    );

};

export default Wishlist;
