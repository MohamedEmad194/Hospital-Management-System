import React, { useEffect, useState, useContext, useCallback } from 'react';
import { fetchBills, overdueBills, outstandingAmount } from '../api/bills';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { AuthContext } from '../context/AuthContext';

export default function BillsList() {
    const { t, i18n } = useTranslation();
    const { user } = useContext(AuthContext);
    const isArabic = i18n.language === 'ar';
    const [bills, setBills] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [summary, setSummary] = useState(null);

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
            const data = await fetchBills();
            const overdue = await overdueBills();
            const outstanding = await outstandingAmount();
            setBills(data || []);
            setSummary({ overdueCount: overdue.length, totalOutstanding: outstanding.totalOutstandingAmount });
        } catch (e) {
            const errorMsg = e?.response?.data?.message || e?.response?.data?.details;
            if (errorMsg?.includes('Insufficient permissions') || errorMsg?.includes('لا تملك الصلاحيات')) {
                if (isPatient && !hasPatientId) {
                    setError('حسابك غير مرتبط بسجل مريض. يرجى التواصل مع الإدارة لربط حسابك.');
                } else {
                    setError('ليس لديك صلاحية للوصول إلى الفواتير. يرجى التواصل مع الإدارة.');
                }
            } else if (errorMsg?.includes('PatientId') || errorMsg?.includes('مريض')) {
                setError('تعذر العثور على بيانات المريض. يرجى تسجيل الخروج وتسجيل الدخول مرة أخرى.');
            } else {
                setError(errorMsg || t('bills.failed') || 'تعذر تحميل الفواتير');
            }
        } finally {
            setLoading(false);
        }
    }, [t, user]);

    useEffect(() => { load(); }, [load]);

    const getStatusInfo = (status) => {
        const statusMap = {
            'Paid': { 
                bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', 
                border: '#43e97b',
                label: status,
                icon: '✓'
            },
            'Pending': { 
                bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', 
                border: '#fa709a',
                label: status,
                icon: '⏳'
            },
            'Overdue': { 
                bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', 
                border: '#f5576c',
                label: status,
                icon: '⚠️'
            },
            'Partial': { 
                bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', 
                border: '#4facfe',
                label: status,
                icon: '◐'
            }
        };
        return statusMap[status] || { 
            bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', 
            border: '#667eea',
            label: status,
            icon: '📄'
        };
    };

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 20 }}>
                <h2 style={{ margin: 0, fontSize: 28, fontWeight: 700, color: '#1a1a1a' }}>
                    {user?.roles?.includes('Patient') ? t('common.myBills') : t('bills.title')}
                </h2>
                {user?.roles?.includes('Admin') || user?.roles?.includes('Doctor') ? (
                    <Link to="/bills/add" style={{ 
                        padding: '12px 20px', 
                        borderRadius: 12, 
                        border: '1px solid var(--hms-primary-600)', 
                        background: 'var(--hms-primary)', 
                        color: '#fff', 
                        fontWeight: 700, 
                        textDecoration: 'none',
                        transition: 'all 0.3s ease',
                        boxShadow: '0 4px 6px rgba(102, 126, 234, 0.3)'
                    }}
                    onMouseEnter={(e) => {
                        e.currentTarget.style.transform = 'translateY(-2px)';
                        e.currentTarget.style.boxShadow = '0 6px 12px rgba(102, 126, 234, 0.4)';
                    }}
                    onMouseLeave={(e) => {
                        e.currentTarget.style.transform = 'translateY(0)';
                        e.currentTarget.style.boxShadow = '0 4px 6px rgba(102, 126, 234, 0.3)';
                    }}>+ {t('bills.addButton')}</Link>
                ) : null}
            </div>
            {summary && (
                <div style={{ 
                    display: 'grid', 
                    gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))', 
                    gap: 20, 
                    marginBottom: 30 
                }}>
                    <div style={{ 
                        padding: 24, 
                        background: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
                        borderRadius: 16, 
                        boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
                        color: '#fff'
                    }}>
                        <div style={{ fontSize: 14, opacity: 0.9, marginBottom: 8 }}>{t('bills.summary.overdue')}</div>
                        <div style={{ fontSize: 32, fontWeight: 700 }}>{summary.overdueCount}</div>
                    </div>
                    <div style={{ 
                        padding: 24, 
                        background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                        borderRadius: 16, 
                        boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
                        color: '#fff'
                    }}>
                        <div style={{ fontSize: 14, opacity: 0.9, marginBottom: 8 }}>{t('bills.summary.outstanding')}</div>
                        <div style={{ fontSize: 32, fontWeight: 700 }}>
                            {typeof summary.totalOutstanding === 'number' 
                                ? summary.totalOutstanding.toFixed(2) 
                                : summary.totalOutstanding} {t('common.currency')}
                        </div>
                    </div>
                </div>
            )}
            {loading && <div style={{ padding: 20, textAlign: 'center', color: '#666' }}>{t('bills.loading')}</div>}
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
                    gap: 20
                }}>
                    {bills.map((b) => {
                        const statusInfo = getStatusInfo(b.status);
                        const isOverdue = b.status === 'Overdue';
                        const isPaid = b.status === 'Paid';
                        
                        return (
                            <div key={b.id} style={{
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
                                    💰
                                </div>
                                <div style={{ 
                                    marginBottom: 16,
                                    padding: '8px 12px',
                                    borderRadius: 8,
                                    background: statusInfo.bg,
                                    color: '#fff',
                                    display: 'inline-block',
                                    fontSize: 12,
                                    fontWeight: 600
                                }}>
                                    {statusInfo.icon} {statusInfo.label}
                                </div>
                                {b.patientName && (
                                    <div style={{ marginBottom: 12 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('bills.table.patient')}:</div>
                                        <div style={{ fontSize: 16, color: '#1a1a1a', fontWeight: 600 }}>
                                            👤 {b.patientName}
                                        </div>
                                    </div>
                                )}
                                <div style={{ 
                                    marginTop: 16, 
                                    paddingTop: 16, 
                                    borderTop: '1px solid #eee',
                                    display: 'grid',
                                    gridTemplateColumns: '1fr 1fr',
                                    gap: 12
                                }}>
                                    <div>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('bills.table.total')}:</div>
                                        <div style={{ fontSize: 18, fontWeight: 700, color: '#1a1a1a' }}>
                                            {typeof b.totalAmount === 'number' ? b.totalAmount.toFixed(2) : b.totalAmount} {t('common.currency')}
                                        </div>
                                    </div>
                                    <div>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('bills.table.paid')}:</div>
                                        <div style={{ fontSize: 18, fontWeight: 700, color: isPaid ? '#4caf50' : '#667eea' }}>
                                            {typeof b.paidAmount === 'number' ? b.paidAmount.toFixed(2) : b.paidAmount} {t('common.currency')}
                                        </div>
                                    </div>
                                </div>
                                <div style={{ 
                                    marginTop: 12, 
                                    padding: '12px 16px',
                                    borderRadius: 8,
                                    background: isOverdue ? '#ffebee' : isPaid ? '#e8f5e9' : '#e3f2fd',
                                    border: `1px solid ${isOverdue ? '#f5576c' : isPaid ? '#43e97b' : '#4facfe'}`
                                }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('bills.table.remaining')}:</div>
                                    <div style={{ 
                                        fontSize: 20, 
                                        fontWeight: 700, 
                                        color: isOverdue ? '#c62828' : isPaid ? '#2e7d32' : '#1976d2'
                                    }}>
                                        {typeof b.remainingAmount === 'number' ? b.remainingAmount.toFixed(2) : b.remainingAmount} {t('common.currency')}
                                    </div>
                                </div>
                                {!isPaid && b.remainingAmount > 0 && (
                                    <Link 
                                        to={`/bills/${b.id}/payment`}
                                        style={{
                                            marginTop: 12,
                                            display: 'block',
                                            padding: '12px 20px',
                                            borderRadius: 8,
                                            background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                            color: 'white',
                                            textDecoration: 'none',
                                            textAlign: 'center',
                                            fontWeight: 600,
                                            fontSize: 14,
                                            transition: 'all 0.2s',
                                            boxShadow: '0 2px 8px rgba(102, 126, 234, 0.3)'
                                        }}
                                        onMouseEnter={(e) => {
                                            e.currentTarget.style.transform = 'translateY(-2px)';
                                            e.currentTarget.style.boxShadow = '0 4px 12px rgba(102, 126, 234, 0.4)';
                                        }}
                                        onMouseLeave={(e) => {
                                            e.currentTarget.style.transform = 'translateY(0)';
                                            e.currentTarget.style.boxShadow = '0 2px 8px rgba(102, 126, 234, 0.3)';
                                        }}
                                    >
                                        💳 {isArabic ? 'دفع الآن' : 'Pay Now'}
                                    </Link>
                                )}
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
}


