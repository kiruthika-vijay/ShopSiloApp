import React, { useState, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext } from '../../customer/Auth/AuthContext';
import { apiClient, storeToken, fetchUserId, storeUserId } from '../../common/Axios/auth';

const SellerLogin = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [passwordStrength, setPasswordStrength] = useState('');
    const { setIsLoggedIn } = useContext(AuthContext);
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await apiClient.post('/Aauth/login', {
                email,
                password,
            });

            storeToken(response.data);
            fetchUserId(response.data);
            setIsLoggedIn(true);
            console.log("Login successful", response.data);
            navigate('/seller/dashboard'); // Navigate to seller dashboard
        } catch (err) {
            setError('Invalid email or password');
            console.error('Login failed:', err);
        }
    };

    const checkPasswordStrength = (password) => {
        const strength = password.length < 6 ? 'Weak' :
            password.length < 10 ? 'Moderate' : 'Strong';
        setPasswordStrength(strength);
    };

    return (
        <div className="h-screen flex w-full bg-gradient-to-b from-blue-400 to-purple-300 relative">
            <div className="flex-1 flex justify-center items-center">
                <div className="max-w-md w-full p-8 rounded-lg bg-white shadow-lg transition-transform transform hover:scale-105 relative overflow-hidden">
                    {/* Adding a subtle background animation */}
                    <div className="absolute inset-0 bg-gradient-to-br from-blue-400 to-purple-200 opacity-30 rounded-lg animate-pulse"></div>
                    <h2 className="text-4xl font-extrabold text-purple-900 mb-6 relative z-10">Seller Login</h2>
                    <form onSubmit={handleLogin} className="space-y-6 relative z-10">
                        <div>
                            <label className="block text-purple-800 text-sm mb-2 font-bold" htmlFor="email">Email Address</label>
                            <input
                                type="email"
                                id="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="Enter your email"
                                className="w-full p-3 border border-purple-600 rounded bg-white text-purple-900 placeholder-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-400"
                            />
                        </div>
                        <div>
                            <label className="block text-purple-800 text-sm mb-2 font-bold" htmlFor="password">Password</label>
                            <input
                                type="password"
                                id="password"
                                value={password}
                                onChange={(e) => {
                                    setPassword(e.target.value);
                                    checkPasswordStrength(e.target.value);
                                }}
                                placeholder="Enter your password"
                                className="w-full p-3 border border-purple-600 rounded bg-white text-purple-900 placeholder-purple-500 focus:outline-none focus:ring-2 focus:ring-purple-400"
                            />
                            <p className={`text - ${passwordStrength === 'Weak' ? 'red-600' : passwordStrength === 'Moderate' ? 'yellow-600' : 'green-600'} text-sm mt-1`}>
                            Password Strength: {passwordStrength}
                        </p>
                </div>
                <div className="flex justify-end">
                    <Link to="/customer/forgot-password" className="text-sm text-purple-600 hover:text-blue-500 underline font-semibold">
                        Forgot Password?
                    </Link>
                </div>
                {error && <p className="text-red-500 mt-4">{error}</p>}
                <button
                    type="submit"
                    className="w-full bg-purple-700 text-white py-3 rounded-lg hover:bg-purple-800 transition duration-300 font-semibold"
                >
                    Log In
                        </button>
                        <p className="text-sm text-center mt-6 text-purple-700 font-semibold">
                            Don't have an account? <Link to="/seller/register" className="text-purple-500 hover:underline font-bold">Register here</Link>
                        </p>
                    </form> 
                </div>
            </div >
        </div >
    );
};

export default SellerLogin;