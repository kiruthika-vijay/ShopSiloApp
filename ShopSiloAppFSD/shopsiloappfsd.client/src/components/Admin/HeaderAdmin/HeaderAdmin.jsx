// src/components/Header.js
import React, { useContext, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import TopBar from '../../common/Header/TopBar';
import AdminNavigation from './AdminNavigation';
import SearchBar from '../../common/Header/SearchBar';
import { IoPower } from 'react-icons/io5';
import { Link } from 'react-router-dom';
import { AuthContext } from '../../customer/Auth/AuthContext';
import FooterColumn from '../../common/Footer/FooterColumn';
import { getToken, removeToken } from '../../common/Axios/auth';
import { useEffect } from 'react';

const HeaderAdmin = () => {
    const { isLoggedIn, setIsLoggedIn } = useContext(AuthContext);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const navigate = useNavigate();

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
        navigate('/admin/login');
    };

    const cancelLogout = () => {
        setIsModalOpen(false);
    };

    return (
        <header className="flex flex-col">
            <div className="flex justify-between items-center gap-10 px-16 py-4 bg-white text-black-50 max-md:flex-col max-md:px-5">
                <AdminNavigation />
                <div className="flex items-center gap-6">
                    {/* <SearchBar /> */}
                    {/* Icons Section */}
                    <div className="flex items-center gap-4">
                        {isLoggedIn && (
                            <>
                                <IoPower className="w-6 h-6 hover:text-gray-400 cursor-pointer" onClick={handleLogoutClick} />
                            </>
                        )}
                    </div>
                </div>
            </div>
            {/* <FooterColumn/> */}
            <div className="w-full bg-black border border-black border-solid opacity-30 h-px" />

            {/* Logout Confirmation Modal */}
            {isModalOpen && (
                <div className="fixed inset-0 flex items-center justify-center bg-black bg-opacity-50 z-[1000000000]">
                    <div className="bg-white p-4 rounded shadow-lg z-[10000]">
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

export default HeaderAdmin;