import React, { useState, useContext } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { AuthContext } from '../../customer/Auth/AuthContext';
import { apiClient, storeToken, fetchUserId } from '../../common/Axios/auth';

const AdminLogin = () => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [passwordStrength, setPasswordStrength] = useState('');
    const { setIsLoggedIn } = useContext(AuthContext);
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await apiClient.post('/Aauth/login', { email, password });
            storeToken(response.data);
            fetchUserId(response.data);
            setIsLoggedIn(true);
            navigate('/admin/dashboard');
        } catch (err) {
            setError('Invalid email or password');
        }
    };

    const checkPasswordStrength = (password) => {
        const strength = password.length < 6 ? 'Weak' :
            password.length < 10 ? 'Moderate' : 'Strong';
        setPasswordStrength(strength);
    };

    return (
        <div className="h-screen flex items-center justify-center bg-gradient-to-r from-blue-900 via-blue-800 to-blue-700 px-4 sm:px-6 lg:px-8">
            <div className="max-w-md w-full space-y-8 bg-white p-10 rounded-lg shadow-xl">
                <div>
                    <h2 className="text-center text-3xl font-extrabold text-blue-900">Admin Login</h2>
                    <p className="mt-2 text-center text-sm text-gray-600">
                        Sign in to your admin dashboard
                    </p>
                </div>
                <form onSubmit={handleLogin} className="space-y-6">
                    <div>
                        <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                            Email address
                        </label>
                        <input
                            type="email"
                            id="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            placeholder="you@example.com"
                            className="mt-1 w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            required
                        />
                    </div>

                    <div>
                        <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                            Password
                        </label>
                        <input
                            type="password"
                            id="password"
                            value={password}
                            onChange={(e) => {
                                setPassword(e.target.value);
                                checkPasswordStrength(e.target.value);
                            }}
                            placeholder="Enter your password"
                            className="mt-1 w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                            required
                        />
                        <p
                            className={`mt-1 text-sm ${passwordStrength === 'Weak' ? 'text-red-600' : passwordStrength === 'Moderate' ? 'text-yellow-600' : 'text-green-600'}`}
                        >
                            Password Strength: {passwordStrength}
                        </p>
                    </div>

                    {error && <p className="text-red-500 text-sm mt-2">{error}</p>}

                    <div>
                        <button
                            type="submit"
                            className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500"
                        >
                            Sign in
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
};

export default AdminLogin;
