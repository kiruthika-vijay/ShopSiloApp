import React, { useEffect, useState } from 'react';
import { CloudUpload } from '@mui/icons-material'; // Import an icon for the upload button
import { TextField, Button, Grid, Typography, Input, MenuItem, Select, FormControl, InputLabel, Snackbar, Alert } from '@mui/material';
import { apiClient } from '../../../common/Axios/auth';
import { useNavigate } from 'react-router-dom'; // Import useNavigate for redirection
import './ProductForm.css';

const ProductAddForm = () => {
    const [productData, setProductData] = useState({
        productName: '',
        description: '',
        price: '',
        stockQuantity: '',
        categoryID: '',
        subCategoryID: '',
        categoryName: '',
        subCategoryName: '',
        sellerId: ''
    });
    const [image, setImage] = useState(null);
    const [categories, setCategories] = useState([]);
    const [subCategories, setSubCategories] = useState([]);
    const [sellerID, setSellerID] = useState(null);

    // Snackbar states
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [snackbarMessage, setSnackbarMessage] = useState('');
    const [snackbarSeverity, setSnackbarSeverity] = useState('success');

    const navigate = useNavigate(); // Initialize navigate

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

        fetchCategories();
        fetchSellers();
    }, []);

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
        formData.append('productName', productData.productName);
        formData.append('description', productData.description);
        formData.append('price', productData.price);
        formData.append('stockQuantity', productData.stockQuantity);
        formData.append('categoryID', productData.categoryID);
        formData.append('subCategoryID', productData.subCategoryID);
        formData.append('categoryName', productData.categoryName);
        formData.append('image', image);
        formData.append('sellerId', sellerID);

        try {
            const response = await apiClient.post('/Product', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data',
                },
            });
            console.log('Product Data:', productData);
            console.log('Image:', image);
            console.log('Product added successfully:', response.data);
            setSnackbarMessage('Product added successfully!');
            setSnackbarSeverity('success');
            setSnackbarOpen(true);

            // Redirect to product list after 5 seconds
            setTimeout(() => {
                navigate('/seller/products/productlist'); // Adjust the path according to your routing setup
            }, 5000);
        } catch (error) {
            console.error('Error adding product:', error);
            setSnackbarMessage('Error adding product. Please try again.');
            setSnackbarSeverity('error');
            setSnackbarOpen(true);
        }
    };

    return (
        <form onSubmit={handleSubmit} className="m-5">
            <Typography variant="h5" style={{ marginBottom: '30px', color: 'blue', fontWeight: 'bold', marginLeft: '3px' }}>Add New Product</Typography>
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
                    <input
                        accept="image/*"
                        style={{ display: 'none' }} // Hides the default input
                        id="contained-button-file"
                        type="file"
                        onChange={handleImageChange}
                        required
                    />
                    <label htmlFor="contained-button-file">
                        <Button variant="contained" color="primary" component="span" startIcon={<CloudUpload />}>
                            Upload Image
                        </Button>
                    </label>
                    {image && (
                        <Typography variant="body2" style={{ marginTop: '10px' }}>
                            {image.name} selected
                        </Typography>
                    )}
                </Grid>

                <Grid item xs={12}>
                    <Button type="submit" variant="contained" color="primary" fullWidth>
                        Add Product
                    </Button>
                </Grid>
            </Grid>

            {/* Snackbar for notifications */}
            <Snackbar open={snackbarOpen} autoHideDuration={6000} onClose={handleSnackbarClose}>
                <Alert onClose={handleSnackbarClose} severity={snackbarSeverity} sx={{ width: '100%' }}>
                    {snackbarMessage}
                </Alert>
            </Snackbar>
        </form>
    );
};

export default ProductAddForm;
