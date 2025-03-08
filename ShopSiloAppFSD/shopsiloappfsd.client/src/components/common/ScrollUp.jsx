import { useEffect } from 'react';
import { useLocation } from 'react-router-dom';

const ScrollUp = () => {
    const { pathname } = useLocation();

    useEffect(() => {
        // Enable smooth scroll behavior
        document.documentElement.style.scrollBehavior = 'smooth';

        window.scrollTo(0, 0);

        // Clean up smooth scroll behavior to avoid interference with manual scrolls
        return () => {
            document.documentElement.style.scrollBehavior = 'auto';
        };
    }, [pathname]);

    return null;
};

export default ScrollUp;
