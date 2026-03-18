import React, { useEffect, useState, useCallback } from 'react';
import { fetchRooms } from '../api/rooms';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function RoomsList() {
    const { t } = useTranslation();
    const [rooms, setRooms] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const load = useCallback(async () => {
        setLoading(true);
        setError('');
        try {
            const data = await fetchRooms();
            setRooms(data);
        } catch (e) {
            setError(t('rooms.failed'));
        } finally {
            setLoading(false);
        }
    }, [t]);

    useEffect(() => { load(); }, [load]);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 12 }}>
                <h2 style={{ margin: 0 }}>{t('rooms.title')}</h2>
                <Link to="/rooms/add" style={{ padding: '10px 16px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700, textDecoration: 'none' }}>+ {t('rooms.addButton')}</Link>
            </div>
            {loading && <div>{t('rooms.loading')}</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}
            {!loading && !error && (
                <div style={{ 
                    display: 'grid', 
                    gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', 
                    gap: 20,
                    marginTop: 20
                }}>
                    {rooms.map((r, index) => {
                        const roomTypeColors = {
                            'Consultation': { bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', border: '#667eea' },
                            'ICU': { bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', border: '#f5576c' },
                            'Surgery': { bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', border: '#4facfe' },
                            'Ward': { bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', border: '#43e97b' },
                            'Emergency': { bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', border: '#fa709a' },
                            'Radiology': { bg: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)', border: '#30cfd0' }
                        };
                        const color = roomTypeColors[r.roomType] || { bg: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)', border: '#a8edea' };
                        
                        return (
                            <div key={r.id} style={{
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
                                    🏥
                                </div>
                                <h3 style={{ 
                                    margin: '0 0 12px 0', 
                                    fontSize: 20, 
                                    fontWeight: 700,
                                    color: '#1a1a1a'
                                }}>
                                    {r.roomNumber}
                                </h3>
                                <div style={{ marginBottom: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('rooms.labels.type')}:</div>
                                    <div style={{ 
                                        fontSize: 14, 
                                        fontWeight: 500,
                                        display: 'inline-block',
                                        padding: '4px 12px',
                                        borderRadius: 8,
                                        background: color.bg,
                                        color: '#fff'
                                    }}>
                                        {t(`rooms.types.${r.roomType}`) || r.roomType}
                                    </div>
                                </div>
                                {r.department && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('rooms.labels.department')}:</div>
                                        <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                            {r.department.name || r.departmentId}
                                        </div>
                                    </div>
                                )}
                                {r.floor && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('rooms.labels.floor')}:</div>
                                        <div style={{ fontSize: 14, color: '#333' }}>
                                            📍 {r.floor}
                                        </div>
                                    </div>
                                )}
                                <div style={{ marginTop: 12, paddingTop: 12, borderTop: '1px solid #eee', display: 'flex', alignItems: 'center', gap: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666' }}>{t('rooms.labels.status')}:</div>
                                    <div style={{ 
                                        fontSize: 14, 
                                        fontWeight: 500,
                                        padding: '4px 12px',
                                        borderRadius: 8,
                                        background: r.isAvailable ? '#e8f5e9' : '#ffebee',
                                        color: r.isAvailable ? '#2e7d32' : '#c62828'
                                    }}>
                                        {r.isAvailable ? `✓ ${t('rooms.labels.available')}` : `✗ ${t('rooms.labels.unavailable')}`}
                                    </div>
                                </div>
                            </div>
                        );
                    })}
                </div>
            )}
        </div>
    );
}
