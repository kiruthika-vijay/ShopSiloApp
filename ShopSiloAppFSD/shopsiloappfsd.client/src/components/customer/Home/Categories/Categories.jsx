import React, { useRef, useState, useEffect } from 'react';
import { FaArrowLeft, FaArrowRight } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { getIcon } from './iconMapping'; // Import the getIcon function
import { apiClient } from '../../../common/Axios/auth';
import './Categories.css';

const Categories = () => {
    const scrollContainerRef = useRef(null);
    const [categories, setCategories] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await apiClient.get('/Categories/names');
                setCategories(response.data.$values); // Accessing the categories array directly
                setLoading(false);
            } catch (err) {
                console.error('Error fetching categories:', err);
                setError('Failed to load categories.');
                setLoading(false);
            }
        };

        fetchCategories();
    }, []);

    const scrollLeft = () => {
        if (scrollContainerRef.current) {
            scrollContainerRef.current.scrollBy({
                top: 0,
                left: -300,
                behavior: 'smooth',
            });
        }
    };

    const scrollRight = () => {
        if (scrollContainerRef.current) {
            scrollContainerRef.current.scrollBy({
                top: 0,
                left: 300,
                behavior: 'smooth',
            });
        }
    };

    if (loading) {
        return (
            <div className="mt-10 p-5 bg-gray-100 flex justify-center items-center">
                <p className="text-gray-700">Loading categories...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className="mt-10 p-5 bg-gray-100 flex justify-center items-center">
                <p className="text-red-500">{error}</p>
            </div>
        );
    }

    return (
        <div className="mt-10 p-5">
            <div className="flex items-center mb-6">
                <h3 className="flex items-center text-lg font-bold text-utorange">
                    <span className="block h-10 w-4 bg-orange-500 rounded-md mr-2"></span>
                    Categories
                </h3>
            </div>
            <div className="flex items-center justify-between mb-6">
                <h2 className="text-2xl font-bold">Browse by Category</h2>
                <div className="flex space-x-2">
                    <button
                        onClick={scrollLeft}
                        className="bg-white bg-opacity-75 hover:bg-opacity-100 text-gray-700 rounded-full p-2 shadow-md"
                        aria-label="Scroll Left"
                    >
                        <FaArrowLeft />
                    </button>
                    <button
                        onClick={scrollRight}
                        className="bg-white bg-opacity-75 hover:bg-opacity-100 text-gray-700 rounded-full p-2 shadow-md"
                        aria-label="Scroll Right"
                    >
                        <FaArrowRight />
                    </button>
                </div>
            </div>

            <div
                ref={scrollContainerRef}
                className="flex overflow-x-scroll scrollbar-hide space-x-6 px-4 py-2"
            >
                {categories.length > 0 ? (
                    categories.slice(0, 10).map((category) => (
                        <div
                            key={category.categoryID} // Adjusted to match the response
                            className="categorybox flex-shrink-0 w-40 h-40 bg-white border rounded-lg shadow-md flex flex-col items-center justify-center transition-transform transform hover:scale-105 hover:bg-orange-600 cursor-pointer relative group"
                            onClick={() => navigate(`/customer/browse-categories/${category.categoryID}`)} // Adjusted to match the response
                        >
                            <div className="text-4xl text-gray-700 group-hover:text-white transition-colors">
                                {React.createElement(getIcon(category.icon))} {/* Use getIcon to render icon */}
                            </div>
                            <span className="mt-2 text-sm font-semibold text-gray-700 group-hover:text-white transition-colors">
                                {category.categoryName} {/* Adjusted to match the response */}
                            </span>
                        </div>
                    ))
                ) : (
                    <p className="text-center">No categories available at the moment.</p>
                )}
            </div>
        </div>
    );
};

export default Categories;
