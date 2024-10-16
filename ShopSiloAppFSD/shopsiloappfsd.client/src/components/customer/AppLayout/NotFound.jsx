import React from 'react';
import { Link } from 'react-router-dom';

const NotFound = () => {
    return (
        <div className="min-h-screen flex flex-col justify-center items-center bg-white overflow-hidden">
            <img
                src="/images/404errorrbg.png"
                alt="404error"
                className="w-auto max-w-[150px] md:max-w-[200px] lg:max-w-[330px] mb-8" // Adjusted image sizing
            />
            <h1 className="text-4xl font-bold text-spacecadet mb-4">404 - Page Not Found</h1>
            <p className="text-lg text-spacecadet mb-6">The page you are looking for does not exist.</p>
            <Link to="/" className="text-utorange hover:underline font-bold">Go to Home</Link>
        </div>
    );
}

export default NotFound;
