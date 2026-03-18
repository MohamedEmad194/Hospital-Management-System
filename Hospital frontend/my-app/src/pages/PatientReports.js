import React, { useEffect, useState, useContext, useCallback } from 'react';
import { fetchMedicalRecordsByPatient } from '../api/medicalRecords';
import { useTranslation } from 'react-i18next';
import { AuthContext } from '../context/AuthContext';

export default function PatientReports() {
    const { t } = useTranslation();
    const { user } = useContext(AuthContext);
    const isPatient = user?.roles?.includes('Patient');
    const [reports, setReports] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [selectedReport, setSelectedReport] = useState(null);

    const load = useCallback(async () => {
        if (!user?.patientId) {
            setError('تعذر العثور على بيانات المريض');
            setLoading(false);
            return;
        }

        setLoading(true);
        setError('');
        try {
            const data = await fetchMedicalRecordsByPatient(user.patientId);
            setReports(data || []);
        } catch (e) {
            console.error('Error loading reports:', e);
            setError('تعذر تحميل التقارير الطبية');
        } finally {
            setLoading(false);
        }
    }, [user?.patientId]);

    useEffect(() => { 
        if (user?.patientId) {
            load(); 
        }
    }, [load, user?.patientId]);

    const formatDate = (dateString) => {
        if (!dateString) return '';
        const date = new Date(dateString);
        return date.toLocaleDateString('ar-EG', {
            year: 'numeric',
            month: 'long',
            day: 'numeric'
        });
    };

    const getRecordTypeColor = (type) => {
        const typeMap = {
            'Consultation': '#667eea',
            'Diagnosis': '#f093fb',
            'Treatment': '#4facfe',
            'Surgery': '#f5576c',
            'Lab': '#43e97b',
            'Follow-up': '#fa709a',
            'Emergency': '#ff6b6b'
        };
        return typeMap[type] || '#718096';
    };

    if (!isPatient) {
        return (
            <div style={{ padding: 24, textAlign: 'center' }}>
                <h2 style={{ color: '#b00020' }}>غير مصرح لك بالوصول إلى هذه الصفحة</h2>
                <p>هذه الصفحة مخصصة للمرضى فقط</p>
            </div>
        );
    }

    return (
        <div style={{ padding: 24 }}>
            <div style={{ marginBottom: 24 }}>
                <h2 style={{ margin: 0, fontSize: '2rem', fontWeight: 800, color: '#1a1a1a' }}>
                    📋 تقاريري الطبية
                </h2>
                <p style={{ margin: '8px 0 0 0', color: '#718096', fontSize: '1rem' }}>
                    عرض جميع التقارير الطبية والفحوصات الخاصة بك
                </p>
            </div>

            {loading && (
                <div style={{ 
                    textAlign: 'center', 
                    padding: '40px', 
                    color: '#718096',
                    fontSize: '1.1rem'
                }}>
                    جاري تحميل التقارير...
                </div>
            )}

            {error && (
                <div style={{ 
                    background: '#ffebee', 
                    color: '#c62828', 
                    padding: '16px', 
                    borderRadius: '12px',
                    marginBottom: 20,
                    border: '2px solid #ef5350'
                }}>
                    <strong>⚠️ {error}</strong>
                </div>
            )}

            {!loading && !error && reports.length === 0 && (
                <div style={{ 
                    textAlign: 'center', 
                    padding: '60px 20px',
                    background: '#f8f9fa',
                    borderRadius: '16px',
                    border: '2px dashed #dee2e6'
                }}>
                    <div style={{ fontSize: '4rem', marginBottom: '16px' }}>📄</div>
                    <h3 style={{ color: '#495057', marginBottom: '8px' }}>لا توجد تقارير طبية</h3>
                    <p style={{ color: '#6c757d' }}>لم يتم إنشاء أي تقارير طبية لك حتى الآن</p>
                </div>
            )}

            {!loading && !error && reports.length > 0 && (
                <div style={{ 
                    display: 'grid', 
                    gridTemplateColumns: selectedReport ? '1fr 1fr' : 'repeat(auto-fill, minmax(400px, 1fr))', 
                    gap: 20,
                    marginTop: 20
                }}>
                    {/* Reports List */}
                    <div style={{ 
                        display: 'flex', 
                        flexDirection: 'column', 
                        gap: 16 
                    }}>
                        {reports.map((report) => (
                            <div
                                key={report.id}
                                onClick={() => setSelectedReport(report)}
                                style={{
                                    background: selectedReport?.id === report.id 
                                        ? 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)' 
                                        : '#fff',
                                    color: selectedReport?.id === report.id ? '#fff' : '#1a1a1a',
                                    padding: '20px',
                                    borderRadius: '16px',
                                    border: selectedReport?.id === report.id 
                                        ? 'none' 
                                        : '2px solid #e2e8f0',
                                    cursor: 'pointer',
                                    transition: 'all 0.3s ease',
                                    boxShadow: selectedReport?.id === report.id 
                                        ? '0 8px 20px rgba(102, 126, 234, 0.3)' 
                                        : '0 2px 8px rgba(0,0,0,0.1)',
                                    transform: selectedReport?.id === report.id ? 'translateY(-2px)' : 'none'
                                }}
                                onMouseEnter={(e) => {
                                    if (selectedReport?.id !== report.id) {
                                        e.currentTarget.style.transform = 'translateY(-4px)';
                                        e.currentTarget.style.boxShadow = '0 4px 12px rgba(0,0,0,0.15)';
                                    }
                                }}
                                onMouseLeave={(e) => {
                                    if (selectedReport?.id !== report.id) {
                                        e.currentTarget.style.transform = 'translateY(0)';
                                        e.currentTarget.style.boxShadow = '0 2px 8px rgba(0,0,0,0.1)';
                                    }
                                }}
                            >
                                <div style={{ 
                                    display: 'flex', 
                                    alignItems: 'center', 
                                    justifyContent: 'space-between',
                                    marginBottom: '12px'
                                }}>
                                    <div style={{
                                        background: selectedReport?.id === report.id 
                                            ? 'rgba(255,255,255,0.2)' 
                                            : getRecordTypeColor(report.recordType),
                                        color: selectedReport?.id === report.id ? '#fff' : '#fff',
                                        padding: '6px 12px',
                                        borderRadius: '8px',
                                        fontSize: '0.85rem',
                                        fontWeight: 700
                                    }}>
                                        {report.recordType || 'تقرير طبي'}
                                    </div>
                                    <div style={{ 
                                        fontSize: '0.9rem', 
                                        opacity: 0.9,
                                        fontWeight: 500
                                    }}>
                                        📅 {formatDate(report.recordDate)}
                                    </div>
                                </div>
                                
                                {report.doctorName && (
                                    <div style={{ 
                                        marginBottom: '8px',
                                        fontSize: '0.95rem',
                                        opacity: 0.9
                                    }}>
                                        👨‍⚕️ د. {report.doctorName}
                                    </div>
                                )}

                                {report.diagnosis && (
                                    <div style={{ 
                                        marginTop: '8px',
                                        padding: '10px',
                                        background: selectedReport?.id === report.id 
                                            ? 'rgba(255,255,255,0.15)' 
                                            : '#f8f9fa',
                                        borderRadius: '8px',
                                        fontSize: '0.9rem',
                                        lineHeight: 1.5,
                                        maxHeight: '60px',
                                        overflow: 'hidden',
                                        textOverflow: 'ellipsis'
                                    }}>
                                        <strong>التشخيص:</strong> {report.diagnosis.substring(0, 100)}
                                        {report.diagnosis.length > 100 ? '...' : ''}
                                    </div>
                                )}
                            </div>
                        ))}
                    </div>

                    {/* Report Details */}
                    {selectedReport && (
                        <div style={{
                            background: '#fff',
                            padding: '30px',
                            borderRadius: '16px',
                            border: '2px solid #e2e8f0',
                            boxShadow: '0 4px 12px rgba(0,0,0,0.1)',
                            maxHeight: 'calc(100vh - 200px)',
                            overflowY: 'auto'
                        }}>
                            <div style={{ 
                                display: 'flex', 
                                justifyContent: 'space-between',
                                alignItems: 'center',
                                marginBottom: '24px',
                                paddingBottom: '20px',
                                borderBottom: '2px solid #e2e8f0'
                            }}>
                                <h3 style={{ 
                                    margin: 0, 
                                    fontSize: '1.5rem',
                                    fontWeight: 800,
                                    color: '#1a1a1a'
                                }}>
                                    {selectedReport.recordType || 'تقرير طبي'}
                                </h3>
                                <button
                                    onClick={() => setSelectedReport(null)}
                                    style={{
                                        background: '#f8f9fa',
                                        border: '2px solid #e2e8f0',
                                        borderRadius: '8px',
                                        padding: '8px 16px',
                                        cursor: 'pointer',
                                        fontSize: '0.9rem',
                                        fontWeight: 600,
                                        color: '#495057'
                                    }}
                                >
                                    ✕ إغلاق
                                </button>
                            </div>

                            <div style={{ display: 'grid', gap: '20px' }}>
                                {/* Basic Info */}
                                <div style={{
                                    background: 'linear-gradient(135deg, #667eea10, #764ba210)',
                                    padding: '20px',
                                    borderRadius: '12px'
                                }}>
                                    <div style={{ 
                                        display: 'grid', 
                                        gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                                        gap: '16px'
                                    }}>
                                        <div>
                                            <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '4px' }}>
                                                📅 تاريخ التقرير
                                            </div>
                                            <div style={{ fontSize: '1rem', fontWeight: 600, color: '#1a1a1a' }}>
                                                {formatDate(selectedReport.recordDate)}
                                            </div>
                                        </div>
                                        {selectedReport.doctorName && (
                                            <div>
                                                <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '4px' }}>
                                                    👨‍⚕️ الطبيب المعالج
                                                </div>
                                                <div style={{ fontSize: '1rem', fontWeight: 600, color: '#1a1a1a' }}>
                                                    د. {selectedReport.doctorName}
                                                </div>
                                            </div>
                                        )}
                                    </div>
                                </div>

                                {/* Symptoms */}
                                {selectedReport.symptoms && (
                                    <div>
                                        <h4 style={{ 
                                            margin: '0 0 12px 0',
                                            fontSize: '1.1rem',
                                            fontWeight: 700,
                                            color: '#1a1a1a',
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '8px'
                                        }}>
                                            <span>🩺</span> الأعراض
                                        </h4>
                                        <div style={{
                                            background: '#f8f9fa',
                                            padding: '16px',
                                            borderRadius: '10px',
                                            lineHeight: 1.6,
                                            color: '#495057',
                                            whiteSpace: 'pre-wrap'
                                        }}>
                                            {selectedReport.symptoms}
                                        </div>
                                    </div>
                                )}

                                {/* Diagnosis */}
                                {selectedReport.diagnosis && (
                                    <div>
                                        <h4 style={{ 
                                            margin: '0 0 12px 0',
                                            fontSize: '1.1rem',
                                            fontWeight: 700,
                                            color: '#1a1a1a',
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '8px'
                                        }}>
                                            <span>🔬</span> التشخيص
                                        </h4>
                                        <div style={{
                                            background: '#fff3cd',
                                            padding: '16px',
                                            borderRadius: '10px',
                                            lineHeight: 1.6,
                                            color: '#856404',
                                            border: '1px solid #ffc107',
                                            whiteSpace: 'pre-wrap'
                                        }}>
                                            {selectedReport.diagnosis}
                                        </div>
                                    </div>
                                )}

                                {/* Treatment */}
                                {selectedReport.treatment && (
                                    <div>
                                        <h4 style={{ 
                                            margin: '0 0 12px 0',
                                            fontSize: '1.1rem',
                                            fontWeight: 700,
                                            color: '#1a1a1a',
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '8px'
                                        }}>
                                            <span>💊</span> العلاج
                                        </h4>
                                        <div style={{
                                            background: '#d1ecf1',
                                            padding: '16px',
                                            borderRadius: '10px',
                                            lineHeight: 1.6,
                                            color: '#0c5460',
                                            border: '1px solid #bee5eb',
                                            whiteSpace: 'pre-wrap'
                                        }}>
                                            {selectedReport.treatment}
                                        </div>
                                    </div>
                                )}

                                {/* Prescription */}
                                {selectedReport.prescription && (
                                    <div>
                                        <h4 style={{ 
                                            margin: '0 0 12px 0',
                                            fontSize: '1.1rem',
                                            fontWeight: 700,
                                            color: '#1a1a1a',
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '8px'
                                        }}>
                                            <span>💉</span> الوصفة الطبية
                                        </h4>
                                        <div style={{
                                            background: '#f8f9fa',
                                            padding: '16px',
                                            borderRadius: '10px',
                                            lineHeight: 1.6,
                                            color: '#495057',
                                            whiteSpace: 'pre-wrap'
                                        }}>
                                            {selectedReport.prescription}
                                        </div>
                                    </div>
                                )}

                                {/* Vital Signs */}
                                {(selectedReport.bloodPressure || selectedReport.temperature || 
                                  selectedReport.heartRate || selectedReport.weight || selectedReport.height) && (
                                    <div>
                                        <h4 style={{ 
                                            margin: '0 0 12px 0',
                                            fontSize: '1.1rem',
                                            fontWeight: 700,
                                            color: '#1a1a1a',
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '8px'
                                        }}>
                                            <span>📊</span> العلامات الحيوية
                                        </h4>
                                        <div style={{
                                            background: '#e7f3ff',
                                            padding: '16px',
                                            borderRadius: '10px',
                                            display: 'grid',
                                            gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))',
                                            gap: '12px'
                                        }}>
                                            {selectedReport.bloodPressure && (
                                                <div>
                                                    <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '4px' }}>
                                                        ضغط الدم
                                                    </div>
                                                    <div style={{ fontSize: '1rem', fontWeight: 600, color: '#1a1a1a' }}>
                                                        {selectedReport.bloodPressure}
                                                    </div>
                                                </div>
                                            )}
                                            {selectedReport.temperature && (
                                                <div>
                                                    <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '4px' }}>
                                                        درجة الحرارة
                                                    </div>
                                                    <div style={{ fontSize: '1rem', fontWeight: 600, color: '#1a1a1a' }}>
                                                        {selectedReport.temperature}°C
                                                    </div>
                                                </div>
                                            )}
                                            {selectedReport.heartRate && (
                                                <div>
                                                    <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '4px' }}>
                                                        معدل النبض
                                                    </div>
                                                    <div style={{ fontSize: '1rem', fontWeight: 600, color: '#1a1a1a' }}>
                                                        {selectedReport.heartRate} bpm
                                                    </div>
                                                </div>
                                            )}
                                            {selectedReport.weight && (
                                                <div>
                                                    <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '4px' }}>
                                                        الوزن
                                                    </div>
                                                    <div style={{ fontSize: '1rem', fontWeight: 600, color: '#1a1a1a' }}>
                                                        {selectedReport.weight} kg
                                                    </div>
                                                </div>
                                            )}
                                            {selectedReport.height && (
                                                <div>
                                                    <div style={{ fontSize: '0.85rem', color: '#718096', marginBottom: '4px' }}>
                                                        الطول
                                                    </div>
                                                    <div style={{ fontSize: '1rem', fontWeight: 600, color: '#1a1a1a' }}>
                                                        {selectedReport.height} cm
                                                    </div>
                                                </div>
                                            )}
                                        </div>
                                    </div>
                                )}

                                {/* Lab Results */}
                                {selectedReport.labResults && (
                                    <div>
                                        <h4 style={{ 
                                            margin: '0 0 12px 0',
                                            fontSize: '1.1rem',
                                            fontWeight: 700,
                                            color: '#1a1a1a',
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '8px'
                                        }}>
                                            <span>🧪</span> نتائج المختبر
                                        </h4>
                                        <div style={{
                                            background: '#f8f9fa',
                                            padding: '16px',
                                            borderRadius: '10px',
                                            lineHeight: 1.6,
                                            color: '#495057',
                                            whiteSpace: 'pre-wrap'
                                        }}>
                                            {selectedReport.labResults}
                                        </div>
                                    </div>
                                )}

                                {/* Imaging Results */}
                                {selectedReport.imagingResults && (
                                    <div>
                                        <h4 style={{ 
                                            margin: '0 0 12px 0',
                                            fontSize: '1.1rem',
                                            fontWeight: 700,
                                            color: '#1a1a1a',
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '8px'
                                        }}>
                                            <span>🩻</span> نتائج الأشعة
                                        </h4>
                                        <div style={{
                                            background: '#f8f9fa',
                                            padding: '16px',
                                            borderRadius: '10px',
                                            lineHeight: 1.6,
                                            color: '#495057',
                                            whiteSpace: 'pre-wrap'
                                        }}>
                                            {selectedReport.imagingResults}
                                        </div>
                                    </div>
                                )}

                                {/* Notes */}
                                {selectedReport.notes && (
                                    <div>
                                        <h4 style={{ 
                                            margin: '0 0 12px 0',
                                            fontSize: '1.1rem',
                                            fontWeight: 700,
                                            color: '#1a1a1a',
                                            display: 'flex',
                                            alignItems: 'center',
                                            gap: '8px'
                                        }}>
                                            <span>📝</span> ملاحظات
                                        </h4>
                                        <div style={{
                                            background: '#f8f9fa',
                                            padding: '16px',
                                            borderRadius: '10px',
                                            lineHeight: 1.6,
                                            color: '#495057',
                                            whiteSpace: 'pre-wrap'
                                        }}>
                                            {selectedReport.notes}
                                        </div>
                                    </div>
                                )}
                            </div>
                        </div>
                    )}
                </div>
            )}
        </div>
    );
}

