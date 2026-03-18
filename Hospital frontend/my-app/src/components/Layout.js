import React, { useContext, useEffect, useState } from 'react';
import { Link, NavLink, Outlet, useLocation } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import { useTranslation } from 'react-i18next';
import ChatbotWidget from './ChatbotWidget';

export default function Layout() {
    const { t, i18n } = useTranslation();
    const { user, logout } = useContext(AuthContext);
    const location = useLocation();
    const [menuOpen, setMenuOpen] = useState(false);
    const isArabic = i18n.language === 'ar';

    useEffect(() => {
        setMenuOpen(false);
    }, [location.pathname]);

    const navLinkClass = ({ isActive }) =>
        `layout-nav__link ${isActive ? 'is-active' : ''}`;

    const changeLanguage = (lng) => i18n.changeLanguage(lng);
    const navAriaLabel = t('common.navigation', { defaultValue: 'Navigation' });
    const openMenuLabel = t('common.openMenu', { defaultValue: 'Open menu' });
    const closeMenuLabel = t('common.closeMenu', { defaultValue: 'Close menu' });
    const changeLanguageLabel = t('common.changeLanguage', { defaultValue: 'Change language' });
    const handleToggleMenu = () => setMenuOpen((prev) => !prev);
    const roleLabel = user?.roles?.includes('Admin')
        ? 'مدير النظام'
        : user?.roles?.includes('Doctor')
        ? 'طبيب'
        : user?.roles?.includes('Patient')
        ? 'مريض'
        : 'مستخدم';

    const adminNav = (
        <>
            <NavLink to="/nursing" className={navLinkClass}>
                <span aria-hidden="true">🩺</span>
                {t('sidebar.nursing')}
            </NavLink>
            <NavLink to="/patients" className={navLinkClass}>
                <span aria-hidden="true">👥</span>
                {t('sidebar.patients')}
            </NavLink>
            <NavLink to="/doctors" className={navLinkClass}>
                <span aria-hidden="true">👨‍⚕️</span>
                {t('sidebar.doctors')}
            </NavLink>
            <NavLink to="/appointments" className={navLinkClass}>
                <span aria-hidden="true">📅</span>
                {t('sidebar.appointments')}
            </NavLink>
            <NavLink to="/bills" className={navLinkClass}>
                <span aria-hidden="true">💰</span>
                {t('sidebar.bills')}
            </NavLink>
            <NavLink to="/departments" className={navLinkClass}>
                <span aria-hidden="true">🏥</span>
                {t('sidebar.departments')}
            </NavLink>
            <NavLink to="/rooms" className={navLinkClass}>
                <span aria-hidden="true">🚪</span>
                {t('sidebar.rooms')}
            </NavLink>
            <NavLink to="/medicines" className={navLinkClass}>
                <span aria-hidden="true">💊</span>
                {t('sidebar.medicines')}
            </NavLink>
        </>
    );

    const guestNav = (
        <>
            <NavLink to="/nursing" className={navLinkClass}>
                <span aria-hidden="true">🩺</span>
                {t('sidebar.nursing')}
            </NavLink>
            <NavLink to="/appointments" className={navLinkClass}>
                <span aria-hidden="true">📅</span>
                {t('common.myAppointments')}
            </NavLink>
            <NavLink to="/bills" className={navLinkClass}>
                <span aria-hidden="true">💰</span>
                {t('common.myBills')}
            </NavLink>
            <NavLink to="/reports" className={navLinkClass}>
                <span aria-hidden="true">📋</span>
                تقاريري الطبية
            </NavLink>
        </>
    );

    return (
        <div className="layout-shell">
            <header className="layout-header">
                <div className={`layout-header__inner ${isArabic ? 'layout-header__inner--rtl' : ''}`}>
                    <Link to="/" className="layout-brand" style={{ 
                        textDecoration: 'none',
                        border: 'none',
                        background: 'transparent',
                        boxShadow: 'none',
                        padding: '8px 12px'
                    }}>
                        <img 
                            src={`${process.env.PUBLIC_URL}/hospital-logo.png`}
                            alt="Al-Hayat Hospital Logo"
                            style={{
                                width: 85,
                                height: 85,
                                objectFit: 'contain',
                                marginRight: isArabic ? 0 : 12,
                                marginLeft: isArabic ? 12 : 0,
                                filter: 'drop-shadow(0 2px 8px rgba(0, 0, 0, 0.15))',
                                imageRendering: 'crisp-edges'
                            }}
                            onError={(e) => {
                                e.target.src = `${process.env.PUBLIC_URL}/logo.png`;
                            }}
                        />
                        <div className="layout-brand__text">
                            <div className="layout-brand__title" style={{ 
                                color: '#2d3748',
                                fontSize: '1.2rem',
                                fontWeight: 700
                            }}>
                                {t('dashboard.hospitalName')}
                            </div>
                        </div>
                    </Link>

                    <nav
                        className={`layout-nav ${menuOpen ? 'layout-nav--open' : ''}`}
                        aria-label={navAriaLabel}
                    >
                        {user?.roles?.includes('Admin') || user?.roles?.includes('Doctor')
                            ? adminNav
                            : guestNav}
                    </nav>

                    <button 
                        type="button"
                        className="layout-nav__toggle"
                        onClick={handleToggleMenu}
                        aria-label={menuOpen ? closeMenuLabel : openMenuLabel}
                        aria-expanded={menuOpen}
                    >
                        {menuOpen ? '✕' : '☰'}
                    </button>

                    <div className="layout-header__actions">
                        {user ? (
                            <>
                                <div className="layout-user" aria-live="polite">
                                    <span className="layout-user__avatar" aria-hidden="true">
                                        👤
                                    </span>
                                    <div>
                                        <div className="layout-user__name">
                                            {user.firstName} {user.lastName}
                                        </div>
                                        <div className="layout-user__role">{roleLabel}</div>
                                    </div>
                                </div>
                                <select
                                    value={i18n.language}
                                    onChange={(e) => changeLanguage(e.target.value)}
                                    className="layout-lang-select"
                                    aria-label={changeLanguageLabel}
                                >
                                    <option value="en">{t('common.english')}</option>
                                    <option value="ar">{t('common.arabic')}</option>
                                </select>
                                <button
                                    type="button"
                                    onClick={logout}
                                    className="layout-btn layout-btn--primary"
                                >
                                    {t('common.logout')}
                                </button>
                            </>
                        ) : (
                            <>
                                <select
                                    value={i18n.language}
                                    onChange={(e) => changeLanguage(e.target.value)}
                                    className="layout-lang-select"
                                    aria-label={changeLanguageLabel}
                                >
                                    <option value="en">{t('common.english')}</option>
                                    <option value="ar">{t('common.arabic')}</option>
                                </select>
                                <Link to="/login" className="layout-btn layout-btn--primary">
                                    {t('common.login')}
                                </Link>
                                <Link to="/register" className="layout-btn layout-btn--outline">
                                    {t('common.createAccount')}
                                </Link>
                            </>
                        )}
                    </div>
                </div>
                </header>

                <div 
                className={`layout-nav__overlay ${menuOpen ? 'is-visible' : ''}`}
                role="presentation"
                    onClick={() => setMenuOpen(false)} 
            />

            <main className="layout-main">
                    <Outlet />
            </main>

            {user && <ChatbotWidget />}
        </div>
    );
}
