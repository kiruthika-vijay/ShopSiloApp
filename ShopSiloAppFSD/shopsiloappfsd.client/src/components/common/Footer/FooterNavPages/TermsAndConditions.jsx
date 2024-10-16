import React from 'react';
import { Box, Typography, Divider } from '@mui/material';

const TermsAndConditions = () => {
    return (
        <Box sx={{ p: 4, backgroundColor: '#f9f9f9', borderRadius: '8px' }}>
            <Typography variant="h4" gutterBottom align="center" sx={{ fontWeight: 'bold', color: '#333' }}>
                Terms and Conditions
            </Typography>
            <Divider sx={{ my: 3 }} />

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                1. Introduction
            </Typography>
            <Typography variant="body1" paragraph>
                Welcome to our e-commerce website. By using our services, you agree to comply with and be bound by the following terms and conditions.
            </Typography>

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                2. Intellectual Property
            </Typography>
            <Typography variant="body1" paragraph>
                All content, logos, trademarks, and data displayed on this website are the property of our company or its content suppliers. Unauthorized use of this content is strictly prohibited.
            </Typography>

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                3. User Responsibilities
            </Typography>
            <Typography variant="body1" paragraph>
                Users are responsible for ensuring that their use of the website is lawful. Any misconduct, including the use of fake accounts or false information, will lead to immediate termination of service.
            </Typography>

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                4. Limitation of Liability
            </Typography>
            <Typography variant="body1" paragraph>
                Our company is not liable for any damages resulting from the use of our website, including any direct or indirect losses or damages.
            </Typography>

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                5. Changes to Terms
            </Typography>
            <Typography variant="body1" paragraph>
                We reserve the right to amend these terms at any time. Please review this page periodically to stay updated on any changes.
            </Typography>

            <Divider sx={{ my: 4 }} />

            <Typography variant="caption" display="block" align="center" sx={{ color: '#888' }}>
                Last updated: September 26, 2024
            </Typography>
        </Box>
    );
};

export default TermsAndConditions;
