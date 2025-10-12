import React, { useEffect, useState } from 'react';
import { fetchDepartments } from '../api/departments';
import { Link } from 'react-router-dom';

export default function DepartmentsList() {
    const [departments, setDepartments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    async function load() {
        setLoading(true);
        setError('');
        try {
            const data = await fetchDepartments();
            setDepartments(data);
        } catch (e) {
            setError('Failed to load departments');
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => { load(); /* eslint-disable-next-line */ }, []);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 12 }}>
                <h2 style={{ margin: 0 }}>Departments</h2>
                <Link to="/departments/add" style={{ padding: '10px 16px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700, textDecoration: 'none' }}>+ Add Department</Link>
            </div>
            {loading && <div>Loading departments…</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}
            {!loading && !error && (
                <div style={{ overflowX: 'auto' }}>
                    <table className="card" style={{ width: '100%', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr style={{ textAlign: 'left' }}>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Name</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Description</th>
                            </tr>
                        </thead>
                        <tbody>
                            {departments.map((d) => (
                                <tr key={d.id}>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{d.name}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{d.description}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}


