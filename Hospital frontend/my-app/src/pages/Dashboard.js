import React, { useEffect, useState } from 'react';
import apiClient from '../api/client';

export default function Dashboard() {
    const [stats, setStats] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        let cancelled = false;
        (async () => {
            setLoading(true);
            try {
                const { data } = await apiClient.get('/Dashboard/stats');
                if (!cancelled) setStats(data);
            } catch (err) {
                if (!cancelled) setError('Failed to load stats');
            } finally {
                if (!cancelled) setLoading(false);
            }
        })();
        return () => { cancelled = true; };
    }, []);

    if (loading) return <div style={{ padding: 24 }}>Loading dashboard…</div>;
    if (error) return <div style={{ padding: 24, color: '#b00020' }}>{error}</div>;
    if (!stats) return null;

    const items = [
        { label: 'Patients', value: stats.totalPatients },
        { label: 'Doctors', value: stats.totalDoctors },
        { label: 'Appointments', value: stats.totalAppointments },
        { label: 'Departments', value: stats.totalDepartments },
        { label: 'Rooms', value: stats.totalRooms },
        { label: 'Medicines', value: stats.totalMedicines },
        { label: 'Bills', value: stats.totalBills },
        { label: 'Pending Appointments', value: stats.pendingAppointments },
        { label: 'Completed Appointments', value: stats.completedAppointments },
        { label: 'Overdue Bills', value: stats.overdueBills },
        { label: 'Low Stock Medicines', value: stats.lowStockMedicines },
    ];

    return (
        <div style={{ padding: 24 }}>
            <h2 style={{ marginTop: 0, marginBottom: 16 }}>Dashboard</h2>
            <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(220px, 1fr))', gap: 16 }}>
                {items.map((item) => (
                    <div key={item.label} className="card elevate" style={{ padding: 18 }}>
                        <div style={{ color: 'var(--hms-text-dim)', fontSize: 14 }}>{item.label}</div>
                        <div style={{ fontSize: 32, fontWeight: 800, color: 'var(--hms-primary)' }}>{item.value}</div>
                    </div>
                ))}
            </div>
        </div>
    );
}


