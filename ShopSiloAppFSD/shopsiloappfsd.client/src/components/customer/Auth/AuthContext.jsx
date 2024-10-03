// src/context/AuthContext.js
import React, { createContext, useState, useEffect } from 'react';
import { getToken, isAuthenticated } from '../../common/Axios/auth';

export const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isLoggedIn, setIsLoggedIn] = useState(isAuthenticated());

    useEffect(() => {
        const handleStorageChange = () => {
            setIsLoggedIn(isAuthenticated());
        };

        window.addEventListener('storage', handleStorageChange);

        return () => {
            window.removeEventListener('storage', handleStorageChange);
        };
    }, []);

    return (
        <AuthContext.Provider value={{ isLoggedIn, setIsLoggedIn }}>
            {children}
        </AuthContext.Provider>
    );
};
