import React, { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import axios from 'axios';
import { apiClient } from '../../common/Axios/auth';

const ResetPassword = () => {
    const { token } = useParams();
    const navigate = useNavigate();
    const [newPassword, setNewPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [message, setMessage] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false); // Optional: Loading state

    const handleResetPassword = async (e) => {
        e.preventDefault();
        setMessage('');
        setError('');

        // Basic client-side validation
        if (!newPassword || !confirmPassword) {
            setError('Please enter and confirm your new password.');
            return;
        }

        if (newPassword !== confirmPassword) {
            setError('Passwords do not match.');
            return;
        }

        setLoading(true); // Start loading

        try {
            const response = await apiClient.post(`/Aauth/reset-password/${token}`, {
                password: newPassword
            });

            setMessage(response.data.message);
            setError('');
            // Optionally, redirect to login after a short delay
            setTimeout(() => navigate('/customer/login'), 3000);
        }
        catch (error) {
            console.error(error); // Log the error for debugging
            if (error.response) {
                setError(error.response.data.message || 'Failed to reset password. Please try again.');
            } else if (error.request) {
                setError('No response from server. Please try again later.');
            } else {
                setError('An error occurred. Please try again.');
            }
        }
        finally {
            setLoading(false); // End loading
        }
    };

    return (
        <div className="h-screen flex flex-col bg-lightjasmine">
            {/* Navbar */}
            <nav className="bg-white shadow-md py-4">
                <div className="container mx-auto px-6 flex justify-between items-center">
                    {/* Logo */}
                    <div>
                        <Link to="/">
                            <img
                                src="/images/shopSiloLogo.png"
                                alt="ShopSilo Logo"
                                className="h-10 w-auto" // Adjust the height as needed
                            />
                        </Link>
                    </div>

                    {/* Navigation Links */}
                    <div>
                        <Link to="/customer/login" className="text-spacecadet hover:text-darkjasmine px-4 text-l font-bold">Login</Link>
                        <Link to="/customer/register" className="text-spacecadet hover:text-darkjasmine px-4 text-l font-bold">Register</Link>
                    </div>
                </div>
            </nav>

            {/* Content */}
            <div className="flex-grow flex justify-center items-center">
                <div className="max-w-md w-full bg-white p-8 rounded-lg shadow-lg">
                    <h2 className="text-3xl font-bold text-spacecadet mb-6">Reset Password</h2>
                    <form onSubmit={handleResetPassword}>
                        <div className="mb-4">
                            <label className="block text-spacecadet text-sm mb-2 font-bold" htmlFor="newPassword">New Password</label>
                            <input
                                type="password"
                                id="newPassword"
                                value={newPassword}
                                onChange={(e) => setNewPassword(e.target.value)}
                                placeholder="Enter your new password"
                                className="w-full p-3 border border-jasmine rounded bg-accent text-spacecadet placeholder-spacecadet focus:outline-none focus:ring-2 focus:ring-jasmine"
                                required
                            />
                        </div>
                        <div className="mb-4">
                            <label className="block text-spacecadet text-sm mb-2 font-bold" htmlFor="confirmPassword">Confirm Password</label>
                            <input
                                type="password"
                                id="confirmPassword"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                placeholder="Confirm your new password"
                                className="w-full p-3 border border-jasmine rounded bg-accent text-spacecadet placeholder-spacecadet focus:outline-none focus:ring-2 focus:ring-jasmine"
                                required
                            />
                        </div>
                        <button
                            type="submit"
                            className="w-full bg-jasmine text-background py-3 rounded-lg hover:bg-darkjasmine transition duration-300 font-semibold"
                            disabled={loading} // Disable while loading
                        >
                            {loading ? 'Resetting...' : 'Reset Password'}
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

export default ResetPassword;
