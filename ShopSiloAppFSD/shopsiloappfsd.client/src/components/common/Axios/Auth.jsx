// src/auth.js

import axios from 'axios'; // Ensure axios is imported

// Utility function to decode JWT tokens manually
export const decodeToken = (token) => {
    try {
        const payload = token.split('.')[1];
        const base64 = payload.replace(/-/g, '+').replace(/_/g, '/'); // Handle URL-safe base64
        const decodedPayload = atob(base64);
        const jsonPayload = decodeURIComponent(
            decodedPayload
                .split('')
                .map(function (c) {
                    return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
                })
                .join('')
        );
        return JSON.parse(jsonPayload);
    } catch (error) {
        console.error('Failed to decode token:', error);
        return null;
    }
};


// Store token in localStorage
export const storeToken = (token) => {
    localStorage.setItem('authToken', token);
};

// Retrieve token from localStorage
export const getToken = () => {
    return localStorage.getItem('authToken');
};

// Remove token from localStorage
export const removeToken = () => {
    localStorage.removeItem('authToken');
};

// Check if the user is authenticated
export const isAuthenticated = () => {
    const token = getToken();
    if (!token) return false;

    const decoded = decodeToken(token);
    if (!decoded || !decoded.exp) return false;

    if (Date.now() >= decoded.exp * 1000) {
        removeToken();
        return false;
    }
    return true;
};

// Function to decode JWT token and get user ID
export const getEmailFromToken = (token) => {
    const decodedToken = decodeToken(token);
    const email = decodedToken.email; // Adjust according to your JWT claim structure
    return email;
};

export const storeUserId = (id) => {
    localStorage.setItem('userid', id);
};

// Retrieve token from localStorage
export const getUserId = () => {
    return localStorage.getItem('userid');
};

// Remove token from localStorage
export const removeUserId = () => {
    localStorage.removeItem('userid');
};

export const fetchUserId = async () => {
    try {
        const token = getToken();
        const email = getEmailFromToken(token);

        if (!email) {
            console.error("Email not found in token.");
            return;
        }

        console.log("Email: ", email);

        const response = await apiClient.get(`/Users/email/${email}`);
        const userArray = response.data.$values; // Access the array in $values

        if (userArray && userArray.length > 0) {
            const loggedUserId = userArray[0].userID; // Access the first item in the array
            storeUserId(loggedUserId); // Store the userID
            return loggedUserId;
        } else {
            console.error("User not found or empty response.");
        }
    } catch (error) {
        console.error("Error fetching user ID:", error);
    }
};


// Axios instance with JWT token
export const apiClient = axios.create({
    baseURL: 'https://localhost:7002/api', // Replace with your API URL
    headers: {
        'Content-Type': 'application/json',
    },
});

// Axios interceptor to add Authorization header
apiClient.interceptors.request.use(
    (config) => {
        const token = getToken();
        if (token) {
            config.headers['Authorization'] = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        return Promise.reject(error);
    }
);

apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response && error.response.status === 401) {
            // Token might be invalid or expired
            removeToken();

            // Check the current path and redirect accordingly
            const currentPath = window.location.pathname;

            if (currentPath.startsWith('/customer')) {
                window.location.href = '/customer/login'; // Redirect to customer login
            } else if (currentPath.startsWith('/seller')) {
                window.location.href = '/seller/login'; // Redirect to seller login
            } else if (currentPath.startsWith('/admin')) {
                window.location.href = '/admin/login'; // Redirect to seller login
            } 
            else {
                window.location.href = '/customer/home'; // Default login page if no specific path
            }
        }
        return Promise.reject(error);
    }
);
