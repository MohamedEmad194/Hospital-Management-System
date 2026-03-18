import React, { useContext, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';

export default function Register() {
    const { register } = useContext(AuthContext);
    const navigate = useNavigate();

    const [form, setForm] = useState({
        firstName: '',
        lastName: '',
        email: '',
        password: '',
        confirmPassword: ''
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    function handleChange(e) {
        const { name, value } = e.target;
        setForm((prev) => ({ ...prev, [name]: value }));
    }

    function validatePasswordRules(password) {
        const hasMinLength = password.length >= 8;
        const hasUpper = /[A-Z]/.test(password);
        const hasLower = /[a-z]/.test(password);
        const hasDigit = /\d/.test(password);
        return {
            valid: hasMinLength && hasUpper && hasLower && hasDigit,
            hasMinLength,
            hasUpper,
            hasLower,
            hasDigit
        };
    }

    async function handleSubmit(e) {
        e.preventDefault();
        setError('');
        setLoading(true);
        try {
            // Client-side validation to match backend Identity rules
            if (form.password !== form.confirmPassword) {
                throw new Error('Passwords do not match');
            }
            const rules = validatePasswordRules(form.password);
            if (!rules.valid) {
                const unmet = [
                    !rules.hasMinLength ? 'at least 8 characters' : null,
                    !rules.hasUpper ? 'one uppercase letter' : null,
                    !rules.hasLower ? 'one lowercase letter' : null,
                    !rules.hasDigit ? 'one number' : null
                ].filter(Boolean).join(', ');
                throw new Error(`Password must contain ${unmet}.`);
            }

            await register({
                firstName: form.firstName,
                lastName: form.lastName,
                email: form.email,
                password: form.password,
                confirmPassword: form.confirmPassword
            });
            navigate('/');
        } catch (err) {
            let errorMessage = 'فشل التسجيل';
            
            if (err?.response?.data) {
                const responseData = err.response.data;
                
                // Check for email/username already taken
                if (responseData.errors) {
                    const errors = responseData.errors;
                    const errorMessages = [];
                    
                    Object.entries(errors).forEach(([field, messages]) => {
                        const msgArray = Array.isArray(messages) ? messages : [messages];
                        msgArray.forEach(msg => {
                            if (msg.includes('already taken') || msg.includes('موجود')) {
                                errorMessages.push('البريد الإلكتروني مستخدم بالفعل');
                            } else {
                                errorMessages.push(msg);
                            }
                        });
                    });
                    
                    errorMessage = errorMessages[0] || 'خطأ في البيانات المدخلة';
                } 
                // Check for validation errors
                else if (responseData.message) {
                    errorMessage = responseData.message;
                }
                // String response
                else if (typeof responseData === 'string') {
                    errorMessage = responseData;
                }
            } else if (err?.message) {
                errorMessage = err.message.length > 50 ? 'حدث خطأ أثناء التسجيل' : err.message;
            }
            
            setError(errorMessage);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div style={{
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            minHeight: '100vh',
            backgroundImage: `url(${process.env.PUBLIC_URL}/hospital-hero.jpg)`,
            backgroundSize: 'cover',
            backgroundPosition: 'center'
        }}>
            <form onSubmit={handleSubmit} className="card elevate" style={{ width: 420, padding: 28, background: 'rgba(255,255,255,0.92)', backdropFilter: 'blur(2px)' }}>
                <div style={{ display: 'grid', gap: 6, marginBottom: 16 }}>
                    <div style={{ color: 'var(--hms-primary)', fontWeight: 800, fontSize: 14, letterSpacing: 1.5 }}>HMS</div>
                    <h2 style={{ margin: 0, color: 'var(--hms-text)' }}>Patient Registration</h2>
                    <p style={{ color: 'var(--hms-text-dim)', margin: 0 }}>Register as a patient to access your medical records</p>
                </div>
                {error ? (
                    <div style={{ 
                        color: '#c62828', 
                        marginBottom: 16, 
                        padding: 14, 
                        borderRadius: 10, 
                        background: '#ffebee', 
                        border: '2px solid #ef5350',
                        fontSize: 13,
                        lineHeight: 1.5
                    }}>
                        <div style={{ display: 'flex', alignItems: 'flex-start', gap: 10 }}>
                            <span style={{ fontSize: 20 }}>⚠️</span>
                            <div style={{ flex: 1, fontWeight: 500 }}>
                                {String(error)}
                            </div>
                        </div>
                    </div>
                ) : null}
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
                    <div>
                        <label htmlFor="firstName" style={{ display: 'block', marginBottom: 6, color: 'var(--hms-text-dim)' }}>First name</label>
                        <input id="firstName" name="firstName" value={form.firstName} onChange={handleChange} required style={{ width: '100%', padding: 12, borderRadius: 10 }} />
                    </div>
                    <div>
                        <label htmlFor="lastName" style={{ display: 'block', marginBottom: 6, color: 'var(--hms-text-dim)' }}>Last name</label>
                        <input id="lastName" name="lastName" value={form.lastName} onChange={handleChange} required style={{ width: '100%', padding: 12, borderRadius: 10 }} />
                    </div>
                </div>
                <div style={{ marginTop: 12 }}>
                    <label htmlFor="email" style={{ display: 'block', marginBottom: 6, color: 'var(--hms-text-dim)' }}>Email</label>
                    <input id="email" name="email" type="email" value={form.email} onChange={handleChange} required style={{ width: '100%', padding: 12, borderRadius: 10 }} />
                </div>
                <div style={{ marginTop: 12 }}>
                    <label htmlFor="password" style={{ display: 'block', marginBottom: 6, color: 'var(--hms-text-dim)' }}>Password</label>
                    <input id="password" name="password" type="password" value={form.password} onChange={handleChange} required style={{ width: '100%', padding: 12, borderRadius: 10 }} />
                    <div style={{ marginTop: 6, fontSize: 12, color: 'var(--hms-text-dim)' }}>
                        Must be at least 8 characters and include uppercase, lowercase, and a number.
                    </div>
                </div>
                <div style={{ marginTop: 12 }}>
                    <label htmlFor="confirmPassword" style={{ display: 'block', marginBottom: 6, color: 'var(--hms-text-dim)' }}>Confirm password</label>
                    <input id="confirmPassword" name="confirmPassword" type="password" value={form.confirmPassword} onChange={handleChange} required style={{ width: '100%', padding: 12, borderRadius: 10 }} />
                </div>
                <button type="submit" disabled={loading} style={{ marginTop: 16, width: '100%', padding: 12, borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'linear-gradient(135deg, var(--hms-primary) 0%, var(--hms-accent) 100%)', color: '#ffffff', fontWeight: 800 }}>
                    {loading ? 'Creating account…' : 'Sign up'}
                </button>
                <div style={{ marginTop: 12, textAlign: 'center' }}>
                    <span style={{ color: 'var(--hms-text-dim)' }}>Already have an account?</span>{' '}
                    <a href="/login" style={{ color: 'var(--hms-primary)', fontWeight: 700 }}>Sign in</a>
                </div>
            </form>
        </div>
    );
}


