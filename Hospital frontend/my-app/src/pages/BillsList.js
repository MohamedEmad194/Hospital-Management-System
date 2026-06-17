import React, { useCallback, useContext, useEffect, useState } from 'react';
import {
    fetchBills,
    fetchBillsPaged,
    overdueBills,
    outstandingAmount,
} from '../api/bills';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { AuthContext } from '../context/AuthContext';
import PagedList from '../components/PagedList';
import { SkeletonGrid } from '../components/Skeleton';

const STATUS_INFO = {
    Paid: {
        bg: 'linear-gradient(135deg, #43e97b 0%, #38f9d7 100%)',
        border: '#43e97b',
        icon: '✓',
    },
    Pending: {
        bg: 'linear-gradient(135deg, #fa709a 0%, #fee140 100%)',
        border: '#fa709a',
        icon: '⏳',
    },
    Overdue: {
        bg: 'linear-gradient(135deg, #f093fb 0%, #f5576c 100%)',
        border: '#f5576c',
        icon: '⚠️',
    },
    Partial: {
        bg: 'linear-gradient(135deg, #4facfe 0%, #00f2fe 100%)',
        border: '#4facfe',
        icon: '◐',
    },
};

const FALLBACK_INFO = {
    bg: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
    border: '#667eea',
    icon: '📄',
};

export default function BillsList() {
    const { t } = useTranslation();
    const { user } = useContext(AuthContext);
    const isAdminOrStaff =
        user?.roles?.includes('Admin') || user?.roles?.includes('Staff');

    const renderBill = (b) => <BillCard key={b.id} bill={b} />;

    if (isAdminOrStaff) {
        return (
            <>
                <BillsSummary />
                <PagedList
                    fetcher={fetchBillsPaged}
                    renderItem={renderBill}
                    addLink="/bills/add"
                    addLabel={t('bills.addButton')}
                    searchPlaceholder={t('bills.table.patient')}
                    emptyLabel={t('bills.failed')}
                    errorLabel={t('bills.failed')}
                    gridMinWidth={350}
                />
            </>
        );
    }

    return <PatientBills renderBill={renderBill} />;
}

function BillCard({ bill: b }) {
    const { t } = useTranslation();
    const info = STATUS_INFO[b.status] || FALLBACK_INFO;
    const currency = t('common.currency');
    const isPaid = b.status === 'Paid';
    const isOverdue = b.status === 'Overdue';

    return (
        <div className="entity-card" style={{ borderTopColor: info.border }}>
            <div className="entity-card__avatar" style={{ background: info.bg }}>
                💰
            </div>
            <div className="entity-card__badge-row">
                <span className="entity-card__badge" style={{ background: info.bg }}>
                    {info.icon} {b.status}
                </span>
            </div>
            {b.patientName && (
                <div className="entity-card__row">
                    <span className="entity-card__label">{t('bills.table.patient')}</span>
                    <span className="entity-card__value entity-card__value--strong">
                        👤 {b.patientName}
                    </span>
                </div>
            )}
            <div className="entity-card__row entity-card__row--divided entity-card__row--columns">
                <div>
                    <span className="entity-card__label">{t('bills.table.total')}</span>
                    <span className="entity-card__value entity-card__value--strong">
                        {fmt(b.totalAmount)} {currency}
                    </span>
                </div>
                <div>
                    <span className="entity-card__label">{t('bills.table.paid')}</span>
                    <span
                        className="entity-card__value entity-card__value--strong"
                        style={{ color: isPaid ? '#10b981' : '#4f46e5' }}
                    >
                        {fmt(b.paidAmount)} {currency}
                    </span>
                </div>
            </div>
            <div
                className="entity-card__remaining"
                data-state={isOverdue ? 'overdue' : isPaid ? 'paid' : 'pending'}
            >
                <span className="entity-card__label">{t('bills.table.remaining')}</span>
                <span className="entity-card__value entity-card__value--strong">
                    {fmt(b.remainingAmount)} {currency}
                </span>
            </div>
            {!isPaid && b.remainingAmount > 0 && (
                <Link to={`/bills/${b.id}/payment`} className="entity-card__pay-btn">
                    💳 {t('common.add', { defaultValue: 'Pay Now' })}
                </Link>
            )}
        </div>
    );
}

function fmt(value) {
    return typeof value === 'number' ? value.toFixed(2) : value ?? '0.00';
}

function BillsSummary() {
    const { t } = useTranslation();
    const currency = t('common.currency');
    const [summary, setSummary] = useState(null);

    useEffect(() => {
        let cancelled = false;
        (async () => {
            try {
                const [overdue, outstanding] = await Promise.all([
                    overdueBills(),
                    outstandingAmount(),
                ]);
                if (!cancelled)
                    setSummary({
                        overdueCount: overdue?.length || 0,
                        totalOutstanding: outstanding?.totalOutstandingAmount || 0,
                    });
            } catch {
                if (!cancelled) setSummary(null);
            }
        })();
        return () => {
            cancelled = true;
        };
    }, []);

    if (!summary) return null;
    return (
        <div className="bills-summary">
            <div className="bills-summary__card bills-summary__card--overdue">
                <div className="bills-summary__label">{t('bills.summary.overdue')}</div>
                <div className="bills-summary__value">{summary.overdueCount}</div>
            </div>
            <div className="bills-summary__card bills-summary__card--outstanding">
                <div className="bills-summary__label">{t('bills.summary.outstanding')}</div>
                <div className="bills-summary__value">
                    {fmt(summary.totalOutstanding)} {currency}
                </div>
            </div>
        </div>
    );
}

function PatientBills({ renderBill }) {
    const { t } = useTranslation();
    const { user } = useContext(AuthContext);
    const [bills, setBills] = useState([]);
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
            const data = await fetchBills();
            setBills(data || []);
        } catch (e) {
            setError(e?.response?.data?.message || t('bills.failed'));
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
                <h2 style={{ margin: 0 }}>{t('common.myBills')}</h2>
            </div>
            {error && <div className="paged-list__error">{error}</div>}
            {loading ? (
                <SkeletonGrid count={3} rows={5} />
            ) : bills.length === 0 ? (
                <div className="paged-list__empty">{t('bills.failed')}</div>
            ) : (
                <div
                    style={{
                        display: 'grid',
                        gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))',
                        gap: 20,
                        marginTop: 20,
                    }}
                >
                    {bills.map(renderBill)}
                </div>
            )}
        </div>
    );
}
