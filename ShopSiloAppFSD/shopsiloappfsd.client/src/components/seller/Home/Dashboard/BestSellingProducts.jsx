import React, { useEffect, useState } from 'react';
import { FaEdit, FaTrash } from 'react-icons/fa';
import { apiClient } from '../../../common/Axios/auth';
import MenuItem from '@mui/material/MenuItem';
import Pagination from '@mui/material/Pagination';
import FormControl from '@mui/material/FormControl';
import Select from '@mui/material/Select';
import { FaStar } from "react-icons/fa";
import ProductForm from '../Product/ProductForm';
import { useNavigate } from 'react-router-dom';

const BestSellingProducts = () => {
    const [products, setProducts] = useState([]);
    const [selectedProduct, setSelectedProduct] = useState('all');
    const [filteredProducts, setFilteredProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [subCategories, setSubCategories] = useState([]);
    const [productNames, setProductNames] = useState([]);
    const [totalPages, setTotalPages] = useState(1);
    const [pageCount, setPageCount] = useState(1);
    const [rating, setRating] = useState('');
    const [categoryId, setCategoryId] = useState('all');
    const [subcategoryId, setSubcategoryId] = useState('all');
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const itemsPerPage = 5;
    const navigate = useNavigate();

    useEffect(() => {
        const fetchProducts = async () => {
            setLoading(true);
            try {
                const response = await apiClient.get(`/Seller/products`);
                const fetchedProducts = response.data.$values;
                setProducts(fetchedProducts);
                console.log(fetchedProducts);
                setFilteredProducts(fetchedProducts);
                setTotalPages(Math.ceil(response.data.$values.length / itemsPerPage));
                console.log(Math.ceil(response.data.$values.length / itemsPerPage), itemsPerPage);
            } catch (err) {
                setError(err.message || 'Error fetching products');
            } finally {
                setLoading(false);
            }
        };

        const fetchCategories = async () => {
            try {
                const response = await apiClient.get('/Categories/names');
                setCategories(response.data.$values);
                console.log(response.data.$values);
            } catch (err) {
                setError(err.message || 'Error fetching categories');
            }
        };

        const fetchProductNames = async () => {
            try {
                const response = await apiClient.get('/SellerDashboard/products');
                setProductNames(response.data.$values);
                console.log(response.data.$values);
            } catch (err) {
                setError(err.message || 'Error fetching product names');
            }
        };

        fetchProducts();
        fetchCategories();
        fetchProductNames();
    }, []);

    useEffect(() => {
        // Fetch subcategories when a new category is selected
        const fetchSubcategories = async () => {
            if (categoryId && categoryId !== 'all') {
                try {
                    const response = await apiClient.get(`/Categories/Subcategories/${categoryId}`);
                    setSubCategories(response.data.$values);
                } catch (error) {
                    console.error("Error fetching subcategories:", error);
                }
            } else {
                setSubCategories([]);
            }
        };
        fetchSubcategories();
    }, [categoryId]);

    const handleCategoryChange = (event) => {
        const selectedValue = event.target.value;
        setCategoryId(selectedValue);
        setSelectedProduct('all');
        setSubcategoryId('all'); // Reset subcategory when category changes
        filterProducts(selectedValue, 'all');
    };

    const handleSubcategoryChange = (event) => {
        const selectedValue = event.target.value;
        setSubcategoryId(selectedValue);
        filterProducts(categoryId, selectedValue);
    };

    const handleRatingChange = (event) => {
        const selectedRating = event.target.value;
        setSelectedProduct('all');
        setRating(selectedRating);
        filterProducts(categoryId, subcategoryId, selectedRating); // Update filtering on rating change
    };

    const handleProductChange = (event) => {
        const chosenProduct = event.target.value;
        setSelectedProduct(chosenProduct);
        filterProducts(categoryId, subcategoryId, rating, chosenProduct); // Update filtering on product name change
    };

    const filterProducts = async (selectedCategoryId, selectedSubcategoryId, selectedRating, selectedProduct) => {
        let filtered = products;

        // Filter by category
        if (selectedCategoryId !== 'all') {
            const response = await apiClient.get(`/Product/Category/${selectedCategoryId}`);
            filtered = response.data.$values || [];
        }

        // Filter by subcategory
        if (selectedSubcategoryId !== 'all') {
            const response = await apiClient.get(`/Product/Category/${selectedSubcategoryId}/m`);
            filtered = response.data.$values || [];
        }

        // Filter by rating
        if (selectedRating) {
            filtered = filtered.filter(product => product.averageRating >= selectedRating);
        }

        if (selectedProduct) {
            filtered = filtered.filter(product => product.productName == selectedProduct);
        }

        setFilteredProducts(filtered);
        setTotalPages(Math.ceil(filtered.length / itemsPerPage));

        // Reset page count to 1 when filtering
        setPageCount(1);
    };

    // Calculate the products to display for the current page
    const startIndex = (pageCount - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const displayedProducts = filteredProducts.slice(startIndex, endIndex);


    const handleEditClick = (productId) => {
        // Navigate to the edit product page with the selected productId
        navigate(`/seller/editproduct/${productId}`);
    };

    const handleDelete = async (productId) => {
        try {
            await apiClient.delete(`/Product/${productId}`);
            setProducts(products.filter(item => item.productID !== productId));
        } catch (err) {
            setError(err.message || 'Error deleting product');
        }
    };

    //if (loading) return <p className="text-center text-gray-500">Loading...</p>;
    if (error) return <p className="text-center text-red-500">{error}</p>;

    return (
        <div className="p-6 bg-gray-50 rounded-lg shadow-md mt-4">
            <h3 className="text-xl font-bold mb-4">Best Selling Products</h3>
            <div className="grid grid-cols-4 gap-4 mb-4">
                <div>
                    <h4 className="text-m font-semibold mb-2">BY PRODUCT NAME</h4>
                    <FormControl size="small" className="w-full auto">
                        <Select
                            value={selectedProduct}
                            onChange={handleProductChange}
                            displayEmpty
                            inputProps={{ 'aria-label': 'Without label' }}
                            className="w-full"
                        >
                            <MenuItem value="all">
                                <em>All Products</em>
                            </MenuItem>
                            {products.map((item) => (
                                <MenuItem key={item.productName} value={item.productName}>
                                    {item.productName}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </div>
                <div>
                    <h4 className="text-m font-semibold mb-2">BY CATEGORY</h4>
                    <FormControl size="small" className="w-full auto">
                        <Select
                            value={categoryId}
                            onChange={handleCategoryChange}
                            displayEmpty
                            inputProps={{ 'aria-label': 'Without label' }}
                            className="w-full"
                        >
                            <MenuItem value="all">
                                <em>All Categories</em>
                            </MenuItem>
                            {categories.map((category) => (
                                <MenuItem key={category.categoryID} value={category.categoryID}>
                                    {category.categoryName}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </div>
                <div>
                    <h4 className="text-m font-semibold mb-2">BY SUB CATEGORY</h4>
                    <FormControl size="small" className="w-full auto">
                        <Select
                            value={subcategoryId}
                            onChange={handleSubcategoryChange}
                            displayEmpty
                            inputProps={{ 'aria-label': 'Without label' }}
                            className="w-full"
                        >
                            <MenuItem value="all">
                                <em>All Sub Categories</em>
                            </MenuItem>
                            {subCategories.map((category) => (
                                <MenuItem key={category.categoryID} value={category.categoryID}>
                                    {category.categoryName}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </div>
                <div>
                    <h4 className="text-m font-semibold mb-2">BY RATING</h4>
                    <FormControl size="small" className="w-full auto">
                        <Select
                            value={rating}
                            onChange={handleRatingChange}
                            displayEmpty
                            inputProps={{ 'aria-label': 'Without label' }}
                            className="w-full"
                        >
                            <MenuItem value="">
                                <em>All Ratings</em>
                            </MenuItem>
                            {[1, 2, 3, 4, 5].map((star) => (
                                <MenuItem key={star} value={star} className="flex items-center">
                                    <div className="flex">
                                        {Array(star).fill().map((_, i) => (
                                            <FaStar key={i} className="text-yellow-500" />
                                        ))}
                                    </div>
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </div>
            </div>

            <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-gray-200">
                    <thead className="bg-blue-200">
                        <tr>
                            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">PID</th>
                            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">PRODUCT</th>
                            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">CATEGORY</th>
                            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">PRICE</th>
                            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">STOCK</th>
                            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">RATING</th>
                            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">ORDER</th>
                            <th className="px-4 py-2 text-left text-sm font-semibold text-gray-600">ACTION</th>
                        </tr>
                    </thead>
                    <tbody className="bg-white divide-y divide-gray-200">
                        {displayedProducts.map((item, index) => (
                            <tr key={item.productID}>
                                <td className="px-4 py-2 text-sm text-gray-700">#{(pageCount - 1) * itemsPerPage + (index + 1)}</td>
                                <td className="px-4 py-2 text-sm text-gray-700">{item.productName}</td>
                                <td className="px-4 py-2 text-sm text-gray-700">{item.categoryName || 'Unknown'}</td>
                                <td className="px-4 py-2 text-sm text-gray-700">{item.price.toFixed(2)}</td>
                                <td className="px-4 py-2 text-sm text-gray-700">{item.stockQuantity}</td>
                                <td className="px-4 py-2 text-sm text-gray-700 flex items-center">
                                    <FaStar color="#FABC3F" />
                                    <span className="ml-1 text-gray-500 font-semibold">{item.averageRating}</span> ({item.reviewCount})
                                </td>
                                <td className="px-4 py-2 text-sm text-gray-700">{item.totalOrders || 0}</td>
                                <td className="px-4 py-2 text-sm text-gray-700 flex space-x-2">
                                    <button to="/seller/add-product" className="bg-yellow-500 text-white px-3 py-1 rounded hover:bg-yellow-400" onClick={() => handleEditClick(item.productID)}>
                                        <FaEdit />
                                    </button>
                                    <button className="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-400" onClick={() => handleDelete(item.productID)}>
                                        <FaTrash />
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            </div>
            <Pagination
                count={totalPages}
                page={pageCount} // Corrected here
                onChange={(e, value) => setPageCount(value)}
                color="primary"
                className="mt-4"
                showFirstButton
                showLastButton
            />
        </div>
    );
};

export default BestSellingProducts;
