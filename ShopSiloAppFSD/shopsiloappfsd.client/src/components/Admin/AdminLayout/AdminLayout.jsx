import React, { useEffect, useState } from 'react';
import { Link, Outlet } from 'react-router-dom';
import HeaderAdmin from '../HeaderAdmin/HeaderAdmin';
import CustomerFooter from '../../common/Footer/Footer';

const AdminLayout = () => {


    return (
        <div>
            <main className="main">
                <HeaderAdmin />
                <Outlet />  {/* This renders the child routes, now below the Header */}
            </main>
            <CustomerFooter />
        </div>
    )
    const scrollToTop = () => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };
}

export default AdminLayout;