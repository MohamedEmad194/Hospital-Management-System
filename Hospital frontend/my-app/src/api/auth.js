import apiClient from './client';

export async function login(email, password, role) {
    try {
        const { data } = await apiClient.post('/auth/login', { email, password, role });
        return data; // { token, expiration, user }
    } catch (error) {
        // Error is already logged by client.js interceptor
        throw error;
    }
}

export async function register(payload) {
    const { data } = await apiClient.post('/auth/register', payload);
    return data;
}

export async function forgotPassword(email) {
    const { data } = await apiClient.post('/auth/forgot-password', { email });
    return data;
}

export async function getProfile() {
    const { data } = await apiClient.get('/auth/profile');
    return data;
}

export async function updateProfile(payload) {
    const { data } = await apiClient.put('/auth/profile', payload);
    return data;
}


