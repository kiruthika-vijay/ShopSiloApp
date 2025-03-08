import React, { useEffect, useState } from 'react';
import { apiClient } from '../../../common/Axios/auth';
import './Leaderboard.css';

const Leaderboard = () => {
    const [topSellers, setTopSellers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchTopSellers = async () => {
            try {
                const response = await apiClient.get('/Seller/top/5');
                setTopSellers(response.data.$values);
            } catch (err) {
                setError(err);
            } finally {
                setLoading(false);
            }
        };

        fetchTopSellers();
    }, []);

    if (loading) return <div>Loading...</div>;
    if (error) return <div>Error: {error.message}</div>;

    return (
        <div className="leaderboard-container">
            {topSellers.map((seller, index) => (
                <div key={seller.sellerId} className={`seller-card rank-${index + 1}`}>
                    <div className={`seller-circle medal-${index + 1}`}>
                        {/* Only show rank number for ranks 4 and 5 */}
                        {index >= 3 && <span className="rank-number">{index + 1}</span>}
                    </div>
                    <div className="seller-details">
                        <h2>{seller.companyName}</h2>
                        <h6>{seller.contactPerson}</h6>
                    </div>
                </div>
            ))}
        </div>
    );
};

export default Leaderboard;
