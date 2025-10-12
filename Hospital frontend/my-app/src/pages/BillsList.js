import React, { useEffect, useState } from 'react';
import { fetchBills, overdueBills, outstandingAmount } from '../api/bills';
import { Link } from 'react-router-dom';

export default function BillsList() {
    const [bills, setBills] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [summary, setSummary] = useState(null);

    async function load() {
        setLoading(true);
        setError('');
        try {
            const data = await fetchBills();
            const overdue = await overdueBills();
            const outstanding = await outstandingAmount();
            setBills(data);
            setSummary({ overdueCount: overdue.length, totalOutstanding: outstanding.totalOutstandingAmount });
        } catch (e) {
            setError('Failed to load bills');
        } finally {
            setLoading(false);
        }
    }

    useEffect(() => { load(); /* eslint-disable-next-line */ }, []);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 12 }}>
                <h2 style={{ margin: 0 }}>Bills</h2>
                <Link to="/bills/add" style={{ padding: '10px 16px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700, textDecoration: 'none' }}>+ Add Bill</Link>
            </div>
            {summary && (
                <div style={{ display: 'flex', gap: 16, marginBottom: 12 }}>
                    <div style={{ padding: 12, background: '#fff', border: '1px solid #eee', borderRadius: 8 }}>Overdue: <strong>{summary.overdueCount}</strong></div>
                    <div style={{ padding: 12, background: '#fff', border: '1px solid #eee', borderRadius: 8 }}>Outstanding: <strong>{summary.totalOutstanding}</strong></div>
                </div>
            )}
            {loading && <div>Loading bills…</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}
            {!loading && !error && (
                <div style={{ overflowX: 'auto' }}>
                    <table className="card" style={{ width: '100%', borderCollapse: 'collapse' }}>
                        <thead>
                            <tr style={{ textAlign: 'left', background: '#f7f7f7' }}>
                                <th style={{ padding: 12, borderBottom: '1px solid #eee' }}>Patient</th>
                                <th style={{ padding: 12, borderBottom: '1px solid #eee' }}>Total</th>
                                <th style={{ padding: 12, borderBottom: '1px solid #eee' }}>Paid</th>
                                <th style={{ padding: 12, borderBottom: '1px solid #eee' }}>Remaining</th>
                                <th style={{ padding: 12, borderBottom: '1px solid #eee' }}>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {bills.map((b) => (
                                <tr key={b.id}>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{b.patientName}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{b.totalAmount}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{b.paidAmount}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{b.remainingAmount}</td>
                                    <td style={{ padding: 12, borderBottom: '1px solid #f0f0f0' }}>{b.status}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            )}
        </div>
    );
}


