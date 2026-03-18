import axios from 'axios';

// For development, use HTTP to avoid SSL certificate issues
// In production, this should be HTTPS
const RAW_API_BASE_URL = (process.env.REACT_APP_API_BASE_URL || '').trim();

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

const API_BASE_URL = normalizeBaseUrl(RAW_API_BASE_URL);

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
    timeout: 10000, // 10 seconds timeout
    withCredentials: true // Enable credentials for CORS
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
        
        // Also log data as JSON for better readability
        if (error?.response?.data) {
            console.error('❌ Error Data (JSON):', JSON.stringify(error.response.data, null, 2));
        }

        // Check if it's a connection error
        if (error?.message?.includes('ERR_CONNECTION_REFUSED') || 
            error?.message?.includes('Network Error') ||
            error?.code === 'ECONNREFUSED') {
            console.error('🚨 Connection refused! Check if backend is running on:', error?.config?.baseURL);
        }

        if (error?.response?.status === 401) {
            // 401 = Unauthorized (not authenticated or token expired)
            console.warn('🔐 Authentication error (401):', {
                status: error?.response?.status,
                message: error?.response?.data?.message || error?.message,
                path: window.location.pathname
            });
            
            // Don't redirect if already on login page (avoid redirect loop)
            if (window.location.pathname !== '/login') {
                // Clear invalid/expired token
                localStorage.removeItem('token');
                // Show user-friendly message
                console.warn('⚠️ Session expired or invalid. Please login again.');
                // Redirect to login after a short delay to allow user to see the message
                setTimeout(() => {
                    if (window.location.pathname !== '/login') {
                        window.location.replace('/login');
                    }
                }, 1000);
            }
            // If on login page, let the error be handled by the login form
        } else if (error?.response?.status === 403) {
            // 403 = Forbidden (authenticated but insufficient permissions)
            // Don't clear token or redirect - user is logged in but doesn't have permission
            console.warn('🚫 Authorization error (403):', {
                status: error?.response?.status,
                message: error?.response?.data?.message || error?.message,
                details: error?.response?.data?.details,
                path: window.location.pathname
            });
            // Let the component handle the 403 error (show error message to user)
            // Don't redirect or clear token
        }
        return Promise.reject(error);
    }
);

export default apiClient;


