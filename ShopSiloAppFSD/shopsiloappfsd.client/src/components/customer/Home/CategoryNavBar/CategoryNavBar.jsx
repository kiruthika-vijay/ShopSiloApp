import React, { useState } from 'react';
import { FaChevronDown, FaChevronRight } from 'react-icons/fa';
import PropTypes from 'prop-types';
import { Link } from 'react-router-dom'; // Import Link from react-router-dom

const CategoryNavbar = ({ categories }) => {
    const [hoveredCategory, setHoveredCategory] = useState(null);

    const handleMouseEnter = (categoryId) => {
        setHoveredCategory(categoryId);
    };

    const handleMouseLeave = () => {
        setHoveredCategory(null);
    };

    return (
        <div className="flex flex-col p-4 bg-white shadow-lg rounded-lg">
            {categories.map((category) => (
                <div
                    key={category.categoryID}
                    className="relative mb-2"
                    onMouseEnter={() => handleMouseEnter(category.categoryID)}
                    onMouseLeave={handleMouseLeave}
                >
                    <Link
                        to={`/customer/browse-categories/${category.categoryID}`} // Link for the category
                        className="flex justify-between items-center cursor-pointer p-3 text-gray-700 bg-gray-100 hover:bg-orange-100 rounded transition-all duration-300 ease-in-out"
                    >
                        <span className="font-medium">{category.categoryName}</span>
                        {category.subCategories.$values.length > 0 && (
                            hoveredCategory === category.categoryID ? (
                                <FaChevronDown className="text-gray-600" />
                            ) : (
                                <FaChevronRight className="text-gray-600" />
                            )
                        )}
                    </Link>
                    {hoveredCategory === category.categoryID && category.subCategories.$values.length > 0 && (
                        <div className="ml-6 mt-2 bg-orange-50 rounded-lg shadow-inner p-3 border-l-4 border-orange-300">
                            {category.subCategories.$values.map((sub) => (
                                <div key={sub.categoryID} className="p-2 hover:bg-orange-100 hover:rounded transition-colors duration-200 ease-in-out">
                                    <Link
                                        to={`/customer/categories/${sub.categoryID}`}
                                        className="text-gray-700 hover:text-orange-600 font-medium"
                                    >
                                        {sub.categoryName}
                                    </Link> {/* Link for subcategory */}
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            ))}
        </div>
    );
};

CategoryNavbar.propTypes = {
    categories: PropTypes.arrayOf(
        PropTypes.shape({
            categoryID: PropTypes.number.isRequired,
            categoryName: PropTypes.string.isRequired,
            subCategories: PropTypes.shape({
                $values: PropTypes.arrayOf(
                    PropTypes.shape({
                        categoryID: PropTypes.number.isRequired,
                        categoryName: PropTypes.string.isRequired,
                    })
                ).isRequired,
            }).isRequired,
        })
    ).isRequired,
};

export default CategoryNavbar;
