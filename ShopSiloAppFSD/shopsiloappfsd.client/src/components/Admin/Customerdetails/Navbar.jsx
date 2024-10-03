import React, { useEffect, useState } from 'react';
import { Link, Route, Routes } from 'react-router-dom';
import CustomerList from './Customerdetails/Customerlist';
import CustomerForm from './Customerdetails/CustomerForm';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faMoon, faSun } from '@fortawesome/free-solid-svg-icons';

const Navbar = () => {
    const [darkMode, setDarkMode] = useState(() => {
        const storedTheme = localStorage.getItem('theme');
        return storedTheme === 'dark';
    });

    const toggleTheme = () => {
        setDarkMode(!darkMode);
    };

    useEffect(() => {
        if (darkMode) {
            document.documentElement.classList.add('dark');
            localStorage.setItem('theme', 'dark');
        } else {
            document.documentElement.classList.remove('dark');
            localStorage.setItem('theme', 'light');
        }
    }, [darkMode]);

    return (
        <div className={darkMode ? 'dark' : ''}>
            <div className="flex">
                {/* Sidebar */}
                <div className="w-1/4 h-screen bg-gray-800 dark:bg-gray-900 text-white p-4">
                    <nav>
                        <ul>
                            <li className="mb-4">
                                <Link
                                    to="/admin/customers"
                                    className="text-lg font-bold hover:text-gray-300"
                                >
                                    Customer Details
                                </Link>
                            </li>
                        </ul>
                    </nav>

                    {/* Theme Toggle Button */}
                    <button
                        onClick={toggleTheme}
                        className="mt-4 p-2 rounded bg-gray-600 dark:bg-gray-400 text-white dark:text-black flex items-center justify-center"
                    >
                        <FontAwesomeIcon icon={darkMode ? faSun : faMoon} className="mr-2" />
                        Toggle {darkMode ? 'Light' : 'Dark'} Mode
                    </button>
                </div>

                {/* Main Content Area */}
                <div className="w-3/4 p-4 dark:bg-gray-800 dark:text-white">
                    <Routes>
                        <Route path="/admin/customers" element={<CustomerList />} />
                        <Route path="/admin/customerform" element={<CustomerForm />} />
                    </Routes>
                </div>
            </div>
        </div>
    );
};

export default Navbar;