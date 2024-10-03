import React, { useState } from 'react';
import axios from 'axios';
import { apiClient } from '../../../common/Axios/auth';
const LoginPopup = ({ onClose }) => {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [errorMessage, setErrorMessage] = useState('');

    const handleLogin = async (e) => {
        e.preventDefault();
        setErrorMessage('');

        try {
            const response = await apiClient.post('/Aauth/login', { email, password });
            localStorage.setItem('token', response.data.token); // Store the token in local storage
            onClose(); // Close the popup after successful login
            window.location.reload(); // Refresh the page to update the UI
        } catch (error) {
            setErrorMessage('Invalid email or password. Please try again.');
            console.error('Login error:', error);
        }
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex justify-center items-center z-50">
            <div className="bg-white p-5 rounded-lg shadow-lg w-80">
                <h2 className="text-lg font-bold mb-4">Login</h2>
                {errorMessage && <p className="text-red-500 text-sm mb-2">{errorMessage}</p>}
                <form onSubmit={handleLogin}>
                    <div className="mb-4">
                        <label className="block text-sm font-semibold mb-1">Email</label>
                        <input
                            type="email"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            className="border rounded w-full p-2"
                            required
                        />
                    </div>
                    <div className="mb-4">
                        <label className="block text-sm font-semibold mb-1">Password</label>
                        <input
                            type="password"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                            className="border rounded w-full p-2"
                            required
                        />
                    </div>
                    <button
                        type="submit"
                        className="bg-blue-500 text-white px-4 py-2 rounded"
                    >
                        Login
                    </button>
                    <button
                        type="button"
                        onClick={onClose}
                        className="mt-2 text-sm text-gray-500"
                    >
                        Cancel
                    </button>
                </form>
            </div>
        </div>
    );
};

export default LoginPopup;
