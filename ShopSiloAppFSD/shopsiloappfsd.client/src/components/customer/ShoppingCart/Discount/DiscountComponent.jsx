import React, { useState } from 'react';
import axios from 'axios';
import { apiClient } from '../../../common/Axios/auth';

const ApplyDiscount = ({ onDiscountApplied }) => {
    const [discountCode, setDiscountCode] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');

    const handleApplyDiscount = async (e) => {
        e.preventDefault();
        setErrorMessage('');
        setSuccessMessage('');

        try {
            const response = await apiClient.post('/Discount/apply', { discountCode });
            const discount = response.data.$values; // Get the entire discount DTO
            onDiscountApplied(discount.discountPercentage); // Pass the discount percentage to the parent component
            setSuccessMessage(`Discount applied: ${discount.discountPercentage}% - ${discount.description}`);
        } catch (error) {
            if (error.response) {
                setErrorMessage(error.response.data);
            } else {
                setErrorMessage('Error applying discount. Please try again.');
            }
        }
    };

    return (
        <div>
            <form onSubmit={handleApplyDiscount}>
                <input
                    type="text"
                    value={discountCode}
                    onChange={(e) => setDiscountCode(e.target.value)}
                    placeholder="Enter discount code"
                    required
                />
                <button type="submit">Apply Discount</button>
            </form>
            {errorMessage && <p style={{ color: 'red' }}>{errorMessage}</p>}
            {successMessage && <p style={{ color: 'green' }}>{successMessage}</p>}
        </div>
    );
};

export default ApplyDiscount;
