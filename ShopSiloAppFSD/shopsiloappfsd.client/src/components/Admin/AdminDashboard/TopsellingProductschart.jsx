import React, { useEffect, useState, useId } from 'react';

import ReactECharts from 'echarts-for-react';
import { apiClient } from '../../common/Axios/auth';

const options = {
    title: {
        text: 'Product Data',
        left: 'center'
    },
    tooltip: {
        trigger: 'axis',
        axisPointer: {
            type: 'shadow'
        }
    },
    xAxis: {
        type: 'category',
        data: [],
        axisLabel: {
            interval: 0, // Show all labels, no skipping
            formatter: function (value) {
                // Add word wrap logic (break the label into chunks)
                return value.length > 10 ? value.substring(0, 10) + '\n' + value.substring(10) : value;
            },
            textStyle: {
                fontSize: 10, // Adjust font size
            },
        },
        barWidth: '20%',
    },
    yAxis: {
        name: 'Sales Quantity',
        nameLocation: "middle",
        nameTextStyle: {
            align: "center",
            padding: [17, 17, 17, 17]
        }
    },
    legend: {
        orient: 'vertical',
        left: 'left',
        textStyle: {
            width: 100, // Limit the width for word wrapping
            overflow: 'break', // Enable word wrapping
        },
    },
    series: [
        {
            name: 'Top selling products',
            type: 'bar',
            data: [],
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


const TopSellingProductsChart = () => {
    const [topProducts, setTopProducts] = useState([]);
    const [topQuantity, seTopQuantity] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [option, setOption] = useState(options)

    useEffect(() => {
        const fetchTopProducts = async () => {
            try {
                const response = await apiClient.get('/Admin/top-selling-products/chart?limit=5');
                setTopProducts(response.data.$values.map(d => (d.productName)));
                seTopQuantity(response.data.$values.map(d => (d.quantity)));

            } catch (err) {
                setError('Failed to fetch top-selling products.');
                console.error(err);
            } finally {
                setLoading(false);
            }
        };
        fetchTopProducts();
    }, []);

    useEffect(() => {
        let op = { ...option }
        op.series[0].data = topQuantity
        op.xAxis.data = topProducts
        setOption(op)


    }, [topProducts, topQuantity]);


    if (loading) return <p className="text-blue-500">Loading top-selling products...</p>;

    return (
        <div className="bg-white p-6 rounded-lg shadow-lg">
            <h2 className="text-2xl font-bold mb-4">Top Selling Products</h2>
            {error ? (
                <p className="text-red-500">{error}</p>
            ) : (
                <ReactECharts option={option} />
            )}
        </div>
    );
};

export default TopSellingProductsChart;