import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { apiClient } from '../../common/Axios/auth';

const SellerRegister = () => {
    const [name, setName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [confirmPassword, setConfirmPassword] = useState('');
    const [error, setError] = useState('');
    const [successMessage, setSuccessMessage] = useState('');
    const navigate = useNavigate();

    const handleRegister = async (e) => {
        e.preventDefault();
        if (password !== confirmPassword) {
            setError("Passwords don't match");
            return;
        }
        try {
            const response = await apiClient.post('/Aauth/register', {
                name,
                email,
                password,
            });

            setSuccessMessage("Registration successful! Please log in.");
            setError('');
            console.log("Registration successful", response.data);
            setTimeout(() => {
                navigate('/seller/login');
            }, 2000);
        } catch (err) {
            setError('Registration failed, please try again');
            console.error('Registration failed:', err);
        }
    };

    return (
        <div className="h-screen flex w-full bg-[#bcd2fd]"> {/* Light blue background for the entire page */}
            <div className="flex-1 flex justify-center items-center bg-gradient-to-br from-[#bcd2fd] to-[#d1c4e9]"> {/* Gradient background with light purple */}
                <div className="max-w-md w-full p-8 rounded-lg bg-[#f3f4f6] shadow-md"> {/* Light grey background for the card */}
                    <h2 className="text-4xl font-extrabold text-[#6a5acd] mb-6">Seller Registration</h2> {/* Violet color for the title */}
                    <form onSubmit={handleRegister} className="space-y-6">
                        <div>
                            <label className="block text-[#6a5acd] text-sm mb-2 font-bold" htmlFor="name">Name</label> {/* Violet color for labels */}
                            <input
                                type="text"
                                id="name"
                                value={name}
                                onChange={(e) => setName(e.target.value)}
                                placeholder="Enter your name"
                                className="w-full p-3 border border-[#5C7457] rounded bg-white text-[#6a5acd] placeholder-[#6a5acd] focus:outline-none focus:ring-2 focus:ring-[#C1BCAC]"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-[#6a5acd] text-sm mb-2 font-bold" htmlFor="email">Email Address</label> {/* Violet color for labels */}
                            <input
                                type="email"
                                id="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                placeholder="Enter your email"
                                className="w-full p-3 border border-[#5C7457] rounded bg-white text-[#6a5acd] placeholder-[#6a5acd] focus:outline-none focus:ring-2 focus:ring-[#C1BCAC]"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-[#6a5acd] text-sm mb-2 font-bold" htmlFor="password">Password</label> {/* Violet color for labels */}
                            <input
                                type="password"
                                id="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                placeholder="Enter your password"
                                className="w-full p-3 border border-[#5C7457] rounded bg-white text-[#6a5acd] placeholder-[#6a5acd] focus:outline-none focus:ring-2 focus:ring-[#C1BCAC]"
                                required
                            />
                        </div>
                        <div>
                            <label className="block text-[#6a5acd] text-sm mb-2 font-bold" htmlFor="confirm-password">Confirm Password</label> {/* Violet color for labels */}
                            <input
                                type="password"
                                id="confirm-password"
                                value={confirmPassword}
                                onChange={(e) => setConfirmPassword(e.target.value)}
                                placeholder="Confirm your password"
                                className="w-full p-3 border border-[#5C7457] rounded bg-white text-[#6a5acd] placeholder-[#6a5acd] focus:outline-none focus:ring-2 focus:ring-[#C1BCAC]"
                                required
                            />
                        </div>
                        {error && <p className="text-[#6a5acd] mt-4">{error}</p>} {/* Purple error message */}
                        {successMessage && <p className="text-[#6a5acd] mt-4">{successMessage}</p>} {/* Purple success message */}
                        <button
                            type="submit"
                            className="w-full bg-[#6a5acd] text-white py-3 rounded-lg hover:bg-[#a76fba] transition duration-300 font-semibold" // Adjusted hover color for less transparency
                        >
                            Register
                        </button>
                    </form>
                    <p className="text-sm text-center mt-6 text-[#214E34] font-semibold">
                        Already have an account?
                        <Link to="/seller/login" className="text-[#6a5acd] hover:underline font-bold hover:text-[#8e79c5]">
                             Log in here
                        </Link>
                    </p>
                </div>
            </div>
        </div>
    );
};

export default SellerRegister;