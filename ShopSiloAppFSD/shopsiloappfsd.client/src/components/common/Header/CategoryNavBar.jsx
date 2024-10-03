import React, { useState } from 'react';
import { VscTriangleDown } from "react-icons/vsc";
import { Link } from 'react-router-dom'; // Import Link for routing

const Categories = ({ categories }) => {
    if (!categories.length) {
        return <p className="text-center text-gray-500">No categories available</p>;
    }

    return (
        <nav className="flex space-x-6 p-4 bg-white-800">
            {categories.map((category) => (
                <CategoryCard key={category.categoryID} category={category} />
            ))}
        </nav>
    );
};

const CategoryCard = ({ category }) => {
    const [isOpen, setIsOpen] = useState(false);

    const handleMouseEnter = () => setIsOpen(true);
    const handleMouseLeave = () => setIsOpen(false);

    return (
        <div
            className="relative"
            onMouseEnter={handleMouseEnter}
            onMouseLeave={handleMouseLeave}
        >
            <div className="flex items-center cursor-pointer p-2 text-gray hover:bg-white-700 rounded transition">
                <h3 className="text-sm font-semibold whitespace-nowrap">{category.categoryName}</h3>
                {category.subCategories && category.subCategories.$values.length > 0 && (
                    <span className="ml-2"><VscTriangleDown /></span>
                )}
            </div>
            {isOpen && category.subCategories && category.subCategories.$values.length > 0 && (
                <div className="absolute left-0 bg-white shadow-lg mt-1 rounded-lg z-20 w-48">
                    <div
                        onMouseEnter={handleMouseEnter} // Keep open when hovering over dropdown
                        onMouseLeave={handleMouseLeave} // Close when not hovering
                    >
                        {category.subCategories.$values.map((sub) => (
                            <Link
                                key={sub.categoryID}
                                to={`/category/${sub.categoryID}`} // Route to subcategory products
                                className="block p-2 text-gray-700 hover:bg-gray-100"
                            >
                                {sub.categoryName}
                            </Link>
                        ))}
                    </div>
                </div>
            )}
        </div>
    );
};

export default Categories;
