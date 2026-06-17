import React, { useContext, useEffect, useMemo, useState } from 'react';
import { Link, NavLink, Outlet, useLocation } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';
import { useTranslation } from 'react-i18next';
import ChatbotWidget from './ChatbotWidget';

const ALL_NAV_ITEMS = [
    { key: 'nursing', to: '/nursing', icon: '🩺', i18n: 'sidebar.nursing', roles: ['*'] },
    { key: 'xrayAi', to: '/xray-ai', icon: '🫁', i18n: 'sidebar.xrayAi', roles: ['Admin', 'Doctor'] },
    { key: 'patients', to: '/patients', icon: '👥', i18n: 'sidebar.patients', roles: ['Admin', 'Doctor', 'Staff', 'Nurse'] },
    { key: 'doctors', to: '/doctors', icon: '👨‍⚕️', i18n: 'sidebar.doctors', roles: ['Admin', 'Doctor', 'Staff', 'Patient'] },
    { key: 'appointments', to: '/appointments', icon: '📅', i18n: 'sidebar.appointments', roles: ['Admin', 'Doctor', 'Staff', 'Nurse'] },
    { key: 'myAppointments', to: '/appointments', icon: '📅', i18n: 'common.myAppointments', roles: ['Patient'] },
    { key: 'bills', to: '/bills', icon: '💰', i18n: 'sidebar.bills', roles: ['Admin', 'Staff'] },
    { key: 'myBills', to: '/bills', icon: '💰', i18n: 'common.myBills', roles: ['Patient'] },
    { key: 'departments', to: '/departments', icon: '🏥', i18n: 'sidebar.departments', roles: ['Admin', 'Doctor', 'Staff'] },
    { key: 'rooms', to: '/rooms', icon: '🚪', i18n: 'sidebar.rooms', roles: ['Admin', 'Doctor', 'Staff', 'Nurse'] },
    { key: 'medicines', to: '/medicines', icon: '💊', i18n: 'sidebar.medicines', roles: ['Admin', 'Doctor', 'Staff', 'Nurse'] },
    { key: 'reports', to: '/reports', icon: '📋', i18n: 'sidebar.reports', roles: ['Patient'] },
];

function pickItemsForRoles(roles) {
    return ALL_NAV_ITEMS.filter((item) => {
        if (item.roles.includes('*')) return true;
        if (!roles || roles.length === 0) {
            // unauthenticated: only public items (those with '*' covered above)
            return false;
        }
        return item.roles.some((r) => roles.includes(r));
    });
}

const ROLE_PRIORITY = ['Admin', 'Doctor', 'Nurse', 'Pharmacist', 'Staff', 'Patient'];

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

    const primaryRole = useMemo(() => {
        if (!user?.roles) return null;
        return ROLE_PRIORITY.find((r) => user.roles.includes(r)) || user.roles[0];
    }, [user]);

    const roleLabel = primaryRole
        ? t(`common.roles.${primaryRole.toLowerCase()}`, {
              defaultValue: primaryRole,
          })
        : t('common.roles.guest', { defaultValue: 'Guest' });

    const navItems = useMemo(() => pickItemsForRoles(user?.roles || []), [user]);

    return (
        <div className="layout-shell">
            <header className="layout-header">
                <div className={`layout-header__inner ${isArabic ? 'layout-header__inner--rtl' : ''}`}>
                    <Link to="/" className="layout-brand layout-brand--highlight">
                        <img
                            className="layout-brand__logo-img"
                            src={`${process.env.PUBLIC_URL}/logo.png`}
                            alt="Al-Hayat Hospital Logo"
                        />
                        <div className="layout-brand__text">
                            <span className="layout-brand__title">
                                {t('dashboard.hospitalName')}
                            </span>
                            <span className="layout-brand__subtitle">
                                {t('dashboard.subtitle')}
                            </span>
                        </div>
                    </Link>

                    <nav
                        className={`layout-nav ${menuOpen ? 'layout-nav--open' : ''}`}
                        aria-label={navAriaLabel}
                    >
                        {navItems.map((item) => (
                            <NavLink key={item.key} to={item.to} className={navLinkClass} end={false}>
                                <span aria-hidden="true">{item.icon}</span>
                                {t(item.i18n)}
                            </NavLink>
                        ))}
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
