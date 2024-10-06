import React, { useState, useEffect } from 'react';
import useProduct from '../Home/HomeUtils/useProductDetails';
import {
    Grid, Typography, Button, Card, CardMedia, CardContent, Rating, CircularProgress, Alert, Divider, TextField, Box
} from '@mui/material';
import { ShoppingCart, Favorite, FavoriteBorder } from '@mui/icons-material';
import axios from 'axios';
import { apiClient, getToken } from '../../common/Axios/auth';
import { useNavigate } from 'react-router-dom';
import useCart from '../Home/HomeUtils/useCart';
import useWishlist from '../Home/HomeUtils/useWishlist';

const ProductDescriptionPage = () => {
    const { product, loading, error } = useProduct();
    const [reviews, setReviews] = useState([]);
    const [review, setReview] = useState({ rating: 0, reviewText: '' });
    const [reviewError, setReviewError] = useState('');
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [isWishlist, setIsWishlist] = useState(false);
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

    // Fetch product reviews
    useEffect(() => {
        const fetchReviews = async () => {
            if (product && product.productID) {
                try {
                    const response = await apiClient.get(`/ProductReview/Product/${product.productID}`, {
                        headers: {
                            Authorization: `Bearer ${getToken()}`
                        }
                    });
                    setReviews(response.data.reviews.$values);
                } catch (error) {
                    console.error('Error fetching reviews:', error);
                }
            }
        };
        fetchReviews();
    }, [product]);

    const handleInputChange = (e) => {
        setReview({ ...review, [e.target.name]: e.target.value });
    };

    const handleRatingChange = (e, newValue) => {
        setReview({ ...review, rating: newValue });
    };

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
            const fetchReviews = async () => {
                const response = await apiClient.get(`/ProductReview/Product/${product.productID}`, {
                    headers: {
                        Authorization: `Bearer ${getToken()}`
                    }
                });
                setReviews(response.data.reviews.$values);
            };
            await fetchReviews();
            setReview({ rating: 0, reviewText: '' });
            setReviewError('');
            alert('Review submitted successfully!');
        } catch (error) {
            console.error('Error submitting review:', error);
            setReviewError('Failed to submit the review. Please try again later.');
        } finally {
            setIsSubmitting(false);
        }
    };

    const handleAddToCart = async () => {
        const priceToUse = product.discountPrice || product.flashSalePrice || product.price; // Use discount price or flash sale price if available
        await addToCart(product.productID, 1, priceToUse); // Pass the price to the addToCart function
    };

    const handleBuyNow = async () => {
        await handleAddToCart(); // Add to cart first
        navigate('/customer/cart'); // Redirect to shopping cart
    };

    const handleAddToWishlist = async () => {
        try {
            if (isWishlist) {
                await removeFromWishlist(product.productID);
                setIsWishlist(false);
            } else {
                await addToWishlist(product.productID);
                setIsWishlist(true);
            }
        } catch (error) {
            console.error(`Error managing Wishlist: ${error.message}`);
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
                <Grid item xs={12} md={6}>
                    <Card style={{ position: 'relative', overflow: 'hidden' }}>
                        <CardMedia
                            component="img"
                            image={product.imageURL || 'https://via.placeholder.com/500'}
                            alt={product.productName}
                            className="zoom-image"
                            style={{ objectFit: 'cover', transition: 'transform 0.3s ease' }}
                        />
                    </Card>
                </Grid>

                <Grid item xs={12} md={6}>
                    <Typography variant="h4" component="h1" gutterBottom>
                        {product.productName}
                    </Typography>
                    <Typography variant="h6" color="textSecondary" gutterBottom>
                        {product.description}
                    </Typography>

                    {/* Price Display */}
                    
                    {product.discountedPrice ? (
                        <>
                            <p className="text-2xl font-bold text-green-600">&#8377;{product.discountedPrice}</p>
                            <p className="text-l text-gray-500 line-through">&#8377;{product.price}</p>
                        </>
                    ) : (
                        <p className="text-xl font-bold text-green-600">&#8377;{product.price}</p>
                    )}

                    <Grid container spacing={2}>
                        <Grid item>
                            <Button variant="contained" color="primary" startIcon={<ShoppingCart />} onClick={handleAddToCart} style={{ backgroundColor: 'orange' }}>
                                Add to Cart
                            </Button>
                        </Grid>
                        <Grid item>
                            <Button
                                variant="outlined"
                                color={isWishlist ? "error" : "secondary"}
                                startIcon={isWishlist ? <Favorite /> : <FavoriteBorder />}
                                onClick={handleAddToWishlist}
                                style={{ borderColor: 'orange' }}
                            >
                                {isWishlist ? 'Remove from Wishlist' : 'Add to Wishlist'}
                            </Button>
                        </Grid>
                    </Grid>

                    <Grid item style={{ marginTop: '20px' }}>
                        <Button variant="contained" color="success" fullWidth onClick={handleBuyNow} style={{ backgroundColor: 'orange' }}>
                            Buy Now
                        </Button>
                    </Grid>

                    <Divider style={{ margin: '20px 0' }} />

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
                        <Typography variant="body2" color="textSecondary">
                            No reviews yet.
                        </Typography>
                    )}
                </Grid>
            </Grid>

            {/* Add Review Section */}
            <Grid container spacing={2} style={{ marginTop: '20px' }}>
                <Grid item xs={12}>
                    <Typography variant="h6" gutterBottom>
                        Add Your Review
                    </Typography>
                    <Rating
                        name="rating"
                        value={review.rating}
                        onChange={handleRatingChange}
                    />
                    <TextField
                        name="reviewText"
                        label="Write your review here"
                        value={review.reviewText}
                        onChange={handleInputChange}
                        multiline
                        rows={4}
                        variant="outlined"
                        fullWidth
                        margin="normal"
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
                </Grid>
            </Grid>
        </div>
    );
};

export default ProductDescriptionPage;
