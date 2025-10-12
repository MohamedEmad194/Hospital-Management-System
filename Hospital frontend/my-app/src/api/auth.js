import apiClient from './client';

export async function login(email, password) {
    const fullUrl = apiClient.defaults.baseURL + '/auth/login';
    console.log('🔐 Attempting login to:', fullUrl);
    console.log('📧 Email:', email);
    console.log('🔑 Password length:', password.length);
    
    try {
        const { data } = await apiClient.post('/auth/login', { email, password });
        console.log('✅ Login successful!');
        return data; // { token, expiration, user }
    } catch (error) {
        console.error('❌ Login failed:', error);
        console.error('❌ Error URL:', error.config?.url);
        console.error('❌ Error baseURL:', error.config?.baseURL);
        throw error;
    }
}

export async function register(payload) {
    const { data } = await apiClient.post('/auth/register', payload);
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


