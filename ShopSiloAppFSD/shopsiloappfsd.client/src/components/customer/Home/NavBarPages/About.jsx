import React from 'react';
import { FaBullseye, FaLightbulb, FaUsers } from 'react-icons/fa';

const AboutPage = () => {
    return (
        <div className="flex flex-col items-center p-8 max-w-7xl mx-auto">
            <h2 className="text-3xl font-bold text-center text-gray-800 mb-6">About ShopSilo</h2>

            {/* About Us Section */}
            <section className="flex flex-col items-center bg-gray-100 p-8 rounded-lg shadow-md mb-10">
                <h3 className="text-2xl font-bold text-gray-800 mb-4">Who We Are</h3>
                <p className="text-gray-600 text-center max-w-4xl">
                    ShopSilo is your one-stop online destination for top-quality products at competitive prices. We specialize in a wide range of categories from electronics to home appliances, offering a seamless shopping experience to all our customers. Our mission is to make shopping easier, faster, and more reliable by providing a user-friendly platform and exceptional customer service.
                </p>
            </section>

            {/* Mission and Vision */}
            <section className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-10">
                <div className="flex flex-col items-center text-center bg-white p-8 rounded-lg shadow-md">
                    <FaBullseye className="text-4xl text-utorange mb-4" />
                    <h3 className="font-semibold text-xl text-gray-800 mb-2">Our Mission</h3>
                    <p className="text-gray-600">
                        To deliver a world-class online shopping experience by providing high-quality products, reliable delivery, and 24/7 customer support.
                    </p>
                </div>
                <div className="flex flex-col items-center text-center bg-white p-8 rounded-lg shadow-md">
                    <FaLightbulb className="text-4xl text-utorange mb-4" />
                    <h3 className="font-semibold text-xl text-gray-800 mb-2">Our Vision</h3>
                    <p className="text-gray-600">
                        To be the go-to eCommerce platform for customers worldwide, known for quality, innovation, and customer satisfaction.
                    </p>
                </div>
            </section>

            {/* Core Values Section */}
            <section className="bg-gray-100 p-8 rounded-lg shadow-md mb-10 w-full">
                <h3 className="text-2xl font-bold text-center text-gray-800 mb-6">Our Core Values</h3>
                <ul className="grid grid-cols-1 md:grid-cols-3 gap-6">
                    <li className="flex flex-col items-center text-center">
                        <h4 className="font-semibold text-lg text-gray-800 mb-2">Customer First</h4>
                        <p className="text-gray-600">We put our customers at the heart of everything we do.</p>
                    </li>
                    <li className="flex flex-col items-center text-center">
                        <h4 className="font-semibold text-lg text-gray-800 mb-2">Integrity</h4>
                        <p className="text-gray-600">We believe in honest and transparent business practices.</p>
                    </li>
                    <li className="flex flex-col items-center text-center">
                        <h4 className="font-semibold text-lg text-gray-800 mb-2">Innovation</h4>
                        <p className="text-gray-600">We continually seek to improve and innovate in all areas.</p>
                    </li>
                </ul>
            </section>

            {/* Team Section */}
            <section className="bg-white p-8 rounded-lg shadow-md w-full">
                <h3 className="text-2xl font-bold text-center text-gray-800 mb-6">Meet Our Team</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                    <div className="flex flex-col items-center">
                        <img
                            src="https://via.placeholder.com/150"
                            alt="Team Member 1"
                            className="rounded-full w-32 h-32 mb-4"
                        />
                        <h4 className="font-semibold text-lg text-gray-800">Jane Doe</h4>
                        <p className="text-gray-600">CEO & Founder</p>
                    </div>
                    <div className="flex flex-col items-center">
                        <img
                            src="https://via.placeholder.com/150"
                            alt="Team Member 2"
                            className="rounded-full w-32 h-32 mb-4"
                        />
                        <h4 className="font-semibold text-lg text-gray-800">John Smith</h4>
                        <p className="text-gray-600">CTO</p>
                    </div>
                    <div className="flex flex-col items-center">
                        <img
                            src="https://via.placeholder.com/150"
                            alt="Team Member 3"
                            className="rounded-full w-32 h-32 mb-4"
                        />
                        <h4 className="font-semibold text-lg text-gray-800">Emily Brown</h4>
                        <p className="text-gray-600">Head of Marketing</p>
                    </div>
                    {/* Add more team members as needed */}
                </div>
            </section>

        </div>
    );
};

export default AboutPage;
