import React from 'react';
import { FaStar, FaStarHalfAlt, FaRegStar } from 'react-icons/fa';
import PropTypes from 'prop-types';

const StarRating = ({ rating, reviewCount }) => {
    const stars = [];
    const fullStars = Math.floor(rating);
    const halfStar = rating % 1 >= 0.5;
    const emptyStars = 5 - fullStars - (halfStar ? 1 : 0);

    for (let i = 0; i < fullStars; i++) {
        stars.push(<FaStar key={`full-${i}`} className="text-yellow-400" />);
    }

    if (halfStar) {
        stars.push(<FaStarHalfAlt key="half" className="text-yellow-400" />);
    }

    for (let i = 0; i < emptyStars; i++) {
        stars.push(<FaRegStar key={`empty-${i}`} className="text-gray-300" />);
    }

    return (
        <div className="flex">
            {stars}
            <span className="text-sm text-gray-600 ml-2">({reviewCount})</span>
        </div>
    );
};

StarRating.propTypes = {
    rating: PropTypes.number.isRequired,
    reviewCount: PropTypes.number.isRequired,
};

export default StarRating;
