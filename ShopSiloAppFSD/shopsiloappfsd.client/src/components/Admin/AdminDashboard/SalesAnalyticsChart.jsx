import React, { useState, useEffect } from 'react';
import ReactECharts from 'echarts-for-react';
//import * as echarts from 'echarts';
import { apiClient } from '../../common/Axios/auth';

const months = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
];
const options = {
    title: {
        text: 'Sales Data',
        left: 'center'
    },
    legend: {
        orient: 'vertical',
        left: 'left'
    },
    tooltip: { trigger: 'item' },
    series: [
        {
            name: 'Sales data',
            type: 'pie',
            radius: '50%',
            data: [

            ],
            emphasis: {
                itemStyle: {
                    shadowBlur: 10,
                    shadowOffsetX: 0,
                    shadowColor: 'rgba(0, 0, 0, 0.5)'
                }
            }
        }
    ]


};
const SalesAnalyticsChart = () => {
    const [salesData, setSalesData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [option, setOption] = useState(options)

    useEffect(() => {
        const fetchSalesData = async () => {
            try {
                const response = await apiClient.get('/Admin/salesReport/byMonth');
                setSalesData(response.data.$values.map(d => ({ value: d.totalSales, name: months[d.month] })));
                //setOption({...option.series,data:salesData})
            } catch (err) {
                setError('Failed to fetch sales data.');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchSalesData();
    }, []);
    useEffect(() => {
        let op = { ...option }
        op.series[0].data = salesData
        setOption(op)

    }, [salesData]);

    const dates = salesData.map(data => data.date);
    const salesAmounts = salesData.map(data => data.totalAmount);

    if (loading) return <p className="text-blue-500">Loading sales data...</p>;

    return (
        <div className="bg-white p-6 rounded-lg shadow-lg">
            <h2 className="text-2xl font-bold mb-4">Sales Analytics</h2>
            {error ? (
                <p className="text-red-500">{error}</p>
            ) : (
                <ReactECharts option={option} />
            )}
        </div>
    );
};

export default SalesAnalyticsChart;