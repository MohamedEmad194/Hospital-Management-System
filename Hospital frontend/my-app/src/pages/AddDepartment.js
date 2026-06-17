import React, { useMemo, useState } from 'react';
import { createDepartment } from '../api/departments';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import ErrorAlert from '../components/ErrorAlert';

export default function AddDepartment() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const [form, setForm] = useState({ name: '', description: '' });

    const canSubmit = useMemo(() => form.name.length > 0, [form]);
    function update(field, value) { setForm((f) => ({ ...f, [field]: value })); }

    async function handleSubmit(e) {
        e.preventDefault(); if (!canSubmit) return; setLoading(true); setError('');
        try {
            await createDepartment({ name: form.name, description: form.description || null });
            navigate('/departments');
        } catch (err) { setError(err?.response?.data || t('addDepartment.failed')); } finally { setLoading(false); }
    }

    return (
        <div style={{ padding: 24 }}>
            <h2 style={{ marginTop: 0, marginBottom: 16 }}>{t('addDepartment.title')}</h2>
            <form onSubmit={handleSubmit} className="card" style={{ padding: 20, display: 'grid', gap: 12, maxWidth: 680 }}>
                <ErrorAlert
                    message={typeof error === 'string' ? error : (error?.message || (error ? String(error) : ''))}
                    onDismiss={() => setError('')}
                />
                <div>
                    <label>{t('addDepartment.fields.name')}</label>
                    <input value={form.name} onChange={(e) => update('name', e.target.value)} required />
                </div>
                <div>
                    <label>{t('addDepartment.fields.description')}</label>
                    <input value={form.description} onChange={(e) => update('description', e.target.value)} />
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
                    <button type="submit" disabled={loading || !canSubmit} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700 }}>{loading ? t('addDepartment.actions.saving') : t('addDepartment.actions.save')}</button>
                    <button type="button" onClick={() => navigate(-1)} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-border)', background: 'var(--hms-surface)' }}>{t('addDepartment.actions.cancel')}</button>
                </div>
            </form>
        </div>
    );
}


