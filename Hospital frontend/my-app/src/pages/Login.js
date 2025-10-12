import React, { useContext, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';

export default function Login() {
    const { login } = useContext(AuthContext);
    const navigate = useNavigate();
    const [email, setEmail] = useState('admin@hospital.com');
    const [password, setPassword] = useState('Admin@123');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    async function handleSubmit(e) {
        e.preventDefault();
        setError('');
        setLoading(true);
        try {
            await login(email, password);
            navigate('/');
        } catch (err) {
            console.error('Login error:', err);
            
            // Extract meaningful error message
            let errorMessage = 'Login failed';
            
            if (err?.response?.data) {
                if (typeof err.response.data === 'string') {
                    errorMessage = err.response.data;
                } else if (err.response.data.message) {
                    errorMessage = err.response.data.message;
                } else if (err.response.data.errors) {
                    // Handle validation errors
                    const errors = err.response.data.errors;
                    errorMessage = Object.values(errors).flat().join(', ');
                }
            } else if (err?.message) {
                if (err.message.includes('Network Error')) {
                    errorMessage = 'Cannot connect to server. Please check if the backend is running.';
                } else if (err.message.includes('ERR_CERT_AUTHORITY_INVALID')) {
                    errorMessage = 'SSL certificate error. Please accept the certificate in your browser.';
                } else {
                    errorMessage = err.message;
                }
            }
            
            setError(errorMessage);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', minHeight: '100vh' }}>
            <form onSubmit={handleSubmit} className="card elevate" style={{ width: 380, padding: 28, background: 'var(--hms-surface)' }}>
                <div style={{ display: 'grid', gap: 6, marginBottom: 16 }}>
                    <div style={{ color: 'var(--hms-primary)', fontWeight: 800, fontSize: 14, letterSpacing: 1.5 }}>HMS</div>
                    <h2 style={{ margin: 0, color: 'var(--hms-text)' }}>Welcome back</h2>
                    <p style={{ color: 'var(--hms-text-dim)', margin: 0 }}>Sign in to continue</p>
                </div>
                {error ? <div style={{ color: '#ff7a7a', marginBottom: 12 }}>{String(error)}</div> : null}
                <div style={{ marginBottom: 12 }}>
                    <label htmlFor="email" style={{ display: 'block', marginBottom: 6, color: 'var(--hms-text-dim)' }}>Email</label>
                    <input id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} required style={{ width: '100%', padding: 12, borderRadius: 10 }} />
                </div>
                <div style={{ marginBottom: 16 }}>
                    <label htmlFor="password" style={{ display: 'block', marginBottom: 6, color: 'var(--hms-text-dim)' }}>Password</label>
                    <input id="password" type="password" value={password} onChange={(e) => setPassword(e.target.value)} required style={{ width: '100%', padding: 12, borderRadius: 10 }} />
                </div>
                <button type="submit" disabled={loading} style={{ width: '100%', padding: 12, borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'linear-gradient(135deg, var(--hms-primary) 0%, var(--hms-accent) 100%)', color: '#ffffff', fontWeight: 800 }}>
                    {loading ? 'Signing in…' : 'Sign In'}
                </button>
            </form>
        </div>
    );
}


