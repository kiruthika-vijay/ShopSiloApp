import React, { useEffect, useState } from 'react';
import { apiClient } from '../../../common/Axios/auth';
import { FaMedal, FaCrown } from 'react-icons/fa'; // Crown for #1
import './Leaderboard.css'; // Custom styles

const Leaderboard = () => {
    const [topSellers, setTopSellers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchTopSellers = async () => {
            try {
                const response = await apiClient.get('/Seller/top/5'); // Adjust API path
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
        <div>
            {/* Top 3 Sellers */}
            <div className="top-sellers">
                <div className="top-seller rank-2">
                    <div className="seller-circle rank-2-circle">
                        <span className="rank-number">2</span>
                    </div>
                    <div className="seller-details">
                        <h4>{topSellers[1]?.companyName}</h4>
                        <p>{topSellers[1]?.contactPerson}</p>
                        <p>{topSellers[1]?.userName}</p>
                    </div>
                </div>
                <div className="top-seller rank-1">
                    <FaCrown className="gold-crown mt-1 ml-4" />
                    <div className="seller-circle rank-1-circle">
                        <span className="rank-number">1</span>
                    </div>
                    <div className="seller-details">
                        <h4>{topSellers[0]?.companyName}</h4>
                        <p>{topSellers[0]?.contactPerson}</p>
                        <p>{topSellers[0]?.userName}</p>
                    </div>
                </div>
                <div className="top-seller rank-3">
                    <div className="seller-circle rank-3-circle">
                        <span className="rank-number">3</span>
                    </div>
                    <div className="seller-details">
                        <h4>{topSellers[2]?.companyName}</h4>
                        <p>{topSellers[2]?.contactPerson}</p>
                    </div>
                </div>
            </div>

            {/* Drawer-style for all sellers */}
            <div className="seller-list">
                {topSellers.map((seller, index) => (
                    <div key={seller.sellerId} className="seller-row">
                        <div className="seller-row-details flex">
                            <span className="row-rank flex mr-4">{index + 1}</span>
                            <div className="row-seller-info">
                                <h4>{seller.companyName}</h4>
                                <p>{seller.contactPerson}</p>
                            </div>
                        </div>
                        {index < topSellers.length - 1 && <hr className="divider" />}
                    </div>
                ))}
            </div>
        </div>
    );
};

export default Leaderboard;
