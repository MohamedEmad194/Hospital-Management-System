import React, { useEffect, useState, useContext, useRef, useMemo } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../api/client';
import { fetchFeatures } from '../api/features';
import { useTranslation } from 'react-i18next';
import { AuthContext } from '../context/AuthContext';
import { SkeletonStats, SkeletonGrid } from '../components/Skeleton';

export default function Dashboard() {
    const { t, i18n } = useTranslation();
    const { user } = useContext(AuthContext);
    const navigate = useNavigate();
    const [stats, setStats] = useState(null);
    const [overview, setOverview] = useState(null);
    const [features, setFeatures] = useState([]);
    const [availableRoomsCount, setAvailableRoomsCount] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const statsSectionRef = useRef(null);

    const isStaff = !!user?.roles?.some((r) => r === 'Staff' || r === 'Nurse');
    const isAdmin = !!user?.roles?.includes('Admin');
    const showStaffPanel = isStaff && !isAdmin;

    useEffect(() => {
        let cancelled = false;
        (async () => {
            setLoading(true);
            try {
                const [statsData, overviewData, featuresData, roomsData] = await Promise.all([
                    apiClient.get('/Dashboard/stats'),
                    apiClient.get('/Dashboard/overview').catch(() => null),
                    fetchFeatures(i18n.language),
                    showStaffPanel
                        ? apiClient.get('/Rooms/available').catch(() => null)
                        : Promise.resolve(null)
                ]);
                if (!cancelled && roomsData?.data) {
                    setAvailableRoomsCount(
                        Array.isArray(roomsData.data) ? roomsData.data.length : 0
                    );
                }
                if (!cancelled) {
                    setStats(statsData.data);
                    setOverview(overviewData?.data || null);
                    // Use features from API if available, otherwise use fallback
                    if (featuresData && featuresData.length > 0) {
                        setFeatures(featuresData);
                    } else {
                        // Use fallback features if API returns empty array
                        setFeatures([
                            {
                                icon: '⚡',
                                title: t('dashboard.features.quickCare.title'),
                                desc: t('dashboard.features.quickCare.desc'),
                                color: '#FFD700'
                            },
                            {
                                icon: '🎯',
                                title: t('dashboard.features.highPrecision.title'),
                                desc: t('dashboard.features.highPrecision.desc'),
                                color: '#00CED1'
                            },
                            {
                                icon: '💚',
                                title: t('dashboard.features.comprehensiveCare.title'),
                                desc: t('dashboard.features.comprehensiveCare.desc'),
                                color: '#32CD32'
                            }
                        ]);
                    }
                }
            } catch (err) {
                if (!cancelled) {
                    console.error('Dashboard error:', err);
                    let errorMessage = t('dashboard.failed');
                    
                    // Provide more specific error messages
                    if (err?.response?.status === 401) {
                        errorMessage = t('dashboard.errors.authRequired');
                    } else if (err?.message?.includes('Network Error') || err?.code === 'ECONNREFUSED') {
                        errorMessage = t('dashboard.errors.backendDown');
                    } else if (err?.response?.status === 500) {
                        errorMessage = t('dashboard.errors.serverError');
                    } else if (err?.response?.data) {
                        errorMessage = typeof err.response.data === 'string' 
                            ? err.response.data 
                            : err.response.data.message || errorMessage;
                    }
                    
                    setError(errorMessage);
                    
                    // If features API fails, use fallback data
                    try {
                        const fallbackFeatures = await fetchFeatures(i18n.language).catch(() => null);
                        if (fallbackFeatures) {
                            setFeatures(fallbackFeatures);
                        } else {
                            setFeatures([
                                { 
                                    icon: '⚡', 
                                    title: t('dashboard.features.quickCare.title'),
                                    desc: t('dashboard.features.quickCare.desc'),
                                    color: '#FFD700'
                                },
                                { 
                                    icon: '🎯', 
                                    title: t('dashboard.features.highPrecision.title'),
                                    desc: t('dashboard.features.highPrecision.desc'),
                                    color: '#00CED1'
                                },
                                { 
                                    icon: '💚', 
                                    title: t('dashboard.features.comprehensiveCare.title'),
                                    desc: t('dashboard.features.comprehensiveCare.desc'),
                                    color: '#32CD32'
                                }
                            ]);
                        }
                    } catch {
                        // Use fallback features if API fails
                        setFeatures([
                            { 
                                icon: '⚡', 
                                title: t('dashboard.features.quickCare.title'),
                                desc: t('dashboard.features.quickCare.desc'),
                                color: '#FFD700'
                            },
                            { 
                                icon: '🎯', 
                                title: t('dashboard.features.highPrecision.title'),
                                desc: t('dashboard.features.highPrecision.desc'),
                                color: '#00CED1'
                            },
                            { 
                                icon: '💚', 
                                title: t('dashboard.features.comprehensiveCare.title'),
                                desc: t('dashboard.features.comprehensiveCare.desc'),
                                color: '#32CD32'
                            }
                        ]);
                    }
                }
            } finally {
                if (!cancelled) setLoading(false);
            }
        })();
        return () => { cancelled = true; };
    }, [i18n.language, t, showStaffPanel]);

    const statusBreakdown = useMemo(() => {
        const statusPalette = {
            Scheduled: '#3b82f6',
            Confirmed: '#0ea5e9',
            Completed: '#22c55e',
            Cancelled: '#ef4444',
            InProgress: '#f59e0b',
            NoShow: '#94a3b8'
        };
        const raw = overview?.appointmentsByStatus || [];
        const total = raw.reduce((sum, s) => sum + (s.count || 0), 0);
        return raw
            .map((s) => ({
                ...s,
                share: total > 0 ? Math.round((s.count / total) * 100) : 0,
                color: statusPalette[s.status] || '#6366f1'
            }))
            .sort((a, b) => b.count - a.count);
    }, [overview]);

    if (loading) {
        return (
            <div style={{ padding: 24, maxWidth: 1400, margin: '0 auto' }}>
                <div style={{ marginBottom: 30 }}>
                    <SkeletonStats count={8} />
                </div>
                <SkeletonGrid count={6} rows={3} />
            </div>
        );
    }
    if (error) {
        return (
            <div style={{ 
                color: '#b00020',
                textAlign: 'center',
                maxWidth: '600px',
                margin: '50px auto',
                background: '#fff',
                borderRadius: '10px',
                boxShadow: '0 2px 8px rgba(0,0,0,0.1)',
                padding: '40px'
            }}>
                <div style={{ fontSize: '3rem', marginBottom: '20px' }}>⚠️</div>
                <h2 style={{ color: '#b00020', marginBottom: '15px' }}>{error}</h2>
                <p style={{ color: '#666', marginBottom: '20px' }}>
                    {t('dashboard.errors.backendHint')}
                </p>
                <button 
                    onClick={() => {
                        setError('');
                        setLoading(true);
                        window.location.reload();
                    }}
                    style={{
                        padding: '12px 24px',
                        background: '#667eea',
                        color: 'white',
                        border: 'none',
                        borderRadius: '8px',
                        cursor: 'pointer',
                        fontSize: '1rem',
                        fontWeight: '600'
                    }}
                >
                    {t('dashboard.errors.retry')}
                </button>
            </div>
        );
    }
    if (!stats) return null;

    // Medical icons for different categories
    const getIcon = (label) => {
        const iconMap = {
            [t('dashboard.items.patients')]: '👥',
            [t('dashboard.items.doctors')]: '👨‍⚕️',
            [t('dashboard.items.appointments')]: '📅',
            [t('dashboard.items.departments')]: '🏥',
            [t('dashboard.items.rooms')]: '🏠',
            [t('dashboard.items.medicines')]: '💊',
            [t('dashboard.items.bills')]: '💰',
            [t('dashboard.items.pendingAppointments')]: '⏳',
            [t('dashboard.items.completedAppointments')]: '✅',
            [t('dashboard.items.overdueBills')]: '⚠️',
            [t('dashboard.items.lowStockMedicines')]: '📉',
        };
        return iconMap[label] || '📊';
    };

    const getColor = (label) => {
        const colorMap = {
            [t('dashboard.items.patients')]: '#4CAF50',
            [t('dashboard.items.doctors')]: '#2196F3',
            [t('dashboard.items.appointments')]: '#FF9800',
            [t('dashboard.items.departments')]: '#9C27B0',
            [t('dashboard.items.rooms')]: '#607D8B',
            [t('dashboard.items.medicines')]: '#E91E63',
            [t('dashboard.items.bills')]: '#795548',
            [t('dashboard.items.pendingAppointments')]: '#FF5722',
            [t('dashboard.items.completedAppointments')]: '#4CAF50',
            [t('dashboard.items.overdueBills')]: '#F44336',
            [t('dashboard.items.lowStockMedicines')]: '#FF9800',
        };
        return colorMap[label] || '#2196F3';
    };

    const items = [
        { label: t('dashboard.items.patients'), value: stats.totalPatients },
        { label: t('dashboard.items.doctors'), value: stats.totalDoctors },
        { label: t('dashboard.items.appointments'), value: stats.totalAppointments },
        { label: t('dashboard.items.departments'), value: stats.totalDepartments },
        { label: t('dashboard.items.rooms'), value: stats.totalRooms },
        { label: t('dashboard.items.medicines'), value: stats.totalMedicines },
        { label: t('dashboard.items.bills'), value: stats.totalBills },
        { label: t('dashboard.items.pendingAppointments'), value: stats.pendingAppointments },
        { label: t('dashboard.items.completedAppointments'), value: stats.completedAppointments },
        { label: t('dashboard.items.overdueBills'), value: stats.overdueBills },
        { label: t('dashboard.items.lowStockMedicines'), value: stats.lowStockMedicines },
    ];

    const currency = t('common.currency');
    const formatCurrency = (value) => {
        const num = Number(value || 0);
        const formatted = num.toLocaleString(i18n.language === 'ar' ? 'ar-EG' : 'en-US', {
            maximumFractionDigits: 0
        });
        return `${formatted} ${currency}`;
    };
    const formatNumber = (value) =>
        Number(value || 0).toLocaleString(i18n.language === 'ar' ? 'ar-EG' : 'en-US');

    const topDepartments = overview?.topDepartments || [];
    const monthlyTrend = overview?.monthlyAppointments || [];
    const trendMax = Math.max(1, ...monthlyTrend.map((m) => m.count || 0));
    const topDeptMax = Math.max(1, ...topDepartments.map((d) => d.appointmentCount || 0));

    const highlightCards = overview
        ? [
              {
                  key: 'totalRevenue',
                  icon: '💵',
                  title: t('dashboard.highlights.revenueTotal'),
                  value: formatCurrency(overview.revenue?.total),
                  hint: t('dashboard.highlights.revenueTotalHint'),
                  tone: 'linear-gradient(135deg, #10b981 0%, #34d399 100%)'
              },
              {
                  key: 'monthRevenue',
                  icon: '📈',
                  title: t('dashboard.highlights.revenueMonth'),
                  value: formatCurrency(overview.revenue?.thisMonth),
                  hint: t('dashboard.highlights.revenueMonthHint'),
                  tone: 'linear-gradient(135deg, #2563eb 0%, #60a5fa 100%)'
              },
              {
                  key: 'pendingRevenue',
                  icon: '⏳',
                  title: t('dashboard.highlights.revenuePending'),
                  value: formatCurrency(overview.revenue?.pending),
                  hint: t('dashboard.highlights.revenuePendingHint'),
                  tone: 'linear-gradient(135deg, #f97316 0%, #fb923c 100%)'
              },
              {
                  key: 'todayAppointments',
                  icon: '📅',
                  title: t('dashboard.highlights.todayAppointments'),
                  value: formatNumber(overview.today?.appointments),
                  hint: t('dashboard.highlights.todayAppointmentsHint'),
                  tone: 'linear-gradient(135deg, #8b5cf6 0%, #c084fc 100%)'
              }
          ]
        : [];

    const aboutContent = t('dashboard.about', { returnObjects: true }) || {};
    const aboutAreas = t('dashboard.about.areas', { returnObjects: true }) || {};
    const aboutCards = [
        { 
            key: 'wards', 
            image: '/images/about/ward-bays.jpg',
            title: 'Open recovery ward',
            desc: 'Blue duvets, pastel curtains and daylight keep the bay calm yet observable.',
            detail: 'Overhead rails let nurses move monitors without disturbing patients.',
            metricLabel: 'Ward B3',
            metricLabelShort: 'beds ready',
            metricValue: '32'
        },
        { 
            key: 'reception', 
            image: '/images/about/reception-lobby.jpg',
            title: 'Reception & triage lobby',
            desc: 'Curved front desk and bilingual signage direct arrivals in seconds.',
            detail: 'Direct sightlines to elevators speed up urgent transfers.',
            metricLabel: 'Arrival lobby',
            metricLabelShort: 'service desks',
            metricValue: '3'
        },
        { 
            key: 'privateSuite', 
            image: '/images/about/private-suite.jpg',
            title: 'Private inpatient suite',
            desc: 'Mint bedding and a sleeper sofa host families during long stays.',
            detail: 'Hidden headwall services keep the room hotel-quiet.',
            metricLabel: 'Family suite',
            metricLabelShort: 'comfort zones',
            metricValue: '2'
        },
        { 
            key: 'surgerySuite', 
            image: '/images/about/surgery-suite.jpg',
            title: 'Hybrid operating theatre',
            desc: 'Dual surgical lights and imaging screens wrap around the table.',
            detail: 'Laminar airflow tiles underpin infection control.',
            metricLabel: 'Hybrid OR',
            metricLabelShort: 'ceiling booms',
            metricValue: '4'
        },
        { 
            key: 'minimalOr', 
            image: '/images/about/operating-theatre.jpg',
            title: 'Day-surgery theatre',
            desc: 'Ultra-white suite made for minimally invasive cases.',
            detail: 'Mobile trolleys let the team reset between specialties.',
            metricLabel: 'Day surgery',
            metricLabelShort: 'avg. turnover',
            metricValue: '18 min'
        },
        { 
            key: 'corridor', 
            image: '/images/about/clinical-corridor.jpg',
            title: 'Critical care corridor',
            desc: 'Wide hallway keeps stretchers and staff moving toward ICU.',
            detail: 'Crash carts live on the wall so the path stays clear.',
            metricLabel: 'Critical path',
            metricLabelShort: 'm coverage',
            metricValue: '60'
        },
        { 
            key: 'exterior', 
            image: '/images/about/hospital-exterior.jpg',
            title: 'Main campus arrival',
            desc: 'Brick façade and porte-cochère protect all drop-offs.',
            detail: 'Separate lanes split visitor traffic from ambulances.',
            metricLabel: 'Main campus',
            metricLabelShort: 'acre site',
            metricValue: '12'
        }
    ];
    const aboutGallery = aboutCards.map(card => {
        const localized = aboutAreas?.[card.key] || {};
        return {
            ...card,
            title: localized.title || card.title,
            desc: localized.desc || card.desc,
            detail: localized.detail || card.detail,
            metricLabel: localized.metricLabel || card.metricLabel,
            metricLabelShort: localized.metricLabelShort || localized.metricLabel || card.metricLabelShort,
            metricValue: localized.metricValue || card.metricValue
        };
    });

    const handleBookAppointment = () => {
        navigate('/appointments/add');
    };

    const handleScrollToStats = () => {
        if (statsSectionRef.current) {
            statsSectionRef.current.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
    };

    const staffFirstName = user?.firstName || user?.email?.split('@')[0] || '';
    const todayLabel = new Date().toLocaleDateString(
        i18n.language === 'ar' ? 'ar-EG' : 'en-US',
        { weekday: 'long', day: 'numeric', month: 'long' }
    );
    const staffTiles = showStaffPanel
        ? [
              {
                  key: 'appointments',
                  icon: '📅',
                  value: formatNumber(overview?.today?.appointments ?? 0),
                  label: t('dashboard.staffPanel.todaysAppointments'),
                  link: t('dashboard.staffPanel.viewAll'),
                  path: '/appointments'
              },
              {
                  key: 'rooms',
                  icon: '🚪',
                  value: formatNumber(availableRoomsCount ?? 0),
                  label: t('dashboard.staffPanel.availableRooms'),
                  link: t('dashboard.staffPanel.manage'),
                  path: '/rooms'
              },
              {
                  key: 'patients',
                  icon: '👥',
                  value: formatNumber(stats?.totalPatients ?? 0),
                  label: t('dashboard.staffPanel.totalPatients'),
                  link: t('dashboard.staffPanel.viewAll'),
                  path: '/patients'
              },
              {
                  key: 'bills',
                  icon: '🧾',
                  value: formatNumber(overview?.today?.billsIssued ?? 0),
                  label: t('dashboard.staffPanel.todaysBills'),
                  link: t('dashboard.staffPanel.viewAll'),
                  path: '/bills'
              }
          ]
        : [];

    return (
        <div className="dashboard-page" style={{
            minHeight: '100vh',
            background: '#f8f9fa',
            padding: '0'
        }}>
            {/* Staff Welcome Panel (Staff role only) */}
            {showStaffPanel && (
                <section className="staff-panel" aria-label="Staff overview">
                    <div className="staff-panel__card">
                        <div className="staff-panel__head">
                            <div>
                                <h1 className="staff-panel__welcome">
                                    {t('dashboard.staffPanel.welcome', { name: staffFirstName })}
                                </h1>
                                <p className="staff-panel__subtitle">
                                    {t('dashboard.staffPanel.subtitle')}
                                </p>
                            </div>
                            <span className="staff-panel__date">
                                {t('dashboard.staffPanel.today', { date: todayLabel })}
                            </span>
                        </div>
                        <div className="staff-panel__grid">
                            {staffTiles.map((tile) => (
                                <button
                                    key={tile.key}
                                    type="button"
                                    className="staff-panel__tile"
                                    onClick={() => navigate(tile.path)}
                                >
                                    <span className="staff-panel__tile-icon" aria-hidden="true">
                                        {tile.icon}
                                    </span>
                                    <span className="staff-panel__tile-value">{tile.value}</span>
                                    <span className="staff-panel__tile-label">{tile.label}</span>
                                    <span className="staff-panel__tile-link">{tile.link} →</span>
                                </button>
                            ))}
                        </div>
                    </div>
                </section>
            )}

            {/* Hero Banner Section */}
            <div
                className="dashboard-hero"
                style={{ backgroundImage: 'url(/hospital-building.jpg)' }}
            >
                <div className="dashboard-hero__overlay" />
                <div className="dashboard-hero__frame" />
            </div>

            {/* Hero Details Section */}
            <section className="dashboard-hero-card-wrapper">
                <div className="dashboard-hero-card">
                    <div style={{
                        width: 160,
                        height: 160,
                        margin: '0 auto 24px',
                        display: 'flex',
                        alignItems: 'center',
                        justifyContent: 'center'
                    }}>
                        <img 
                            src={`${process.env.PUBLIC_URL}/hospital-logo.png`}
                            alt="Al-Hayat Hospital Logo"
                            style={{
                                width: '100%',
                                height: '100%',
                                objectFit: 'contain',
                                filter: 'drop-shadow(0 10px 25px rgba(0, 0, 0, 0.25))',
                                imageRendering: 'crisp-edges'
                            }}
                            onError={(e) => {
                                e.target.src = `${process.env.PUBLIC_URL}/logo.png`;
                            }}
                        />
                    </div>
                    <div className="dashboard-hero-card__text">
                        <h1>{t('dashboard.hospitalName')}</h1>
                        <p>{t('dashboard.description')}</p>
                    </div>
                    <div className="dashboard-hero-card__buttons">
                        <button
                            type="button"
                            onClick={handleScrollToStats}
                            className="pill-btn pill-btn--ghost"
                        >
                            <span aria-hidden="true">ℹ️</span>
                            <span>{t('dashboard.statistics')}</span>
                        </button>
                        <button
                            type="button"
                            onClick={handleBookAppointment}
                            className="pill-btn pill-btn--primary"
                        >
                            <span aria-hidden="true">📅</span>
                            <span>{t('dashboard.bookAppointment')}</span>
                        </button>
                    </div>
                    {features.length > 0 && (
                        <div className="dashboard-feature-grid">
                            {features.map((feature, index) => (
                                <article
                                    key={`${feature.title}-${index}`}
                                    className="dashboard-feature-card"
                                >
                                    <span
                                        className="dashboard-feature-card__icon"
                                        aria-hidden="true"
                                    >
                                        {feature.icon || '⭐'}
                                    </span>
                                    <h3>{feature.title || ''}</h3>
                                    <p>{feature.desc || feature.description || ''}</p>
                                </article>
                            ))}
                        </div>
                    )}
                </div>
            </section>

            {/* About Hospital Section */}
            <section className="dashboard-about">
                <div className="dashboard-about__glow" aria-hidden="true" />
                <div className="dashboard-about__content">
                    <div className="dashboard-about__head">
                        <span className="dashboard-about__badge">
                            <span role="img" aria-hidden="true">
                                🏥
                            </span>
                            {aboutContent.badge || t('dashboard.title')}
                        </span>
                        <h2>{aboutContent.title || ''}</h2>
                        <p>{aboutContent.description || ''}</p>
                    </div>
                    <div className="dashboard-about__grid">
                        {aboutGallery.map((card) => (
                            <article key={card.key} className="dashboard-about__card">
                                <div className="dashboard-about__media">
                                    <img 
                                        src={card.image} 
                                        alt={card.title} 
                                        loading="lazy"
                                    />
                                    <span className="dashboard-about__chip">
                                        {card.metricLabel || ''}
                                    </span>
                                </div>
                                <div className="dashboard-about__body">
                                    <div>
                                        <h3>{card.title}</h3>
                                        <p>{card.desc || ''}</p>
                                    </div>
                                    <p className="dashboard-about__detail">
                                        {card.detail || ''}
                                    </p>
                                    <div className="dashboard-about__metrics">
                                        <span>{card.metricValue || ''}</span>
                                        <small>
                                            {card.metricLabelShort ||
                                                card.metricLabel ||
                                                ''}
                                        </small>
                                    </div>
                                </div>
                            </article>
                        ))}
                    </div>
                </div>
            </section>
            
            {/* Main Content */}
            <div className="dashboard-content">
                {/* Stats Section */}
                <section className="dashboard-section" ref={statsSectionRef}>
                    <div className="section-header">
                        <div className="section-chip">
                            <span aria-hidden="true">📊</span>
                        </div>
                        <h2>{t('dashboard.statistics')}</h2>
                        <p>{t('dashboard.statisticsDesc')}</p>
                    </div>
                    <div className="dashboard-grid">
                        {items.map((item) => (
                            <div key={item.label} className="stats-card">
                                <div className="stats-card__head">
                                    <div
                                        className="stats-card__icon"
                                        style={{
                                            background: `linear-gradient(135deg, ${getColor(
                                                item.label
                                            )}, ${getColor(item.label)}CC)`,
                                            boxShadow: `0 4px 12px ${getColor(
                                                item.label
                                            )}40`,
                                        }}
                                        aria-hidden="true"
                                    >
                                        {getIcon(item.label)}
                                    </div>
                                    <div
                                        className="stats-card__value"
                                        style={{ color: getColor(item.label) }}
                                    >
                                        {item.value}
                                    </div>
                                </div>
                                <div className="stats-card__label">{item.label}</div>
                            </div>
                        ))}
                </div>
                </section>

                {overview && (
                    <section className="dashboard-section dashboard-insights">
                        <div className="section-header">
                            <div className="section-chip">
                                <span aria-hidden="true">📊</span>
                            </div>
                            <h2>{t('dashboard.insights.title')}</h2>
                            <p>{t('dashboard.insights.desc')}</p>
                        </div>

                        <div className="dashboard-highlights">
                            {highlightCards.map((card) => (
                                <article
                                    key={card.key}
                                    className="dashboard-highlight"
                                    style={{ '--highlight-tone': card.tone }}
                                >
                                    <div className="dashboard-highlight__icon" aria-hidden="true">
                                        {card.icon}
                                    </div>
                                    <div className="dashboard-highlight__body">
                                        <span className="dashboard-highlight__label">
                                            {card.title}
                                        </span>
                                        <strong className="dashboard-highlight__value">
                                            {card.value}
                                        </strong>
                                        <small className="dashboard-highlight__hint">
                                            {card.hint}
                                        </small>
                                    </div>
                                </article>
                            ))}
                        </div>

                        <div className="dashboard-insights__grid">
                            <article className="dashboard-panel">
                                <header className="dashboard-panel__head">
                                    <h3>{t('dashboard.insights.statusTitle')}</h3>
                                    <span>{t('dashboard.insights.statusSubtitle')}</span>
                                </header>
                                {statusBreakdown.length === 0 ? (
                                    <p className="dashboard-panel__empty">
                                        {t('dashboard.insights.empty')}
                                    </p>
                                ) : (
                                    <ul className="dashboard-status-list">
                                        {statusBreakdown.map((item) => (
                                            <li key={item.status}>
                                                <div className="dashboard-status-list__head">
                                                    <span
                                                        className="dashboard-status-list__dot"
                                                        style={{ background: item.color }}
                                                    />
                                                    <span className="dashboard-status-list__label">
                                                        {item.status}
                                                    </span>
                                                    <span className="dashboard-status-list__value">
                                                        {formatNumber(item.count)} · {item.share}%
                                                    </span>
                                                </div>
                                                <div className="dashboard-bar">
                                                    <div
                                                        className="dashboard-bar__fill"
                                                        style={{
                                                            width: `${item.share}%`,
                                                            background: item.color
                                                        }}
                                                    />
                                                </div>
                                            </li>
                                        ))}
                                    </ul>
                                )}
                            </article>

                            <article className="dashboard-panel">
                                <header className="dashboard-panel__head">
                                    <h3>{t('dashboard.insights.topDeptsTitle')}</h3>
                                    <span>{t('dashboard.insights.topDeptsSubtitle')}</span>
                                </header>
                                {topDepartments.length === 0 ? (
                                    <p className="dashboard-panel__empty">
                                        {t('dashboard.insights.empty')}
                                    </p>
                                ) : (
                                    <ul className="dashboard-dept-list">
                                        {topDepartments.map((dept) => (
                                            <li key={dept.departmentName}>
                                                <div className="dashboard-dept-list__row">
                                                    <span className="dashboard-dept-list__name">
                                                        {dept.departmentName}
                                                    </span>
                                                    <span className="dashboard-dept-list__count">
                                                        {formatNumber(dept.appointmentCount)}
                                                    </span>
                                                </div>
                                                <div className="dashboard-bar">
                                                    <div
                                                        className="dashboard-bar__fill dashboard-bar__fill--accent"
                                                        style={{
                                                            width: `${
                                                                ((dept.appointmentCount || 0) /
                                                                    topDeptMax) *
                                                                100
                                                            }%`
                                                        }}
                                                    />
                                                </div>
                                                <small className="dashboard-dept-list__meta">
                                                    {formatNumber(dept.doctorCount)}{' '}
                                                    {t('dashboard.items.doctors')}
                                                </small>
                                            </li>
                                        ))}
                                    </ul>
                                )}
                            </article>

                            <article className="dashboard-panel dashboard-panel--wide">
                                <header className="dashboard-panel__head">
                                    <h3>{t('dashboard.insights.trendTitle')}</h3>
                                    <span>{t('dashboard.insights.trendSubtitle')}</span>
                                </header>
                                {monthlyTrend.length === 0 ? (
                                    <p className="dashboard-panel__empty">
                                        {t('dashboard.insights.empty')}
                                    </p>
                                ) : (
                                    <div className="dashboard-trend">
                                        {monthlyTrend.map((point) => {
                                            const height = Math.max(
                                                8,
                                                Math.round(((point.count || 0) / trendMax) * 100)
                                            );
                                            return (
                                                <div
                                                    key={`${point.year}-${point.month}`}
                                                    className="dashboard-trend__bar"
                                                >
                                                    <span className="dashboard-trend__value">
                                                        {formatNumber(point.count)}
                                                    </span>
                                                    <div
                                                        className="dashboard-trend__fill"
                                                        style={{ height: `${height}%` }}
                                                    />
                                                    <span className="dashboard-trend__label">
                                                        {point.label}
                                                    </span>
                                                </div>
                                            );
                                        })}
                                    </div>
                                )}
                            </article>

                            <article className="dashboard-panel dashboard-today">
                                <header className="dashboard-panel__head">
                                    <h3>{t('dashboard.insights.todayTitle')}</h3>
                                    <span>{t('dashboard.insights.todaySubtitle')}</span>
                                </header>
                                <div className="dashboard-today__grid">
                                    <div className="dashboard-today__cell">
                                        <span>📅</span>
                                        <strong>
                                            {formatNumber(overview.today?.appointments)}
                                        </strong>
                                        <small>
                                            {t('dashboard.items.appointments')}
                                        </small>
                                    </div>
                                    <div className="dashboard-today__cell">
                                        <span>🧑‍🤝‍🧑</span>
                                        <strong>
                                            {formatNumber(overview.today?.newPatients)}
                                        </strong>
                                        <small>
                                            {t('dashboard.insights.newPatients')}
                                        </small>
                                    </div>
                                    <div className="dashboard-today__cell">
                                        <span>🧾</span>
                                        <strong>
                                            {formatNumber(overview.today?.billsIssued)}
                                        </strong>
                                        <small>
                                            {t('dashboard.insights.billsIssued')}
                                        </small>
                                    </div>
                                </div>
                            </article>
                        </div>
                    </section>
                )}

                {/* Quick Actions Section */}
                <section className="quick-actions-section">
                    <div className="quick-actions-section__head">
                        <div className="quick-actions-section__badge">
                            <span aria-hidden="true">⚡</span>
                        </div>
                        <h3>{t('dashboard.quickActions')}</h3>
                        <p>{t('dashboard.quickActionsDesc')}</p>
                    </div>
                    <div className="quick-actions-grid">
                        {[
                            {
                                label: t('dashboard.addPatient'),
                                icon: '👥',
                                color: '#4CAF50',
                                desc: t('dashboard.addPatientDesc'),
                                path: '/patients/add',
                                requiresAuth: true,
                                allowedRoles: ['Admin', 'Doctor', 'Staff', 'Nurse']
                            },
                            {
                                label: t('dashboard.addDoctor'),
                                icon: '👨‍⚕️',
                                color: '#2196F3',
                                desc: t('dashboard.addDoctorDesc'),
                                path: '/doctors/add',
                                requiresAuth: true,
                                allowedRoles: ['Admin']
                            },
                            {
                                label: t('dashboard.bookAppointmentAction'),
                                icon: '📅',
                                color: '#FF9800',
                                desc: t('dashboard.bookAppointmentDesc'),
                                path: '/appointments/add',
                                requiresAuth: true,
                                allowedRoles: ['Admin', 'Doctor', 'Staff', 'Nurse', 'Patient']
                            },
                            {
                                label: t('dashboard.addBill'),
                                icon: '💰',
                                color: '#9C27B0',
                                desc: t('dashboard.addBillDesc'),
                                path: '/bills/add',
                                requiresAuth: true,
                                allowedRoles: ['Admin', 'Doctor', 'Staff', 'Nurse']
                            }
                        ].filter(action => {
                            // Show all actions, but filter based on user role if logged in
                            if (!action.requiresAuth) return true;
                            if (!user) return true; // Show all actions if not logged in
                            return action.allowedRoles.some(role => user?.roles?.includes(role));
                        }).map((action, index) => {
                            const actionColors = [
                                { bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', border: '#667eea' },
                                { bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', border: '#f5576c' },
                                { bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', border: '#4facfe' },
                                { bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', border: '#43e97b' }
                            ];
                            const actionColor = actionColors[index % actionColors.length];
                            
                            return (
                                <div
                                    key={index}
                                onClick={() => {
                                    if (action.requiresAuth && !user) {
                                        navigate('/login');
                                    } else if (action.requiresAuth && user) {
                                            const hasPermission = action.allowedRoles.some((role) =>
                                                user?.roles?.includes(role)
                                            );
                                        if (hasPermission) {
                                            navigate(action.path);
                                        } else {
                                            navigate('/login');
                                        }
                                    } else {
                                        navigate(action.path);
                                    }
                                }}
                                    className="quick-actions-card"
                                style={{
                                        '--quick-card-border': actionColor.border,
                                        '--quick-card-bg': actionColor.bg,
                                    }}
                                >
                                    <div className="quick-actions-card__icon">
                                        {action.icon}
                                    </div>
                                    <div className="quick-actions-card__title">
                                        {action.label}
                                    </div>
                                    <div className="quick-actions-card__desc">
                                        {action.desc}
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </section>

            </div>
        </div>
    );
}


