import React, { useState } from 'react';
import { Box, Typography, Divider, Accordion, AccordionSummary, AccordionDetails } from '@mui/material';
import ExpandMoreIcon from '@mui/icons-material/ExpandMore';

const FAQ = () => {
    const [expanded, setExpanded] = useState(false);

    const handleChange = (panel) => (event, isExpanded) => {
        setExpanded(isExpanded ? panel : false);
    };

    return (
        <Box sx={{ p: 4, backgroundColor: '#f9f9f9', borderRadius: '8px' }}>
            <Typography variant="h4" gutterBottom align="center" sx={{ fontWeight: 'bold', color: '#333' }}>
                Frequently Asked Questions (FAQ)
            </Typography>
            <Divider sx={{ my: 3 }} />

            <Accordion expanded={expanded === 'panel1'} onChange={handleChange('panel1')} sx={{ mb: 2 }}>
                <AccordionSummary
                    expandIcon={<ExpandMoreIcon />}
                    aria-controls="panel1a-content"
                    id="panel1a-header"
                >
                    <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                        How do I create an account?
                    </Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography variant="body1">
                        Creating an account is easy! Simply click on the 'Sign Up' button on the top-right corner of the website and fill in your details. Once you confirm your email, you’ll be able to enjoy all the features of our website.
                    </Typography>
                </AccordionDetails>
            </Accordion>

            <Accordion expanded={expanded === 'panel2'} onChange={handleChange('panel2')} sx={{ mb: 2 }}>
                <AccordionSummary
                    expandIcon={<ExpandMoreIcon />}
                    aria-controls="panel2a-content"
                    id="panel2a-header"
                >
                    <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                        What is your return policy?
                    </Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography variant="body1">
                        We offer a 30-day return policy for most products. If you are not satisfied with your purchase, you can return it within 30 days for a full refund, provided that the product is unused and in its original packaging.
                    </Typography>
                </AccordionDetails>
            </Accordion>

            <Accordion expanded={expanded === 'panel3'} onChange={handleChange('panel3')} sx={{ mb: 2 }}>
                <AccordionSummary
                    expandIcon={<ExpandMoreIcon />}
                    aria-controls="panel3a-content"
                    id="panel3a-header"
                >
                    <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                        How do I track my order?
                    </Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography variant="body1">
                        Once your order has been shipped, we will send you an email with the tracking details. You can also track your order by logging into your account and navigating to the 'Orders' section.
                    </Typography>
                </AccordionDetails>
            </Accordion>

            <Accordion expanded={expanded === 'panel4'} onChange={handleChange('panel4')} sx={{ mb: 2 }}>
                <AccordionSummary
                    expandIcon={<ExpandMoreIcon />}
                    aria-controls="panel4a-content"
                    id="panel4a-header"
                >
                    <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                        Can I change or cancel my order after it’s been placed?
                    </Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography variant="body1">
                        Yes, you can change or cancel your order before it is shipped. Please contact our customer service as soon as possible, and we will assist you in modifying or canceling your order.
                    </Typography>
                </AccordionDetails>
            </Accordion>

            <Accordion expanded={expanded === 'panel5'} onChange={handleChange('panel5')} sx={{ mb: 2 }}>
                <AccordionSummary
                    expandIcon={<ExpandMoreIcon />}
                    aria-controls="panel5a-content"
                    id="panel5a-header"
                >
                    <Typography variant="h6" sx={{ fontWeight: 'bold' }}>
                        How do I contact customer support?
                    </Typography>
                </AccordionSummary>
                <AccordionDetails>
                    <Typography variant="body1">
                        You can reach our customer support team by emailing support@ourwebsite.com or calling our toll-free number 1-800-123-4567. We’re here to help 24/7!
                    </Typography>
                </AccordionDetails>
            </Accordion>

            <Divider sx={{ my: 4 }} />

            <Typography variant="caption" display="block" align="center" sx={{ color: '#888' }}>
                Last updated: September 26, 2024
            </Typography>
        </Box>
    );
};

export default FAQ;
