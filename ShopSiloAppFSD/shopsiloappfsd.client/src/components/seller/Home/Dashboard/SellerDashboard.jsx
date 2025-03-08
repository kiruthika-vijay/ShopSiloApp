import React, { useEffect, useState } from 'react';
import { apiClient } from '../../../common/Axios/auth';
import DashboardBox from '../common/DashboardBox';
import { GiCash } from "react-icons/gi";
import { FaTags } from "react-icons/fa6";
import { FaCartShopping } from "react-icons/fa6";
import { GiSellCard } from "react-icons/gi";
import Leaderboard from './Leaderboard';
import BestSellingProducts from './BestSellingProducts';
import './Leaderboard.css';

const SellerDashboard = ({ sellerId }) => {
    const [totalSales, setTotalSales] = useState(0);
    const [totalOrders, setTotalOrders] = useState(0);
    const [totalProducts, setTotalProducts] = useState(0);
    const [totalRevenue, setTotalRevenue] = useState(0);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchDashboardData = async () => {
            try {
                const response = await apiClient.get(`/SellerDashboard`);
                const { totalSales, totalOrders, totalProducts, totalRevenue } = response.data;

                setTotalSales(totalSales);
                setTotalOrders(totalOrders);
                setTotalProducts(totalProducts);
                setTotalRevenue(totalRevenue);
            } catch (error) {
                console.error('Error fetching dashboard data:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchDashboardData();
    }, [sellerId]);

    return (
        <div className="right-content w-full">
            
            {/* DashboardBox components in a single full-width row */}
            <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-4 ml-4 row dashboardBoxWrapperRow dashboardBoxWrapper">
                <DashboardBox color={["#1DA256", "#48D483"]} icon={<GiSellCard />} value={"Sales"} count={totalSales} grow={true} />
                <DashboardBox color={["#C012E2", "#EB64FE"]} icon={<FaTags />} value={"Products"} count={totalProducts} grow={false} />
                <DashboardBox color={["#2C78E5", "#60AFF5"]} icon={<FaCartShopping />} value={"Orders"} count={totalOrders} grow={false} />
                <DashboardBox color={["#E1950E", "#F3CD29"]} icon={<GiCash />} value={"Revenue"} count={totalRevenue} grow={true} />
            </div>

            {/* Leaderboard section at the top, full-width */}
            <div className="w-full p-4 mb-4 box graphBox text-center">
                <h2 className="leaderboard-title">Top 5 Sellers</h2>
                <Leaderboard />
            </div>

            {/* Best Selling Products section */}
            <BestSellingProducts />
        </div>
    );
};

export default SellerDashboard;
