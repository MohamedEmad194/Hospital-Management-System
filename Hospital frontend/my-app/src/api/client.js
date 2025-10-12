import axios from 'axios';

// For development, use HTTP to avoid SSL certificate issues
// In production, this should be HTTPS
const API_BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5230';

// Force the correct URL to prevent any caching issues
console.log('🔧 API_BASE_URL set to:', API_BASE_URL);
console.log('🔧 Environment variables:', {
    REACT_APP_API_BASE_URL: process.env.REACT_APP_API_BASE_URL,
    NODE_ENV: process.env.NODE_ENV
});

// For development, we might need to handle self-signed certificates
if (process.env.NODE_ENV === 'development') {
    // This is handled by the browser's certificate acceptance
    // The user needs to accept the certificate when first visiting the backend URL
}

export const apiClient = axios.create({
    baseURL: `${API_BASE_URL}/api`,
    headers: {
        'Content-Type': 'application/json'
    },
    timeout: 10000, // 10 seconds timeout
    withCredentials: false // Disable credentials for CORS
});

// Debug: Log the API base URL
console.log('🚀 API Base URL:', `${API_BASE_URL}/api`);
console.log('🔗 Full API URL will be:', `${API_BASE_URL}/api/auth/login`);

apiClient.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers = config.headers || {};
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

apiClient.interceptors.response.use(
    (response) => {
        console.log('✅ API Response received:', response.config.url);
        return response;
    },
    (error) => {
        // Log detailed error information for debugging
        console.error('❌ API Error Details:', {
            status: error?.response?.status,
            statusText: error?.response?.statusText,
            data: error?.response?.data,
            message: error?.message,
            url: error?.config?.url,
            baseURL: error?.config?.baseURL,
            fullURL: error?.config?.baseURL + error?.config?.url
        });

        // Check if it's a connection error
        if (error?.message?.includes('ERR_CONNECTION_REFUSED') || 
            error?.message?.includes('Network Error') ||
            error?.code === 'ECONNREFUSED') {
            console.error('🚨 Connection refused! Check if backend is running on:', error?.config?.baseURL);
        }

        if (error?.response?.status === 401) {
            // Token invalid/expired: clear and redirect to login
            localStorage.removeItem('token');
            if (window.location.pathname !== '/login') {
                window.location.replace('/login');
            }
        }
        return Promise.reject(error);
    }
);

export default apiClient;


