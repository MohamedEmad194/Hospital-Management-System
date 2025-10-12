import React, { useEffect, useState } from 'react';
import apiClient from '../api/client';
import { Link } from 'react-router-dom';

export default function MedicinesList() {
    const [medicines, setMedicines] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    async function load() {
        setLoading(true);
        setError('');
        try {
            const { data } = await apiClient.get('/Medicines');
            setMedicines(data);
        } catch (e) {
            setError('Failed to load medicines');
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => { load(); /* eslint-disable-next-line */ }, []);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 12 }}>
                <h2 style={{ margin: 0 }}>Medicines</h2>
                <Link to="/medicines/add" style={{ padding: '10px 16px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700, textDecoration: 'none' }}>+ Add Medicine</Link>
            </div>
            {loading && <div>Loading medicines…</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}
            {!loading && !error && (
                <div style={{ overflowX: 'auto' }}>
                    <table className="card" style={{ width: '100%', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr style={{ textAlign: 'left' }}>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Name</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Stock</th>
                                <th style={{ padding: 12, borderBottom: '1px solid var(--hms-border)', color: 'var(--hms-text-dim)' }}>Minimum Stock</th>
                            </tr>
                        </thead>
                        <tbody>
                            {medicines.map((m) => (
                                <tr key={m.id}>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{m.name}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{m.stockQuantity}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid var(--hms-border)' }}>{m.minimumStockLevel}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}


