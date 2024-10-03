import React from 'react';
import Slider from 'react-slick';
import './Slider.css';
import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

const ImageSlider = () => {
    const settings = {
        dots: false,
        infinite: true,
        speed: 500,
        slidesToShow: 1,
        autoplay: true,
        autoplaySpeed: 3000,
    };

    const slides = [
        '/images/image1.png',
        '/images/image2.png',
        '/images/image3.png',
        '/images/image4.png',
        '/images/image5.png',
    ];

    return (
        <div className="slider-container" style={{ padding: '20px' }}>
            <Slider {...settings}>
                {slides.map((slide, index) => (
                    <div key={index}>
                        <img src={slide} alt={`Slide ${index + 1}`} className="w-full" />
                    </div>
                ))}
            </Slider>
        </div>
    );
};

export default ImageSlider;
