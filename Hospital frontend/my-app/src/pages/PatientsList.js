import React from 'react';
import { fetchPatientsPaged } from '../api/patients';
import { useTranslation } from 'react-i18next';
import PagedList from '../components/PagedList';

const cardColors = [
    { bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', border: '#667eea' },
    { bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', border: '#f5576c' },
    { bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', border: '#4facfe' },
    { bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', border: '#43e97b' },
    { bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', border: '#fa709a' },
    { bg: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)', border: '#30cfd0' },
    { bg: 'linear-gradient(135deg, #a8edea 0%, #fed6e3 100%)', border: '#a8edea' },
    { bg: 'linear-gradient(135deg, #ff9a9e 0%, #fecfef 100%)', border: '#ff9a9e' },
];

export default function PatientsList() {
    const { t, i18n } = useTranslation();
    const locale = i18n.language === 'ar' ? 'ar-EG' : 'en-US';

    const renderPatient = (p, index) => {
        const color = cardColors[index % cardColors.length];
        const initials = `${p.firstName?.charAt(0) || ''}${
            p.lastName?.charAt(0) || ''
        }`.toUpperCase();

        return (
            <div key={p.id} className="entity-card" style={{ borderTopColor: color.border }}>
                <div className="entity-card__avatar" style={{ background: color.bg }}>
                    {initials || '👤'}
                </div>
                <h3 className="entity-card__title">
                    {p.firstName} {p.lastName}
                </h3>
                <div className="entity-card__row">
                    <span className="entity-card__label">{t('common.email')}</span>
                    <span className="entity-card__value entity-card__value--accent">
                        📧 {p.email}
                    </span>
                </div>
                <div className="entity-card__row">
                    <span className="entity-card__label">{t('common.phone')}</span>
                    <span className="entity-card__value">📞 {p.phoneNumber}</span>
                </div>
                {p.nationalId && (
                    <div className="entity-card__row">
                        <span className="entity-card__label">{t('common.nationalId')}</span>
                        <span className="entity-card__value">🆔 {p.nationalId}</span>
                    </div>
                )}
                {p.dateOfBirth && (
                    <div className="entity-card__row">
                        <span className="entity-card__label">
                            {t('addPatient.fields.dateOfBirth')}
                        </span>
                        <span className="entity-card__value">
                            📅 {new Date(p.dateOfBirth).toLocaleDateString(locale)}
                        </span>
                    </div>
                )}
                {p.gender && (
                    <div className="entity-card__row">
                        <span className="entity-card__label">{t('addPatient.fields.gender')}</span>
                        <span className="entity-card__value">
                            {p.gender === 'Male'
                                ? `👨 ${t('addPatient.fields.male')}`
                                : p.gender === 'Female'
                                ? `👩 ${t('addPatient.fields.female')}`
                                : p.gender}
                        </span>
                    </div>
                )}
                {p.address && (
                    <div className="entity-card__row entity-card__row--divided">
                        <span className="entity-card__label">{t('addPatient.fields.address')}</span>
                        <span className="entity-card__value">📍 {p.address}</span>
                    </div>
                )}
                {p.insuranceProvider && (
                    <div className="entity-card__row entity-card__row--divided">
                        <span className="entity-card__label">
                            {t('addBill.fields.insuranceProvider')}
                        </span>
                        <span className="entity-card__value entity-card__value--success">
                            🏥 {p.insuranceProvider}
                        </span>
                        {p.insuranceNumber && (
                            <span className="entity-card__sub">
                                {t('addBill.fields.insuranceNumber')}: {p.insuranceNumber}
                            </span>
                        )}
                    </div>
                )}
            </div>
        );
    };

    return (
        <PagedList
            fetcher={fetchPatientsPaged}
            renderItem={renderPatient}
            addLink="/patients/add"
            addLabel={t('common.add')}
            searchPlaceholder={t('patients.searchPlaceholder')}
            emptyLabel={t('patients.failed')}
            errorLabel={t('patients.failed')}
        />
    );
}
