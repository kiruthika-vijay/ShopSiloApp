import React, { useEffect, useState } from 'react';
import { CloudUpload } from '@mui/icons-material';
import { TextField, Button, Grid, Typography, Input, MenuItem, Select, FormControl, InputLabel, Snackbar, Alert } from '@mui/material';
import { apiClient } from '../../../common/Axios/auth';
import { useNavigate, useParams } from 'react-router-dom';
import './ProductForm.css';

const EditProductForm = () => {
    const { productId } = useParams();
    const [productData, setProductData] = useState({
        productID: '',
        productName: '',
        description: '',
        price: '',
        stockQuantity: '',
        categoryID: '',
        subCategoryID: '',
        categoryName: '',
        subCategoryName: '',
        sellerId: '',
        imageUrl: '', // Add field for the current image URL
        publicId: '' // Field to store the Cloudinary image's public ID
    });
    const [image, setImage] = useState(null);
    const [categories, setCategories] = useState([]);
    const [subCategories, setSubCategories] = useState([]);
    const [sellerID, setSellerID] = useState(null);

    // Snackbar states
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [snackbarMessage, setSnackbarMessage] = useState('');
    const [snackbarSeverity, setSnackbarSeverity] = useState('success');

    const navigate = useNavigate();

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await apiClient.get('/Categories/names');
                setCategories(response.data.$values);
            } catch (error) {
                console.error('Error fetching categories:', error);
            }
        };

        const fetchSellers = async () => {
            try {
                const response = await apiClient.get('/Seller');
                setSellerID(response.data.sellerID);
            } catch (error) {
                console.error('Error fetching sellers:', error);
            }
        };

        const fetchProductData = async () => {
            if (productId) {
                try {
                    const response = await apiClient.get(`/Product/${productId}`);
                    const product = response.data;
                    setProductData({
                        productName: product.productName,
                        description: product.description,
                        price: product.price,
                        stockQuantity: product.stockQuantity,
                        categoryID: product.subCategoryID,
                        sellerId: product.sellerId,
                        imageUrl: product.imageUrl, // Populate with existing Cloudinary image URL
                        publicId: product.publicId // Public ID for future deletions or updates
                    });
                    setSubCategories(product.subCategories.$values);
                } catch (error) {
                    console.error('Error fetching product data:', error);
                }
            }
        };

        fetchCategories();
        fetchSellers();
        if (productId) fetchProductData();
    }, [productId]);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setProductData({ ...productData, [name]: value });
    };

    const handleImageChange = (e) => {
        setImage(e.target.files[0]);
    };

    const handleCategoryChange = (e) => {
        const selectedCategory = categories.find(category => category.categoryName === e.target.value);
        setProductData({
            ...productData,
            categoryID: selectedCategory.categoryID,
            categoryName: selectedCategory.categoryName,
            subCategoryID: '',
            subCategoryName: ''
        });
        setSubCategories(selectedCategory.subCategories.$values);
    };

    const handleSubCategoryChange = (e) => {
        const selectedSubCategory = subCategories.find(subCategory => subCategory.categoryName === e.target.value);
        setProductData({ ...productData, subCategoryID: selectedSubCategory.categoryID, subCategoryName: selectedSubCategory.categoryName });
    };

    const handleSnackbarClose = () => {
        setSnackbarOpen(false);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        const formData = new FormData();
        formData.append('productID', productId);
        formData.append('ProductName', productData.productName);
        formData.append('description', productData.description);
        formData.append('price', productData.price);
        formData.append('stockQuantity', productData.stockQuantity);
        formData.append('categoryID', productData.categoryID);
        formData.append('sellerId', sellerID);

        if (image) {
            // If a new image is uploaded, append it
            formData.append('image', image);
            formData.append('publicId', productData.publicId); // Append current public ID for image replacement
        }

        try {
            let response;
            if (productId) {
                // If editing, update the product
                response = await apiClient.put(`/Product/${productId}`, formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                });
                setSnackbarMessage('Product updated successfully!');
            } else {
                // If adding a new product
                response = await apiClient.post('/Product', formData, {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                });
                setSnackbarMessage('Product added successfully!');
            }

            setSnackbarSeverity('success');
            setSnackbarOpen(true);

            // Redirect to product list after 5 seconds
            setTimeout(() => {
                navigate('/seller/products/productlist');
            }, 5000);
        } catch (error) {
            console.error('Error submitting product:', error);
            setSnackbarMessage('Error submitting product. Please try again.');
            setSnackbarSeverity('error');
            setSnackbarOpen(true);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="m-5">
            <Typography variant="h5" style={{ marginBottom: '30px', color: 'blue', fontWeight: 'bold', marginLeft: '3px' }}>
                {productId ? 'Edit Product' : 'Add New Product'}
            </Typography>
            <Grid container spacing={2}>
                <Grid item xs={12}>
                    <TextField
                        label="Product Name"
                        variant="outlined"
                        fullWidth
                        name="productName"
                        value={productData.productName}
                        onChange={handleInputChange}
                        required
                    />
                </Grid>
                <Grid item xs={12}>
                    <TextField
                        label="Description"
                        variant="outlined"
                        fullWidth
                        name="description"
                        value={productData.description}
                        onChange={handleInputChange}
                        multiline
                        rows={4}
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <TextField
                        label="Price"
                        variant="outlined"
                        fullWidth
                        name="price"
                        type="number"
                        value={productData.price}
                        onChange={handleInputChange}
                        required
                    />
                </Grid>
                <Grid item xs={12} sm={6}>
                    <TextField
                        label="Stock Quantity"
                        variant="outlined"
                        fullWidth
                        name="stockQuantity"
                        type="number"
                        value={productData.stockQuantity}
                        onChange={handleInputChange}
                        required
                    />
                </Grid>
                <Grid item xs={12}>
                    <FormControl fullWidth variant="outlined">
                        <InputLabel id="category-select-label">Category</InputLabel>
                        <Select
                            labelId="category-select-label"
                            value={productData.categoryName}
                            onChange={handleCategoryChange}
                            required
                        >
                            {categories.map(category => (
                                <MenuItem key={category.categoryID} value={category.categoryName}>
                                    {category.categoryName}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                </Grid>
                {subCategories.length > 0 && (
                    <Grid item xs={12}>
                        <FormControl fullWidth variant="outlined">
                            <InputLabel id="subcategory-select-label">Subcategory</InputLabel>
                            <Select
                                labelId="subcategory-select-label"
                                value={productData.subCategoryName}
                                onChange={handleSubCategoryChange}
                                required
                            >
                                {subCategories.map(subCategory => (
                                    <MenuItem key={subCategory.categoryID} value={subCategory.categoryName}>
                                        {subCategory.categoryName}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                )}
                <Grid item xs={12}>
                    {productData.imageUrl && !image && (
                        <div className="image-preview">
                            <img src={productData.imageUrl} alt="Product" style={{ width: '150px', marginBottom: '10px' }} />
                        </div>
                    )}
                    <Button variant="contained" component="label" startIcon={<CloudUpload />}>
                        {image ? 'Replace Image' : 'Upload Image'}
                        <Input type="file" name="image" onChange={handleImageChange} hidden />
                    </Button>
                </Grid>
                <Grid item xs={12}>
                    <Button type="submit" variant="contained" color="primary">
                        {productId ? 'Update Product' : 'Add Product'}
                    </Button>
                </Grid>
            </Grid>

            {/* Snackbar for displaying success/error messages */}
            <Snackbar open={snackbarOpen} autoHideDuration={6000} onClose={handleSnackbarClose}>
                <Alert onClose={handleSnackbarClose} severity={snackbarSeverity} sx={{ width: '100%' }}>
                    {snackbarMessage}
                </Alert>
            </Snackbar>
        </form>
    );
};

export default EditProductForm;
