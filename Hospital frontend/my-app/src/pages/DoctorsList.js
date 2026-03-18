import React, { useCallback, useEffect, useState } from 'react';
import { fetchDoctors, searchDoctors } from '../api/doctors';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function DoctorsList() {
    const { t } = useTranslation();
    const [doctors, setDoctors] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [query, setQuery] = useState('');

    const load = useCallback(async () => {
        setLoading(true);
        setError('');
        try {
            const data = query ? await searchDoctors(query) : await fetchDoctors();
            setDoctors(data);
        } catch (e) {
            setError(t('doctors.failed'));
        } finally {
            setLoading(false);
        }
    }, [query, t]);

    useEffect(() => { load(); }, [load]);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, marginBottom: 16 }}>
                <input
                    placeholder={t('doctors.searchPlaceholder')}
                    value={query}
                    onChange={(e) => setQuery(e.target.value)}
                    style={{ flex: 1, padding: 12, borderRadius: 10 }}
                />
                <button onClick={load} style={{ padding: '12px 18px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700 }}>{t('common.search')}</button>
                <Link to="/doctors/add" style={{ padding: '12px 18px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'transparent', color: 'var(--hms-primary)', fontWeight: 700, textDecoration: 'none' }}>+ {t('common.add')}</Link>
            </div>

            {loading && <div>{t('doctors.loading')}</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}

            {!loading && !error && (
                <div style={{ 
                    display: 'grid', 
                    gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', 
                    gap: 20,
                    marginTop: 20
                }}>
                    {doctors.map((d, index) => {
                        const colors = [
                            { bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', border: '#667eea' },
                            { bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', border: '#f5576c' },
                            { bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', border: '#4facfe' },
                            { bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', border: '#43e97b' },
                            { bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', border: '#fa709a' },
                            { bg: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)', border: '#30cfd0' }
                        ];
                        const color = colors[index % colors.length];
                        
                        return (
                            <div key={d.id} style={{
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
                                    fontSize: 24,
                                    fontWeight: 'bold',
                                    color: '#fff'
                                }}>
                                    👨‍⚕️
                                </div>
                                <h3 style={{ 
                                    margin: '0 0 12px 0', 
                                    fontSize: 20, 
                                    fontWeight: 700,
                                    color: '#1a1a1a'
                                }}>
                                    {d.firstName} {d.lastName}
                                </h3>
                                <div style={{ marginBottom: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('doctors.labels.email')}:</div>
                                    <div style={{ fontSize: 14, color: '#667eea', fontWeight: 500 }}>
                                        📧 {d.email}
                                    </div>
                                </div>
                                <div style={{ marginBottom: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('doctors.labels.phone')}:</div>
                                    <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                        📞 {d.phoneNumber}
                                    </div>
                                </div>
                                {d.departmentName && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('doctors.labels.department')}:</div>
                                        <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                            🏥 {d.departmentName}
                                        </div>
                                    </div>
                                )}
                                {d.specialization && (
                                    <div style={{ marginTop: 12, paddingTop: 12, borderTop: '1px solid #eee' }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('doctors.labels.specialization')}:</div>
                                        <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                            {d.specialization}
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


