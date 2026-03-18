import React, { useCallback, useEffect, useState } from 'react';
import { fetchPatients, searchPatients } from '../api/patients';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function PatientsList() {
    const { t } = useTranslation();
    const [patients, setPatients] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [query, setQuery] = useState('');

    const load = useCallback(async () => {
        setLoading(true);
        setError('');
        try {
            const data = query ? await searchPatients(query) : await fetchPatients();
            setPatients(data);
        } catch (e) {
            setError(t('patients.failed'));
        } finally {
            setLoading(false);
        }
    }, [query, t]);

    useEffect(() => { load(); }, [load]);

    return (
        <div style={{ padding: 24 }}>
            <div className="page-toolbar">
                <input
                    placeholder={t('patients.searchPlaceholder')}
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    className="page-toolbar__field"
                />
                <button
                    type="button"
                    onClick={load}
                    className="page-toolbar__action page-toolbar__action--primary"
                >
                    {t('common.search')}
                </button>
                <Link
                    to="/patients/add"
                    className="page-toolbar__action page-toolbar__action--ghost"
                >
                    + {t('common.add')}
                </Link>
            </div>

            {loading && <div>{t('patients.loading')}</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}

            {!loading && !error && (
                <div style={{ 
                    display: 'grid', 
                    gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', 
                    gap: 20,
                    marginTop: 20
                }}>
                    {patients.map((p, index) => {
                        const colors = [
                            { bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', border: '#667eea' },
                            { bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', border: '#f5576c' },
                            { bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', border: '#4facfe' },
                            { bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', border: '#43e97b' },
                            { bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', border: '#fa709a' },
                            { bg: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)', border: '#30cfd0' },
                            { bg: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)', border: '#a8edea' },
                            { bg: 'linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%)', border: '#ff9a9e' }
                        ];
                        const color = colors[index % colors.length];
                        const initials = `${p.firstName?.charAt(0) || ''}${p.lastName?.charAt(0) || ''}`.toUpperCase();
                        
                        return (
                            <div key={p.id} style={{
                                background: '#fff',
                                borderRadius: 16,
                                padding: 24,
                                boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
                                borderTop: `4px solid ${color.border}`,
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
                                    background: color.bg,
                                    width: 50,
                                    height: 50,
                                    borderRadius: 12,
                                    display: 'flex',
                                    alignItems: 'center',
                                    justifyContent: 'center',
                                    marginBottom: 16,
                                    fontSize: 20,
                                    fontWeight: 'bold',
                                    color: '#fff'
                                }}>
                                    {initials || '👤'}
                                </div>
                                <h3 style={{ 
                                    margin: '0 0 12px 0', 
                                    fontSize: 20, 
                                    fontWeight: 700,
                                    color: '#1a1a1a'
                                }}>
                                    {p.firstName} {p.lastName}
                                </h3>
                                <div style={{ marginBottom: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>البريد الإلكتروني:</div>
                                    <div style={{ fontSize: 14, color: '#667eea', fontWeight: 500 }}>
                                        📧 {p.email}
                                    </div>
                                </div>
                                <div style={{ marginBottom: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>الهاتف:</div>
                                    <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                        📞 {p.phoneNumber}
                                    </div>
                                </div>
                                {p.nationalId && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>الرقم القومي:</div>
                                        <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                            🆔 {p.nationalId}
                                        </div>
                                    </div>
                                )}
                                {p.dateOfBirth && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>تاريخ الميلاد:</div>
                                        <div style={{ fontSize: 14, color: '#333' }}>
                                            📅 {new Date(p.dateOfBirth).toLocaleDateString('ar-EG')}
                                        </div>
                                    </div>
                                )}
                                {p.gender && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>الجنس:</div>
                                        <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                            {p.gender === 'Male' ? '👨 ذكر' : p.gender === 'Female' ? '👩 أنثى' : p.gender}
                                        </div>
                                    </div>
                                )}
                                {p.address && (
                                    <div style={{ marginTop: 12, paddingTop: 12, borderTop: '1px solid #eee' }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>العنوان:</div>
                                        <div style={{ fontSize: 14, color: '#333' }}>
                                            📍 {p.address}
                                        </div>
                                    </div>
                                )}
                                {p.insuranceProvider && (
                                    <div style={{ marginTop: 12, paddingTop: 12, borderTop: '1px solid #eee' }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>شركة التأمين:</div>
                                        <div style={{ fontSize: 14, color: '#4caf50', fontWeight: 500 }}>
                                            🏥 {p.insuranceProvider}
                                        </div>
                                        {p.insuranceNumber && (
                                            <div style={{ fontSize: 12, color: '#666', marginTop: 4 }}>
                                                رقم التأمين: {p.insuranceNumber}
                                            </div>
                                        )}
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


