import React from 'react';
import { Link, useLocation } from 'react-router-dom';

const AdminNavigation = () => {
    const navItems = [
        { name: 'Customers', path: 'customers' },
        { name: 'Products', path: 'products' },
        { name: 'Sellers', path: 'sellers' },
        { name: 'Audit logs', path: 'autitlogs' },
    ];
    const location = useLocation(); // Get the current location

    return (
        <nav className="flex flex-wrap gap-10 items-start justify-between w-full md:justify-start my-auto">
            <h1 className="text-2xl font-bold tracking-wider leading-none whitespace-nowrap w-full md:w-auto text-utorange">
                <Link to='/admin/dashboard' className="hover:text-gray-600">Admin Dashboard</Link>
            </h1>
            <ul className="flex flex-wrap gap-10 items-start text-base text-center">
                {navItems.map((item, index) => (
                    <li key={index} className="whitespace-nowrap">
                        <Link to={item.path} className={`hover:text-gray-600 ${location.pathname.split('/')[2] === item.path ? 'text-utorange font-bold' : ''}`}>{item.name}</Link>
                    </li>
                ))}
        </ul>
        </nav >
    );
};

export default AdminNavigation;