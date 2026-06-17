import React, { useContext } from 'react';
import { Navigate, Outlet } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';

const ALLOWED = ['Admin', 'Doctor', 'Staff', 'Nurse'];

export default function StaffRoute() {
    const { isAuthenticated, user } = useContext(AuthContext);
    if (!isAuthenticated) return <Navigate to="/login" replace />;
    if (!user?.roles?.some((r) => ALLOWED.includes(r))) {
        return <Navigate to="/" replace />;
    }
    return <Outlet />;
}
