import React, { createContext, useCallback, useEffect, useMemo, useState } from 'react';
import { login as apiLogin, register as apiRegister, getProfile } from '../api/auth';

export const AuthContext = createContext({
    user: null,
    token: null,
    isAuthenticated: false,
    login: async () => {},
    register: async () => {},
    logout: () => {},
});

export function AuthProvider({ children }) {
    const [token, setToken] = useState(() => localStorage.getItem('token'));
    const [user, setUser] = useState(null);
    const isAuthenticated = !!token;

    const logout = useCallback(() => {
        localStorage.removeItem('token');
        setToken(null);
        setUser(null);
    }, []);

    const login = useCallback(async (email, password) => {
        const data = await apiLogin(email, password);
        localStorage.setItem('token', data.token);
        setToken(data.token);
        setUser(data.user);
        return data;
    }, []);

    const register = useCallback(async (payload) => {
        const data = await apiRegister(payload);
        localStorage.setItem('token', data.token);
        setToken(data.token);
        setUser(data.user);
        return data;
    }, []);

    useEffect(() => {
        if (!token) return;
        let cancelled = false;
        (async () => {
            try {
                const me = await getProfile();
                if (!cancelled) setUser(me);
            } catch {
                logout();
            }
        })();
        return () => { cancelled = true; };
    }, [token, logout]);

    const value = useMemo(() => ({ user, token, isAuthenticated, login, register, logout }), [user, token, isAuthenticated, login, register, logout]);

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}


