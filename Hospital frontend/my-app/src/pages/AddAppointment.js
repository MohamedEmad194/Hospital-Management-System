import React, { useEffect, useMemo, useState } from 'react';
import { createAppointment } from '../api/appointments';
import { fetchPatients } from '../api/patients';
import { fetchDoctors } from '../api/doctors';
import { useNavigate } from 'react-router-dom';

export default function AddAppointment() {
    const navigate = useNavigate();
    const [patients, setPatients] = useState([]);
    const [doctors, setDoctors] = useState([]);
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const [form, setForm] = useState({
        appointmentDate: '',
        appointmentTime: '10:00',
        reason: '',
        notes: '',
        isFollowUp: false,
        patientId: '',
        doctorId: '',
        roomId: ''
    });

    useEffect(() => {
        (async () => {
            try { setPatients(await fetchPatients()); } catch {}
            try { setDoctors(await fetchDoctors()); } catch {}
        })();
    }, []);

    const canSubmit = useMemo(() => form.appointmentDate && form.appointmentTime && form.patientId && form.doctorId, [form]);
    function update(field, value) { setForm((f) => ({ ...f, [field]: value })); }

    async function handleSubmit(e) {
        e.preventDefault(); if (!canSubmit) return; setLoading(true); setError('');
        try {
            const payload = {
                appointmentDate: new Date(form.appointmentDate).toISOString(),
                appointmentTime: form.appointmentTime + ':00',
                reason: form.reason || null,
                notes: form.notes || null,
                isFollowUp: !!form.isFollowUp,
                patientId: Number(form.patientId),
                doctorId: Number(form.doctorId),
                roomId: form.roomId ? Number(form.roomId) : null
            };
            await createAppointment(payload);
            navigate('/appointments');
        } catch (err) { setError(err?.response?.data || 'Failed to create appointment'); }
        finally { setLoading(false); }
    }

    return (
        <div style={{ padding: 24 }}>
            <h2 style={{ marginTop: 0, marginBottom: 16 }}>Add Appointment</h2>
            <form onSubmit={handleSubmit} className="card" style={{ padding: 20, display: 'grid', gap: 12, maxWidth: 800 }}>
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
                        <label>Doctor</label>
                        <select value={form.doctorId} onChange={(e) => update('doctorId', e.target.value)} required>
                            <option value="">Select doctor</option>
                            {doctors.map((d) => (<option key={d.id} value={d.id}>{d.firstName} {d.lastName}</option>))}
                        </select>
                    </div>
                    <div>
                        <label>Date</label>
                        <input type="date" value={form.appointmentDate} onChange={(e) => update('appointmentDate', e.target.value)} required />
                    </div>
                    <div>
                        <label>Time</label>
                        <input type="time" value={form.appointmentTime} onChange={(e) => update('appointmentTime', e.target.value)} required />
                    </div>
                    <div>
                        <label>Reason</label>
                        <input value={form.reason} onChange={(e) => update('reason', e.target.value)} />
                    </div>
                    <div>
                        <label>Notes</label>
                        <input value={form.notes} onChange={(e) => update('notes', e.target.value)} />
                    </div>
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
                    <button type="submit" disabled={loading || !canSubmit} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700 }}>{loading ? 'Saving…' : 'Save Appointment'}</button>
                    <button type="button" onClick={() => navigate(-1)} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-border)', background: 'var(--hms-surface)' }}>Cancel</button>
                </div>
            </form>
        </div>
    );
}


