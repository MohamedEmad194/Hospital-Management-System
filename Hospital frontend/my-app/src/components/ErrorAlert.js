import React from 'react';

/**
 * Consistent error display for forms and pages.
 * Usage: <ErrorAlert message={error} hint={hint} onDismiss={() => setError('')} />
 */
export default function ErrorAlert({ message, hint, onDismiss, variant = 'error' }) {
    if (!message) return null;

    const palette = {
        error: { bg: '#fee2e2', border: '#fca5a5', text: '#991b1b', icon: '⚠️' },
        warning: { bg: '#fef3c7', border: '#fcd34d', text: '#92400e', icon: '⚠️' },
        info: { bg: '#dbeafe', border: '#93c5fd', text: '#1e40af', icon: 'ℹ️' },
        success: { bg: '#d1fae5', border: '#6ee7b7', text: '#065f46', icon: '✅' }
    };
    const c = palette[variant] || palette.error;

    return (
        <div
            role={variant === 'error' ? 'alert' : 'status'}
            className="error-alert"
            style={{
                background: c.bg,
                border: `1px solid ${c.border}`,
                color: c.text,
                padding: 14,
                borderRadius: 12,
                marginBottom: 16,
                display: 'flex',
                alignItems: 'flex-start',
                gap: 12
            }}
        >
            <span aria-hidden="true" style={{ fontSize: 20, lineHeight: 1 }}>
                {c.icon}
            </span>
            <div style={{ flex: 1, fontSize: 14, lineHeight: 1.5 }}>
                <div style={{ fontWeight: 600 }}>{message}</div>
                {hint && (
                    <div style={{ marginTop: 6, fontSize: 13, opacity: 0.85 }}>
                        {hint}
                    </div>
                )}
            </div>
            {onDismiss && (
                <button
                    type="button"
                    onClick={onDismiss}
                    aria-label="Dismiss"
                    style={{
                        background: 'transparent',
                        border: 'none',
                        color: c.text,
                        cursor: 'pointer',
                        fontSize: 18,
                        padding: 0,
                        lineHeight: 1
                    }}
                >
                    ×
                </button>
            )}
        </div>
    );
}
