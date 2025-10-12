import React, { useEffect, useState } from 'react';
import { fetchAppointments } from '../api/appointments';
import { Link } from 'react-router-dom';

export default function AppointmentsList() {
    const [appointments, setAppointments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    async function load() {
        setLoading(true);
        setError('');
        try {
            const data = await fetchAppointments();
            setAppointments(data);
        } catch (e) {
            setError('Failed to load appointments');
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => { load(); /* eslint-disable-next-line */ }, []);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 12 }}>
                <h2 style={{ margin: 0 }}>Appointments</h2>
                <Link to="/appointments/add" style={{ padding: '10px 16px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700, textDecoration: 'none' }}>+ Add Appointment</Link>
            </div>
            {loading && <div>Loading appointments…</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}
            {!loading && !error && (
                <div style={{ overflowX: 'auto' }}>
                    <table className="card" style={{ width: '100%', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr style={{ textAlign: 'left' }}>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Patient</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Doctor</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Date</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Time</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {appointments.map((a) => (
                                <tr key={a.id}>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{a.patientName}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{a.doctorName}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{a.appointmentDate?.substring(0,10)}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{a.appointmentTime}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{a.status}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}


