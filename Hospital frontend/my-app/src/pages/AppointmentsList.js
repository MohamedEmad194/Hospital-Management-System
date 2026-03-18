import React, { useEffect, useState, useContext, useCallback } from 'react';
import { fetchAppointments } from '../api/appointments';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { AuthContext } from '../context/AuthContext';

export default function AppointmentsList() {
    const { t } = useTranslation();
    const { user } = useContext(AuthContext);
    const [appointments, setAppointments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const load = useCallback(async () => {
        // Wait for user to be loaded
        if (!user) {
            setLoading(true);
            return;
        }

        // Check if patient has PatientId before making API call
        const isPatient = user?.roles?.includes('Patient');
        const hasPatientId = user?.patientId != null && user?.patientId !== undefined;
        
        if (isPatient && !hasPatientId) {
            setError('حسابك غير مرتبط بسجل مريض. يرجى التواصل مع الإدارة لربط حسابك.');
            setLoading(false);
            return;
        }

        setLoading(true);
        setError('');
        try {
            const data = await fetchAppointments();
            setAppointments(data || []);
        } catch (e) {
            const errorMsg = e?.response?.data?.message || e?.response?.data?.details;
            if (errorMsg?.includes('Insufficient permissions') || errorMsg?.includes('لا تملك الصلاحيات')) {
                if (isPatient && !hasPatientId) {
                    setError('حسابك غير مرتبط بسجل مريض. يرجى التواصل مع الإدارة لربط حسابك.');
                } else {
                    setError('ليس لديك صلاحية للوصول إلى المواعيد. يرجى التواصل مع الإدارة.');
                }
            } else if (errorMsg?.includes('PatientId') || errorMsg?.includes('مريض')) {
                setError('تعذر العثور على بيانات المريض. يرجى تسجيل الخروج وتسجيل الدخول مرة أخرى.');
            } else {
                setError(errorMsg || t('appointments.failed') || 'تعذر تحميل المواعيد');
            }
        } finally {
            setLoading(false);
        }
    }, [t, user]);

    useEffect(() => { load(); }, [load]);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 12 }}>
                <h2 style={{ margin: 0 }}>
                    {user?.roles?.includes('Patient') ? t('common.myAppointments') : t('appointments.title')}
                </h2>
                {user?.roles?.includes('Admin') || user?.roles?.includes('Doctor') ? (
                    <Link to="/appointments/add" style={{ padding: '10px 16px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700, textDecoration: 'none' }}>+ {t('appointments.addButton')}</Link>
                ) : null}
            </div>
            {loading && <div>{t('appointments.loading')}</div>}
            {error && (
                <div style={{ 
                    color: '#c62828', 
                    padding: 16, 
                    borderRadius: 10, 
                    background: '#ffebee', 
                    border: '2px solid #ef5350',
                    marginBottom: 20,
                    fontSize: 14,
                    lineHeight: 1.5
                }}>
                    <div style={{ display: 'flex', alignItems: 'flex-start', gap: 10 }}>
                        <span style={{ fontSize: 20 }}>⚠️</span>
                        <div style={{ flex: 1 }}>
                            <div style={{ fontWeight: 600, marginBottom: 8 }}>{error}</div>
                            {(error.includes('PatientId') || error.includes('مريض') || error.includes('غير مرتبط')) ? (
                                <div style={{ 
                                    fontSize: 12, 
                                    opacity: 0.9, 
                                    marginTop: 8,
                                    padding: 10,
                                    background: '#fff3cd',
                                    borderRadius: 6,
                                    border: '1px solid #ffc107'
                                }}>
                                    <strong>💡 الحلول الممكنة:</strong>
                                    <ul style={{ margin: '6px 0 0 0', paddingLeft: 20 }}>
                                        <li>سجل الخروج ثم سجل الدخول مرة أخرى</li>
                                        <li>تواصل مع الإدارة لربط حسابك بسجل مريض في النظام</li>
                                    </ul>
                                </div>
                            ) : null}
                        </div>
                    </div>
                </div>
            )}
            {!loading && !error && (
                <div style={{ 
                    display: 'grid', 
                    gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))', 
                    gap: 20,
                    marginTop: 20
                }}>
                    {appointments.map((a) => {
                        const getStatusColor = (status) => {
                            const statusMap = {
                                'Scheduled': { bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', border: '#4facfe', label: 'مجدول', color: '#0277bd' },
                                'Confirmed': { bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', border: '#43e97b', label: 'مؤكد', color: '#2e7d32' },
                                'Completed': { bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', border: '#667eea', label: 'مكتمل', color: '#512da8' },
                                'Cancelled': { bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', border: '#f5576c', label: 'ملغي', color: '#c62828' },
                                'NoShow': { bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', border: '#fa709a', label: 'لم يحضر', color: '#e65100' }
                            };
                            return statusMap[status] || { bg: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)', border: '#a8edea', label: status, color: '#666' };
                        };
                        const statusInfo = getStatusColor(a.status);
                        const appointmentDate = a.appointmentDate ? new Date(a.appointmentDate) : null;
                        const formattedDate = appointmentDate ? appointmentDate.toLocaleDateString('ar-EG', { 
                            year: 'numeric', 
                            month: 'long', 
                            day: 'numeric' 
                        }) : 'غير محدد';
                        
                        return (
                            <div key={a.id} style={{
                                background: '#fff',
                                borderRadius: 16,
                                padding: 24,
                                boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
                                borderTop: `4px solid ${statusInfo.border}`,
                                transition: 'transform 0.2s, box-shadow 0.2s',
                                cursor: 'pointer'
                            }}
                            onMouseEnter={(e) => {
                                e.currentTarget.style.transform = 'translateY(-4px)';
                                e.currentTarget.style.boxShadow = '0 8px 12px rgba(0,0,0,0.15)';
                            }}
                            onMouseLeave={(e) => {
                                e.currentTarget.style.transform = 'translateY(0)';
                                e.currentTarget.style.boxShadow = '0 4px 6px rgba(0,0,0,0.1)';
                            }}>
                                <div style={{
                                    background: statusInfo.bg,
                                    width: 50,
                                    height: 50,
                                    borderRadius: 12,
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    marginBottom: 16,
                                    fontSize: 24,
                                    fontWeight: 'bold',
                                    color: '#fff'
                                }}>
                                    📅
                                </div>
                                <div style={{ marginBottom: 12 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>الحالة:</div>
                                    <div style={{ 
                                        fontSize: 14, 
                                        fontWeight: 500,
                                        padding: '4px 12px',
                                        borderRadius: 8,
                                        background: statusInfo.bg,
                                        color: '#fff',
                                        display: 'inline-block'
                                    }}>
                                        {statusInfo.label}
                                    </div>
                                </div>
                                {a.patientName && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>المريض:</div>
                                        <div style={{ fontSize: 16, color: '#1a1a1a', fontWeight: 600 }}>
                                            👤 {a.patientName}
                                        </div>
                                    </div>
                                )}
                                {a.doctorName && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>الطبيب:</div>
                                        <div style={{ fontSize: 16, color: '#1a1a1a', fontWeight: 600 }}>
                                            👨‍⚕️ {a.doctorName}
                                        </div>
                                    </div>
                                )}
                                <div style={{ marginBottom: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>التاريخ:</div>
                                    <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                        📅 {formattedDate}
                                    </div>
                                </div>
                                {a.appointmentTime && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>الوقت:</div>
                                        <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                            ⏰ {a.appointmentTime}
                                        </div>
                                    </div>
                                )}
                                {a.reason && (
                                    <div style={{ marginTop: 12, paddingTop: 12, borderTop: '1px solid #eee' }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>السبب:</div>
                                        <div style={{ fontSize: 14, color: '#333', lineHeight: 1.6 }}>
                                            {a.reason}
                                        </div>
                                    </div>
                                )}
                                {a.consultationFee && (
                                    <div style={{ marginTop: 12, paddingTop: 12, borderTop: '1px solid #eee' }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>رسوم الاستشارة:</div>
                                        <div style={{ fontSize: 16, fontWeight: 700, color: '#667eea' }}>
                                            {a.consultationFee.toFixed(2)} جنيه
                                        </div>
                                    </div>
                                )}
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
}


