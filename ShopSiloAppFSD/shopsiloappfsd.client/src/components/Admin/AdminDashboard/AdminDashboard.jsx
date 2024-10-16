import React, { useEffect, useState } from 'react';
import { Link, Outlet } from 'react-router-dom';

import SalesAnalyticsChart from './SalesAnalyticsChart';
import TopSellingProductsChart from './TopsellingProductschart';

const AdminDashboard = () => {

    return (
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mt-6">

            <TopSellingProductsChart />
            <SalesAnalyticsChart />

        </div>
    )

    const scrollToTop = () => {
        window.scrollTo({ top: 0, behavior: 'smooth' });
    };
}

export default AdminDashboard;