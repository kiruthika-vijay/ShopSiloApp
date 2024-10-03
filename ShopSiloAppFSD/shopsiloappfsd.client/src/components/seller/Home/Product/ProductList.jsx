import React, { useEffect, useState } from 'react';
import { Grid, Card, CardMedia, CardContent, Typography, Button, Switch, IconButton, Collapse } from '@mui/material';
import { Star, StarBorder, ExpandMore } from '@mui/icons-material';
import { styled } from '@mui/material/styles';
import { apiClient, getToken } from '../../../common/Axios/auth';

const ExpandMoreIcon = styled(IconButton)(({ theme, expand }) => ({
    transform: !expand ? 'rotate(0deg)' : 'rotate(180deg)',
    transition: theme.transitions.create('transform', {
        duration: theme.transitions.duration.shortest,
    }),
}));

const ProductList = () => {
    const [products, setProducts] = useState([]);
    const [expanded, setExpanded] = useState({}); // Track expanded state of each product

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const response = await apiClient.get('/Seller/products');
                setProducts(response.data.$values); // Assuming your data format
            } catch (error) {
                console.error('Error fetching products:', error);
            }
        };

        fetchProducts();
    }, []);

    const handleToggleActive = async (productID, isActive) => {
        // Optimistically update the product's active status in the UI
        setProducts((prevProducts) =>
            prevProducts.map(product =>
                product.productID === productID ? { ...product, isActive: !isActive } : product
            )
        );

        try {
            const token = getToken();
            await apiClient.put(`/Product/toggle/${productID}`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });
        } catch (error) {
            console.error('Error toggling active status:', error);
        }
    };

    const handleExpandClick = (productID) => {
        setExpanded((prev) => ({ ...prev, [productID]: !prev[productID] }));
    };

    const renderStars = (rating) => {
        return Array.from({ length: 5 }, (_, index) =>
            index < rating
                ? <Star key={index} style={{ color: '#FFD700' }} /> // Dark yellow for filled stars
                : <StarBorder key={index} style={{ color: '#FFD700' }} /> // Dark yellow for border stars
        );
    };

    return (
        <Grid container spacing={3} padding={3} className="bg-gray-100">
            {products.map((product) => (
                <Grid item xs={12} key={product.productID}>
                    <Card className="w-full shadow-lg transition-transform transform hover:scale-105">
                        <div className="flex">
                            {/* Product Image */}
                            <div style={{ flex: '0 0 auto', width: '33.33%', overflow: 'hidden' }}> {/* Set width and hide overflow */}
                                <CardMedia
                                    component="img"
                                    height="100%" // Allow height to adjust within the parent
                                    image={product.imageURL}
                                    alt={product.productName}
                                    style={{
                                        width: '90%', // Ensure the image takes the full width of the container
                                        height: 'auto', // Let height adjust based on the width
                                        objectFit: 'contain', // Ensure the entire image is visible
                                    }}
                                />
                            </div>

                            {/* Product Info */}
                            <CardContent className="w-2/3 flex flex-col justify-between">
                                <div>
                                    <Typography variant="h6" className="font-bold text-gray-800">{product.productName}</Typography>
                                    <div className="flex items-center mt-1">
                                        <Typography variant="body2" className="mr-2 font-medium text-indigo-600">
                                            {product.averageRating.toFixed(1)}
                                        </Typography>
                                        {renderStars(product.averageRating)}
                                        <Typography variant="body2" className="ml-2 text-gray-500">
                                            ({product.reviewCount} reviews)
                                        </Typography>
                                    </div>
                                    <Typography variant="body2" className="text-green-600 mt-2 font-semibold">
                                        {product.price.toLocaleString('en-IN', { style: 'currency', currency: 'INR' })}
                                    </Typography>
                                </div>
                                <div className="flex justify-between items-center mt-4">
                                    {/* Active/Inactive Toggle */}
                                    <div className="flex items-center">
                                        <Typography variant="body2" className={product.isActive ? "text-green-600" : "text-red-600"}>
                                            {product.isActive ? 'Active' : 'Inactive'}
                                        </Typography>
                                        <Switch
                                            checked={product.isActive}
                                            onChange={() => handleToggleActive(product.productID, product.isActive)}
                                            color="primary"
                                            className="ml-2"
                                        />
                                    </div>
                                    {/* Expand More Button */}
                                    <ExpandMoreIcon
                                        expand={expanded[product.productID]}
                                        onClick={() => handleExpandClick(product.productID)}
                                    >
                                        <ExpandMore />
                                    </ExpandMoreIcon>
                                </div>
                            </CardContent>
                        </div>
                        {/* Expandable Section */}
                        <Collapse in={expanded[product.productID]} timeout="auto" unmountOnExit>
                            <CardContent className="bg-gray-50 p-4 border-t border-gray-300">
                                <Typography variant="h6" className="font-bold text-gray-800 mb-2">Product Details</Typography>
                                <div className="space-y-2">
                                    <Typography paragraph className="text-gray-600">
                                        <strong>Description:</strong> {product.description}
                                    </Typography>
                                    <Typography paragraph className="text-gray-600">
                                        <strong>Product ID:</strong> {product.productID}
                                    </Typography>
                                    <Typography paragraph className="text-gray-600">
                                        <strong>Category:</strong> {product.categoryName}
                                    </Typography>
                                    <Typography paragraph className="text-gray-600">
                                        <strong>Stock Quantity:</strong> {product.stockQuantity}
                                    </Typography>
                                    <Typography paragraph className="text-gray-600">
                                        <strong>Total Orders:</strong> {product.totalOrders}
                                    </Typography>
                                    <Typography paragraph className="text-gray-600">
                                        <strong>Created Date:</strong> {new Date(product.createdDate).toLocaleDateString()}
                                    </Typography>
                                </div>
                            </CardContent>
                        </Collapse>

                    </Card>
                </Grid>
            ))}
        </Grid>
    );
};

export default ProductList;
