import React, { useState } from 'react';
import axios from 'axios';
import { IoSend } from "react-icons/io5";

const SubscribeForm = () => {
    const [email, setEmail] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError('');
        setSuccess('');

        if (!email) {
            setError("Please enter a valid email address.");
            return;
        }

        setLoading(true);
        try {
            const response = await axios.post('https://localhost:7002/api/Users/subscribe', { email });
            setSuccess(response.data); // Adjust this if your API response structure is different
            setEmail(''); // Clear the input field
        } catch (error) {
            console.error('Error sending email:', error);
            setError('Failed to send email. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div>
        <form className="flex items-center mt-4 max-w-full" onSubmit={handleSubmit}>
            <div className="relative w-[217px]">
                <input
                    type="email"
                    id="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    placeholder="Enter your email"
                    className="py-2 pl-3 pr-10 w-full rounded border border-neutral-50 bg-transparent text-neutral-50 outline-none"
                    aria-label="Enter your email"
                    required
                />
                <button
                    type="submit"
                    aria-label="Subscribe"
                    disabled={loading}
                    className="absolute right-2 top-1/2 transform -translate-y-1/2 bg-transparent border-none cursor-pointer"
                >
                    {loading ? (
                        <span className="loader">Loading...</span> // You can replace this with a spinner if you prefer
                    ) : (
                        <IoSend className="text-neutral-50" />
                    )}
                </button>
            </div>
        </form>
        { error && <p className="text-red-500 mt-2">{error}</p> }
        { success && <p className="text-green-500 mt-2">{success}</p> }
      </div>
    );
}

export default SubscribeForm;
