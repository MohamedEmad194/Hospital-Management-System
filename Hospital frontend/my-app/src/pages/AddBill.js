import React, { useEffect, useMemo, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { fetchPatients } from '../api/patients';
import { createBill } from '../api/bills';

export default function AddBill() {
    const navigate = useNavigate();
    const [patients, setPatients] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const [form, setForm] = useState({
        billDate: '',
        dueDate: '',
        notes: '',
        patientId: '',
        insuranceProvider: '',
        insuranceNumber: '',
        insuranceCoverage: ''
    });
    const [items, setItems] = useState([{ description: '', quantity: 1, unitPrice: 0 }]);

    useEffect(() => { (async () => { try { setPatients(await fetchPatients()); } catch {} })(); }, []);

    const canSubmit = useMemo(() => form.billDate && form.dueDate && form.patientId && items.every(i => i.description && i.quantity > 0), [form, items]);
    function update(field, value) { setForm((f) => ({ ...f, [field]: value })); }
    function updateItem(idx, field, value) { setItems((arr) => arr.map((it, i) => i === idx ? { ...it, [field]: value } : it)); }
    function addItem() { setItems((arr) => [...arr, { description: '', quantity: 1, unitPrice: 0 }]); }
    function removeItem(idx) { setItems((arr) => arr.filter((_, i) => i !== idx)); }

    async function handleSubmit(e) {
        e.preventDefault(); if (!canSubmit) return; setLoading(true); setError('');
        try {
            const payload = {
                billDate: new Date(form.billDate).toISOString(),
                dueDate: new Date(form.dueDate).toISOString(),
                notes: form.notes || null,
                insuranceProvider: form.insuranceProvider || null,
                insuranceNumber: form.insuranceNumber || null,
                insuranceCoverage: form.insuranceCoverage ? Number(form.insuranceCoverage) : null,
                patientId: Number(form.patientId),
                billItems: items.map(i => ({ description: i.description, category: null, quantity: Number(i.quantity), unitPrice: Number(i.unitPrice), notes: null }))
            };
            await createBill(payload);
            navigate('/bills');
        } catch (err) { setError(err?.response?.data || 'Failed to create bill'); }
        finally { setLoading(false); }
    }

    return (
        <div style={{ padding: 24 }}>
            <h2 style={{ marginTop: 0, marginBottom: 16 }}>Add Bill</h2>
            <form onSubmit={handleSubmit} className="card" style={{ padding: 20, display: 'grid', gap: 12, maxWidth: 900 }}>
                {error ? <div style={{ color: '#b00020' }}>{String(error)}</div> : null}
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 12 }}>
                    <div>
                        <label>Patient</label>
                        <select value={form.patientId} onChange={(e) => update('patientId', e.target.value)} required>
                            <option value="">Select patient</option>
                            {patients.map((p) => (<option key={p.id} value={p.id}>{p.firstName} {p.lastName}</option>))}
                        </select>
                    </div>
                    <div>
                        <label>Bill Date</label>
                        <input type="date" value={form.billDate} onChange={(e) => update('billDate', e.target.value)} required />
                    </div>
                    <div>
                        <label>Due Date</label>
                        <input type="date" value={form.dueDate} onChange={(e) => update('dueDate', e.target.value)} required />
                    </div>
                    <div>
                        <label>Insurance Provider</label>
                        <input value={form.insuranceProvider} onChange={(e) => update('insuranceProvider', e.target.value)} />
                    </div>
                    <div>
                        <label>Insurance Number</label>
                        <input value={form.insuranceNumber} onChange={(e) => update('insuranceNumber', e.target.value)} />
                    </div>
                    <div>
                        <label>Insurance Coverage</label>
                        <input type="number" min="0" step="0.01" value={form.insuranceCoverage} onChange={(e) => update('insuranceCoverage', e.target.value)} />
                    </div>
                </div>
                <div>
                    <h3 style={{ marginTop: 8 }}>Items</h3>
                    <div style={{ display: 'grid', gap: 8 }}>
                        {items.map((it, idx) => (
                            <div key={idx} style={{ display: 'grid', gridTemplateColumns: '2fr 1fr 1fr auto', gap: 8 }}>
                                <input placeholder="Description" value={it.description} onChange={(e) => updateItem(idx, 'description', e.target.value)} required />
                                <input type="number" min="1" placeholder="Qty" value={it.quantity} onChange={(e) => updateItem(idx, 'quantity', e.target.value)} required />
                                <input type="number" min="0" step="0.01" placeholder="Unit Price" value={it.unitPrice} onChange={(e) => updateItem(idx, 'unitPrice', e.target.value)} required />
                                <button type="button" onClick={() => removeItem(idx)} style={{ borderRadius: 8, border: '1px solid var(--hms-border)', background: 'var(--hms-surface)' }}>Remove</button>
                            </div>
                        ))}
                        <button type="button" onClick={addItem} style={{ width: 'fit-content', padding: '8px 12px', borderRadius: 8, border: '1px solid var(--hms-primary-600)', background: 'transparent', color: 'var(--hms-primary)', fontWeight: 700 }}>+ Add Item</button>
                    </div>
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
                    <button type="submit" disabled={loading || !canSubmit} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700 }}>{loading ? 'Saving…' : 'Save Bill'}</button>
                    <button type="button" onClick={() => navigate(-1)} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-border)', background: 'var(--hms-surface)' }}>Cancel</button>
                </div>
            </form>
        </div>
    );
}


