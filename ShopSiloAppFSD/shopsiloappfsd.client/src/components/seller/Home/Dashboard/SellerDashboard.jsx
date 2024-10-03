import React, { useEffect, useState } from 'react';
import { apiClient } from '../../../common/Axios/auth';
import DashboardBox from '../common/DashboardBox';
import { GiCash } from "react-icons/gi";
import { FaTags } from "react-icons/fa6";
import { FaCartShopping } from "react-icons/fa6";
import { GiSellCard } from "react-icons/gi";
import { HiDotsVertical } from "react-icons/hi";
import BestSellingProducts from './BestSellingProducts';
import Leaderboard from './Leaderboard';

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
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 row dashboardBoxWrapperRow"> {/* Three columns layout */}
                <div className="col-span-1 md:col-span-2"> {/* Left column for 4 boxes */}
                    <div className="dashboardBoxWrapper flex grid grid-cols-2 gap-4"> {/* 2x2 grid for DashboardBox */}
                        <DashboardBox color={["#1DA256", "#48D483"]} icon={<GiSellCard />} value={"Sales"} count={totalSales} grow={true} />
                        <DashboardBox color={["#C012E2", "#EB64FE"]} icon={<FaTags />} value={"Products"} count={totalProducts} grow={false} />
                        <DashboardBox color={["#2C78E5", "#60AFF5"]} icon={<FaCartShopping />} value={"Orders"} count={totalOrders} grow={false} />
                        <DashboardBox color={["#E1950E", "#F3CD29"]} icon={<GiCash />} value={"Revenue"} count={totalRevenue} grow={true} />
                    </div>
                </div>
                <div className="col-span-1 md:col-span-1 h-full"> {/* Right column for the revenue graph */}                    
                    <div className="box graphBox h-full p-4 text-center">
                        <h2 className="leaderboard-title">Top 5 Sellers</h2>
                        <Leaderboard />
                    </div>
                </div>
            </div>
            <BestSellingProducts />
        </div>
    );
};

export default SellerDashboard;
