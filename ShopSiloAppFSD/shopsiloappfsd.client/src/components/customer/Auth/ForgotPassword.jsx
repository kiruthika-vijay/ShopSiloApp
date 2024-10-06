import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import axios from 'axios';
import { apiClient } from '../../common/Axios/auth';

const ForgotPassword = () => {
    const [email, setEmail] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const [isLoading, setIsLoading] = useState(false);  // New loading state

    const handleForgotPassword = async (e) => {
        e.preventDefault();
        setMessage('');
        setError('');
        setIsLoading(true);  // Set loading to true when the request starts

        // Basic client-side validation
        if (!email) {
            setError('Please enter your email address.');
            setIsLoading(false);  // Stop loading if validation fails
            return;
        }

        try {
            const response = await apiClient.post('/Users/forgot-password', { email });
            setMessage(response.data.message);
            setError('');
        } catch (error) {
            if (error.response) {
                // Server responded with a status other than 2xx
                setError(error.response.data.message || 'Failed to send reset link. Please try again.');
            } else if (error.request) {
                // Request was made but no response received
                setError('No response from server. Please try again later.');
            } else {
                // Something else caused the error
                setError('An error occurred. Please try again.');
            }
        } finally {
            setIsLoading(false);  // Set loading to false when the request is done
        }
    };

    return (
        <div className="h-screen flex flex-col bg-lightjasmine">
            
            {/* Content */}
            <div className="flex-grow flex justify-center items-center">
                <div className="max-w-md w-full bg-white p-8 rounded-lg shadow-lg">
                    <h2 className="text-3xl font-bold text-spacecadet mb-6">Forgot Password</h2>
                    <form onSubmit={handleForgotPassword}>
                        <div className="mb-4">
                            <label className="block text-spacecadet text-sm mb-2 font-bold" htmlFor="email">Email Address</label>
                            <input
                                type="email"
                                id="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="Enter your email"
                                className="w-full p-3 border border-jasmine rounded bg-accent text-spacecadet placeholder-spacecadet focus:outline-none focus:ring-2 focus:ring-jasmine"
                                required
                            />
                        </div>
                        <button
                            type="submit"
                            className="w-full bg-jasmine text-background py-3 rounded-lg hover:bg-darkjasmine transition duration-300 font-semibold"
                            disabled={isLoading}  // Disable button while loading
                        >
                            {isLoading ? 'Sending Reset Link...' : 'Send Reset Link'}  {/* Show loading message */}
                        </button>
                    </form>
                    {message && <p className="text-green-500 mt-4">{message}</p>}
                    {error && <p className="text-red-500 mt-4">{error}</p>}
                    <p className="text-sm text-center mt-6 text-spacecadet font-semibold">
                        Remembered your password? <Link to="/customer/login" className="text-utorange hover:underline font-bold">Log in here</Link>
                    </p>
                </div>
            </div>
        </div>
    );
}

export default ForgotPassword;
