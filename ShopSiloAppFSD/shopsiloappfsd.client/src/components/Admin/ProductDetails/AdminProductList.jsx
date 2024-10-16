import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';
import ProductForm from './ProductForm';
import SelectDropdown from './SelectDropdown';
import Pagination from '@mui/material/Pagination';

const AdminProductList = () => {
    const [products, setProducts] = useState([]);
    const [filteredProducts, setFilteredProducts] = useState([]);
    const [searchTerm, setSearchTerm] = useState('');
    const [selectedCategory, setSelectedCategory] = useState('');
    const [categories, setCategories] = useState([]);
    const [showProductList, setShowProductList] = useState(true);
    const [productID, setProductID] = useState(undefined);
    const [reviews, setReviews] = useState({});

    // Pagination state
    const [currentPage, setCurrentPage] = useState(1);
    const [pageSize] = useState(5); // Set number of products per page

    useEffect(() => {
        fetchProducts();
        fetchCategories();
    }, []);

    useEffect(() => {
        filterProducts();
    }, [products, searchTerm, selectedCategory]);

    const fetchProducts = async () => {
        try {
            const response = await apiClient.get('/Product');
            setProducts(response.data.$values);
            console.log(response.data.$values);
            setFilteredProducts(response.data.$values);
            response.data.$values.forEach(product => fetchProductReviews(product.productID));
        } catch (error) {
            console.error('Error fetching products:', error);
        }
    };

    const fetchCategories = async () => {
        try {
            const response = await apiClient.get('/Categories/names');
            const fetchedCategories = response.data.$values.map(d => ({ id: d.categoryID, name: d.categoryName }));
            setCategories(fetchedCategories);
            console.log('Fetched categories:', fetchedCategories);  // Ensure this logs the correct data
        } catch (error) {
            console.error('Error fetching categories:', error);
        }
    };

    const fetchProductReviews = async (productId) => {
        try {
            const response = await apiClient.get(`/ProductReview/Product/${productId}`);
            setReviews(prevReviews => ({ ...prevReviews, [productId]: response.data.reviews.$values }));
        } catch (error) {
            console.error(`Error fetching reviews for product ${productId}:`, error);
        }
    };

    const onClose = async () => {
        setProductID(undefined);
        setShowProductList(true);
        await fetchProducts(); // Refresh products after closing the form
    };

    const handleDelete = async (id) => {
        if (window.confirm('Are you sure you want to delete this product?')) {
            try {
                await apiClient.delete(`/Product/${id}`);
                fetchProducts();
            } catch (error) {
                console.error('Error deleting product:', error);
            }
        }
    };

    const handleProductUpdated = async () => {
        await fetchProducts(); // Fetch products to update the list dynamically
    };

    const getAverageRating = (productReviews) => {
        if (!productReviews || productReviews.length === 0) return 'No ratings';
        const totalRating = productReviews.reduce((sum, review) => sum + review.rating, 0);
        return (totalRating / productReviews.length).toFixed(1);
    };

    // Function to handle filtering logic
    const filterProducts = () => {
        const filtered = products.filter((product) => {
            const matchesCategory = selectedCategory ? product.categoryID === Number(selectedCategory) : true;
            const matchesSearch = product.productName.toLowerCase().includes(searchTerm.toLowerCase());
            return matchesCategory && matchesSearch;
        });
        setFilteredProducts(filtered);
        setCurrentPage(1); // Reset to the first page whenever filters change
    };

    const handleChange = (event, value) => {
        setCurrentPage(value);
    };

    //// Pagination logic
    const indexOfLastProduct = currentPage * pageSize;
    const indexOfFirstProduct = indexOfLastProduct - pageSize;
    const currentProducts = filteredProducts.slice(indexOfFirstProduct, indexOfLastProduct);
    const totalPages = Math.ceil(filteredProducts.length / pageSize);

    return (
        <div className="container mx-auto px-4 py-6">
            {showProductList && (
                <div>
                    <h2 className="text-2xl font-bold mb-4">Product List</h2>
                    <div className="flex justify-between mb-4">
                        <button onClick={() => { setShowProductList(false); setProductID(undefined); }} className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">Add New Product</button>
                        <div className="flex">
                            <SelectDropdown data={categories} onSelect={setSelectedCategory} title={"All Categories"} />
                            <input
                                type="text"
                                placeholder="Search products..."
                                className="border rounded p-2 ml-2"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                    </div>

                    <div className="space-y-4">
                        {currentProducts.map((product) => (
                            <div key={product.productID} className="bg-white rounded-lg shadow-lg overflow-hidden hover:shadow-2xl transition-shadow duration-300 flex items-center">
                                {/* Left: Product Image */}
                                <div className="w-1/3">
                                    <img src={product.imageURL} alt={product.productName} className="w-full h-auto object-cover" />
                                </div>

                                {/* Right: Product Details */}
                                <div className="p-6 w-2/3">
                                    <h3 className="text-lg font-semibold mb-1">{product.productName}</h3>
                                    <p className="text-gray-700 mb-2">₹{product.price.toFixed(2)}</p>
                                    <p className="text-gray-600 mb-2">Stock: {product.stockQuantity}</p>
                                    <p className="text-gray-500 mb-2">Category: {product.categoryName}</p>
                                    <p className="text-yellow-500 mb-2">Rating: {getAverageRating(reviews[product.productID])}</p>

                                    {/* Reviews */}
                                    {reviews[product.productID] && reviews[product.productID].map(review => (
                                        <div key={review.reviewID} className="mt-2">
                                            <p className="text-gray-800">{review.reviewText}</p>
                                            <p className="text-sm text-gray-600">- {review.userID}, {new Date(review.reviewDate).toLocaleDateString()}</p>
                                        </div>
                                    ))}

                                    {/* Edit and Delete Buttons */}
                                    <div className="flex justify-end mt-4">
                                        <button onClick={() => { setShowProductList(false); setProductID(product.productID); }} className="text-blue-500 hover:underline mr-4">Edit</button>
                                        <button onClick={() => handleDelete(product.productID)} className="text-red-500 hover:underline">Delete</button>
                                    </div>
                                </div>
                            </div>
                        ))}
                    </div>

                    <div className="flex justify-center mt-4">
                        <Pagination
                            count={totalPages}
                            page={currentPage}
                            onChange={handleChange}
                            color="primary"
                            showFirstButton
                            showLastButton
                        />
                    </div>
                </div>
            )}

            {!showProductList && (
                <ProductForm
                    id={productID}
                    onClose={onClose}
                    onProductUpdated={handleProductUpdated} // Pass the callback function
                />
            )}
        </div>
    );
};

export default AdminProductList;
