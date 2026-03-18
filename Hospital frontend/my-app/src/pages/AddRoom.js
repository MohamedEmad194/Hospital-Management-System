import React, { useState } from 'react';
import { createRoom } from '../api/rooms';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function AddRoom() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [form, setForm] = useState({
        roomNumber: '',
        roomType: '',
        floor: '',
        building: '',
        description: '',
        capacity: 1,
        hourlyRate: 0,
        isAvailable: true,
        departmentId: ''
    });
    const [saving, setSaving] = useState(false);
    const [error, setError] = useState('');

    const onChange = (e) => {
        const { name, value, type, checked } = e.target;
        setForm((f) => ({ ...f, [name]: type === 'checkbox' ? checked : value }));
    };

    const onSubmit = async (e) => {
        e.preventDefault();
        setSaving(true);
        setError('');
        try {
            await createRoom({
                roomNumber: form.roomNumber,
                roomType: form.roomType,
                floor: form.floor,
                building: form.building,
                description: form.description,
                capacity: Number(form.capacity),
                hourlyRate: Number(form.hourlyRate),
                isAvailable: !!form.isAvailable,
                departmentId: Number(form.departmentId)
            });
            navigate('/rooms');
        } catch (e1) {
            setError(t('addRoom.failed'));
        } finally {
            setSaving(false);
        }
    };

    return (
        <div style={{ padding: 24 }}>
            <h2 style={{ marginBottom: 16 }}>{t('addRoom.title')}</h2>
            {error && <div style={{ color: '#b00020', marginBottom: 12 }}>{error}</div>}
            <form onSubmit={onSubmit} className="card" style={{ padding: 16, maxWidth: 720 }}>
                <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 12 }}>
                    <label>
                        <div>{t('addRoom.fields.roomNumber')}</div>
                        <input name="roomNumber" value={form.roomNumber} onChange={onChange} required />
                    </label>
                    <label>
                        <div>{t('addRoom.fields.roomType')}</div>
                        <select name="roomType" value={form.roomType} onChange={onChange} required style={{ width: '100%', padding: '8px', borderRadius: '8px', border: '1px solid #ddd' }}>
                            <option value="">{t('common.select')}</option>
                            <option value="Consultation">{t('rooms.types.Consultation')}</option>
                            <option value="ICU">{t('rooms.types.ICU')}</option>
                            <option value="Surgery">{t('rooms.types.Surgery')}</option>
                            <option value="Ward">{t('rooms.types.Ward')}</option>
                            <option value="Emergency">{t('rooms.types.Emergency')}</option>
                            <option value="Radiology">{t('rooms.types.Radiology')}</option>
                        </select>
                    </label>
                    <label>
                        <div>{t('addRoom.fields.floor')}</div>
                        <input name="floor" value={form.floor} onChange={onChange} />
                    </label>
                    <label>
                        <div>{t('addRoom.fields.building')}</div>
                        <input name="building" value={form.building} onChange={onChange} />
                    </label>
                    <label style={{ gridColumn: '1 / -1' }}>
                        <div>{t('addRoom.fields.description')}</div>
                        <input name="description" value={form.description} onChange={onChange} />
                    </label>
                    <label>
                        <div>{t('addRoom.fields.capacity')}</div>
                        <input name="capacity" type="number" min="1" value={form.capacity} onChange={onChange} />
                    </label>
                    <label>
                        <div>{t('addRoom.fields.hourlyRate')}</div>
                        <input name="hourlyRate" type="number" min="0" step="0.01" value={form.hourlyRate} onChange={onChange} />
                    </label>
                    <label>
                        <div>{t('addRoom.fields.departmentId')}</div>
                        <input name="departmentId" type="number" min="1" value={form.departmentId} onChange={onChange} required />
                    </label>
                    <label style={{ alignSelf: 'end' }}>
                        <input name="isAvailable" type="checkbox" checked={form.isAvailable} onChange={onChange} /> {t('addRoom.fields.isAvailable')}
                    </label>
                </div>
                <div style={{ marginTop: 16, display: 'flex', gap: 8 }}>
                    <button type="submit" className="btn-primary" disabled={saving}>
                        {saving ? t('addRoom.actions.saving') : t('addRoom.actions.save')}
                    </button>
                    <button type="button" onClick={() => navigate(-1)} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-border)', background: 'var(--hms-surface)' }}>
                        {t('addRoom.actions.cancel')}
                    </button>
                </div>
            </form>
        </div>
    );
}
