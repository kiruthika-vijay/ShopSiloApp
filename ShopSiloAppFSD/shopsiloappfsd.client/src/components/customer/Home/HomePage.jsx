import React, { useEffect, useState } from 'react';
import CategoryNavbar from './CategoryNavBar/CategoryNavBar';
import Slider from './SliderComponent/Slider';
import TodayDeals from './FlashSales/TodayDeals';
import BestSellingProducts from './BestSelling/BestSelling';
import PromotionalBanner from './PromotionalBanner/PromotionalBanner';
import ExploreProducts from './ExploreProducts/ExploreProducts';
import ServiceLogos from './ServiceSection/ServiceLogos';
import ScrollToTop from './ScrollToTop';
import Categories from './Categories/Categories';
import './SliderComponent/Slider.css';
import { apiClient } from '../../common/Axios/auth';

const HomePage = () => {
    const [categories, setCategories] = useState([]);
    const [flashSales, setFlashSales] = useState([]); // Default to empty array
    const [bestSellingProducts, setBestSellingProducts] = useState([]); // Default to empty array
    const [exploreProducts, setExploreProducts] = useState([]); // Default to empty array
    const imageUrl = "/images/promobanner.png";

    const slides = [
        { image: '/images/image1.png', alt: 'Slide 1' },
        { image: '/images/image2.png', alt: 'Slide 2' },
        { image: '/images/image3.png', alt: 'Slide 3' },
        { image: '/images/image4.png', alt: 'Slide 4' },
        { image: '/images/image5.png', alt: 'Slide 5' },
    ];

    useEffect(() => {
        const fetchData = async () => {
            // Fetch Categories
            try {
                const categoriesResponse = await apiClient.get('/Categories/names');
                setCategories(categoriesResponse.data.$values);
            } catch (error) {
                console.error('Error fetching categories:', error);
            }

            // Fetch Flash Sales
            try {
                const flashSalesResponse = await apiClient.get('/Product/flashsales');
                setFlashSales(flashSalesResponse.data.$values);
            } catch (error) {
                console.error('Error fetching flash sales:', error);
                setFlashSales([]); // Make sure to reset the state to empty array
            }

            // Fetch Best Selling Products
            try {
                const bestSellingResponse = await apiClient.get('/Admin/top-selling-products?limit=4');
                setBestSellingProducts(bestSellingResponse.data.$values);
            } catch (error) {
                console.error('Error fetching best-selling products:', error);
                setBestSellingProducts([]); // Reset state to empty array if error occurs
            }

            // Fetch Explore Products
            try {
                const exploreProductsResponse = await apiClient.get('/Product/explore');
                setExploreProducts(exploreProductsResponse.data.$values);
            } catch (error) {
                console.error('Error fetching explore products:', error);
                setExploreProducts([]); // Reset state to empty array if error occurs
            }
        };

        fetchData();
    }, []);

    return (
        <div>
            <div className="flex">
                <CategoryNavbar categories={categories} />
                <Slider slides={slides} className="slider-margin" /> {/* Apply margin directly to the Slider */}
            </div>

            {/* Flash sales section only rendered if there is data */}
            {flashSales.length > 0 && (
                <section>
                    <TodayDeals deals={flashSales} />
                </section>
            )}

            {/* Category section should always render */}
            <section>
                <Categories />
            </section>

            {/* Best selling products section only rendered if there is data */}
            {bestSellingProducts.length > 0 && (
                <section>
                    <BestSellingProducts products={bestSellingProducts} />
                </section>
            )}

            {/* Promotional banner always rendered */}
            <section>
                <PromotionalBanner imageUrl={imageUrl} productId={70} />
            </section>

            {/* Explore products section only rendered if there is data */}
            {exploreProducts.length > 0 && (
                <section>
                    <ExploreProducts products={exploreProducts} />
                </section>
            )}

            {/* Service logos and Scroll to top should always render */}
            <section>
                <ServiceLogos />
            </section>

            <ScrollToTop />
        </div>
    );
};

export default HomePage;