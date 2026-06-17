import axios from 'axios';

// For development, use HTTP to avoid SSL certificate issues
// In production, this should be HTTPS
const RAW_API_BASE_URL = (process.env.REACT_APP_API_BASE_URL || process.env.REACT_APP_API_URL || '').trim();

function normalizeBaseUrl(value) {
    // If empty, fallback to localhost default with HTTP to avoid SSL issues
    if (!value) return 'http://localhost:5230';

    let url = value.trim();

    // If someone passed only a port like ":5230" or "5230", treat as localhost
    if (/^:?\d{2,5}$/.test(url)) {
        const port = url.replace(/^:/, '');
        // Use HTTP for all ports in development to avoid SSL issues
        return `http://localhost:${port}`;
    }

    // Auto-prepend protocol if missing - prefer HTTP for development to avoid SSL issues
    if (!/^https?:\/\//i.test(url)) {
        // Use HTTP for all ports in development
        url = `http://${url}`;
    }

    try {
        const u = new URL(url);
        // Remove any trailing slash for consistency
        const host = u.origin.replace(/\/$/, '');
        return host;
    } catch {
        // As a last resort, use HTTP localhost
        return 'http://localhost:5230';
    }
}

export const API_BASE_URL = normalizeBaseUrl(RAW_API_BASE_URL);

// Debug environment + resolved base URL
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
    timeout: 30000, // 30 seconds — remote DB (databaseasp.net) can be slow on cold queries
    withCredentials: false
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
        const url = error?.config?.url || '';
        const isAuthAttempt =
            url.includes('/auth/login') ||
            url.includes('/auth/register');
        const status = error?.response?.status;

        // Failed login/register is expected — don't spam the console
        if (!(isAuthAttempt && (status === 401 || status === 400))) {
            // Flat one-line summary so you can read it without expanding an Object
            const dataPreview = error?.response?.data
                ? (typeof error.response.data === 'string'
                    ? error.response.data.slice(0, 200)
                    : JSON.stringify(error.response.data).slice(0, 200))
                : '(no body)';
            console.error(
                `❌ API ${error?.config?.method?.toUpperCase() || 'REQ'} ${url} → ${status || error?.code || 'ERR'}: ${error?.message} | data: ${dataPreview}`
            );
        }

        if (error?.message?.includes('ERR_CONNECTION_REFUSED') ||
            error?.message?.includes('Network Error') ||
            error?.code === 'ECONNREFUSED') {
            console.error('🚨 Connection refused! Check if backend is running on:', error?.config?.baseURL);
        }

        if (status === 401) {
            if (!isAuthAttempt && window.location.pathname !== '/login') {
                localStorage.removeItem('token');
                setTimeout(() => {
                    if (window.location.pathname !== '/login') {
                        window.location.replace('/login');
                    }
                }, 1000);
            }
        }

        return Promise.reject(error);
    }
);

export default apiClient;


