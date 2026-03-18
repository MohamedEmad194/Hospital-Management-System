import React, { useEffect, useState, useCallback } from 'react';
import apiClient from '../api/client';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

export default function MedicinesList() {
    const { t } = useTranslation();
    const [medicines, setMedicines] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const load = useCallback(async () => {
        setLoading(true);
        setError('');
        try {
            const { data } = await apiClient.get('/Medicines');
            setMedicines(data);
        } catch (e) {
            setError(t('medicines.failed'));
        } finally {
            setLoading(false);
        }
    }, [t]);

    useEffect(() => { load(); }, [load]);

    return (
        <div style={{ padding: 24 }}>
            <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 12 }}>
                <h2 style={{ margin: 0 }}>{t('medicines.title')}</h2>
                <Link to="/medicines/add" style={{ padding: '10px 16px', borderRadius: 10, border: '1px solid var(--hms-primary-600)', background: 'var(--hms-primary)', color: '#fff', fontWeight: 700, textDecoration: 'none' }}>+ {t('medicines.addButton')}</Link>
            </div>
            {loading && <div>{t('medicines.loading')}</div>}
            {error && <div style={{ color: '#b00020' }}>{error}</div>}
            {!loading && !error && (
                <div style={{ 
                    display: 'grid', 
                    gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', 
                    gap: 20,
                    marginTop: 20
                }}>
                    {medicines.map((m, index) => {
                        const isLowStock = m.stockQuantity < m.minimumStockLevel;
                        const colors = [
                            { bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', border: '#667eea' },
                            { bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', border: '#f5576c' },
                            { bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', border: '#4facfe' },
                            { bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', border: '#43e97b' },
                            { bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', border: '#fa709a' }
                        ];
                        const color = colors[index % colors.length];
                        
                        return (
                            <div key={m.id} style={{
                                background: '#fff',
                                borderRadius: 16,
                                padding: 24,
                                boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
                                borderTop: `4px solid ${isLowStock ? '#f44336' : color.border}`,
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
                                    💊
                                </div>
                                <h3 style={{ 
                                    margin: '0 0 12px 0', 
                                    fontSize: 20, 
                                    fontWeight: 700,
                                    color: '#1a1a1a'
                                }}>
                                    {m.name}
                                </h3>
                                {m.genericName && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('medicines.labels.genericName')}:</div>
                                        <div style={{ fontSize: 14, color: '#333' }}>
                                            {m.genericName}
                                        </div>
                                    </div>
                                )}
                                {m.strength && (
                                    <div style={{ marginBottom: 8 }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('medicines.labels.strength')}:</div>
                                        <div style={{ fontSize: 14, color: '#333', fontWeight: 500 }}>
                                            {m.strength}
                                        </div>
                                    </div>
                                )}
                                <div style={{ marginBottom: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('medicines.labels.availableQuantity')}:</div>
                                    <div style={{ 
                                        fontSize: 18, 
                                        fontWeight: 700,
                                        color: isLowStock ? '#f44336' : '#4caf50'
                                    }}>
                                        {m.stockQuantity} {m.unit || t('common.unit')}
                                    </div>
                                </div>
                                <div style={{ marginBottom: 8 }}>
                                    <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('medicines.labels.minimumStock')}:</div>
                                    <div style={{ fontSize: 14, color: '#333' }}>
                                        {m.minimumStockLevel} {m.unit || t('common.unit')}
                                    </div>
                                </div>
                                {m.price && (
                                    <div style={{ marginTop: 12, paddingTop: 12, borderTop: '1px solid #eee' }}>
                                        <div style={{ fontSize: 12, color: '#666', marginBottom: 4 }}>{t('medicines.labels.price')}:</div>
                                        <div style={{ fontSize: 18, fontWeight: 700, color: '#667eea' }}>
                                            {m.price.toFixed(2)} {t('common.currency')}
                                        </div>
                                    </div>
                                )}
                                {isLowStock && (
                                    <div style={{ 
                                        marginTop: 12, 
                                        padding: 8, 
                                        borderRadius: 8, 
                                        background: '#ffebee',
                                        color: '#c62828',
                                        fontSize: 12,
                                        fontWeight: 500
                                    }}>
                                        ⚠️ {t('medicines.labels.lowStockWarning')}
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


