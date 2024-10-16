import React from 'react';
import { FaEye } from 'react-icons/fa';
import PropTypes from 'prop-types';

const ViewDetailsButton = ({ onViewDetails, productId }) => (
    <button
        onClick={() => onViewDetails(productId)}
        className="text-white bg-blue-500 rounded-full p-2 hover:bg-blue-600 transition-colors"
        aria-label="View Details"
    >
        <FaEye />
    </button>
);

ViewDetailsButton.propTypes = {
    productId: PropTypes.number.isRequired,
    onViewDetails: PropTypes.func.isRequired,
};

export default ViewDetailsButton;
