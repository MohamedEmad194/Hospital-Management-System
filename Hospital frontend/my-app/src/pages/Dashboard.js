import React, { useEffect, useState, useContext, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import apiClient from '../api/client';
import { fetchFeatures } from '../api/features';
import { useTranslation } from 'react-i18next';
import { AuthContext } from '../context/AuthContext';

export default function Dashboard() {
    const { t, i18n } = useTranslation();
    const { user } = useContext(AuthContext);
    const navigate = useNavigate();
    const [stats, setStats] = useState(null);
    const [features, setFeatures] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const statsSectionRef = useRef(null);

    useEffect(() => {
        let cancelled = false;
        (async () => {
            setLoading(true);
            try {
                const [statsData, featuresData] = await Promise.all([
                    apiClient.get('/Dashboard/stats'),
                    fetchFeatures(i18n.language)
                ]);
                if (!cancelled) {
                    setStats(statsData.data);
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
                        errorMessage = i18n.language === 'ar' 
                            ? 'يجب تسجيل الدخول لعرض الإحصائيات' 
                            : 'Please login to view statistics';
                    } else if (err?.message?.includes('Network Error') || err?.code === 'ECONNREFUSED') {
                        errorMessage = i18n.language === 'ar' 
                            ? 'لا يمكن الاتصال بالخادم. تأكد من تشغيل الباك إند.' 
                            : 'Cannot connect to server. Please ensure the backend is running.';
                    } else if (err?.response?.status === 500) {
                        errorMessage = i18n.language === 'ar' 
                            ? 'خطأ في الخادم. يرجى المحاولة لاحقاً.' 
                            : 'Server error. Please try again later.';
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
    }, [i18n.language, t]);

    if (loading) return <div style={{ padding: 24 }}>{t('dashboard.loading')}</div>;
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
                    {i18n.language === 'ar' 
                        ? 'تأكد من أن الباك إند يعمل على http://localhost:5230' 
                        : 'Please ensure the backend is running on http://localhost:5230'}
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
                    {i18n.language === 'ar' ? 'إعادة المحاولة' : 'Retry'}
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

    return (
        <div className="dashboard-page" style={{ 
            minHeight: '100vh', 
            background: '#f8f9fa',
            padding: '0'
        }}>
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
                                allowedRoles: ['Admin', 'Doctor']
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
                                allowedRoles: ['Admin', 'Doctor', 'Patient']
                            },
                            { 
                                label: t('dashboard.addBill'), 
                                icon: '💰', 
                                color: '#9C27B0', 
                                desc: t('dashboard.addBillDesc'),
                                path: '/bills/add',
                                requiresAuth: true,
                                allowedRoles: ['Admin', 'Doctor']
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


