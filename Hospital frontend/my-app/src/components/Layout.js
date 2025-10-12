import React, { useContext } from 'react';
import { Link, NavLink, Outlet } from 'react-router-dom';
import { AuthContext } from '../context/AuthContext';

export default function Layout() {
    const { user, logout } = useContext(AuthContext);
    return (
        <div style={{ minHeight: '100vh', display: 'grid', gridTemplateColumns: '260px 1fr', background: 'transparent' }}>
            <aside className="card" style={{ borderRight: '1px solid var(--hms-border)', padding: 18, margin: 16, marginRight: 0, background: 'var(--hms-surface)' }}>
                <div style={{ fontWeight: 800, marginBottom: 16, fontSize: 18, letterSpacing: 0.3 }}>
                    <Link to="/" style={{ textDecoration: 'none', color: 'var(--hms-text)' }}>HMS Admin</Link>
                </div>
                <nav style={{ display: 'grid', gap: 8 }}>
                    <NavLink to="/" end style={({ isActive }) => ({ padding: 12, borderRadius: 8, textDecoration: 'none', color: isActive ? '#ffffff' : 'var(--hms-text)', background: isActive ? 'var(--hms-primary)' : 'transparent' })}>Dashboard</NavLink>
                    <NavLink to="/patients" style={({ isActive }) => ({ padding: 12, borderRadius: 8, textDecoration: 'none', color: isActive ? '#ffffff' : 'var(--hms-text)', background: isActive ? 'var(--hms-primary)' : 'transparent' })}>Patients</NavLink>
                    <NavLink to="/doctors" style={({ isActive }) => ({ padding: 12, borderRadius: 8, textDecoration: 'none', color: isActive ? '#ffffff' : 'var(--hms-text)', background: isActive ? 'var(--hms-primary)' : 'transparent' })}>Doctors</NavLink>
                    <NavLink to="/appointments" style={({ isActive }) => ({ padding: 12, borderRadius: 8, textDecoration: 'none', color: isActive ? '#ffffff' : 'var(--hms-text)', background: isActive ? 'var(--hms-primary)' : 'transparent' })}>Appointments</NavLink>
                    <NavLink to="/bills" style={({ isActive }) => ({ padding: 12, borderRadius: 8, textDecoration: 'none', color: isActive ? '#ffffff' : 'var(--hms-text)', background: isActive ? 'var(--hms-primary)' : 'transparent' })}>Bills</NavLink>
                    <NavLink to="/departments" style={({ isActive }) => ({ padding: 12, borderRadius: 8, textDecoration: 'none', color: isActive ? '#ffffff' : 'var(--hms-text)', background: isActive ? 'var(--hms-primary)' : 'transparent' })}>Departments</NavLink>
                    <NavLink to="/medicines" style={({ isActive }) => ({ padding: 12, borderRadius: 8, textDecoration: 'none', color: isActive ? '#ffffff' : 'var(--hms-text)', background: isActive ? 'var(--hms-primary)' : 'transparent' })}>Medicines</NavLink>
                </nav>
            </aside>
            <main>
                <header className="card" style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '12px 16px', margin: 16, background: 'var(--hms-surface)' }}>
                    <div />
                    <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                        <div style={{ color: 'var(--hms-text-dim)' }}>{user ? `${user.firstName} ${user.lastName}` : ''}</div>
                        <button onClick={logout} style={{ padding: '8px 12px', border: '1px solid var(--hms-border)', borderRadius: 8, background: 'var(--hms-primary)', color: '#ffffff' }}>Logout</button>
                    </div>
                </header>
                <div>
                    <Outlet />
                </div>
            </main>
        </div>
    );
}


