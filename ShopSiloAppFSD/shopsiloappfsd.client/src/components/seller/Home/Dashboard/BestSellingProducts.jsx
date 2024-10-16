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
    const [editModalOpen, setEditModalOpen] = useState(false);
    const [selectedProductId, setSelectedProductId] = useState(null);
    const [filteredProducts, setFilteredProducts] = useState([]);
    const [categories, setCategories] = useState([]);
    const [productNames, setProductNames] = useState([]);
    const [totalPages, setTotalPages] = useState(1);
    const [pageCount, setPageCount] = useState(1);
    const [productName, setProductName] = useState('');
    const [rating, setRating] = useState('');
    const [categoryName, setCategoryName] = useState('');
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
            } catch (err) {
                setError(err.message || 'Error fetching product names');
            }
        };

        fetchProducts();
        fetchCategories();
        fetchProductNames();
    }, []);

    useEffect(() => {
        const handleFilter = () => {
            let filtered = [...products];

            if (productName) {
                filtered = filtered.filter((product) => product.productName === productName);
            }

            if (categoryName) {
                filtered = filtered.filter((product) => product.categoryName === categoryName);
            }

            if (rating) {
                filtered = filtered.filter((product) => product.averageRating >= rating);
            }

            setFilteredProducts(filtered);
            setTotalPages(Math.ceil(filtered.length / itemsPerPage));

            // Reset page count to 1 when filtering
            setPageCount(1);
        };

        handleFilter();
    }, [productName, categoryName, rating, products]); // Make sure to include products

    // Calculate the products to display for the current page
    const startIndex = (pageCount - 1) * itemsPerPage;
    const endIndex = startIndex + itemsPerPage;
    const displayedProducts = filteredProducts.slice(startIndex, endIndex);


    const handleEditClick = (productId) => {
        // Navigate to the edit product page with the selected productId
        navigate(`/seller/editproduct/${productId}`);
    };

    const handleClose = () => {
        setEditModalOpen(false); // Close the edit modal
        setAddModalOpen(false); // Close the add modal
        setSelectedProductId(null); // Reset the selected product ID
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
            <div className="grid grid-cols-3 gap-4 mb-4">
                <div>
                    <h4 className="text-lg font-semibold">BY PRODUCT NAME</h4>
                    <FormControl size="small" className="w-full">
                        <Select
                            value={productName}
                            onChange={(e) => setProductName(e.target.value)}
                            displayEmpty
                            inputProps={{ 'aria-label': 'Without label' }}
                            className="w-full"
                        >
                            <MenuItem value="">
                                <em>None</em>
                            </MenuItem>
                            {productNames.map((productName, index) => (
                                <MenuItem key={index} value={productName}>
                                    {productName}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </div>
                <div>
                    <h4 className="text-lg font-semibold">BY CATEGORY</h4>
                    <FormControl size="small" className="w-full">
                        <Select
                            value={categoryName}
                            onChange={(e) => setCategoryName(e.target.value)}
                            displayEmpty
                            inputProps={{ 'aria-label': 'Without label' }}
                            className="w-full"
                        >
                            <MenuItem value="">
                                <em>None</em>
                            </MenuItem>
                            {categories.map((category) => (
                                <MenuItem key={category.categoryName} value={category.categoryName}>
                                    {category.categoryName}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </div>
                <div>
                    <h4 className="text-lg font-semibold">BY RATING</h4>
                    <FormControl size="small" className="w-full">
                        <Select
                            value={rating}
                            onChange={(e) => setRating(e.target.value)}
                            displayEmpty
                            inputProps={{ 'aria-label': 'Without label' }}
                            className="w-full"
                        >
                            <MenuItem value="">
                                <em>0</em>
                            </MenuItem>
                            {[1, 2, 3, 4, 5].map((rate) => (
                                <MenuItem key={rate} value={rate}>
                                    {rate}
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
                                    <span className="ml-1 text-gray-500 font-semibold">{(item.averageRating).toFixed(1)}</span> ({item.reviewCount})
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
                page={itemsPerPage}
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
