// ScrollToTop.js
import React, { useEffect, useState } from 'react';
import { FaArrowUp } from "react-icons/fa";

const ScrollToTop = () => {
    const [isVisible, setIsVisible] = useState(false);

    const toggleVisibility = () => {
        if (window.scrollY > 300) {
            setIsVisible(true);
        } else {
            setIsVisible(false);
        }
    };

    const scrollToTop = () => {
        window.scrollTo({
            top: 0,
            behavior: 'smooth' // Smooth scrolling effect
        });
    };

    useEffect(() => {
        window.addEventListener('scroll', toggleVisibility);
        return () => {
            window.removeEventListener('scroll', toggleVisibility);
        };
    }, []);

    return (
        <button
            onClick={scrollToTop}
            className={`fixed bottom-4 right-4 p-2 rounded bg-orange-500 text-white shadow-lg transition-opacity ${isVisible ? 'opacity-100' : 'opacity-0'}`}
            style={{ display: isVisible ? 'block' : 'none' }} // Show button only when scrolled down
            aria-label="Scroll to top"
        >
            <FaArrowUp/>
        </button>
    );
};

export default ScrollToTop;
