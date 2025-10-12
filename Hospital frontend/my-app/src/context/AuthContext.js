import React, { createContext, useCallback, useEffect, useMemo, useState } from 'react';
import { login as apiLogin, getProfile } from '../api/auth';

export const AuthContext = createContext({
    user: null,
    token: null,
    isAuthenticated: false,
    login: async () => {},
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

    const value = useMemo(() => ({ user, token, isAuthenticated, login, logout }), [user, token, isAuthenticated, login, logout]);

    return (
        <AuthContext.Provider value={value}>
            {children}
        </AuthContext.Provider>
    );
}


