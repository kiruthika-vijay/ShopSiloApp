import React, { useState, useEffect } from 'react';
import SellerNavbar from '../Home/common/SellerNavbar';
import SellerSidebar from '../Home/common/SellerSidebar';
import SellerFooter from '../Home/common/SellerFooter';
import { Outlet } from 'react-router-dom'; // Import Outlet
import "./SellerLayout.css";
import { getToken } from '../../common/Axios/auth';

const SellerLayout = () => {
    const [sidebarCollapsed, setSidebarCollapsed] = useState(false);
    const [isLoggedIn, setIsLoggedIn] = useState(false); // Track login status

    useEffect(() => {
        const token = getToken();
        if (token) {
            setIsLoggedIn(true);
        } else {
            setIsLoggedIn(false); // Ensure user is marked as not logged in
        }
    }, [setIsLoggedIn]);

    const handleSidebarToggle = (collapsed) => {
        setSidebarCollapsed(collapsed); // Update the sidebar collapsed state
    };

    return (
        <div>
            {isLoggedIn && <SellerNavbar />}
            <div style={{ display: 'flex' }}>
                <div className="main d-flex">
                    <div className="sidebarWrapper">
                        <SellerSidebar />
                    </div>
                </div>
                <main
                    style={{
                        flex: 1,
                        padding: '20px',
                        paddingTop: '20px',
                        marginTop: '60px',
                        marginLeft: sidebarCollapsed ? '70px' : '240px', // Adjust main content based on sidebar state
                        transition: 'margin-left 0.4s ease' // Smooth transition effect
                    }}
                    className="bg-gray-100"
                >
                    <Outlet /> {/* This renders the child routes */}
                </main>
            </div>
            {/*{isLoggedIn && <SellerFooter sidebarCollapsed={sidebarCollapsed} />}*/}
        </div>
    );
};

export default SellerLayout;
