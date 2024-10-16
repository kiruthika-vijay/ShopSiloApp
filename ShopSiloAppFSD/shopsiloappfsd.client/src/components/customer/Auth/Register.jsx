import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { apiClient } from '../../common/Axios/auth';

const Register = () => {
    const [username, setUserName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleRegister = async (e) => {
        e.preventDefault();
        setError(''); // Clear any previous error messages

        // Validate inputs before sending the request
        if (!username || !email || !password) {
            setError('All fields are required.');
            return;
        }

        try {
            const response = await apiClient.post('/Aauth/customer-register', {
                username,
                email,
                password,
            });

            console.log('Registration successful:', response.data);
            navigate('/customer/login'); // Redirect to login after successful registration

        } catch (error) {
            // Handle error
            if (error.response) {
                setError(error.response.data || 'Registration failed. Please try again.');
            } else {
                setError('An error occurred. Please try again later.');
            }
        }
    };

    return (
        <div className="h-screen bg-white flex">
            <div className="flex w-full">
                <div className="hidden lg:flex flex-1 items-center justify-center bg-white">
                    <img
                        src="/images/login.png"
                        alt="Register"
                        className="object-cover h-full w-full opacity-90"
                    />
                </div>

                <div className="flex-1 flex justify-center items-center px-4 py-8 lg:py-0 bg-white shadow-lg">
                    <div className="max-w-md w-full">
                        <h2 className="text-4xl font-extrabold text-spacecadet mb-4">Create an Account</h2>
                        <p className="text-lg text-utorange mb-6">
                            Join ShopSilo today and get access to exclusive offers!
                        </p>
                        {error && <p className="text-red-500 mb-4">{error}</p>}
                        <form onSubmit={handleRegister} className="space-y-4">
                            <div>
                                <label className="block text-spacecadet text-sm mb-2 font-bold" htmlFor="username">Username</label>
                                <input
                                    type="text"
                                    id="username"
                                    value={username}
                                    onChange={(e) => setUserName(e.target.value)}
                                    placeholder="Enter your Username"
                                    className="w-full p-3 border border-jasmine rounded bg-accent text-spacecadet placeholder-spacecadet focus:outline-none focus:ring-2 focus:ring-jasmine"
                                />
                            </div>
                            <div>
                                <label className="block text-spacecadet text-sm mb-2 font-bold" htmlFor="email">Email Address</label>
                                <input
                                    type="email"
                                    id="email"
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    placeholder="Enter your email"
                                    className="w-full p-3 border border-jasmine rounded bg-accent text-spacecadet placeholder-spacecadet focus:outline-none focus:ring-2 focus:ring-jasmine"
                                />
                            </div>
                            <div>
                                <label className="block text-spacecadet text-sm mb-2 font-bold" htmlFor="password">Password</label>
                                <input
                                    type="password"
                                    id="password"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    placeholder="Create a password"
                                    className="w-full p-3 border border-jasmine rounded bg-accent text-spacecadet placeholder-spacecadet focus:outline-none focus:ring-2 focus:ring-jasmine"
                                />
                            </div>
                            <button type="submit" className="w-full bg-jasmine text-background py-3 rounded-lg hover:bg-darkjasmine transition duration-300">
                                Register
                            </button>
                        </form>
                        <p className="text-sm text-center mt-6 text-spacecadet font-semibold">
                            Already have an account? <Link to="/customer/login" className="text-utorange hover:underline font-bold">Log in here</Link>
                        </p>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Register;
