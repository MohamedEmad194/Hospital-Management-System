import React, { useMemo, useState } from 'react';
import { createPatient } from '../api/patients';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function AddPatient() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const [form, setForm] = useState({
        firstName: '',
        lastName: '',
        nationalId: '',
        email: '',
        phoneNumber: '',
        dateOfBirth: '',
        gender: 'Male',
        address: ''
    });

    const canSubmit = useMemo(() => {
        return form.firstName && form.lastName && form.nationalId && form.email && form.phoneNumber && form.dateOfBirth && form.gender;
    }, [form]);

    function update(field, value) { setForm((f) => ({ ...f, [field]: value })); }

    async function handleSubmit(e) {
        e.preventDefault();
        if (!canSubmit) return;
        setLoading(true); setError('');
        try {
            const payload = {
                firstName: form.firstName,
                lastName: form.lastName,
                nationalId: form.nationalId,
                email: form.email,
                phoneNumber: form.phoneNumber,
                dateOfBirth: new Date(form.dateOfBirth).toISOString(),
                gender: form.gender,
                address: form.address || null
            };
            await createPatient(payload);
            navigate('/patients');
        } catch (err) {
            setError(err?.response?.data || t('addPatient.failed'));
        } finally { setLoading(false); }
    }

    return (
        <div style={{ padding: 24 }}>
            <h2 style={{ marginTop: 0, marginBottom: 16 }}>{t('addPatient.title')}</h2>
            <form onSubmit={handleSubmit} className="card" style={{ padding: 20, display: 'grid', gap: 12, maxWidth: 760 }}>
                {error ? <div style={{ color: '#b00020' }}>{String(error)}</div> : null}
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 12 }}>
                    <div>
                        <label>{t('addPatient.fields.firstName')}</label>
                        <input value={form.firstName} onChange={(e) => update('firstName', e.target.value)} required />
                    </div>
                    <div>
                        <label>{t('addPatient.fields.lastName')}</label>
                        <input value={form.lastName} onChange={(e) => update('lastName', e.target.value)} required />
                    </div>
                    <div>
                        <label>{t('addPatient.fields.nationalId')}</label>
                        <input value={form.nationalId} onChange={(e) => update('nationalId', e.target.value)} required />
                    </div>
                    <div>
                        <label>{t('addPatient.fields.email')}</label>
                        <input type="email" value={form.email} onChange={(e) => update('email', e.target.value)} required />
                    </div>
                    <div>
                        <label>{t('addPatient.fields.phone')}</label>
                        <input value={form.phoneNumber} onChange={(e) => update('phoneNumber', e.target.value)} required />
                    </div>
                    <div>
                        <label>{t('addPatient.fields.dateOfBirth')}</label>
                        <input type="date" value={form.dateOfBirth} onChange={(e) => update('dateOfBirth', e.target.value)} required />
                    </div>
                    <div>
                        <label>{t('addPatient.fields.gender')}</label>
                        <select value={form.gender} onChange={(e) => update('gender', e.target.value)} required>
                            <option>{t('addPatient.fields.male')}</option>
                            <option>{t('addPatient.fields.female')}</option>
                        </select>
                    </div>
                    <div>
                        <label>{t('addPatient.fields.address')}</label>
                        <input value={form.address} onChange={(e) => update('address', e.target.value)} />
                    </div>
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
                    <button type="submit" disabled={loading || !canSubmit} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700 }}>{loading ? t('addPatient.actions.saving') : t('addPatient.actions.save')}</button>
                    <button type="button" onClick={() => navigate(-1)} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-border)', background: 'var(--hms-surface)' }}>{t('addPatient.actions.cancel')}</button>
                </div>
            </form>
        </div>
    );
}


