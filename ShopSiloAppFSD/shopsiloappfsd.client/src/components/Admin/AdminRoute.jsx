import React, { useContext } from 'react';
import { Route } from 'react-router-dom';
import { AuthContext } from '../customer/Auth/AuthContext';
const AdminRoute = ({ children, role, ...rest }) => {
    if (role == 'Admin') {
        return children;
    }
};

export default AdminRoute;