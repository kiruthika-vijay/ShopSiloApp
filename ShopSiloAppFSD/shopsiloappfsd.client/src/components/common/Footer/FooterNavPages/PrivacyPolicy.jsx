import React from 'react';
import { Box, Typography, Divider } from '@mui/material';

const PrivacyPolicy = () => {
    return (
        <Box sx={{ p: 4, backgroundColor: '#f9f9f9', borderRadius: '8px' }}>
            <Typography variant="h4" gutterBottom align="center" sx={{ fontWeight: 'bold', color: '#333' }}>
                Privacy Policy
            </Typography>
            <Divider sx={{ my: 3 }} />

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                1. Data Collection
            </Typography>
            <Typography variant="body1" paragraph>
                We collect personal information such as your name, email address, and payment details when you make purchases or sign up for an account. Your data is securely stored and used solely to improve our services.
            </Typography>

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                2. How We Use Your Data
            </Typography>
            <Typography variant="body1" paragraph>
                We use your data to process transactions, communicate with you, and enhance your experience on our platform. We do not share your personal information with third parties unless required by law.
            </Typography>

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                3. Cookies
            </Typography>
            <Typography variant="body1" paragraph>
                Our website uses cookies to personalize your experience and provide relevant content. You can opt out of cookies by adjusting your browser settings.
            </Typography>

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                4. Data Security
            </Typography>
            <Typography variant="body1" paragraph>
                We take appropriate security measures to protect your personal data from unauthorized access. However, no system is 100% secure, and we cannot guarantee the security of your data.
            </Typography>

            <Typography variant="h6" sx={{ fontWeight: 'bold', mb: 2 }}>
                5. Your Rights
            </Typography>
            <Typography variant="body1" paragraph>
                You have the right to access, correct, or delete your personal data at any time. Please contact us if you would like to exercise these rights.
            </Typography>

            <Divider sx={{ my: 4 }} />

            <Typography variant="caption" display="block" align="center" sx={{ color: '#888' }}>
                Last updated: September 26, 2024
            </Typography>
        </Box>
    );
};

export default PrivacyPolicy;
