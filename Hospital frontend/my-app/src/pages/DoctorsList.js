import React from 'react';
import { fetchDoctorsPaged } from '../api/doctors';
import { useTranslation } from 'react-i18next';
import PagedList from '../components/PagedList';

const cardColors = [
    { bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)', border: '#667eea' },
    { bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)', border: '#f5576c' },
    { bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)', border: '#4facfe' },
    { bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)', border: '#43e97b' },
    { bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)', border: '#fa709a' },
    { bg: 'linear-gradient(135deg, #30cfd0 0%, #330867 100%)', border: '#30cfd0' },
];

export default function DoctorsList() {
    const { t } = useTranslation();

    const renderDoctor = (d, index) => {
        const color = cardColors[index % cardColors.length];
        return (
            <div key={d.id} className="entity-card" style={{ borderTopColor: color.border }}>
                <div className="entity-card__avatar" style={{ background: color.bg }}>
                    👨‍⚕️
                </div>
                <h3 className="entity-card__title">
                    {d.firstName} {d.lastName}
                </h3>
                <div className="entity-card__row">
                    <span className="entity-card__label">{t('doctors.labels.email')}</span>
                    <span className="entity-card__value entity-card__value--accent">
                        📧 {d.email}
                    </span>
                </div>
                <div className="entity-card__row">
                    <span className="entity-card__label">{t('doctors.labels.phone')}</span>
                    <span className="entity-card__value">📞 {d.phoneNumber}</span>
                </div>
                {d.departmentName && (
                    <div className="entity-card__row">
                        <span className="entity-card__label">
                            {t('doctors.labels.department')}
                        </span>
                        <span className="entity-card__value">🏥 {d.departmentName}</span>
                    </div>
                )}
                {d.specialization && (
                    <div className="entity-card__row entity-card__row--divided">
                        <span className="entity-card__label">
                            {t('doctors.labels.specialization')}
                        </span>
                        <span className="entity-card__value">{d.specialization}</span>
                    </div>
                )}
            </div>
        );
    };

    return (
        <PagedList
            fetcher={fetchDoctorsPaged}
            renderItem={renderDoctor}
            addLink="/doctors/add"
            addLabel={t('common.add')}
            searchPlaceholder={t('doctors.searchPlaceholder')}
            emptyLabel={t('doctors.failed')}
            errorLabel={t('doctors.failed')}
        />
    );
}
