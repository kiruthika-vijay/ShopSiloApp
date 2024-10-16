import React, { useState } from 'react';
import { Box, Button, Modal, TextField, Typography } from '@mui/material';
import { apiClient, fetchUserId } from '../../../common/Axios/auth';

// Modal styling
const modalStyle = {
    position: 'absolute',
    top: '50%',
    left: '50%',
    transform: 'translate(-50%, -50%)',
    width: 400,
    bgcolor: 'background.paper',
    border: '2px solid #000',
    boxShadow: 24,
    p: 4,
    borderRadius: '10px',
};

// Button styling
const buttonStyle = {
    marginTop: '20px',
    backgroundColor: '#FF5722', // Orange theme
    color: 'white',
    '&:hover': {
        backgroundColor: '#E64A19',
    },
};

const CustomerDetailsForm = ({ token, userId, onSubmit, open, handleClose }) => {
    const [customerDetails, setCustomerDetails] = useState({
        customerID: userId,  // Set customer ID to the logged-in user's ID
        firstName: '',
        lastName: '',
        phoneNumber: '',
    });
    console.log(fetchUserId());
    const handleChange = (e) => {
        setCustomerDetails({
            ...customerDetails,
            [e.target.name]: e.target.value,
        });
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            // API call to create customer details
            await apiClient.post('/CustomerDetail', {
                customerID: customerDetails.customerID,
                firstName: customerDetails.firstName,
                lastName: customerDetails.lastName,
                phoneNumber: customerDetails.phoneNumber,
            }, {
                headers: { Authorization: `Bearer ${token}` }
            });
            onSubmit(); // Callback to refetch data and show profile page
            handleClose(); // Close the modal after submission
        } catch (err) {
            console.error('Error adding customer details:', err);
        }
    };

    return (
        <Modal
            open={open}
            onClose={handleClose}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"
        >
            <Box sx={modalStyle}>
                <Typography variant="h6" component="h2" gutterBottom>
                    Customer Details
                </Typography>
                <form onSubmit={handleSubmit}>
                    <TextField
                        label="Customer ID"
                        name="customerID"
                        value={customerDetails.customerID}
                        fullWidth
                        margin="normal"
                        disabled // Non-editable field
                    />
                    <TextField
                        label="First Name"
                        name="firstName"
                        value={customerDetails.firstName}
                        onChange={handleChange}
                        fullWidth
                        margin="normal"
                        required
                    />
                    <TextField
                        label="Last Name"
                        name="lastName"
                        value={customerDetails.lastName}
                        onChange={handleChange}
                        fullWidth
                        margin="normal"
                        required
                    />
                    <TextField
                        label="Phone Number"
                        name="phoneNumber"
                        value={customerDetails.phoneNumber}
                        onChange={handleChange}
                        fullWidth
                        margin="normal"
                        required
                    />
                    <Button
                        type="submit"
                        variant="contained"
                        sx={buttonStyle}
                        fullWidth
                    >
                        Submit
                    </Button>
                </form>
            </Box>
        </Modal>
    );
};

export default CustomerDetailsForm;
