import React, { useContext } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { AuthContext } from '../../customer/Auth/AuthContext'; // Adjust the import path as needed
import './Navigation.css';

const Navigation = () => {
    const { isLoggedIn } = useContext(AuthContext);
    const location = useLocation(); // Get the current route

    const navItems = [
        { name: 'Home', path: '/customer/home'},
        { name: 'Contact', path: '/customer/contact' },
        { name: 'About', path: '/customer/about' },
    ];

    // Include "Sign Up" and "Login" only if the user is not logged in
    if (!isLoggedIn) {
        navItems.push(
            { name: 'Sign Up', path: '/customer/register' },
            { name: 'Login', path: '/customer/login' }
        );
    } else {
        // Optionally, add other navigation items for logged-in users
        navItems.push(
            { name: 'Profile', path: '/customer/account/profile' }
            // Add more items as needed
        );
    }

    return (
        <nav className="flex flex-wrap gap-10 items-start self-stretch my-auto min-w-[240px] max-md:max-w-full navlink">
            <h1 className="text-2xl font-bold tracking-wider leading-none whitespace-nowrap w-[118px] text-utorange">
                ShopSilo
            </h1>
            <ul className="flex gap-10 navlink items-start text-base text-center min-w-[240px]">
                {navItems.map((item, index) => (
                    <li
                        key={index}
                        className={`whitespace-nowrap ${item.name === 'Sign Up' ? 'w-[61px]' : 'w-12'
                            }`}
                    >
                        <Link
                            to={item.path}
                            className={`hover:text-gray-600 pb-1 transition-all duration-300 ${location.pathname === item.path
                                    ? 'border-b-4 border-orange-500' // Active page underline
                                    : 'hover:border-b-4 hover:border-gray-400' // Hover underline
                                }`}
                        >
                            {item.name}
                        </Link>
                    </li>
                ))}
            </ul>
        </nav>
    );
};

export default Navigation;
