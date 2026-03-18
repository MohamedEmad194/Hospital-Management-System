import React, { useContext } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';

export default function AdminRoute() {
    const { isAuthenticated, user } = useContext(AuthContext);
    if (!isAuthenticated) return <Navigate to="/login" replace />;
    if (!user?.roles?.includes('Admin') && !user?.roles?.includes('Doctor')) {
        return <Navigate to="/" replace />;
    }
    return <Outlet />;
}

