import React, { useMemo, useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import { fetchNursingUnits } from '../api/nursingUnits';
import apiClient from '../api/client';

export default function NursingServices() {
    const { t } = useTranslation();
    const navigate = useNavigate();
    const [query, setQuery] = useState('');
    const [nursingDataset, setNursingDataset] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [seeding, setSeeding] = useState(false);

    const nursingPillars = [
        { icon: '💙', title: t('nursingPage.pillars.compassion.title'), desc: t('nursingPage.pillars.compassion.desc') },
        { icon: '🕒', title: t('nursingPage.pillars.coverage.title'), desc: t('nursingPage.pillars.coverage.desc') },
        { icon: '🧠', title: t('nursingPage.pillars.expertise.title'), desc: t('nursingPage.pillars.expertise.desc') },
        { icon: '🤝', title: t('nursingPage.pillars.familySupport.title'), desc: t('nursingPage.pillars.familySupport.desc') },
    ];

    const nursingPrograms = [
        { title: t('nursingPage.programs.emergency.title'), desc: t('nursingPage.programs.emergency.desc') },
        { title: t('nursingPage.programs.icu.title'), desc: t('nursingPage.programs.icu.desc') },
        { title: t('nursingPage.programs.pediatrics.title'), desc: t('nursingPage.programs.pediatrics.desc') },
        { title: t('nursingPage.programs.homeCare.title'), desc: t('nursingPage.programs.homeCare.desc') },
    ];

    const supportChannels = [
        { label: t('nursingPage.support.rounds.label'), value: t('nursingPage.support.rounds.value') },
        { label: t('nursingPage.support.consultations.label'), value: t('nursingPage.support.consultations.value') },
        { label: t('nursingPage.support.training.label'), value: t('nursingPage.support.training.value') },
    ];

    useEffect(() => {
        const loadNursingUnits = async () => {
            try {
                setLoading(true);
                const data = await fetchNursingUnits();
                // Transform API data to match frontend format
                // Handle both array and object response
                const unitsArray = Array.isArray(data) ? data : (data.data || []);
                const transformed = unitsArray.map(item => ({
                    id: item.unitId || item.id,
                    unit: item.unit || item.unitName || '',
                    wing: item.wing || '',
                    lead: item.lead || item.leadNurse || '',
                    nurses: item.nurses || item.totalNurses || 0,
                    coverage: item.coverage || '',
                    ratio: item.ratio || '',
                    focus: item.focus || item.focusAreas || ''
                }));
                setNursingDataset(transformed);
            } catch (err) {
                console.error('Failed to load nursing units:', err);
                const errorMessage = err?.response?.data?.message || err?.message || 'Unknown error';
                const errorDetails = err?.response?.data?.details || '';
                setError(`${errorMessage}${errorDetails ? ': ' + errorDetails : ''}`);
                // Fallback to empty array on error
                setNursingDataset([]);
            } finally {
                setLoading(false);
            }
        };

        loadNursingUnits();
    }, []);

    const handleSeedData = async () => {
        try {
            setSeeding(true);
            setError(null);
            console.log('🌱 Seeding nursing units data...');
            const response = await apiClient.post('/NursingUnits/seed');
            console.log('✅ Data seeded successfully:', response.data);
            
            // Reload data after seeding
            const data = await fetchNursingUnits();
            const unitsArray = Array.isArray(data) ? data : (data.data || []);
            const transformed = unitsArray.map(item => ({
                id: item.unitId || item.id,
                unit: item.unit || item.unitName || '',
                wing: item.wing || '',
                lead: item.lead || item.leadNurse || '',
                nurses: item.nurses || item.totalNurses || 0,
                coverage: item.coverage || '',
                ratio: item.ratio || '',
                focus: item.focus || item.focusAreas || ''
            }));
            setNursingDataset(transformed);
        } catch (err) {
            console.error('❌ Error seeding data:', err);
            const errorMessage = err?.response?.data?.message || err?.message || 'Unknown error';
            setError(`Failed to seed data: ${errorMessage}`);
        } finally {
            setSeeding(false);
        }
    };

    const filteredDataset = useMemo(() => {
        if (loading || !nursingDataset || nursingDataset.length === 0) return [];
        
        const normalized = query.trim().toLowerCase();
        if (!normalized) return nursingDataset;
        
        return nursingDataset.filter((item) => {
            return [
                item.unit,
                item.wing,
                item.lead,
                item.focus,
                item.coverage,
                item.ratio,
                item.id
            ].some((field) => field && field.toLowerCase().includes(normalized));
        });
    }, [query, nursingDataset, loading]);

    const unitColors = [
        { gradient: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', accent: '#6a6ff5', icon: '🏥' },
        { gradient: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', accent: '#f5576c', icon: '🩺' },
        { gradient: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', accent: '#25c6ff', icon: '💙' },
        { gradient: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', accent: '#30cf86', icon: '⚡' },
        { gradient: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', accent: '#f47b68', icon: '👶' },
        { gradient: 'linear-gradient(135deg, #fed6e3 0%, #a8edea 100%)', accent: '#fb9ac7', icon: '💡' }
    ];

    return (
        <div className="nursing-page">
            <section className="nursing-hero">
                <div className="nursing-hero-badge">{t('nursingPage.badge')}</div>
                <h1>{t('nursingPage.title')}</h1>
                <p className="nursing-hero-subtitle">{t('nursingPage.subtitle')}</p>
                <div className="nursing-hero-actions">
                    <button className="nursing-btn primary" onClick={() => navigate('/appointments/add')}>
                        {t('nursingPage.cta.primary')}
                    </button>
                    <button className="nursing-btn ghost" onClick={() => navigate('/departments')}>
                        {t('nursingPage.cta.secondary')}
                    </button>
                </div>
                <div className="nursing-hero-metrics">
                    <div>
                        <span>{t('nursingPage.metrics.coverage.value')}</span>
                        <p>{t('nursingPage.metrics.coverage.label')}</p>
                    </div>
                    <div>
                        <span>{t('nursingPage.metrics.response.value')}</span>
                        <p>{t('nursingPage.metrics.response.label')}</p>
                    </div>
                    <div>
                        <span>{t('nursingPage.metrics.satisfaction.value')}</span>
                        <p>{t('nursingPage.metrics.satisfaction.label')}</p>
                    </div>
                </div>
            </section>

            <section id="care-promise" className="nursing-section-card">
                <div className="section-header">
                    <div className="section-chip">{t('nursingPage.promise.label')}</div>
                    <h2>{t('nursingPage.promise.title')}</h2>
                    <p>{t('nursingPage.promise.desc')}</p>
                </div>
                <div className="nursing-pillars-grid">
                    {nursingPillars.map((pill) => (
                        <div className="nursing-pillars-card" key={pill.title}>
                            <span>{pill.icon}</span>
                            <h3>{pill.title}</h3>
                            <p>{pill.desc}</p>
                        </div>
                    ))}
                </div>
            </section>

            <section className="nursing-programs">
                <div className="section-header compact">
                    <div className="section-chip">{t('nursingPage.programs.label')}</div>
                    <h2>{t('nursingPage.programs.title')}</h2>
                    <p>{t('nursingPage.programs.desc')}</p>
                </div>
                <div className="nursing-programs-grid">
                    {nursingPrograms.map((program) => (
                        <div key={program.title} className="nursing-program-card">
                            <h3>{program.title}</h3>
                            <p>{program.desc}</p>
                        </div>
                    ))}
                </div>
            </section>

            <section className="nursing-support">
                <div className="support-card">
                    <h3>{t('nursingPage.support.title')}</h3>
                    <p>{t('nursingPage.support.desc')}</p>
                    <div className="support-stats">
                        {supportChannels.map((channel) => (
                            <div key={channel.label}>
                                <span>{channel.value}</span>
                                <p>{channel.label}</p>
                            </div>
                        ))}
                    </div>
                </div>
                <div className="support-cta">
                    <h3>{t('nursingPage.support.ctaTitle')}</h3>
                    <p>{t('nursingPage.support.ctaDesc')}</p>
                    <button className="nursing-btn primary" onClick={() => navigate('/appointments/add')}>
                        {t('nursingPage.support.ctaButton')}
                    </button>
                </div>
            </section>

            <section className="nursing-dataset">
                <div className="section-header">
                    <div className="section-chip">{t('nursingPage.dataset.label')}</div>
                    <h2>{t('nursingPage.dataset.title')}</h2>
                    <p>{t('nursingPage.dataset.desc')}</p>
                </div>

                <div className="dataset-actions">
                    <input 
                        type="text" 
                        value={query}
                        onChange={(e) => setQuery(e.target.value)}
                        placeholder={t('nursingPage.dataset.searchPlaceholder')}
                        disabled={loading}
                    />
                </div>

                {loading && (
                    <div className="dataset-empty" style={{ padding: '40px', textAlign: 'center' }}>
                        <div>{t('common.loading')}...</div>
                        <div style={{ fontSize: '14px', color: '#666', marginTop: '10px' }}>
                            Loading nursing units from API...
                        </div>
                    </div>
                )}

                {error && !loading && (
                    <div className="dataset-empty" style={{ padding: '40px', textAlign: 'center', background: '#fff5f5', border: '1px solid #ff7a7a', borderRadius: '8px' }}>
                        <div style={{ color: '#ff7a7a', fontWeight: 'bold', marginBottom: '10px' }}>
                            ⚠️ Error loading data
                        </div>
                        <div style={{ fontSize: '14px', color: '#666' }}>
                            {error}
                        </div>
                        <div style={{ fontSize: '12px', color: '#999', marginTop: '15px' }}>
                            <p>Possible solutions:</p>
                            <ul style={{ textAlign: 'left', display: 'inline-block', marginTop: '10px' }}>
                                <li>Check if the table exists: <code>GET /api/NursingUnits/check-table</code></li>
                                <li>Load data: <code>POST /api/NursingUnits/seed</code></li>
                                <li>Check backend console for errors</li>
                            </ul>
                        </div>
                    </div>
                )}

                {!loading && !error && filteredDataset.length === 0 && nursingDataset.length === 0 && (
                    <div className="dataset-empty" style={{ 
                        padding: '40px', 
                        textAlign: 'center',
                        background: '#f8f9fa',
                        border: '2px dashed #dee2e6',
                        borderRadius: '12px',
                        margin: '20px 0'
                    }}>
                        <div style={{ fontSize: '18px', fontWeight: 'bold', marginBottom: '10px', color: '#495057' }}>
                            📋 No nursing units found
                        </div>
                        <div style={{ fontSize: '14px', color: '#666', marginBottom: '20px' }}>
                            The database is empty. Click the button below to load sample data.
                        </div>
                        <button 
                            onClick={handleSeedData}
                            disabled={seeding}
                            style={{
                                padding: '12px 24px',
                                fontSize: '16px',
                                fontWeight: '600',
                                color: '#fff',
                                background: seeding ? '#6c757d' : 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                                border: 'none',
                                borderRadius: '8px',
                                cursor: seeding ? 'not-allowed' : 'pointer',
                                boxShadow: '0 4px 6px rgba(0,0,0,0.1)',
                                transition: 'all 0.3s ease'
                            }}
                            onMouseEnter={(e) => {
                                if (!seeding) {
                                    e.target.style.transform = 'translateY(-2px)';
                                    e.target.style.boxShadow = '0 6px 12px rgba(0,0,0,0.15)';
                                }
                            }}
                            onMouseLeave={(e) => {
                                if (!seeding) {
                                    e.target.style.transform = 'translateY(0)';
                                    e.target.style.boxShadow = '0 4px 6px rgba(0,0,0,0.1)';
                                }
                            }}
                        >
                            {seeding ? '🔄 Loading...' : '🌱 Load Sample Data'}
                        </button>
                        {seeding && (
                            <div style={{ fontSize: '12px', color: '#999', marginTop: '10px' }}>
                                This will add 12 nursing units to the database...
                            </div>
                        )}
                    </div>
                )}

                {!loading && !error && filteredDataset.length === 0 && nursingDataset.length > 0 && (
                    <div className="dataset-empty" style={{ padding: '40px', textAlign: 'center' }}>
                        {query ? `No results found for "${query}"` : t('nursingPage.dataset.emptyState', { query })}
                    </div>
                )}

                <div className="nursing-units-grid">
                    {filteredDataset.map((item, index) => {
                        const color = unitColors[index % unitColors.length];
                        const focusTags = item.focus.split(';').map(tag => tag.trim()).filter(Boolean);
                        return (
                            <article 
                                key={item.id} 
                                className="nursing-unit-card" 
                                style={{ 
                                    '--accent-gradient': color.gradient,
                                    '--accent-color': color.accent
                                }}
                            >
                                <div className="nursing-unit-card__icon">
                                    {color.icon}
                                </div>
                                <h3>{item.unit}</h3>
                                <p className="nursing-unit-card__location">{item.wing}</p>
                                <p className="nursing-unit-card__id">{item.id}</p>

                                <div className="nursing-unit-card__meta">
                                    <div>
                                        <span className="meta-label">{t('nursingPage.dataset.columns.lead')}</span>
                                        <strong>{item.lead}</strong>
                                    </div>
                                    <div>
                                        <span className="meta-label">{t('nursingPage.dataset.columns.nurses')}</span>
                                        <strong>{item.nurses}</strong>
                                    </div>
                                </div>

                                <div className="nursing-unit-card__chips">
                                    <span>{item.coverage}</span>
                                    <span>{item.ratio}</span>
                                </div>

                                <div className="nursing-unit-card__focus">
                                    {focusTags.map((tag) => (
                                        <span key={tag}>{tag}</span>
                                    ))}
                                </div>
                            </article>
                        );
                    })}
                </div>
            </section>
        </div>
    );
}

