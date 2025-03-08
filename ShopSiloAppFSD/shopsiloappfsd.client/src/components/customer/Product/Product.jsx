import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { FaHeart, FaEye, FaCartPlus } from 'react-icons/fa';
import { Select, MenuItem, FormControl, InputLabel } from '@mui/material';
import { apiClient } from '../../common/Axios/auth';
import useProducts from '../Home/HomeUtils/useProducts';
import useWishlist from '../Home/HomeUtils/useWishlist';
import useCart from '../Home/HomeUtils/useCart';

const AllProducts = () => {
    const { products } = useProducts(); // Fetch all products
    const [categories, setCategories] = useState([]);
    const [categoryId, setCategoryId] = useState(null);
    const [subcategories, setSubcategories] = useState([]);
    const [filterProducts, setFilterProducts] = useState([]); // Default is empty
    const [selectedCategory, setSelectedCategory] = useState('all');
    const [selectedSubcategory, setSelectedSubcategory] = useState('all');
    const [sortOption, setSortOption] = useState('none');
    const { wishlistId, addToWishlist } = useWishlist();
    const { addToCart } = useCart();
    const navigate = useNavigate();

    const handleBack = () => navigate(-1);
    const handleProductClick = (productId) => navigate(`/customer/product/${productId}`);

    useEffect(() => {
        setFilterProducts(products); // Initialize with all products on load or update
    }, [products]);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await apiClient.get('/Categories/names');
                setCategories(response.data.$values);
            } catch (error) {
                console.error('Error fetching categories:', error);
            }
        };
        fetchCategories();
    }, []);

    useEffect(() => {
        const fetchSubcategories = async () => {
            if (categoryId) {
                const response = await apiClient.get(`/Categories/Subcategories/${categoryId}`);
                setSubcategories(response.data.$values);
            } else {
                setSubcategories([]);
            }
        };
        fetchSubcategories();
    }, [categoryId]);

    const fetchProductsByCategory = async (selectedCategoryId, selectedSubcategoryId) => {
        try {
            let filteredProducts;

            if (selectedCategoryId === 'all' && selectedSubcategoryId === 'all') {
                filteredProducts = products;
            } else if (selectedSubcategoryId !== 'all') {
                const response = await apiClient.get(`/Product/Category/${selectedSubcategoryId}/m`);
                filteredProducts = response.data.$values || [];
            } else {
                const response = await apiClient.get(`/Product/Category/${selectedCategoryId}`);
                filteredProducts = response.data.$values || [];
            }

            setFilterProducts(filteredProducts);
            setSortOption('none');
        } catch (error) {
            console.error('Error fetching products by category:', error);
            setFilterProducts([]);
        }
    };

    const handleCategoryChange = (event) => {
        const selectedValue = event.target.value;
        setSelectedCategory(selectedValue);
        setCategoryId(selectedValue === 'all' ? null : selectedValue);
        setSelectedSubcategory('all');
        fetchProductsByCategory(selectedValue, 'all');
    };

    const handleSubcategoryChange = (event) => {
        const selectedValue = event.target.value;
        setSelectedSubcategory(selectedValue);
        fetchProductsByCategory(categoryId, selectedValue);
    };

    const handleSortChange = (event) => {
        setSortOption(event.target.value);
    };

    useEffect(() => {
        let sortedProducts = [...filterProducts];

        switch (sortOption) {
            case 'priceLowToHigh':
                sortedProducts.sort((a, b) => a.price - b.price);
                break;
            case 'priceHighToLow':
                sortedProducts.sort((a, b) => b.price - a.price);
                break;
            case 'topRated':
                sortedProducts.sort((a, b) => (b.rating || 0) - (a.rating || 0));
                break;
            default:
                break;
        }
        setFilterProducts(sortedProducts);
    }, [sortOption]);

    const handleAddToWishlist = async (productId) => {
        if (!wishlistId) {
            console.error("Wishlist ID is not available.");
            return;
        }
        try {
            await addToWishlist(productId);
        } catch (error) {
            console.error(`Error adding to Wishlist: ${error.message}`);
        }
    };

    const handleAddToCart = async (productID) => {
        await addToCart(productID);
    };

    return (
        <div className="p-5">
            <h1 className="text-2xl font-bold">All Products</h1>
            <div className="mt-4 p-5 flex justify-between items-center">
                <button onClick={handleBack} className="mb-4 bg-gray-300 text-black px-4 py-2 rounded-md hover:bg-gray-400 transition-colors">
                    Back
                </button>
                <div className="flex space-x-4">
                    <FormControl sx={{ minWidth: 120 }} size="small">
                        <InputLabel sx={{ backgroundColor: 'white' }}>Category</InputLabel>
                        <Select value={selectedCategory} onChange={handleCategoryChange}>
                            <MenuItem value="all">All</MenuItem>
                            {categories.map((category) => (
                                <MenuItem key={category.categoryID} value={category.categoryID}>
                                    {category.categoryName}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    <FormControl sx={{ minWidth: 120 }} size="small">
                        <InputLabel sx={{ backgroundColor: 'white' }}>Subcategory</InputLabel>
                        <Select value={selectedSubcategory} onChange={handleSubcategoryChange} disabled={!categoryId}>
                            <MenuItem value="all">All</MenuItem>
                            {subcategories.map((subcategory) => (
                                <MenuItem key={subcategory.categoryID} value={subcategory.categoryID}>
                                    {subcategory.categoryName}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    <FormControl sx={{ minWidth: 120 }} size="small">
                        <InputLabel sx={{ backgroundColor: 'white' }}>Sort By</InputLabel>
                        <Select value={sortOption} onChange={handleSortChange}>
                            <MenuItem value="none">None</MenuItem>
                            <MenuItem value="priceLowToHigh">Price: Low to High</MenuItem>
                            <MenuItem value="priceHighToLow">Price: High to Low</MenuItem>
                            <MenuItem value="topRated">Top Rating</MenuItem>
                        </Select>
                    </FormControl>
                </div>
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {filterProducts.map((product) => (
                    <div
                        key={product.productID}
                        className="border rounded-lg shadow-md overflow-hidden cursor-pointer"
                        onClick={() => handleProductClick(product.productID)}
                    >
                        <img src={product.imageURL} alt={product.productName} className="w-full h-48 object-cover" />
                        <div className="p-4">
                            <h2 className="text-lg font-semibold">{product.productName}</h2>
                            <div className="flex items-center space-x-2">
                                {product.discountedPrice > 0 ? (
                                    <>
                                        <span className="text-xl font-bold text-green-600">&#8377;{product.discountedPrice}</span>
                                        <span className="text-sm text-gray-500 line-through">&#8377;{product.price}</span>
                                    </>
                                ) : (
                                    <span className="text-xl font-bold text-green-600">&#8377;{product.price}</span>
                                )}
                            </div>
                            <p className="text-sm text-gray-500">{product.description}</p>
                        </div>

                        <div className="flex justify-between items-center p-4 border-t">
                            <button
                                className="text-red-500 hover:text-red-600"
                                onClick={(e) => {
                                    e.stopPropagation();
                                    handleAddToWishlist(product.productID);
                                }}
                            >
                                <FaHeart />
                            </button>
                            <button
                                className="text-blue-500 hover:text-blue-600"
                                onClick={(e) => {
                                    e.stopPropagation();
                                    handleAddToCart(product.productID);
                                }}
                            >
                                <FaCartPlus />
                            </button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
};

export default AllProducts;
