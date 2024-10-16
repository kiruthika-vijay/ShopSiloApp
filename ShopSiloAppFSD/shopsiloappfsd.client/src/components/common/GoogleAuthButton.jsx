import React from 'react';
import { GoogleOAuthProvider, GoogleLogin } from '@react-oauth/google';
import { apiClient, storeToken } from './Axios/auth'; // Assuming your Axios configuration is in this file
import { useNavigate } from 'react-router-dom';

const GoogleAuthButton = () => {
    const navigate = useNavigate();
    const clientId = import.meta.env.VITE_GOOGLE_CLIENT_ID;
    const handleGoogleLoginSuccess = async (response) => {
        const idToken = response.credential;

        try {
            const res = await apiClient.post('/Aauth/login/google', { idToken });
            const { token } = res.data;
            storeToken(token);
            navigate('/customer/home'); // Use navigate instead of window.location.href
        } catch (error) {
            console.error('Google login error:', error.response?.data || error.message);
        }
    };


    const handleGoogleLoginFailure = (error) => {
        console.error('Google login failed', error);
    };

    return (
        <div>
            {/* Google Login Button */}
            <GoogleOAuthProvider clientId={clientId}>
                <GoogleLogin
                    onSuccess={handleGoogleLoginSuccess}
                    onError={handleGoogleLoginFailure}
                />
            </GoogleOAuthProvider>
        </div>
    );
};

export default GoogleAuthButton;
