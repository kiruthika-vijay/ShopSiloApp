// src/index.js

import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import './index.css';
import { BrowserRouter as Router } from 'react-router-dom';
import { AuthProvider } from './components/customer/Auth/AuthContext.jsx';
import App from './App';

createRoot(document.getElementById('root')).render(
    <StrictMode>
        <Router>
            <AuthProvider>                
                <App />                
            </AuthProvider>
        </Router>
    </StrictMode>
);
