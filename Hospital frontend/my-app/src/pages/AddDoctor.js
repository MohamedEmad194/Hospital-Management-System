import React, { useEffect, useMemo, useState } from 'react';
import { createDoctor } from '../api/doctors';
import { fetchDepartments } from '../api/departments';
import { useNavigate } from 'react-router-dom';

export default function AddDoctor() {
    const navigate = useNavigate();
    const [departments, setDepartments] = useState([]);
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
        address: '',
        licenseNumber: '',
        specialization: '',
        yearsOfExperience: 0,
        consultationFee: 0,
        workingHoursStart: '09:00',
        workingHoursEnd: '17:00',
        departmentId: ''
    });

    useEffect(() => {
        (async () => {
            try {
                const data = await fetchDepartments();
                setDepartments(data);
            } catch (e) {
                // ignore
            }
        })();
    }, []);

    const canSubmit = useMemo(() => {
        return (
            form.firstName && form.lastName && form.nationalId && form.email && form.phoneNumber &&
            form.dateOfBirth && form.gender && form.licenseNumber && form.specialization &&
            String(form.yearsOfExperience) !== '' && String(form.consultationFee) !== '' && form.departmentId
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
                firstName: form.firstName,
                lastName: form.lastName,
                nationalId: form.nationalId,
                email: form.email,
                phoneNumber: form.phoneNumber,
                dateOfBirth: new Date(form.dateOfBirth).toISOString(),
                gender: form.gender,
                address: form.address || null,
                licenseNumber: form.licenseNumber,
                specialization: form.specialization,
                yearsOfExperience: Number(form.yearsOfExperience),
                education: null,
                certifications: null,
                languages: null,
                consultationFee: Number(form.consultationFee),
                workingHoursStart: form.workingHoursStart + ':00',
                workingHoursEnd: form.workingHoursEnd + ':00',
                departmentId: Number(form.departmentId)
            };
            await createDoctor(payload);
            navigate('/doctors');
        } catch (err) {
            setError(err?.response?.data || 'Failed to create doctor');
        } finally {
            setLoading(false);
        }
    }

    return (
        <div style={{ padding: 24 }}>
            <h2 style={{ marginTop: 0, marginBottom: 16 }}>Add Doctor</h2>
            <form onSubmit={handleSubmit} className="card" style={{ padding: 20, display: 'grid', gap: 12, maxWidth: 800 }}>
                {error ? <div style={{ color: '#b00020' }}>{String(error)}</div> : null}
                <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 12 }}>
                    <div>
                        <label>First Name</label>
                        <input value={form.firstName} onChange={(e) => update('firstName', e.target.value)} required />
                    </div>
                    <div>
                        <label>Last Name</label>
                        <input value={form.lastName} onChange={(e) => update('lastName', e.target.value)} required />
                    </div>
                    <div>
                        <label>National ID</label>
                        <input value={form.nationalId} onChange={(e) => update('nationalId', e.target.value)} required />
                    </div>
                    <div>
                        <label>Email</label>
                        <input type="email" value={form.email} onChange={(e) => update('email', e.target.value)} required />
                    </div>
                    <div>
                        <label>Phone</label>
                        <input value={form.phoneNumber} onChange={(e) => update('phoneNumber', e.target.value)} required />
                    </div>
                    <div>
                        <label>Date of Birth</label>
                        <input type="date" value={form.dateOfBirth} onChange={(e) => update('dateOfBirth', e.target.value)} required />
                    </div>
                    <div>
                        <label>Gender</label>
                        <select value={form.gender} onChange={(e) => update('gender', e.target.value)} required>
                            <option>Male</option>
                            <option>Female</option>
                        </select>
                    </div>
                    <div>
                        <label>License Number</label>
                        <input value={form.licenseNumber} onChange={(e) => update('licenseNumber', e.target.value)} required />
                    </div>
                    <div>
                        <label>Specialization</label>
                        <input value={form.specialization} onChange={(e) => update('specialization', e.target.value)} required />
                    </div>
                    <div>
                        <label>Years of Experience</label>
                        <input type="number" min="0" max="50" value={form.yearsOfExperience} onChange={(e) => update('yearsOfExperience', e.target.value)} required />
                    </div>
                    <div>
                        <label>Consultation Fee</label>
                        <input type="number" min="0" step="0.01" value={form.consultationFee} onChange={(e) => update('consultationFee', e.target.value)} required />
                    </div>
                    <div>
                        <label>Working Start</label>
                        <input type="time" value={form.workingHoursStart} onChange={(e) => update('workingHoursStart', e.target.value)} required />
                    </div>
                    <div>
                        <label>Working End</label>
                        <input type="time" value={form.workingHoursEnd} onChange={(e) => update('workingHoursEnd', e.target.value)} required />
                    </div>
                    <div>
                        <label>Department</label>
                        <select value={form.departmentId} onChange={(e) => update('departmentId', e.target.value)} required>
                            <option value="">Select department</option>
                            {departments.map((d) => (
                                <option key={d.id} value={d.id}>{d.name}</option>
                            ))}
                        </select>
                    </div>
                </div>
                <div style={{ display: 'flex', gap: 8 }}>
                    <button type="submit" disabled={loading || !canSubmit} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700 }}>{loading ? 'Saving…' : 'Save Doctor'}</button>
                    <button type="button" onClick={() => navigate(-1)} style={{ padding: '10px 16px', borderRadius: 8, border: '1px solid var(--hms-border)', background: 'var(--hms-surface)' }}>Cancel</button>
                </div>
            </form>
        </div>
    );
}


