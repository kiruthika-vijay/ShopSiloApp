import React, { useEffect, useState } from 'react';
import { Link } from "react-router-dom";
import { MdMenuOpen, MdOutlineLightMode, MdDarkMode, MdOutlineMailOutline } from "react-icons/md";
import { IoCartOutline } from "react-icons/io5";
import { FaRegBell } from "react-icons/fa6";
import SearchBox from './SearchBox';
import SwitchAccountIcon from '@mui/icons-material/SwitchAccount';
import SecurityIcon from '@mui/icons-material/Security';
import { Menu, MenuItem } from '@mui/material';
import ListItemIcon from '@mui/material/ListItemIcon';
import Logout from '@mui/icons-material/Logout';
import Divider from '@mui/material/Divider';
import { apiClient } from '../../../common/Axios/auth';

const SellerNavbar = () => {
    const [anchorEl, setAnchorEl] = useState(null);
    const [notificationAnchorEl, setNotificationAnchorEl] = useState(null);
    const openMyAcc = Boolean(anchorEl);
    const openMyNotification = Boolean(notificationAnchorEl);
    const [orderCount, setOrderCount] = useState(0);
    const [sellerName, setSellerName] = useState('');
    const [sellerUsername, setSellerUsername] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState(false);

    useEffect(() => {
        const fetchSeller = async () => {
            setLoading(true);
            try {
                const response = await apiClient.get(`/Seller`);
                setSellerName(response.data.contactPerson);
                setSellerUsername(response.data.user.username);
            } catch (err) {
                setError(err.message || 'Error fetching Seller details');
            } finally {
                setLoading(false);
            }
        };

        fetchSeller();
    }, []);


    const handleOpenMyAccDrop = (event) => {
        setAnchorEl(event.currentTarget);
    };
    const handleCloseMyAccDrop = () => {
        setAnchorEl(null);
    };

    const handleOpenMyNotificationDrop = (event) => {
        setNotificationAnchorEl(event.currentTarget);
    };

    const handleCloseMyNotificationDrop = () => {
        setNotificationAnchorEl(null);
    };

    return (
        <header className="flex items-center bg-white shadow-md">
            <div className="container mx-auto w-full">
                <div className="flex items-center w-full">
                    {/* Logo Wrapper */}
                    <div className="flex items-center justify-start w-1/4 ml-5">
                        <Link to="/seller/dashboard" className="flex items-center">
                            <img src="/images/sellerLogo.png" alt="Logo" className="h-8 mr-2" />
                            <span className="text-lg font-bold">SHOPSILO</span>
                        </Link>
                    </div>

                    <div className="flex items-center w-1/2 pl-4">
                        <button className="rounded-full p-2 bg-gray-200 hover:bg-gray-300">
                            <MdMenuOpen />
                        </button>
                        <SearchBox />
                    </div>

                    <div className="flex items-center justify-end w-1/4 mr-10">

                        <button className="rounded-full p-2 bg-blue-200 hover:bg-blue-400 mr-4 w-10 h-10 text-2xl">
                            <Link to="/seller/orders/orderview"><IoCartOutline /></Link>
                        </button>
                        
                        <div className="myAccWrapper flex items-center justify-between">
                            <button onClick={handleOpenMyAccDrop} className="myAcc flex items-center">
                                <div className="userImg w-12 h-12 mr-3">
                                    <span className="rounded-full">
                                        <img src="https://avatar.iran.liara.run/public" alt="User" />
                                    </span>
                                </div>
                                <div className="userInfo">
                                    <h4>{sellerName}</h4>
                                    <p className="mb-0">@{sellerUsername}</p>
                                </div>
                            </button>

                            <Menu
                                anchorEl={anchorEl}
                                open={openMyAcc}
                                onClose={handleCloseMyAccDrop}
                            >
                                <MenuItem onClick={handleCloseMyAccDrop}>
                                    <ListItemIcon>
                                        <SwitchAccountIcon />
                                    </ListItemIcon>
                                    <Link to="/seller/account/profile">My account</Link>
                                </MenuItem>
                                <MenuItem onClick={handleCloseMyAccDrop}>
                                    <ListItemIcon>
                                        <SecurityIcon />
                                    </ListItemIcon>
                                    <Link to="/reset-password/:token">Reset Password</Link>
                                </MenuItem>
                                <MenuItem onClick={handleCloseMyAccDrop}>
                                    <ListItemIcon>
                                        <Logout />
                                    </ListItemIcon>
                                    Logout
                                </MenuItem>
                            </Menu>
                        </div>
                    </div>
                </div>
            </div>
        </header>
    );
};

export default SellerNavbar;
