import React from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { jsPDF } from 'jspdf';
import 'jspdf-autotable';
import {
    Button,
    Card,
    CardContent,
    CardMedia,
    Grid,
    Typography,
    IconButton,
    Box,
    Divider,
} from '@mui/material';
import ContentCopyIcon from '@mui/icons-material/ContentCopy';

const OrderSummary = () => {
    const location = useLocation();
    const navigate = useNavigate(); // Initialize navigate function
    const orderDetails = location.state?.orderDetails.orderDetails;
    const shippingAddress = location.state?.orderDetails.shippingAddress;
    const billingAddress = location.state?.orderDetails.billingAddress;

    // Debugging output
    console.log('Order Details:', orderDetails);

    if (!orderDetails) {
        return (
            <Box
                sx={{
                    display: 'flex',
                    justifyContent: 'center',
                    alignItems: 'center',
                    height: '100vh',
                    backgroundColor: '#f0f0f0',
                    padding: 2,
                }}
            >
                <Typography variant="h6" color="error">
                    Order details are not available!
                </Typography>
            </Box>
        );
    }

    const downloadPDF = () => {
        console.log("Download PDF button clicked"); // Debugging log
        const doc = new jsPDF();
        doc.setFontSize(16);
        doc.text('Order Summary', 105, 20, null, null, 'center');

        // Order Details
        doc.setFontSize(12);
        doc.text(`Order ID: ${orderDetails.orderId || 'N/A'}`, 20, 30);
        doc.text(`Payment ID: ${orderDetails.razorpayPaymentId || 'N/A'}`, 20, 40);
        doc.text(`Total Amount: ${orderDetails.amount || '0'}`, 20, 50);
        doc.text(`Discount Applied: ${orderDetails.discount || 'None'}`, 20, 60);

        // Order Items Table
        const items = orderDetails.orderItems?.map((item) => [
            item.productName,
            item.price,
            item.quantity,
            `${(item.discountedPrice || item.price) * item.quantity}`,
        ]) || [];

        doc.autoTable({
            startY: 70,
            head: [['Item', 'Price', 'Quantity', 'Total']],
            body: items,
        });

        // Grand Total
        doc.text(`Grand Total: ${orderDetails.amount || '0'}`, 20, doc.lastAutoTable.finalY + 10);

        // Shipping & Billing Address
        doc.text(`Shipping Address: ${shippingAddress.addressLine1}, ${shippingAddress.addressLine2}, ${shippingAddress.city}, ${shippingAddress.state}, ${shippingAddress.postalCode}, ${shippingAddress.country}`, 20, doc.lastAutoTable.finalY + 20);
        doc.text(`Billing Address: ${billingAddress.addressLine1}, ${billingAddress.addressLine2}, ${billingAddress.city}, ${billingAddress.state}, ${billingAddress.postalCode}, ${billingAddress.country}`, 20, doc.lastAutoTable.finalY + 30);

        // Seller and Tracking Info
        doc.text(`Seller Name: ${orderDetails.sellerName || 'N/A'}`, 20, doc.lastAutoTable.finalY + 40);
        doc.text(`Order Tracking Number: ${orderDetails.trackingNumber || 'N/A'}`, 20, doc.lastAutoTable.finalY + 50);

        // Save the PDF
        doc.save('order-summary.pdf');
    };

    const copyToClipboard = (text) => {
        if (!text) {
            alert('No payment ID to copy!');
            return;
        }
        navigator.clipboard.writeText(text)
            .then(() => alert('Copied to clipboard!'))
            .catch((err) => alert('Failed to copy!'));
    };

    return (
        <Box
            sx={{
                display: 'flex',
                justifyContent: 'center',
                alignItems: 'flex-start', // Align items to the start
                backgroundColor: '#f0f0f0',
                padding: 2,
                minHeight: '100vh', // Ensure it takes full height
                paddingTop: '70px', // Space for the navbar, adjust as needed
                paddingBottom: '70px', // Space for the footer, adjust as needed
            }}
        >
            <Card
                sx={{
                    width: '100%',
                    maxWidth: '800px',
                    boxShadow: 3,
                    borderRadius: 2,
                    padding: 3,
                    transition: 'transform 0.3s',
                    '&:hover': {
                        transform: 'scale(1.02)',
                        boxShadow: 6,
                    },
                }}
            >
                <Typography variant="h4" gutterBottom align="center" sx={{ fontWeight: 'bold', color: '#333' }}>
                    Order Summary
                </Typography>
                <Divider sx={{ marginY: 2 }} />

                <Typography variant="h6" gutterBottom>
                    Order ID: {orderDetails.orderId || 'N/A'}
                </Typography>
                <Typography variant="h6" gutterBottom>
                    Payment ID: {orderDetails.razorpayPaymentId || 'N/A'}
                    <IconButton
                        onClick={() => copyToClipboard(orderDetails.razorpayPaymentId)}
                        size="small"
                        sx={{ marginLeft: 1, '&:hover': { color: 'blue' } }}
                    >
                        <ContentCopyIcon fontSize="small" />
                    </IconButton>
                </Typography>

                <Divider sx={{ marginY: 2 }} />

                <Grid container spacing={2}>
                    {orderDetails.orderItems?.length > 0 ? (
                        orderDetails.orderItems.map((item, index) => (
                            <Grid item xs={12} sm={6} key={index}>
                                <Card
                                    sx={{
                                        display: 'flex',
                                        alignItems: 'center',
                                        marginBottom: 2,
                                        padding: 2,
                                        borderRadius: 1,
                                        transition: 'transform 0.3s',
                                        '&:hover': {
                                            transform: 'scale(1.03)',
                                            boxShadow: 4,
                                        },
                                    }}
                                >
                                    <CardMedia
                                        component="img"
                                        sx={{ width: 100, borderRadius: 1 }}
                                        image={item.imageUrl || ''}
                                        alt={item.productName}
                                    />
                                    <CardContent>
                                        <Typography variant="h6" sx={{ fontWeight: 'bold' }}>{item.productName}</Typography>
                                        <Typography variant="body1">Price: ₹{(item.price || 0)}</Typography>
                                        <Typography variant="body1">Quantity: {item.quantity || 1}</Typography>
                                    </CardContent>
                                </Card>
                            </Grid>
                        ))
                    ) : (
                        <Typography variant="body1" gutterBottom>No items found in this order.</Typography>
                    )}
                </Grid>

                <Divider sx={{ marginY: 2 }} />

                <Typography variant="h5" gutterBottom sx={{ fontWeight: 'bold' }}>
                    Total Amount: ₹{(orderDetails.amount || '0')}
                </Typography>
                <Typography variant="body1" gutterBottom>
                    Discount Applied: {orderDetails.discount || 'None'}
                </Typography>

                <Divider sx={{ marginY: 2 }} />

                <Typography variant="h6" gutterBottom>
                    Shipping Address:
                </Typography>
                <Typography variant="body1" gutterBottom>
                    {shippingAddress.addressLine1}, {shippingAddress.addressLine2}, {shippingAddress.city}, {shippingAddress.state}, {shippingAddress.postalCode}, {shippingAddress.country}
                </Typography>

                <Typography variant="h6" gutterBottom>
                    Billing Address:
                </Typography>
                <Typography variant="body1" gutterBottom>
                    {billingAddress.addressLine1}, {billingAddress.addressLine2}, {billingAddress.city}, {billingAddress.state}, {billingAddress.postalCode}, {billingAddress.country}
                </Typography>

                <Button
                    variant="contained"
                    color="primary"
                    onClick={() => navigate('/customer/home')} // Adjust the route as necessary
                    sx={{ marginTop: 2 }}
                >
                    Return to Home
                </Button>
                
                <Button
                    variant="contained"
                    color="primary"
                    fullWidth
                    sx={{ marginTop: 2 }}
                    onClick={downloadPDF}
                >
                    Download PDF
                </Button>
            </Card>
        </Box>
    );
};

export default OrderSummary;
