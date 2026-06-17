import React, { useCallback, useContext, useEffect, useState } from 'react';
import { fetchAppointments, fetchAppointmentsPaged } from '../api/appointments';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { AuthContext } from '../context/AuthContext';
import PagedList from '../components/PagedList';
import { SkeletonGrid } from '../components/Skeleton';

const STATUS_INFO = {
    Scheduled: {
        bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
        border: '#4facfe',
    },
    Confirmed: {
        bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
        border: '#43e97b',
    },
    Completed: {
        bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
        border: '#667eea',
    },
    Cancelled: {
        bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
        border: '#f5576c',
    },
    NoShow: {
        bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
        border: '#fa709a',
    },
};

const FALLBACK_INFO = {
    bg: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)',
    border: '#a8edea',
};

export default function AppointmentsList() {
    const { t, i18n } = useTranslation();
    const { user } = useContext(AuthContext);
    const locale = i18n.language === 'ar' ? 'ar-EG' : 'en-US';
    const currency = t('common.currency');

    const isAdminOrDoctor =
        user?.roles?.includes('Admin') ||
        user?.roles?.includes('Doctor') ||
        user?.roles?.includes('Staff');

    const renderAppointment = (a) => {
        const info = STATUS_INFO[a.status] || FALLBACK_INFO;
        const appointmentDate = a.appointmentDate ? new Date(a.appointmentDate) : null;
        const formattedDate = appointmentDate
            ? appointmentDate.toLocaleDateString(locale, {
                  year: 'numeric',
                  month: 'long',
                  day: 'numeric',
              })
            : '—';

        return (
            <div key={a.id} className="entity-card" style={{ borderTopColor: info.border }}>
                <div className="entity-card__avatar" style={{ background: info.bg }}>
                    📅
                </div>
                <div className="entity-card__row">
                    <span className="entity-card__label">
                        {t('appointments.table.status')}
                    </span>
                    <span
                        className="entity-card__badge"
                        style={{ background: info.bg }}
                    >
                        {a.status}
                    </span>
                </div>
                {a.patientName && (
                    <div className="entity-card__row">
                        <span className="entity-card__label">
                            {t('appointments.table.patient')}
                        </span>
                        <span className="entity-card__value entity-card__value--strong">
                            👤 {a.patientName}
                        </span>
                    </div>
                )}
                {a.doctorName && (
                    <div className="entity-card__row">
                        <span className="entity-card__label">
                            {t('appointments.table.doctor')}
                        </span>
                        <span className="entity-card__value entity-card__value--strong">
                            👨‍⚕️ {a.doctorName}
                        </span>
                    </div>
                )}
                <div className="entity-card__row">
                    <span className="entity-card__label">
                        {t('appointments.table.date')}
                    </span>
                    <span className="entity-card__value">📅 {formattedDate}</span>
                </div>
                {a.appointmentTime && (
                    <div className="entity-card__row">
                        <span className="entity-card__label">
                            {t('appointments.table.time')}
                        </span>
                        <span className="entity-card__value">⏰ {a.appointmentTime}</span>
                    </div>
                )}
                {a.reason && (
                    <div className="entity-card__row entity-card__row--divided">
                        <span className="entity-card__label">
                            {t('addAppointment.fields.reason')}
                        </span>
                        <span className="entity-card__value">{a.reason}</span>
                    </div>
                )}
                {a.consultationFee != null && (
                    <div className="entity-card__row entity-card__row--divided">
                        <span className="entity-card__label">
                            {t('addDoctor.fields.consultationFee')}
                        </span>
                        <span className="entity-card__value entity-card__value--accent entity-card__value--strong">
                            {Number(a.consultationFee).toFixed(2)} {currency}
                        </span>
                    </div>
                )}
            </div>
        );
    };

    // Admin / Doctor / Staff get paged listing
    if (isAdminOrDoctor) {
        return (
            <PagedList
                fetcher={fetchAppointmentsPaged}
                renderItem={renderAppointment}
                addLink="/appointments/add"
                addLabel={t('appointments.addButton')}
                searchPlaceholder={t('appointments.table.patient')}
                emptyLabel={t('appointments.failed')}
                errorLabel={t('appointments.failed')}
                gridMinWidth={350}
            />
        );
    }

    // Patient — keep the simple role-filtered list (small dataset)
    return <PatientAppointments renderAppointment={renderAppointment} />;
}

function PatientAppointments({ renderAppointment }) {
    const { t } = useTranslation();
    const { user } = useContext(AuthContext);
    const [appointments, setAppointments] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    const load = useCallback(async () => {
        if (!user) {
            setLoading(true);
            return;
        }
        setLoading(true);
        setError('');
        try {
            const data = await fetchAppointments();
            setAppointments(data || []);
        } catch (e) {
            setError(e?.response?.data?.message || t('appointments.failed'));
        } finally {
            setLoading(false);
        }
    }, [t, user]);

    useEffect(() => {
        load();
    }, [load]);

    return (
        <div style={{ padding: 24 }}>
            <div className="page-toolbar">
                <h2 style={{ margin: 0 }}>{t('common.myAppointments')}</h2>
            </div>
            {error && <div className="paged-list__error">{error}</div>}
            {loading ? (
                <SkeletonGrid count={3} rows={5} />
            ) : appointments.length === 0 ? (
                <div className="paged-list__empty">{t('appointments.failed')}</div>
            ) : (
                <div
                    style={{
                        display: 'grid',
                        gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))',
                        gap: 20,
                        marginTop: 20,
                    }}
                >
                    {appointments.map(renderAppointment)}
                </div>
            )}
        </div>
    );
}
