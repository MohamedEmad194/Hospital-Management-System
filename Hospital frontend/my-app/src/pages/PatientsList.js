import React, { useCallback, useEffect, useState } from 'react';
import { fetchPatients, searchPatients } from '../api/patients';
import { Link } from 'react-router-dom';

export default function PatientsList() {
    const [patients, setPatients] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [query, setQuery] = useState('');

    const load = useCallback(async () => {
        setLoading(true);
        setError('');
        try {
            const data = query ? await searchPatients(query) : await fetchPatients();
            setPatients(data);
        } catch (e) {
            setError('Failed to load patients');
        } finally {
            setLoading(false);
        }
    }, [query]);

    useEffect(() => { load(); }, [load]);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 16 }}>
                <input
                    placeholder="Search patients by name, email, phone, national id"
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    style={{ flex: 1, padding: 12, borderRadius: 10 }}
                />
                <button onClick={load} style={{ padding: '12px 18px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'linear-gradient(135deg, var(--hms-primary) 0%, var(--hms-accent) 100%)', color: '#07111d', fontWeight: 800 }}>Search</button>
                <Link to="/patients/add" style={{ padding: '12px 18px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'transparent', color: 'var(--hms-primary)', fontWeight: 700, textDecoration: 'none' }}>+ Add</Link>
            </div>

            {loading && <div>Loading patients…</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}

            {!loading && !error && (
                <div style={{ overflowX: 'auto' }}>
                    <table className="card" style={{ width: '100%', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr style={{ textAlign: 'left' }}>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Name</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Email</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Phone</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>National ID</th>
                            </tr>
                        </thead>
                        <tbody>
                            {patients.map((p) => (
                                <tr key={p.id}>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{p.firstName} {p.lastName}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{p.email}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{p.phoneNumber}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{p.nationalId}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}


