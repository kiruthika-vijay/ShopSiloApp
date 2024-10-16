import React, { useEffect, useState, useContext } from 'react';
import TopBar from './TopBar';
import Navigation from './Navigation';
import SearchBar from './SearchBar';
import { IoCart, IoHeart, IoPerson, IoPower } from 'react-icons/io5';
import { Link } from 'react-router-dom';
import { AuthContext } from '../../customer/Auth/AuthContext';
import { apiClient, getToken, removeToken } from '../Axios/auth';
import { CountContext } from './CountContext';

const Header = () => {
    const { cartCount, wishlistCount } = useContext(CountContext);
    const [isModalOpen, setIsModalOpen] = useState(false);

    const { isLoggedIn, setIsLoggedIn } = useContext(AuthContext);

    useEffect(() => {
        const token = getToken();
        if (token) {
            setIsLoggedIn(true);
        } else {
            setIsLoggedIn(false); // Ensure user is marked as not logged in
        }
    }, [setIsLoggedIn]);

    const handleLogoutClick = () => {
        setIsModalOpen(true);
    };

    const confirmLogout = () => {
        removeToken();
        setIsLoggedIn(false);
        setIsModalOpen(false);
        // Optionally, you could redirect to a different page here
    };

    const cancelLogout = () => {
        setIsModalOpen(false);
    };

    return (
        <header className="flex flex-col">
            <TopBar/>
            <div className="flex justify-between items-center gap-10 px-16 py-4 bg-white text-black-50 max-md:flex-col max-md:px-5 pt-7 pb-0 mb-5">
                <Navigation />
                <div className="flex items-center gap-6">
                    <SearchBar />
                    <div className="flex items-center gap-4">
                        {isLoggedIn && (
                            <>
                                <Link to="/customer/wishlist" className="relative group">
                                    <IoHeart className="w-6 h-6 hover:text-gray-400" />
                                    <span className="absolute -top-2 -right-3 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                                        {wishlistCount > 0 ? wishlistCount : 0}
                                    </span>
                                </Link>
                                <Link to="/customer/cart" className="relative group">
                                    <IoCart className="w-6 h-6 hover:text-gray-400" />
                                    <span className="absolute -top-2 -right-3 bg-red-500 text-white text-xs rounded-full w-5 h-5 flex items-center justify-center">
                                        {cartCount > 0 ? cartCount : 0}
                                    </span>
                                </Link>
                                <Link to="/customer/profile">
                                    <IoPerson className="w-6 h-6 hover:text-gray-400" />
                                </Link>
                                <IoPower className="w-6 h-6 hover:text-gray-400 cursor-pointer" onClick={handleLogoutClick} />
                            </>
                        )}
                    </div>
                </div>
            </div>

            {/* Logout Confirmation Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-50">
                    <div className="bg-white p-4 rounded shadow-lg">
                        <h2 className="text-lg font-semibold">Confirm Logout</h2>
                        <p>Are you sure you want to log out?</p>
                        <div className="flex justify-between mt-4">
                            <button className="bg-red-500 text-white px-4 py-2 rounded" onClick={confirmLogout}>Yes</button>
                            <button className="bg-gray-300 px-4 py-2 rounded" onClick={cancelLogout}>No</button>
                        </div>
                    </div>
                </div>
            )}
        </header>
    );
};

export default Header;
