import React, { useState, useEffect } from 'react';
import useProduct from '../Home/HomeUtils/useProductDetails';
import { Grid, Typography, Button, Card, CardMedia, CardContent, Rating, CircularProgress, Alert, Divider, TextField, Box } from '@mui/material';
import { ShoppingCart, FavoriteBorder } from '@mui/icons-material';
import axios from 'axios';
import { apiClient, getToken } from '../../common/Axios/auth';
import { useNavigate } from 'react-router-dom';
import useCart from '../Home/HomeUtils/useCart';
import useWishlist from '../Home/HomeUtils/useWishlist';

const ProductDescriptionPage = () => {
    const { product, loading, error } = useProduct();
    const [reviews, setReviews] = useState([]); // State to store reviews
    const [review, setReview] = useState({ rating: 0, reviewText: ''});
    const [reviewError, setReviewError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const navigate = useNavigate();
    const { addToCart } = useCart();
    const { wishlistId, addToWishlist, removeFromWishlist, wishlistLoading, wishlistError } = useWishlist();

    // Retrieve userID from JWT token in local storage
    useEffect(() => {
        const token = getToken();
        if (token) {
            const userData = JSON.parse(atob(token.split('.')[1])); // Decode JWT payload
            setReview({ ...review, userID: userData.userID }); // Set userID from token
        }
    }, []);

    useEffect(() => {
        const fetchReviews = async () => {
            if (product && product.productID) {
                try {
                    const response = await apiClient.get(`/ProductReview/Product/${product.productID}`, {
                        headers: {
                            Authorization: `Bearer ${getToken()}` // Adjust based on your authentication mechanism
                        }
                    });
                    console.log(response.data.reviews.$values);
                    setReviews(response.data.reviews.$values); // Assuming the API returns an array of reviews
                } catch (error) {
                    console.error('Error fetching reviews:', error);
                }
            }
        };

        fetchReviews();
    }, [product]); // Fetch reviews whenever the product changes

    // Handle input change
    const handleInputChange = (e) => {
        setReview({ ...review, [e.target.name]: e.target.value });
    };

    // Handle Rating Change
    const handleRatingChange = (e, newValue) => {
        setReview({ ...review, rating: newValue });
    };

    // Submit review to backend
    const submitReview = async () => {
        if (!review.rating || !review.reviewText) {
            setReviewError('Rating and review text are required.');
            return;
        }

        try {
            setIsSubmitting(true);
            const response = await apiClient.post('/ProductReview', {
                productID: product.productID,
                rating: review.rating,
                reviewText: review.reviewText,
                reviewDate: new Date().toISOString(),
            });
            console.log(response.data);
            // Fetch reviews again after submitting
            const fetchReviews = async () => {
                const response = await apiClient.get(`/ProductReview/Product/${product.productID}`, {
                    headers: {
                        Authorization: `Bearer ${getToken()}` // Adjust based on your authentication mechanism
                    }
                });
                setReviews(response.data.reviews.$values);
            };
            await fetchReviews();
            setReview({ rating: 0, reviewText: ''});
            setReviewError('');
            alert('Review submitted successfully!');
        } catch (error) {
            console.error('Error submitting review:', error);
            setReviewError('Failed to submit the review. Please try again later.');
        } finally {
            setIsSubmitting(false);
        }
    };

    // Handle Add to Cart
    const handleAddToCart = async () => {
        await addToCart(product.productID, 1); // Pass the price to the addToCart function
    };

    // Handle Buy Now
    const handleBuyNow = async () => {
        await handleAddToCart(); // Add to cart first
        navigate('/customer/cart'); // Redirect to shopping cart
    };

    // Handle Add to Wishlist
    const handleAddToWishlist = async (shouldAdd) => {
        try {
            if (shouldAdd) {
                await addToWishlist(product.productID);
                alert('Product added to wishlist!');
            }
            else {
                await removeFromWishlist(product.productID);
                alert('Product removed to wishlist!');
            }
        } catch (error) {
            console.error(`Error adding to Wishlist: ${error.message}`);
        }
    };
  
    if (loading) {
        return <div style={{ textAlign: 'center', marginTop: '20px' }}><CircularProgress /></div>;
    }

    if (error) {
        return <Alert severity="error">Error fetching product details.</Alert>;
    }

    return (
        <div style={{ padding: '20px', backgroundColor: '#fff', borderRadius: '8px', boxShadow: '0 4px 8px rgba(0, 0, 0, 0.1)' }}>
            <Grid container spacing={4}>
                {/* Left Column - Product Image */}
                <Grid item xs={12} md={6}>
                    <Card style={{ position: 'relative', overflow: 'hidden' }}>
                        <CardMedia
                            component="img"
                            image={product.imageURL || 'https://via.placeholder.com/500'}
                            alt={product.productName}
                            className="zoom-image" // Class for zoom effect
                            style={{ objectFit: 'cover', transition: 'transform 0.3s ease' }}
                        />
                    </Card>
                </Grid>

                {/* Right Column - Product Details */}
                <Grid item xs={12} md={6}>
                    <Typography variant="h4" component="h1" gutterBottom>
                        {product.productName}
                    </Typography>
                    <Typography variant="h6" color="textSecondary" gutterBottom>
                        {product.description}
                    </Typography>
                    <Typography variant="h5" color="primary" gutterBottom>
                        Price: &#8377;{product.price}
                    </Typography>
                    <Typography variant="body2" color="textSecondary" paragraph>
                        Available Stock: {product.stockQuantity}
                    </Typography>

                    {/* Action Buttons */}
                    <Grid container spacing={2}>
                        <Grid item>
                            <Button variant="contained" color="primary" startIcon={<ShoppingCart />} onClick={handleAddToCart} style={{ backgroundColor: 'orange' }}>
                                Add to Cart
                            </Button>
                        </Grid>
                        <Grid item>
                            <Button variant="outlined" color="secondary" startIcon={<FavoriteBorder />} onClick={handleAddToWishlist} style={{ borderColor: 'orange' }}>
                                Add to Wishlist
                            </Button>
                        </Grid>
                    </Grid>

                    {/* Buy Now Button */}
                    <Grid item style={{ marginTop: '20px' }}>
                        <Button variant="contained" color="success" fullWidth onClick={handleBuyNow} style={{ backgroundColor: 'orange' }}>
                            Buy Now
                        </Button>
                    </Grid>

                    <Divider style={{ margin: '20px 0' }} />

                    {/* Seller Information */}
                    <Typography variant="h6" gutterBottom>
                        Seller: {product.seller.companyName}
                    </Typography>
                    <Typography variant="body2" color="textSecondary">
                        {product.seller.storeDescription}
                    </Typography>
                </Grid>
            </Grid>

            {/* Product Reviews Section */}
            <Grid container spacing={4} style={{ marginTop: '40px' }}>
                <Grid item xs={12}>
                    <Typography variant="h5" gutterBottom>
                        Customer Reviews
                    </Typography>
                    {reviews.length > 0 ? (
                        reviews.map((review, index) => (
                            <Card key={index} style={{ marginBottom: '20px' }}>
                                <CardContent>
                                    <Typography variant="h6">{review.customerName}</Typography>
                                    <Rating value={review.rating} readOnly />
                                    <Typography variant="body2" color="textSecondary">
                                        {review.reviewText}
                                    </Typography>
                                </CardContent>
                            </Card>
                        ))
                    ) : (
                        <Typography variant="body1" color="textSecondary">
                            No reviews yet. Be the first to review this product!
                        </Typography>
                    )}
                </Grid>
            </Grid>

            {/* Add a Review Section */}
            <Grid container spacing={4} style={{ marginTop: '40px' }}>
                <Grid item xs={12}>
                    <Typography variant="h5" gutterBottom>
                        Add a Review
                    </Typography>
                    <Box component="form" noValidate autoComplete="off" style={{ maxWidth: '600px' }}>
                        <Rating
                            name="rating"
                            value={review.rating}
                            onChange={handleRatingChange}
                            style={{ marginBottom: '20px' }}
                        />
                        <TextField
                            fullWidth
                            name="reviewText"
                            label="Your Review"
                            variant="outlined"
                            multiline
                            rows={4}
                            value={review.reviewText}
                            onChange={handleInputChange}
                            style={{ marginBottom: '20px' }}
                        />
                        {reviewError && <Alert severity="error">{reviewError}</Alert>}

                        <Button
                            variant="contained"
                            color="primary"
                            onClick={submitReview}
                            disabled={isSubmitting}
                            style={{ backgroundColor: 'orange' }}
                        >
                            {isSubmitting ? 'Submitting...' : 'Submit Review'}
                        </Button>
                    </Box>
                </Grid>
            </Grid>
        </div>
    );
};

export default ProductDescriptionPage;

// CSS for image zoom effect
const styles = `
.zoom-image:hover {
    transform: scale(1.1);
}
`;
