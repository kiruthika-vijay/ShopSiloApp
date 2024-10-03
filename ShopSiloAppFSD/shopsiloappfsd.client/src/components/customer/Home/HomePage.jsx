import React, { useEffect, useState } from 'react';
import CategoryNavbar from './CategoryNavBar/CategoryNavBar';
import Slider from './SliderComponent/Slider';
import TodayDeals from './FlashSales/TodayDeals';
import BestSellingProducts from './BestSelling/BestSelling';
import PromotionalBanner from './PromotionalBanner/PromotionalBanner';
import ExploreProducts from './ExploreProducts/ExploreProducts';
import NewArrivals from './NewArrivals/NewArrivals';
import ServiceLogos from './ServiceSection/ServiceLogos';
import ScrollToTop from './ScrollToTop';
import Categories from './Categories/Categories';
import './SliderComponent/Slider.css';
import { apiClient } from '../../common/Axios/auth';

const HomePage = () => {
    const [categories, setCategories] = useState([]);
    const [flashSales, setFlashSales] = useState([]);
    const [bestSellingProducts, setBestSellingProducts] = useState([]);
    const imageUrl = "/images/promobanner.png";
    const [exploreProducts, setExploreProducts] = useState([]);
    //const [newArrivals, setNewArrivals] = useState([]);

    const slides = [
        { image: '/images/image1.png', alt: 'Slide 1' },
        { image: '/images/image2.png', alt: 'Slide 2' },
        { image: '/images/image3.png', alt: 'Slide 3' },
        { image: '/images/image4.png', alt: 'Slide 4' },
        { image: '/images/image5.png', alt: 'Slide 5' },
    ];

    useEffect(() => {
        const fetchData = async () => {
            try {
                const categoriesResponse = await apiClient.get('/Categories/names');
                const flashSalesResponse = await apiClient.get('/Product/flashsales');
                const bestSellingResponse = await apiClient.get('/Admin/top-selling-products?limit=4');
                const exploreProductsResponse = await apiClient.get('/Product/explore');
                //const newArrivalsResponse = await apiClient.get('/Product/new-arrivals?limit=4');

                setCategories(categoriesResponse.data.$values);
                setFlashSales(flashSalesResponse.data.$values);
                setBestSellingProducts(bestSellingResponse.data.$values);
                setExploreProducts(exploreProductsResponse.data.$values);

                //setNewArrivals(newArrivalsResponse.data.$values);
            } catch (error) {
                console.error('Error fetching data:', error);
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
            <section>
                <TodayDeals deals={flashSales} />
            </section>
            <section>
                <Categories />
            </section>
            <section>
                <BestSellingProducts products={bestSellingProducts} />
            </section>
            <section>
                <PromotionalBanner imageUrl={imageUrl} />
            </section>
            <section>
                <ExploreProducts products={exploreProducts} />
            </section>
            {/*<section>*/}
            {/*    <NewArrivals arrivals={newArrivals} />*/}
            {/*</section>*/}
            <section>
                <ServiceLogos />
            </section>
            <ScrollToTop />
        </div>
    );
};

const scrollToTop = () => {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};

export default HomePage;
