import React, { useMemo, useState } from 'react';
import { createMedicine } from '../api/medicines';
import { useNavigate } from 'react-router-dom';

export default function AddMedicine() {
    const navigate = useNavigate();
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const [form, setForm] = useState({
        name: '',
        genericName: '',
        dosageForm: '',
        strength: '',
        manufacturer: '',
        price: '',
        stockQuantity: '',
        minimumStockLevel: '',
        unit: '',
        expiryDate: '',
        batchNumber: '',
        requiresPrescription: true
    });

    const canSubmit = useMemo(() => {
        return (
            form.name && String(form.price) !== '' && String(form.stockQuantity) !== '' && String(form.minimumStockLevel) !== ''
        );
    }, [form]);

    function update(field, value) {
        setForm((f) => ({ ...f, [field]: value }));
    }

    async function handleSubmit(e) {
        e.preventDefault();
        if (!canSubmit) return;
        setLoading(true);
        setError('');
        try {
            const payload = {
                name: form.name,
                genericName: form.genericName || null,
                dosageForm: form.dosageForm || null,
                strength: form.strength || null,
                manufacturer: form.manufacturer || null,
                description: null,
                indications: null,
                contraindications: null,
                sideEffects: null,
                dosageInstructions: null,
                price: Number(form.price),
                stockQuantity: Number(form.stockQuantity),
                minimumStockLevel: Number(form.minimumStockLevel),
                unit: form.unit || null,
                expiryDate: form.expiryDate ? new Date(form.expiryDate).toISOString() : null,
                batchNumber: form.batchNumber || null,
                requiresPrescription: !!form.requiresPrescription
            };
            await createMedicine(payload);
            navigate('/medicines');
        } catch (err) {
            setError(err?.response?.data || 'Failed to create medicine');
        } finally {
            setLoading(false);
        }
    }

    return (
        <div style={{ padding: 24 }}>
            <h2 style={{ marginTop: 0, marginBottom: 16 }}>Add Medicine</h2>
            <form onSubmit={handleSubmit} className="card" style={{ padding: 20, display: 'grid', gap: 12, maxWidth: 760 }}>
                {error ? <div style={{ color: '#b00020' }}>{String(error)}</div> : null}
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 12 }}>
                    <div>
                        <label>Name</label>
                        <input value={form.name} onChange={(e) => update('name', e.target.value)} required />
                    </div>
                    <div>
                        <label>Generic Name</label>
                        <input value={form.genericName} onChange={(e) => update('genericName', e.target.value)} />
                    </div>
                    <div>
                        <label>Dosage Form</label>
                        <input value={form.dosageForm} onChange={(e) => update('dosageForm', e.target.value)} />
                    </div>
                    <div>
                        <label>Strength</label>
                        <input value={form.strength} onChange={(e) => update('strength', e.target.value)} />
                    </div>
                    <div>
                        <label>Manufacturer</label>
                        <input value={form.manufacturer} onChange={(e) => update('manufacturer', e.target.value)} />
                    </div>
                    <div>
                        <label>Price</label>
                        <input type="number" min="0" step="0.01" value={form.price} onChange={(e) => update('price', e.target.value)} required />
                    </div>
                    <div>
                        <label>Stock Quantity</label>
                        <input type="number" min="0" value={form.stockQuantity} onChange={(e) => update('stockQuantity', e.target.value)} required />
                    </div>
                    <div>
                        <label>Minimum Stock</label>
                        <input type="number" min="0" value={form.minimumStockLevel} onChange={(e) => update('minimumStockLevel', e.target.value)} required />
                    </div>
                    <div>
                        <label>Unit</label>
                        <input value={form.unit} onChange={(e) => update('unit', e.target.value)} />
                    </div>
                    <div>
                        <label>Expiry Date</label>
                        <input type="date" value={form.expiryDate} onChange={(e) => update('expiryDate', e.target.value)} />
                    </div>
                    <div>
                        <label>Batch Number</label>
                        <input value={form.batchNumber} onChange={(e) => update('batchNumber', e.target.value)} />
                    </div>
                    <div>
                        <label>Requires Prescription</label>
                        <select value={form.requiresPrescription ? 'yes' : 'no'} onChange={(e) => update('requiresPrescription', e.target.value === 'yes')}>
                            <option value="yes">Yes</option>
                            <option value="no">No</option>
                        </select>
                    </div>
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
                    <button type="submit" disabled={loading || !canSubmit} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700 }}>{loading ? 'Saving…' : 'Save Medicine'}</button>
                    <button type="button" onClick={() => navigate(-1)} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-border)', background: 'var(--hms-surface)' }}>Cancel</button>
                </div>
            </form>
        </div>
    );
}


