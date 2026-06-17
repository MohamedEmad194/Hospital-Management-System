import React, { useContext, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import { useTranslation } from 'react-i18next';
import { forgotPassword } from '../api/auth';

export default function Login() {
    const { t } = useTranslation();
    const { login } = useContext(AuthContext);
    const navigate = useNavigate();
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [showReset, setShowReset] = useState(false);
    const [resetEmail, setResetEmail] = useState('');
    const [resetLoading, setResetLoading] = useState(false);
    const [resetStatus, setResetStatus] = useState({ type: '', message: '' });

    async function handleSubmit(e) {
        e.preventDefault();
        setError('');
        setLoading(true);
        try {
            await login(email, password);
            navigate('/');
        } catch (err) {
            // Simplified error handling with short messages
            let errorMessage = t('login.errors.invalidCredentials');
            let errorHint = '';

            // Network errors
            if (err?.message?.includes('Network Error') || err?.code === 'ECONNREFUSED') {
                errorMessage = t('login.errors.serverConnection');
                setError(errorMessage);
                return;
            }

            // SSL errors
            if (err?.message?.includes('ERR_CERT_AUTHORITY_INVALID')) {
                errorMessage = t('login.errors.sslCertificate');
                setError(errorMessage);
                return;
            }

            // API response errors
            if (err?.response?.data) {
                const responseData = err.response.data;
                const details = responseData.details || '';

                if (details.includes('Incorrect email or password') ||
                    details.includes('Incorrect password')) {
                    errorMessage = t('login.errors.invalidCredentials');
                } else if (details.includes('Account not found') ||
                           details.includes('not provisioned')) {
                    errorMessage = t('login.errors.accountNotFound');
                    errorHint = t('login.errors.accountHint');
                } else if (responseData.message?.includes('Invalid email or password')) {
                    errorMessage = t('login.errors.invalidCredentials');
                } else if (responseData.message) {
                    errorMessage = responseData.message;
                    if (details) errorHint = details;
                } else if (responseData.title && responseData.errors) {
                    const first = Object.values(responseData.errors).flat()[0];
                    errorMessage = first || responseData.title;
                }
            }
            // Generic error
            else if (err?.message) {
                errorMessage = err.message.length > 50 ? t('login.errors.generic') : err.message;
            }

            // Set error with hint if available
            setError(errorHint ? `${errorMessage}\n\n💡 ${errorHint}` : errorMessage);
        } finally {
            setLoading(false);
        }
    }

    async function handleForgotPassword(e) {
        e.preventDefault();
        setResetStatus({ type: '', message: '' });
        setResetLoading(true);

        try {
            const targetEmail = resetEmail || email;
            if (!targetEmail) {
                setResetStatus({ type: 'error', message: t('login.passwordReset.emptyEmail') });
                return;
            }

            await forgotPassword(targetEmail);
            setResetStatus({ type: 'success', message: t('login.passwordReset.sent') });
        } catch (err) {
            console.error('Forgot password error:', err);
            setResetStatus({ type: 'error', message: t('login.passwordReset.failed') });
        } finally {
            setResetLoading(false);
        }
    }

    return (
        <div style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            minHeight: '100vh',
            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 50%, #f093fb 100%)',
            position: 'relative',
            padding: '20px',
            overflow: 'hidden'
        }}>
            <div style={{
                position: 'absolute',
                top: '-50%',
                right: '-50%',
                width: '800px',
                height: '800px',
                background: 'radial-gradient(circle, rgba(255,255,255,0.1) 0%, transparent 70%)',
                borderRadius: '50%',
                zIndex: 0
            }} />
            <div style={{
                position: 'absolute',
                bottom: '-30%',
                left: '-30%',
                width: '600px',
                height: '600px',
                background: 'radial-gradient(circle, rgba(255,255,255,0.08) 0%, transparent 70%)',
                borderRadius: '50%',
                zIndex: 0
            }} />
            
            <form 
                onSubmit={handleSubmit} 
                className="card elevate fade-in" 
                style={{ 
                    width: '100%',
                    maxWidth: 420, 
                    padding: '40px 32px',
                    background: 'rgba(255, 255, 255, 0.98)', 
                    backdropFilter: 'blur(10px)',
                    borderRadius: '28px',
                    boxShadow: '0 20px 60px rgba(0,0,0,0.2), 0 0 0 1px rgba(255,255,255,0.3)',
                    border: '1px solid rgba(255,255,255,0.5)',
                    position: 'relative',
                    zIndex: 1
                }}
            >
                {/* Logo/Brand Section */}
                <div style={{ 
                    display: 'flex', 
                    flexDirection: 'column',
                    alignItems: 'center',
                    gap: 16, 
                    marginBottom: 32,
                    textAlign: 'center'
                }}>
                    <div style={{
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center',
                        marginBottom: 8,
                        background: 'transparent',
                        position: 'relative'
                    }}>
                        <div style={{
                            background: 'white',
                            display: 'inline-block',
                            position: 'relative'
                    }}>
                        <img 
                            src={`${process.env.PUBLIC_URL}/hospital-logo.png`}
                                alt="AL HAYAT Hospital Management System Logo"
                            style={{
                                    maxWidth: '380px',
                                    width: 'auto',
                                    height: 'auto',
                                    maxHeight: '280px',
                                objectFit: 'contain',
                                display: 'block',
                                    mixBlendMode: 'multiply',
                                    filter: 'drop-shadow(0 8px 20px rgba(102, 126, 234, 0.3))'
                            }}
                            onError={(e) => {
                                // Fallback to logo.png if hospital-logo.png doesn't exist
                                e.target.src = `${process.env.PUBLIC_URL}/logo.png`;
                            }}
                        />
                        </div>
                    </div>
                    <div style={{ 
                        display: 'flex',
                        flexDirection: 'column',
                        gap: 4,
                        alignItems: 'center'
                    }}>
                        <h1 style={{ 
                            margin: 0, 
                            color: '#1a365d',
                            fontSize: '32px',
                            fontWeight: 800,
                            letterSpacing: '-0.5px',
                            fontFamily: 'sans-serif'
                        }}>
                            AL HAYAT
                        </h1>
                        <p style={{ 
                            margin: 0,
                            color: '#4a5568',
                            fontSize: '14px',
                            fontWeight: 500,
                            letterSpacing: '0.5px'
                    }}>
                            Hospital Management System
                        </p>
                    </div>
                    <h2 style={{ 
                        margin: 0, 
                        marginTop: 8,
                        background: 'linear-gradient(135deg, #1a202c 0%, #2d3748 100%)',
                        WebkitBackgroundClip: 'text',
                        WebkitTextFillColor: 'transparent',
                        backgroundClip: 'text',
                        fontSize: '24px',
                        fontWeight: 700,
                        letterSpacing: '-0.5px'
                    }}>
                        {t('login.welcome')}
                    </h2>
                    <p style={{ 
                        color: '#718096', 
                        margin: 0,
                        fontSize: '14px',
                        lineHeight: 1.6
                    }}>
                        {t('login.subtitle')}
                    </p>
                </div>
                {error ? (
                    <div style={{ 
                        color: '#c62828', 
                        marginBottom: 20, 
                        padding: 16, 
                        borderRadius: 14, 
                        background: 'linear-gradient(135deg, #ffebee 0%, #fce4ec 100%)', 
                        border: '2px solid #ef5350',
                        fontSize: 13,
                        boxShadow: '0 4px 12px rgba(239, 83, 80, 0.2)',
                        animation: 'slideUp 0.3s ease-out'
                    }}>
                        <div style={{ 
                            display: 'flex', 
                            alignItems: 'flex-start', 
                            gap: 12,
                            marginBottom: error.includes('💡') ? 12 : 0
                        }}>
                            <span style={{ fontSize: 24, lineHeight: 1 }}>⚠️</span>
                            <div style={{ flex: 1 }}>
                                <div style={{ 
                                    fontWeight: 700, 
                                    marginBottom: error.includes('💡') ? 10 : 0,
                                    fontSize: 14,
                                    lineHeight: 1.6,
                                    color: '#b71c1c'
                                }}>
                                    {error.split('\n\n')[0]}
                                </div>
                                {error.includes('💡') && (
                                    <div style={{ 
                                        marginTop: 12,
                                        padding: 12,
                                        background: '#fff8e1',
                                        borderRadius: 8,
                                        border: '1px solid #ffc107',
                                        fontSize: 12,
                                        color: '#856404',
                                        lineHeight: 1.6,
                                        boxShadow: '0 2px 8px rgba(255, 193, 7, 0.15)'
                                    }}>
                                        <span style={{ fontSize: 16, marginLeft: 4 }}>💡</span> {error.split('💡')[1]?.trim()}
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                ) : null}
                <div style={{ marginBottom: 16 }}>
                    <label htmlFor="email" style={{
                        display: 'block', 
                        marginBottom: 8, 
                        color: '#4a5568',
                        fontSize: '13px',
                        fontWeight: 600
                    }}>
                        {t('login.email')}
                    </label>
                    <input 
                        id="email" 
                        type="email" 
                        value={email} 
                        onChange={(e) => setEmail(e.target.value)} 
                        required 
                        placeholder="example@hospital.com"
                        style={{ 
                            width: '100%', 
                            padding: '12px 16px', 
                            borderRadius: 14,
                            border: '2px solid #e2e8f0',
                            background: 'white',
                            fontSize: 14,
                            color: '#2d3748',
                            transition: 'all 0.2s ease',
                            outline: 'none',
                            boxSizing: 'border-box',
                            boxShadow: '0 2px 4px rgba(0,0,0,0.02)'
                        }}
                        onFocus={(e) => {
                            e.target.style.borderColor = '#667eea';
                            e.target.style.boxShadow = '0 0 0 3px rgba(102, 126, 234, 0.1), 0 2px 8px rgba(102, 126, 234, 0.15)';
                        }}
                        onBlur={(e) => {
                            e.target.style.borderColor = '#e2e8f0';
                            e.target.style.boxShadow = '0 2px 4px rgba(0,0,0,0.02)';
                        }}
                    />
                </div>
                <div style={{ marginBottom: 18 }}>
                    <label htmlFor="password" style={{ 
                        display: 'block', 
                        marginBottom: 8, 
                        color: '#4a5568',
                        fontSize: '13px',
                        fontWeight: 600
                    }}>
                        {t('login.password')}
                    </label>
                    <input 
                        id="password" 
                        type="password" 
                        value={password} 
                        onChange={(e) => setPassword(e.target.value)} 
                        required 
                        placeholder="••••••••"
                        style={{ 
                            width: '100%', 
                            padding: '12px 16px', 
                            borderRadius: 14,
                            border: '2px solid #e2e8f0',
                            background: 'white',
                            fontSize: 14,
                            color: '#2d3748',
                            transition: 'all 0.2s ease',
                            outline: 'none',
                            boxSizing: 'border-box',
                            boxShadow: '0 2px 4px rgba(0,0,0,0.02)'
                        }}
                        onFocus={(e) => {
                            e.target.style.borderColor = '#667eea';
                            e.target.style.boxShadow = '0 0 0 3px rgba(102, 126, 234, 0.1), 0 2px 8px rgba(102, 126, 234, 0.15)';
                        }}
                        onBlur={(e) => {
                            e.target.style.borderColor = '#e2e8f0';
                            e.target.style.boxShadow = '0 2px 4px rgba(0,0,0,0.02)';
                        }}
                    />
                    <div style={{ 
                        display: 'flex', 
                        justifyContent: 'space-between', 
                        alignItems: 'center', 
                        marginTop: 10, 
                        fontSize: 13
                    }}>
                        <button
                            type="button"
                            onClick={() => { setShowReset(!showReset); setResetEmail(email); }}
                            style={{ 
                                background: 'transparent', 
                                border: 'none', 
                                color: '#667eea', 
                                cursor: 'pointer', 
                                padding: '4px 0',
                                fontWeight: 600,
                                fontSize: 13,
                                transition: 'color 0.2s ease',
                                textDecoration: 'none'
                            }}
                            onMouseEnter={(e) => e.target.style.color = '#764ba2'}
                            onMouseLeave={(e) => e.target.style.color = '#667eea'}
                        >
                            {t('login.passwordReset.forgot')}
                        </button>
                        {resetStatus.message && (
                            <span style={{ 
                                color: resetStatus.type === 'success' ? '#48bb78' : '#f56565',
                                fontSize: 12,
                                fontWeight: 500
                            }}>
                                {resetStatus.message}
                            </span>
                        )}
                    </div>
                </div>
                {showReset && (
                    <div style={{ 
                        marginBottom: 20, 
                        padding: 20, 
                        borderRadius: 14, 
                        background: 'linear-gradient(135deg, #f6f7fb 0%, #eef2ff 100%)', 
                        border: '2px solid #e0e3f0',
                        boxShadow: '0 4px 12px rgba(0,0,0,0.05)',
                        animation: 'slideUp 0.3s ease-out'
                    }}>
                        <div style={{ 
                            fontWeight: 700, 
                            marginBottom: 12, 
                            color: '#667eea',
                            fontSize: 16
                        }}>
                            {t('login.passwordReset.title')}
                        </div>
                        <div style={{ display: 'grid', gap: 12 }}>
                            <input
                                type="email"
                                placeholder={t('login.passwordReset.placeholder')}
                                value={resetEmail || email}
                                onChange={(e) => setResetEmail(e.target.value)}
                                style={{ 
                                    width: '100%', 
                                    padding: '12px 14px', 
                                    borderRadius: 10, 
                                    border: '2px solid #e2e8f0',
                                    background: 'white',
                                    fontSize: 14,
                                    outline: 'none',
                                    transition: 'all 0.2s ease',
                                    boxSizing: 'border-box'
                                }}
                                onFocus={(e) => {
                                    e.target.style.borderColor = '#667eea';
                                    e.target.style.boxShadow = '0 0 0 3px rgba(102, 126, 234, 0.1)';
                                }}
                                onBlur={(e) => {
                                    e.target.style.borderColor = '#e2e8f0';
                                    e.target.style.boxShadow = 'none';
                                }}
                            />
                            <button
                                type="button"
                                onClick={handleForgotPassword}
                                disabled={resetLoading}
                                style={{ 
                                    padding: '12px 20px', 
                                    borderRadius: 10, 
                                    border: 'none',
                                    background: resetLoading ? '#a0aec0' : 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                    color: '#fff', 
                                    fontWeight: 700,
                                    fontSize: 14,
                                    cursor: resetLoading ? 'not-allowed' : 'pointer',
                                    transition: 'all 0.2s ease',
                                    boxShadow: resetLoading ? 'none' : '0 4px 12px rgba(102, 126, 234, 0.3)'
                                }}
                                onMouseEnter={(e) => {
                                    if (!resetLoading) {
                                        e.target.style.transform = 'translateY(-2px)';
                                        e.target.style.boxShadow = '0 6px 16px rgba(102, 126, 234, 0.4)';
                                    }
                                }}
                                onMouseLeave={(e) => {
                                    if (!resetLoading) {
                                        e.target.style.transform = 'translateY(0)';
                                        e.target.style.boxShadow = '0 4px 12px rgba(102, 126, 234, 0.3)';
                                    }
                                }}
                            >
                                {resetLoading ? t('login.passwordReset.sending') : t('login.passwordReset.send')}
                            </button>
                        </div>
                    </div>
                )}
                <div style={{ display: 'flex', gap: 10, marginTop: 6 }}>
                    <button 
                        type="submit" 
                        disabled={loading} 
                        style={{ 
                            flex: 1, 
                            padding: '14px 20px', 
                            borderRadius: 14, 
                            border: 'none',
                            background: loading 
                                ? 'linear-gradient(135deg, #a0aec0 0%, #718096 100%)' 
                                : 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', 
                            color: '#ffffff', 
                            fontWeight: 700,
                            fontSize: 14,
                            cursor: loading ? 'not-allowed' : 'pointer',
                            transition: 'all 0.3s ease',
                            boxShadow: loading 
                                ? 'none' 
                                : '0 6px 20px rgba(102, 126, 234, 0.35)',
                            position: 'relative',
                            overflow: 'hidden'
                        }}
                        onMouseEnter={(e) => {
                            if (!loading) {
                                e.target.style.transform = 'translateY(-2px)';
                                e.target.style.boxShadow = '0 10px 25px rgba(102, 126, 234, 0.45)';
                            }
                        }}
                        onMouseLeave={(e) => {
                            if (!loading) {
                                e.target.style.transform = 'translateY(0)';
                                e.target.style.boxShadow = '0 6px 20px rgba(102, 126, 234, 0.35)';
                            }
                        }}
                    >
                        {loading ? (
                            <span style={{ display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 8 }}>
                                <span className="loading-spinner" style={{ width: 16, height: 16, borderWidth: 2 }} />
                                {t('login.signingIn')}
                            </span>
                        ) : (
                            t('login.signIn')
                        )}
                    </button>
                    <button 
                        type="button" 
                        onClick={() => navigate('/register')} 
                        style={{ 
                            flex: 1, 
                            padding: '14px 20px', 
                            borderRadius: 14, 
                            border: '2px solid #667eea', 
                            background: 'transparent', 
                            color: '#667eea', 
                            fontWeight: 700,
                            fontSize: 14,
                            cursor: 'pointer',
                            transition: 'all 0.3s ease',
                            boxShadow: '0 2px 8px rgba(102, 126, 234, 0.1)'
                        }}
                        onMouseEnter={(e) => {
                            e.target.style.background = 'rgba(102, 126, 234, 0.08)';
                            e.target.style.transform = 'translateY(-2px)';
                            e.target.style.boxShadow = '0 4px 12px rgba(102, 126, 234, 0.2)';
                        }}
                        onMouseLeave={(e) => {
                            e.target.style.background = 'transparent';
                            e.target.style.transform = 'translateY(0)';
                            e.target.style.boxShadow = '0 2px 8px rgba(102, 126, 234, 0.1)';
                        }}
                    >
                        {t('login.createAccount')}
                    </button>
                </div>
            </form>
            
            <style>{`
                @keyframes pulse {
                    0%, 100% { opacity: 1; }
                    50% { opacity: 0.8; }
                }
                @keyframes slideUp {
                    from {
                        opacity: 0;
                        transform: translateY(10px);
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0);
                    }
                }
                img[alt="AL HAYAT Hospital Management System Logo"] {
                    mix-blend-mode: multiply !important;
                }
            `}</style>
        </div>
    );
}


