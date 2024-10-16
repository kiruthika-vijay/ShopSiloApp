import React, { useContext, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import Button from "@mui/material/Button";
import '../../SellerLayout/SellerLayout.css';
import { MdDashboard } from "react-icons/md";
import { FaTags } from "react-icons/fa6";
import { MdLock } from "react-icons/md";
import { FaAngleRight } from "react-icons/fa6";
import { FaCartShopping } from "react-icons/fa6";
import { IoNotifications } from "react-icons/io5";
import { BiSolidReport } from "react-icons/bi";
import { BsPeopleFill } from "react-icons/bs";
import { getToken, removeToken } from '../../../common/Axios/auth';
import { AuthContext } from '../../../customer/Auth/AuthContext';
import { useEffect } from 'react';
const SellerSidebar = () => {

    const [activeTab, setActiveTab] = useState(0);
    const [isToggleSubmenu, setIsToggleSubmenu] = useState(false);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const navigate = useNavigate();

    const { isLoggedIn, setIsLoggedIn } = useContext(AuthContext);

    useEffect(() => {
        const token = getToken();
        if (token) {
            setIsLoggedIn(true);
        } else {
            setIsLoggedIn(false); // Ensure user is marked as not logged in
        }
    }, [setIsLoggedIn]);

    const isOpenProductSubMenu = (index) => {
        setActiveTab(index);
        setIsToggleSubmenu(!isToggleSubmenu);
    }

    const handleLogoutClick = () => {
        setIsModalOpen(true);
    };

    const confirmLogout = () => {
        removeToken();
        setIsLoggedIn(false);
        setIsModalOpen(false);
        navigate('/seller/login'); // Navigate to seller dashboard
    };

    const cancelLogout = () => {
        setIsModalOpen(false);
    };

    return (
        <div className="sidebar">
            <ul>
                <li>
                    <Link to="/seller/dashboard">
                        <Button className={`w-100 navButton ${activeTab === 0 ? 'active' : ''}`} onClick={() => isOpenProductSubMenu(0)} >
                            <span className="icon"><MdDashboard /></span>
                            Dashboard
                            <span className="arrow"><FaAngleRight /></span>
                        </Button>
                    </Link>
                </li>
                <li>
                    <Button className={`w-100 navButton ${activeTab === 1 && isToggleSubmenu === true ? 'active' : ''}`} onClick={() => isOpenProductSubMenu(1)}>
                        <span className="icon"><FaTags /></span>
                        Products
                        <span className="arrow"><FaAngleRight /></span>
                    </Button>
                    <div className={`submenuWrapper ${activeTab === 1 && isToggleSubmenu === true ? 'colapse' : 'colapsed'}`}>
                        <ul className="submenu">
                            <li><Link to="/seller/products/productlist">Product List</Link></li>
                            <li><Link to="/seller/products/productupload">Product Upload</Link></li>
                            <li><Link to="/seller/products/inventoryManagement">Inventory Management</Link></li>
                        </ul>
                    </div>
                </li>
                <li>
                    <Button className={`w-100 navButton ${activeTab === 2 && isToggleSubmenu === true ? 'active' : ''}`} onClick={() => isOpenProductSubMenu(2)} >
                        <span className="icon"><FaCartShopping /></span>
                        Orders
                        <span className="arrow"><FaAngleRight /></span>
                    </Button>
                    <div className={`submenuWrapper ${activeTab === 2 && isToggleSubmenu === true ? 'colapse' : 'colapsed'}`}>
                        <ul className="submenu">
                            <li><Link to="/seller/orders/orderview">Order View</Link></li>
                        </ul>
                    </div>
                </li>
                {/*<li>*/}
                {/*    <Link to="/seller/reports">*/}
                {/*        <Button className={`w-100 navButton ${activeTab === 3 ? 'active' : ''}`} onClick={() => isOpenProductSubMenu(3)} >*/}
                {/*            <span className="icon"><BiSolidReport /> </span>*/}
                {/*            Reports*/}
                {/*            <span className="arrow"><FaAngleRight /></span>*/}
                {/*        </Button>*/}
                {/*    </Link>*/}
                {/*</li>*/}
                <li>
                    <Link to="/seller/account/profile">
                        <Button className={`w-100 navButton ${activeTab === 4 ? 'active' : ''}`} onClick={() => isOpenProductSubMenu(4)}>
                            <span className="icon"><BsPeopleFill /></span>
                            Account
                            <span className="arrow"><FaAngleRight /></span>
                        </Button>
                    </Link>
                </li>
            </ul>

            <br />
            {isLoggedIn && (
            <div className="logoutWrapper">
                <div className="logoutBox">
                        <Button variant="contained" onClick={handleLogoutClick} ><MdLock color="white"/> Logout</Button>
                </div>
            </div>
            )}

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

        </div>
    );
};

export default SellerSidebar;
