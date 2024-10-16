import React, { useState, useEffect, useRef } from 'react';
import { IoIosArrowUp, IoIosArrowDown } from "react-icons/io";

const TopBar = () => {
    const [isDropdownOpen, setIsDropdownOpen] = useState(false);
    const [language, setLanguage] = useState('English');
    const dropdownRef = useRef(null);

    const toggleDropdown = () => {
        setIsDropdownOpen(!isDropdownOpen);
    };

    const selectLanguage = (lang) => {
        setLanguage(lang);
        setIsDropdownOpen(false);
        // Add any additional logic for language change here
    };

    // Close the dropdown when clicking outside
    useEffect(() => {
        const handleClickOutside = (event) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target)) {
                setIsDropdownOpen(false);
            }
        };

        document.addEventListener('mousedown', handleClickOutside);
        return () => {
            document.removeEventListener('mousedown', handleClickOutside);
        };
    }, []);

    return (
        <header className="flex justify-between items-center px-16 py-4 pb-3 w-full text-sm bg-black text-neutral-50 max-md:px-5 h-9">
            {/* Left Side: Sale Message and ShopNow Link */}
            <div className="flex items-center gap-2 max-md:flex-col max-md:items-center max-md:text-center max-md:w-full">
                <p className="text-center max-md:max-w-full">
                    Summer Sale For All Swim Suits And Free Express Delivery - OFF 50%!
                </p>
                <a href="#" className="font-semibold leading-6 underline">
                    ShopNow
                </a>
            </div>

            {/* Right Side: Language Selector */}
            <div className="relative flex items-center gap-1.5" ref={dropdownRef}>
                <span>{language}</span>
                <button
                    onClick={toggleDropdown}
                    className="flex items-center justify-center focus:outline-none ml-1"
                    aria-haspopup="true"
                    aria-expanded={isDropdownOpen}
                >
                    {isDropdownOpen ? <IoIosArrowUp /> : <IoIosArrowDown />}
                </button>
                {isDropdownOpen && (
                    <ul
                        className="absolute right-0 top-full mt-1 w-32 bg-white text-black border border-gray-300 rounded shadow-lg z-20"
                        role="menu"
                        aria-label="Language selection"
                    >
                        <li
                            className="px-4 py-2 hover:bg-gray-100 cursor-pointer"
                            role="menuitem"
                            onClick={() => selectLanguage('English')}
                        >
                            English
                        </li>
                        <li
                            className="px-4 py-2 hover:bg-gray-100 cursor-pointer"
                            role="menuitem"
                            onClick={() => selectLanguage('Tamil')}
                        >
                            Tamil
                        </li>
                        <li
                            className="px-4 py-2 hover:bg-gray-100 cursor-pointer"
                            role="menuitem"
                            onClick={() => selectLanguage('Telugu')}
                        >
                            Telugu
                        </li>
                        {/* Add more languages here as needed */}
                    </ul>
                )}
            </div>
        </header>
    );
};

export default TopBar;
