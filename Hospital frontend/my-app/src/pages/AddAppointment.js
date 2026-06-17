import React, { useContext, useEffect, useMemo, useState } from 'react';
import { createAppointment } from '../api/appointments';
import { fetchPatients } from '../api/patients';
import { fetchDoctors } from '../api/doctors';
import { fetchRooms } from '../api/rooms';
import { fetchPatient } from '../api/patients';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { AuthContext } from '../context/AuthContext';

export default function AddAppointment() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const { user } = useContext(AuthContext);
    const isPatient = user?.roles?.includes('Patient');
    const [patients, setPatients] = useState([]);
    const [doctors, setDoctors] = useState([]);
    const [rooms, setRooms] = useState([]);
    const [currentPatient, setCurrentPatient] = useState(null);
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
            // Only fetch patients list if user is Admin/Doctor/Staff
            if (!isPatient) {
                try { 
                    setPatients(await fetchPatients()); 
                } catch (err) {
                    console.warn('Could not fetch patients list:', err);
                }
            } else {
                // For patients, fetch their own data
                try {
                    // Try to get patient ID from user object
                    if (user?.patientId) {
                        const patient = await fetchPatient(user.patientId);
                        setCurrentPatient(patient);
                        setForm(f => ({ ...f, patientId: String(patient.id) }));
                    } else {
                        // If patientId is not in user object, show error
                        setError('تعذر العثور على بيانات المريض. يرجى تسجيل الخروج وتسجيل الدخول مرة أخرى.');
                    }
                } catch (err) {
                    console.warn('Could not fetch patient data:', err);
                    const errorMsg = err?.response?.data?.message || err?.response?.data?.details || 'تعذر تحميل بيانات المريض. يرجى المحاولة مرة أخرى.';
                    setError(errorMsg);
                }
            }
            
            try { 
                setDoctors(await fetchDoctors()); 
            } catch (err) {
                console.warn('Could not fetch doctors list:', err);
                setError(prev => (prev ? `${prev} ` : '') + 'تعذر تحميل قائمة الأطباء.');
            }
            
            try { 
                setRooms(await fetchRooms()); 
            } catch (err) {
                console.warn('Could not fetch rooms list:', err);
                // Rooms are optional, so don't show error
            }
        })();
    }, [isPatient, user]);

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
        } catch (err) { setError(err?.response?.data || t('addAppointment.failed')); }
        finally { setLoading(false); }
    }

    const selectedPatient = patients.find(p => p.id === Number(form.patientId));
    const selectedDoctor = doctors.find(d => d.id === Number(form.doctorId));
    const availableRooms = rooms.filter(r => r.isAvailable);

    return (
        <div style={{ 
            minHeight: '100vh', 
            background: 'linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%)',
            padding: '40px 20px'
        }}>
            <div style={{ maxWidth: '900px', margin: '0 auto' }}>
                {/* Header Section */}
                <div style={{
                    textAlign: 'center',
                    marginBottom: '40px'
                }}>
                    <div style={{
                        display: 'inline-block',
                        padding: '15px 35px',
                        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                        borderRadius: '50px',
                        marginBottom: '25px',
                        boxShadow: '0 6px 20px rgba(102, 126, 234, 0.3)'
                    }}>
                        <span style={{ fontSize: '2rem', marginRight: '12px' }}>📅</span>
                    </div>
                    <h1 style={{ 
                        margin: '0 0 15px 0',
                        fontSize: '2.8rem',
                        fontWeight: '800',
                        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                        WebkitBackgroundClip: 'text',
                        WebkitTextFillColor: 'transparent',
                        backgroundClip: 'text',
                        letterSpacing: '1px'
                    }}>
                        {t('addAppointment.title')}
                    </h1>
                    <p style={{
                        fontSize: '1.1rem',
                        color: '#718096',
                        margin: 0
                    }}>
                        {t('addAppointment.fields.selectPatient')} و {t('addAppointment.fields.selectDoctor')} لتحديد الموعد
                    </p>
                </div>

                {/* Form Card */}
                <form onSubmit={handleSubmit} style={{
                    background: '#fff',
                    borderRadius: '24px',
                    padding: '40px',
                    boxShadow: '0 8px 30px rgba(0,0,0,0.1)',
                    border: '1px solid #e2e8f0'
                }}>
                    {error && (
                        <div style={{
                            background: '#ffebee',
                            color: '#c62828',
                            padding: '16px',
                            borderRadius: '12px',
                            marginBottom: '30px',
                            border: '2px solid #ef5350',
                            display: 'flex',
                            alignItems: 'center',
                            gap: '12px',
                            fontSize: '1rem',
                            fontWeight: '500'
                        }}>
                            <span style={{ fontSize: '1.5rem' }}>⚠️</span>
                            <span>{String(error)}</span>
                        </div>
                    )}

                    {/* Patient Selection */}
                    {!isPatient ? (
                        <div style={{ marginBottom: '30px' }}>
                            <label style={{
                                fontSize: '1rem',
                                fontWeight: '700',
                                color: '#1a1a1a',
                                marginBottom: '12px',
                                display: 'flex',
                                alignItems: 'center',
                                gap: '10px'
                            }}>
                                <span style={{
                                    background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                    width: '40px',
                                    height: '40px',
                                    borderRadius: '10px',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    fontSize: '1.3rem'
                                }}>👤</span>
                                {t('addAppointment.fields.patient')} <span style={{ color: '#f44336' }}>*</span>
                            </label>
                            <select 
                                value={form.patientId} 
                                onChange={(e) => update('patientId', e.target.value)} 
                                required
                                style={{
                                    width: '100%',
                                    padding: '16px',
                                    borderRadius: '12px',
                                    border: '2px solid #e2e8f0',
                                    fontSize: '1rem',
                                    background: '#fff',
                                    color: '#1a1a1a',
                                    transition: 'all 0.3s ease',
                                    cursor: 'pointer'
                                }}
                                onFocus={(e) => {
                                    e.target.style.borderColor = '#667eea';
                                    e.target.style.boxShadow = '0 0 0 3px rgba(102, 126, 234, 0.1)';
                                }}
                                onBlur={(e) => {
                                    e.target.style.borderColor = '#e2e8f0';
                                    e.target.style.boxShadow = 'none';
                                }}
                            >
                                <option value="">{t('addAppointment.fields.selectPatient')}</option>
                                {patients.map((p) => (
                                    <option key={p.id} value={p.id}>
                                        {p.firstName} {p.lastName} {p.nationalId ? `(${p.nationalId})` : ''}
                                    </option>
                                ))}
                            </select>
                            {selectedPatient && (
                                <div style={{
                                    marginTop: '12px',
                                    padding: '12px',
                                    background: 'linear-gradient(135deg, #667eea10, #764ba210)',
                                    borderRadius: '10px',
                                    fontSize: '0.9rem',
                                    color: '#666'
                                }}>
                                    📧 {selectedPatient.email} | 📞 {selectedPatient.phoneNumber}
                                </div>
                            )}
                        </div>
                    ) : (
                        currentPatient && (
                            <div style={{ marginBottom: '30px' }}>
                                <label style={{
                                    fontSize: '1rem',
                                    fontWeight: '700',
                                    color: '#1a1a1a',
                                    marginBottom: '12px',
                                    display: 'flex',
                                    alignItems: 'center',
                                    gap: '10px'
                                }}>
                                    <span style={{
                                        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                        width: '40px',
                                        height: '40px',
                                        borderRadius: '10px',
                                        display: 'flex',
                                        alignItems: 'center',
                                        justifyContent: 'center',
                                        fontSize: '1.3rem'
                                    }}>👤</span>
                                    {t('addAppointment.fields.patient')}
                                </label>
                                <div style={{
                                    width: '100%',
                                    padding: '16px',
                                    borderRadius: '12px',
                                    border: '2px solid #e2e8f0',
                                    fontSize: '1rem',
                                    background: '#f8f9fa',
                                    color: '#1a1a1a'
                                }}>
                                    {currentPatient.firstName} {currentPatient.lastName} {currentPatient.nationalId ? `(${currentPatient.nationalId})` : ''}
                                </div>
                                <div style={{
                                    marginTop: '12px',
                                    padding: '12px',
                                    background: 'linear-gradient(135deg, #667eea10, #764ba210)',
                                    borderRadius: '10px',
                                    fontSize: '0.9rem',
                                    color: '#666'
                                }}>
                                    📧 {currentPatient.email} | 📞 {currentPatient.phoneNumber}
                                </div>
                            </div>
                        )
                    )}

                    {/* Doctor Selection */}
                    <div style={{ marginBottom: '30px' }}>
                        <label style={{
                            fontSize: '1rem',
                            fontWeight: '700',
                            color: '#1a1a1a',
                            marginBottom: '12px',
                            display: 'flex',
                            alignItems: 'center',
                            gap: '10px'
                        }}>
                            <span style={{
                                background: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
                                width: '40px',
                                height: '40px',
                                borderRadius: '10px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontSize: '1.3rem'
                            }}>👨‍⚕️</span>
                            {t('addAppointment.fields.doctor')} <span style={{ color: '#f44336' }}>*</span>
                        </label>
                        <select 
                            value={form.doctorId} 
                            onChange={(e) => update('doctorId', e.target.value)} 
                            required
                            style={{
                                width: '100%',
                                padding: '16px',
                                borderRadius: '12px',
                                border: '2px solid #e2e8f0',
                                fontSize: '1rem',
                                background: '#fff',
                                color: '#1a1a1a',
                                transition: 'all 0.3s ease',
                                cursor: 'pointer'
                            }}
                            onFocus={(e) => {
                                e.target.style.borderColor = '#f5576c';
                                e.target.style.boxShadow = '0 0 0 3px rgba(245, 87, 108, 0.1)';
                            }}
                            onBlur={(e) => {
                                e.target.style.borderColor = '#e2e8f0';
                                e.target.style.boxShadow = 'none';
                            }}
                        >
                            <option value="">{t('addAppointment.fields.selectDoctor')}</option>
                            {doctors.map((d) => (
                                <option key={d.id} value={d.id}>
                                    {d.firstName} {d.lastName} {d.specialization ? `- ${d.specialization}` : ''}
                                </option>
                            ))}
                        </select>
                        {selectedDoctor && (
                            <div style={{
                                marginTop: '12px',
                                padding: '12px',
                                background: 'linear-gradient(135deg, #f093fb10, #f5576c10)',
                                borderRadius: '10px',
                                fontSize: '0.9rem',
                                color: '#666'
                            }}>
                                📧 {selectedDoctor.email} | 📞 {selectedDoctor.phoneNumber} | {selectedDoctor.departmentName || ''}
                            </div>
                        )}
                    </div>

                    {/* Date and Time */}
                    <div style={{ 
                        display: 'grid', 
                        gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', 
                        gap: '20px',
                        marginBottom: '30px'
                    }}>
                        <div>
                            <label style={{
                                fontSize: '1rem',
                                fontWeight: '700',
                                color: '#1a1a1a',
                                marginBottom: '12px',
                                display: 'flex',
                                alignItems: 'center',
                                gap: '10px'
                            }}>
                                <span style={{
                                    background: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
                                    width: '40px',
                                    height: '40px',
                                    borderRadius: '10px',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    fontSize: '1.3rem'
                                }}>📅</span>
                                {t('addAppointment.fields.date')} <span style={{ color: '#f44336' }}>*</span>
                            </label>
                            <input 
                                type="date" 
                                value={form.appointmentDate} 
                                onChange={(e) => update('appointmentDate', e.target.value)} 
                                required
                                min={new Date().toISOString().split('T')[0]}
                                style={{
                                    width: '100%',
                                    padding: '16px',
                                    borderRadius: '12px',
                                    border: '2px solid #e2e8f0',
                                    fontSize: '1rem',
                                    background: '#fff',
                                    color: '#1a1a1a',
                                    transition: 'all 0.3s ease'
                                }}
                                onFocus={(e) => {
                                    e.target.style.borderColor = '#4facfe';
                                    e.target.style.boxShadow = '0 0 0 3px rgba(79, 172, 254, 0.1)';
                                }}
                                onBlur={(e) => {
                                    e.target.style.borderColor = '#e2e8f0';
                                    e.target.style.boxShadow = 'none';
                                }}
                            />
                        </div>
                        <div>
                            <label style={{
                                fontSize: '1rem',
                                fontWeight: '700',
                                color: '#1a1a1a',
                                marginBottom: '12px',
                                display: 'flex',
                                alignItems: 'center',
                                gap: '10px'
                            }}>
                                <span style={{
                                    background: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
                                    width: '40px',
                                    height: '40px',
                                    borderRadius: '10px',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    fontSize: '1.3rem'
                                }}>⏰</span>
                                {t('addAppointment.fields.time')} <span style={{ color: '#f44336' }}>*</span>
                            </label>
                            <input 
                                type="time" 
                                value={form.appointmentTime} 
                                onChange={(e) => update('appointmentTime', e.target.value)} 
                                required
                                style={{
                                    width: '100%',
                                    padding: '16px',
                                    borderRadius: '12px',
                                    border: '2px solid #e2e8f0',
                                    fontSize: '1rem',
                                    background: '#fff',
                                    color: '#1a1a1a',
                                    transition: 'all 0.3s ease'
                                }}
                                onFocus={(e) => {
                                    e.target.style.borderColor = '#43e97b';
                                    e.target.style.boxShadow = '0 0 0 3px rgba(67, 233, 123, 0.1)';
                                }}
                                onBlur={(e) => {
                                    e.target.style.borderColor = '#e2e8f0';
                                    e.target.style.boxShadow = 'none';
                                }}
                            />
                        </div>
                    </div>

                    {/* Room Selection */}
                    {availableRooms.length > 0 && (
                        <div style={{ marginBottom: '30px' }}>
                            <label style={{
                                fontSize: '1rem',
                                fontWeight: '700',
                                color: '#1a1a1a',
                                marginBottom: '12px',
                                display: 'flex',
                                alignItems: 'center',
                                gap: '10px'
                            }}>
                                <span style={{
                                    background: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
                                    width: '40px',
                                    height: '40px',
                                    borderRadius: '10px',
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    fontSize: '1.3rem'
                                }}>🏥</span>
                                {t('addAppointment.fields.room')}
                            </label>
                            <select 
                                value={form.roomId} 
                                onChange={(e) => update('roomId', e.target.value)}
                                style={{
                                    width: '100%',
                                    padding: '16px',
                                    borderRadius: '12px',
                                    border: '2px solid #e2e8f0',
                                    fontSize: '1rem',
                                    background: '#fff',
                                    color: '#1a1a1a',
                                    transition: 'all 0.3s ease',
                                    cursor: 'pointer'
                                }}
                                onFocus={(e) => {
                                    e.target.style.borderColor = '#fa709a';
                                    e.target.style.boxShadow = '0 0 0 3px rgba(250, 112, 154, 0.1)';
                                }}
                                onBlur={(e) => {
                                    e.target.style.borderColor = '#e2e8f0';
                                    e.target.style.boxShadow = 'none';
                                }}
                            >
                                <option value="">{t('addAppointment.fields.selectRoom')}</option>
                                {availableRooms.map((r) => (
                                    <option key={r.id} value={r.id}>
                                        {r.roomNumber} - {t(`rooms.types.${r.roomType}`) || r.roomType} {r.department?.name ? `(${r.department.name})` : ''}
                                    </option>
                                ))}
                            </select>
                        </div>
                    )}

                    {/* Reason */}
                    <div style={{ marginBottom: '30px' }}>
                        <label style={{
                            fontSize: '1rem',
                            fontWeight: '700',
                            color: '#1a1a1a',
                            marginBottom: '12px',
                            display: 'flex',
                            alignItems: 'center',
                            gap: '10px'
                        }}>
                            <span style={{
                                background: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)',
                                width: '40px',
                                height: '40px',
                                borderRadius: '10px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontSize: '1.3rem'
                            }}>📝</span>
                            {t('addAppointment.fields.reason')}
                        </label>
                        <input 
                            value={form.reason} 
                            onChange={(e) => update('reason', e.target.value)}
                            placeholder="مثال: فحص دوري، ألم في الصدر، متابعة..."
                            style={{
                                width: '100%',
                                padding: '16px',
                                borderRadius: '12px',
                                border: '2px solid #e2e8f0',
                                fontSize: '1rem',
                                background: '#fff',
                                color: '#1a1a1a',
                                transition: 'all 0.3s ease'
                            }}
                            onFocus={(e) => {
                                e.target.style.borderColor = '#30cfd0';
                                e.target.style.boxShadow = '0 0 0 3px rgba(48, 207, 208, 0.1)';
                            }}
                            onBlur={(e) => {
                                e.target.style.borderColor = '#e2e8f0';
                                e.target.style.boxShadow = 'none';
                            }}
                        />
                    </div>

                    {/* Notes */}
                    <div style={{ marginBottom: '30px' }}>
                        <label style={{
                            fontSize: '1rem',
                            fontWeight: '700',
                            color: '#1a1a1a',
                            marginBottom: '12px',
                            display: 'flex',
                            alignItems: 'center',
                            gap: '10px'
                        }}>
                            <span style={{
                                background: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)',
                                width: '40px',
                                height: '40px',
                                borderRadius: '10px',
                                display: 'flex',
                                alignItems: 'center',
                                justifyContent: 'center',
                                fontSize: '1.3rem'
                            }}>📋</span>
                            {t('addAppointment.fields.notes')}
                        </label>
                        <textarea 
                            value={form.notes} 
                            onChange={(e) => update('notes', e.target.value)}
                            placeholder="ملاحظات إضافية..."
                            rows={4}
                            style={{
                                width: '100%',
                                padding: '16px',
                                borderRadius: '12px',
                                border: '2px solid #e2e8f0',
                                fontSize: '1rem',
                                background: '#fff',
                                color: '#1a1a1a',
                                transition: 'all 0.3s ease',
                                resize: 'vertical',
                                fontFamily: 'inherit'
                            }}
                            onFocus={(e) => {
                                e.target.style.borderColor = '#a8edea';
                                e.target.style.boxShadow = '0 0 0 3px rgba(168, 237, 234, 0.1)';
                            }}
                            onBlur={(e) => {
                                e.target.style.borderColor = '#e2e8f0';
                                e.target.style.boxShadow = 'none';
                            }}
                        />
                    </div>

                    {/* Follow Up Checkbox */}
                    <div style={{ marginBottom: '40px' }}>
                        <label style={{
                            display: 'flex',
                            alignItems: 'center',
                            gap: '12px',
                            cursor: 'pointer',
                            fontSize: '1rem',
                            color: '#1a1a1a',
                            fontWeight: '500'
                        }}>
                            <input 
                                type="checkbox" 
                                checked={form.isFollowUp} 
                                onChange={(e) => update('isFollowUp', e.target.checked)}
                                style={{
                                    width: '20px',
                                    height: '20px',
                                    cursor: 'pointer',
                                    accentColor: '#667eea'
                                }}
                            />
                            <span>موعد متابعة (Follow-up)</span>
                        </label>
                    </div>

                    {/* Action Buttons */}
                    <div style={{ 
                        display: 'flex', 
                        gap: '15px',
                        justifyContent: 'flex-end',
                        paddingTop: '20px',
                        borderTop: '2px solid #e2e8f0'
                    }}>
                        <button 
                            type="button" 
                            onClick={() => navigate(-1)}
                            style={{ 
                                padding: '16px 32px', 
                                borderRadius: '12px', 
                                border: '2px solid #e2e8f0',
                                background: '#fff',
                                color: '#666',
                                fontWeight: '700',
                                fontSize: '1rem',
                                cursor: 'pointer',
                                transition: 'all 0.3s ease'
                            }}
                            onMouseEnter={(e) => {
                                e.target.style.background = '#f8f9fa';
                                e.target.style.borderColor = '#d0d7de';
                                e.target.style.transform = 'translateY(-2px)';
                            }}
                            onMouseLeave={(e) => {
                                e.target.style.background = '#fff';
                                e.target.style.borderColor = '#e2e8f0';
                                e.target.style.transform = 'translateY(0)';
                            }}
                        >
                            {t('addAppointment.actions.cancel')}
                        </button>
                        <button 
                            type="submit" 
                            disabled={loading || !canSubmit}
                            style={{ 
                                padding: '16px 32px', 
                                borderRadius: '12px', 
                                border: 'none',
                                background: canSubmit ? 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)' : '#ccc',
                                color: '#fff',
                                fontWeight: '700',
                                fontSize: '1rem',
                                cursor: canSubmit ? 'pointer' : 'not-allowed',
                                transition: 'all 0.3s ease',
                                boxShadow: canSubmit ? '0 4px 15px rgba(102, 126, 234, 0.3)' : 'none',
                                display: 'flex',
                                alignItems: 'center',
                                gap: '10px'
                            }}
                            onMouseEnter={(e) => {
                                if (canSubmit) {
                                    e.target.style.transform = 'translateY(-2px)';
                                    e.target.style.boxShadow = '0 6px 20px rgba(102, 126, 234, 0.4)';
                                }
                            }}
                            onMouseLeave={(e) => {
                                if (canSubmit) {
                                    e.target.style.transform = 'translateY(0)';
                                    e.target.style.boxShadow = '0 4px 15px rgba(102, 126, 234, 0.3)';
                                }
                            }}
                        >
                            {loading ? (
                                <>
                                    <span>⏳</span>
                                    <span>{t('addAppointment.actions.saving')}</span>
                                </>
                            ) : (
                                <>
                                    <span>💾</span>
                                    <span>{t('addAppointment.actions.save')}</span>
                                </>
                            )}
                        </button>
                    </div>
                </form>
            </div>
        </div>
    );
}


