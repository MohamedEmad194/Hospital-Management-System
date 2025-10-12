import React, { useCallback, useEffect, useState } from 'react';
import { fetchDoctors, searchDoctors } from '../api/doctors';
import { Link } from 'react-router-dom';

export default function DoctorsList() {
    const [doctors, setDoctors] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [query, setQuery] = useState('');

    const load = useCallback(async () => {
        setLoading(true);
        setError('');
        try {
            const data = query ? await searchDoctors(query) : await fetchDoctors();
            setDoctors(data);
        } catch (e) {
            setError('Failed to load doctors');
        } finally {
            setLoading(false);
        }
    }, [query]);

    useEffect(() => { load(); }, [load]);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 16 }}>
                <input
                    placeholder="Search doctors by name, email, license, department"
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    style={{ flex: 1, padding: 12, borderRadius: 10 }}
                />
                <button onClick={load} style={{ padding: '12px 18px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700 }}>Search</button>
                <Link to="/doctors/add" style={{ padding: '12px 18px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'transparent', color: 'var(--hms-primary)', fontWeight: 700, textDecoration: 'none' }}>+ Add</Link>
            </div>

            {loading && <div>Loading doctors…</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}

            {!loading && !error && (
                <div style={{ overflowX: 'auto' }}>
                    <table className="card" style={{ width: '100%', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr style={{ textAlign: 'left' }}>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Name</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Email</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Phone</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Department</th>
                            </tr>
                        </thead>
                        <tbody>
                            {doctors.map((d) => (
                                <tr key={d.id}>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{d.firstName} {d.lastName}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{d.email}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{d.phoneNumber}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{d.departmentName}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}


