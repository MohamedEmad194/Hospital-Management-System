import React, { useEffect, useState } from 'react';
import { useTranslation } from 'react-i18next';
import apiClient from '../api/client';

export default function LoginHelp() {
    const { t } = useTranslation();
    const [credentials, setCredentials] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchCredentials = async () => {
            try {
                const response = await apiClient.get('/QuickTest/quick-credentials');
                setCredentials(response.data);
            } catch (error) {
                console.error('Error fetching credentials:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchCredentials();
    }, []);

    if (loading) {
        return <div style={{ padding: 20 }}>Loading...</div>;
    }

    return (
        <div style={{ padding: 20, maxWidth: 800, margin: '0 auto' }}>
            <h2>Login Credentials Help</h2>
            <div style={{ background: '#f5f5f5', padding: 20, borderRadius: 8, marginTop: 20 }}>
                <h3>Quick Login Credentials:</h3>
                {credentials && (
                    <div style={{ marginTop: 15 }}>
                        {credentials.admin && (
                            <div style={{ marginBottom: 15, padding: 15, background: 'white', borderRadius: 8 }}>
                                <h4>Admin:</h4>
                                <p><strong>Email:</strong> {credentials.admin.email}</p>
                                <p><strong>Password:</strong> {credentials.admin.password}</p>
                                <p><strong>Role:</strong> {credentials.admin.role}</p>
                                <p><strong>User Exists:</strong> {credentials.admin.exists ? '✅ Yes' : '❌ No - Call POST /api/TestCredentials/ensure-users'}</p>
                            </div>
                        )}
                        {credentials.doctor && (
                            <div style={{ marginBottom: 15, padding: 15, background: 'white', borderRadius: 8 }}>
                                <h4>Doctor:</h4>
                                <p><strong>Email:</strong> {credentials.doctor.email}</p>
                                <p><strong>Password:</strong> {credentials.doctor.password}</p>
                                <p><strong>Role:</strong> {credentials.doctor.role}</p>
                                <p><strong>Name:</strong> {credentials.doctor.name}</p>
                                <p><strong>User Exists:</strong> {credentials.doctor.exists ? '✅ Yes' : '❌ No - Call POST /api/TestCredentials/ensure-users'}</p>
                            </div>
                        )}
                        {credentials.patient && (
                            <div style={{ marginBottom: 15, padding: 15, background: 'white', borderRadius: 8 }}>
                                <h4>Patient:</h4>
                                <p><strong>Email:</strong> {credentials.patient.email}</p>
                                <p><strong>Password:</strong> {credentials.patient.password}</p>
                                <p><strong>Role:</strong> {credentials.patient.role}</p>
                                <p><strong>Name:</strong> {credentials.patient.name}</p>
                                <p><strong>User Exists:</strong> {credentials.patient.exists ? '✅ Yes' : '❌ No - Call POST /api/TestCredentials/ensure-users'}</p>
                            </div>
                        )}
                        {credentials.staff && (
                            <div style={{ marginBottom: 15, padding: 15, background: 'white', borderRadius: 8 }}>
                                <h4>Staff/Nurse:</h4>
                                <p><strong>Email:</strong> {credentials.staff.email}</p>
                                <p><strong>Password:</strong> {credentials.staff.password}</p>
                                <p><strong>Role:</strong> {credentials.staff.role}</p>
                                <p><strong>Name:</strong> {credentials.staff.name}</p>
                                <p><strong>User Exists:</strong> {credentials.staff.exists ? '✅ Yes' : '❌ No - Call POST /api/TestCredentials/ensure-users'}</p>
                            </div>
                        )}
                    </div>
                )}
                <div style={{ marginTop: 20, padding: 15, background: '#fff3cd', borderRadius: 8 }}>
                    <h4>Important:</h4>
                    <p>If "User Exists" is ❌, you need to call:</p>
                    <code style={{ display: 'block', padding: 10, background: 'white', marginTop: 10 }}>
                        POST http://localhost:5230/api/TestCredentials/ensure-users
                    </code>
                </div>
            </div>
        </div>
    );
}

