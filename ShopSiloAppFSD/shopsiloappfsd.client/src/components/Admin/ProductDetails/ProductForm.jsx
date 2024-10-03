import React, { useState, useEffect } from 'react';
import { apiClient } from '../../common/Axios/auth';

const ProductForm = ({ id, onClose, onProductUpdated }) => { // Added onProductUpdated prop
    const [product, setProduct] = useState({
        productName: '',
        description: '',
        price: '',
        stockQuantity: '',
        categoryID: '',
        sellerID: '',
        imageUrl: '',
    });
    const [categories, setCategories] = useState([]);
    const [sellers, setSellers] = useState([]);
    const [errors, setErrors] = useState({});

    useEffect(() => {
        if (id) {
            fetchProduct(id);
        }
        fetchCategories();
        fetchSellers();
    }, [id]);

    const fetchProduct = async (id) => {
        try {
            const response = await apiClient.get(`/Product/${ id }`);
            setProduct(response.data);
        } catch (error) {
            console.error('Error fetching product:', error);
        }
    };

    const fetchCategories = async () => {
        try {
            const response = await apiClient.get('/Categories/names');
            setCategories(response.data.$values.map(d => ({ id: d.categoryID, name: d.categoryName })));
        } catch (error) {
            console.error('Error fetching categories:', error);
        }
    };

    const fetchSellers = async () => {
        try {
            const response = await apiClient.get('/Seller/list'); // Replace with your actual sellers endpoint
            setSellers(response.data.$values.map(d => ({ id: d.sellerID, name: d.companyName })));
        } catch (error) {
            console.error('Error fetching sellers:', error);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setProduct({
            ...product,
            [name]: value,
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (id) {
                await apiClient.put(`/Product/${id}`, product); // Update existing product
            } else {
                await apiClient.post('/Product', product); // Create new product
            }
            onProductUpdated(); // Call this function to update the product list dynamically
            onClose();
        } catch (error) {
            if (error.response && error.response.data) {
                setErrors(error.response.data.errors || {});
            }
            console.error('Error saving product:', error);
        }
    };

    return (
        <div className="container mx-auto px-4 py-6">
            <h2 className="text-2xl font-bold mb-4">{id ? 'Edit Product' : 'Add New Product'}</h2>
            <form onSubmit={handleSubmit}>
                <div className="mb-4">
                    <label className="block text-gray-700">Product Name</label>
                    <input
                        type="text"
                        name="productName"
                        value={product.productName}
                        onChange={handleInputChange}
                        className="border rounded p-2 w-full"
                        required
                    />
                    {errors.productName && <p className="text-red-500">{errors.productName}</p>}
                </div>
                <div className="mb-4">
                    <label className="block text-gray-700">Description</label>
                    <textarea
                        name="description"
                        value={product.description}
                        onChange={handleInputChange}
                        className="border rounded p-2 w-full"
                        rows="4"
                    />
                    {errors.description && <p className="text-red-500">{errors.description}</p>}
                </div>

                <div className="mb-4">
                    <label className="block text-gray-700">Price</label>
                    <input
                        type="number"
                        name="price"
                        value={product.price}
                        onChange={handleInputChange}
                        className="border rounded p-2 w-full"
                        required
                    />
                    {errors.price && <p className="text-red-500">{errors.price}</p>}
                </div>

                <div className="mb-4">
                    <label className="block text-gray-700">Stock Quantity</label>
                    <input
                        type="number"
                        name="stockQuantity"
                        value={product.stockQuantity}
                        onChange={handleInputChange}
                        className="border rounded p-2 w-full"
                        required
                    />
                    {errors.stockQuantity && <p className="text-red-500">{errors.stockQuantity}</p>}
                </div>

                <div className="mb-4">
                    <label className="block text-gray-700">Category</label>
                    <select
                        name="categoryID"
                        value={product.categoryID}
                        onChange={handleInputChange}
                        className="border rounded p-2 w-full"
                        required
                    >
                        <option value="">Select Category</option>
                        {categories.map(category => (
                            <option key={category.id} value={category.id}>
                                {category.name}
                            </option>
                        ))}
                    </select>
                    {errors.categoryID && <p className="text-red-500">{errors.categoryID}</p>}
                </div>

                <div className="mb-4">
                    <label className="block text-gray-700">Seller</label>
                    <select
                        name="sellerID"
                        value={product.sellerID}
                        onChange={handleInputChange}
                        className="border rounded p-2 w-full"
                        required
                    >
                        <option value="">Select Seller</option>
                        {sellers.map(seller => (
                            <option key={seller.id} value={seller.id}>
                                {seller.name}
                            </option>
                        ))}
                    </select>
                    {errors.sellerID && <p className="text-red-500">{errors.sellerID}</p>}
                </div>

                <div className="mb-4">
                    <label className="block text-gray-700">Image URL</label>
                    <input
                        type="text"
                        name="imageUrl"
                        value={product.imageUrl}
                        onChange={handleInputChange}
                        className="border rounded p-2 w-full"
                    />
                    {errors.imageUrl && <p className="text-red-500">{errors.imageUrl}</p>}
                </div>

                <div className="flex justify-between">
                    <button type="submit" className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600">
                        {id ? 'Update Product' : 'Add Product'}
                    </button>
                    <button type="button" onClick={onClose} className="bg-gray-500 text-white px-4 py-2 rounded hover:bg-gray-600">
                        Cancel
                    </button>
                </div>
            </form>
        </div>
    );
};

export default ProductForm;