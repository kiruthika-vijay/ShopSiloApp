import React, { useState } from 'react';
import { FaSearch } from "react-icons/fa";
import { useNavigate } from 'react-router-dom';

const SearchBar = () => {
    const [searchTerm, setSearchTerm] = useState('');
    const navigate = useNavigate();

    const handleSearch = (e) => {
        e.preventDefault();
        if (searchTerm.trim()) {
            navigate(`/customer/search?query=${encodeURIComponent(searchTerm)}`);
            setSearchTerm('');
        }
    };

    return (
        <div className="p-4">
            <form onSubmit={handleSearch} className="flex items-center text-sm min-w-[240px]">
                <div className="flex items-center py-2 pr-4 pl-4 rounded bg-gray-200 shadow-md min-w-[240px]">
                    <label htmlFor="search" className="sr-only">Search</label>
                    <input
                        type="text"
                        id="search"
                        placeholder="What are you looking for?"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        className="flex-grow bg-transparent border-none outline-none text-gray-800 text-sm sm:text-base w-40"
                        aria-label="Search for products"
                    />
                    <button type="submit" aria-label="Submit search" className="ml-2">
                        <FaSearch className="w-3 h-3 sm:w-4 sm:h-4 text-gray-600 hover:text-gray-800 transition-colors duration-200" />
                    </button>
                </div>
            </form>
        </div>
    );
};

export default SearchBar;
