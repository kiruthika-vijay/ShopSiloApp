import React, { useState } from 'react';
import { FaPhoneAlt, FaEnvelope, FaMapMarkerAlt, FaClock } from 'react-icons/fa';
import axios from 'axios';
import { apiClient } from '../../../common/Axios/auth';

const ContactPage = () => {
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState('');
    const [status, setStatus] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();

        const contactForm = { name, email, message };

        try {
            const response = await apiClient.post('/Profile/send', contactForm);
            setStatus('Message sent successfully!');
            // Clear form fields
            setName('');
            setEmail('');
            setMessage('');
        } catch (error) {
            setStatus('Failed to send message. Please try again.');
        }
    };

    return (
        <div className="flex flex-col items-center p-8 max-w-7xl mx-auto">
            <h2 className="text-3xl font-bold text-center text-gray-800 mb-6">Contact Us</h2>

            {/* Contact Info */}
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-10 w-full">
                <div className="flex flex-col items-center text-center">
                    <FaPhoneAlt className="text-4xl text-utorange mb-2" />
                    <h3 className="font-semibold text-lg">Phone</h3>
                    <p className="text-gray-600">+1 (123) 456-7890</p>
                </div>
                <div className="flex flex-col items-center text-center">
                    <FaEnvelope className="text-4xl text-utorange mb-2" />
                    <h3 className="font-semibold text-lg">Email</h3>
                    <p className="text-gray-600">support@shopsilo.com</p>
                </div>
                <div className="flex flex-col items-center text-center">
                    <FaMapMarkerAlt className="text-4xl text-utorange mb-2" />
                    <h3 className="font-semibold text-lg">Location</h3>
                    <p className="text-gray-600">123 ShopSilo Street, City, Country</p>
                </div>
                <div className="flex flex-col items-center text-center">
                    <FaClock className="text-4xl text-utorange mb-2" />
                    <h3 className="font-semibold text-lg">Business Hours</h3>
                    <p className="text-gray-600">Mon - Fri: 9 AM - 5 PM</p>
                </div>
            </div>

            {/* Contact Form */}
            <div className="w-full max-w-3xl p-8 bg-gray-100 rounded-lg shadow-md">
                <h3 className="text-2xl font-bold text-center text-gray-800 mb-6">Send Us a Message</h3>
                <form className="space-y-6" onSubmit={handleSubmit}>
                    <div className="flex flex-col">
                        <label className="text-sm font-semibold text-gray-600 mb-2" htmlFor="name">Your Name</label>
                        <input
                            id="name"
                            type="text"
                            placeholder="Enter your name"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                            className="border border-gray-300 rounded-md px-4 py-2 focus:outline-none focus:ring-2 focus:ring-utorange"
                            required
                        />
                    </div>
                    <div className="flex flex-col">
                        <label className="text-sm font-semibold text-gray-600 mb-2" htmlFor="email">Your Email</label>
                        <input
                            id="email"
                            type="email"
                            placeholder="Enter your email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className="border border-gray-300 rounded-md px-4 py-2 focus:outline-none focus:ring-2 focus:ring-utorange"
                            required
                        />
                    </div>
                    <div className="flex flex-col">
                        <label className="text-sm font-semibold text-gray-600 mb-2" htmlFor="message">Your Message</label>
                        <textarea
                            id="message"
                            rows="5"
                            placeholder="Write your message here"
                            value={message}
                            onChange={(e) => setMessage(e.target.value)}
                            className="border border-gray-300 rounded-md px-4 py-2 focus:outline-none focus:ring-2 focus:ring-utorange"
                            required
                        ></textarea>
                    </div>
                    <button
                        type="submit"
                        className="w-full bg-utorange text-white font-semibold rounded-md px-4 py-2 hover:bg-orange-600 transition-all duration-300"
                    >
                        Send Message
                    </button>
                </form>
                {status && <p className="text-center text-gray-600 mt-4">{status}</p>}
            </div>

            {/* Embedded Google Map */}
            <div className="w-full mt-10">
                <h3 className="text-2xl font-bold text-center text-gray-800 mb-6">Find Us</h3>
                <div className="relative w-full" style={{ paddingTop: '56.25%' }}>
                    <iframe
                        src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d15561.487120599855!2d80.01975088272955!3d12.819235580470245!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x3a52f78540c8f27f%3A0xf489d25aa317c70b!2sSridevi%20Tailors!5e0!3m2!1sen!2sin!4v1727115085526!5m2!1sen!2sin"
                        className="absolute top-0 left-0 w-full h-full rounded-md shadow-md"
                        frameBorder="0"
                        allowFullScreen=""
                        aria-hidden="false"
                        tabIndex="0"
                        loading="lazy"
                        referrerPolicy="no-referrer-when-downgrade"
                        title="ShopSilo Location"
                    ></iframe>
                </div>
            </div>
        </div>
    );
};

export default ContactPage;
