import React, { useCallback, useEffect, useMemo, useRef, useState } from 'react';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { SkeletonGrid } from './Skeleton';

/**
 * Generic paged list with server-side search + pagination.
 * Consumers pass:
 *  - fetcher: ({ page, pageSize, search, signal }) => Promise<{ items, page, pageSize, totalItems, totalPages }>
 *  - renderItem: (item, index) => JSX
 *  - addLink, addLabel, searchPlaceholder, emptyLabel, errorLabel, gridMinWidth
 */
export default function PagedList({
    fetcher,
    renderItem,
    addLink,
    addLabel,
    searchPlaceholder,
    emptyLabel,
    errorLabel,
    pageSize = 20,
    skeletonCount = 6,
    gridMinWidth = 320,
}) {
    const { t } = useTranslation();
    const [items, setItems] = useState([]);
    const [page, setPage] = useState(1);
    const [totalItems, setTotalItems] = useState(0);
    const [totalPages, setTotalPages] = useState(1);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [searchInput, setSearchInput] = useState('');
    const [search, setSearch] = useState('');
    const debounceRef = useRef(null);

    useEffect(() => {
        if (debounceRef.current) clearTimeout(debounceRef.current);
        debounceRef.current = setTimeout(() => {
            setPage(1);
            setSearch(searchInput.trim());
        }, 350);
        return () => debounceRef.current && clearTimeout(debounceRef.current);
    }, [searchInput]);

    const load = useCallback(
        async (signal) => {
            setLoading(true);
            setError('');
            try {
                const result = await fetcher({ page, pageSize, search, signal });
                setItems(result?.items || []);
                setTotalItems(result?.totalItems || 0);
                setTotalPages(result?.totalPages || 1);
            } catch (e) {
                if (e?.name !== 'CanceledError' && e?.code !== 'ERR_CANCELED') {
                    setError(errorLabel || t('common.loading'));
                }
            } finally {
                setLoading(false);
            }
        },
        [fetcher, page, pageSize, search, errorLabel, t]
    );

    useEffect(() => {
        const controller = new AbortController();
        load(controller.signal);
        return () => controller.abort();
    }, [load]);

    const pages = useMemo(() => {
        const span = 2;
        const arr = [];
        const start = Math.max(1, page - span);
        const end = Math.min(totalPages, page + span);
        for (let i = start; i <= end; i++) arr.push(i);
        return arr;
    }, [page, totalPages]);

    const gridStyle = {
        display: 'grid',
        gridTemplateColumns: `repeat(auto-fill, minmax(${gridMinWidth}px, 1fr))`,
        gap: 20,
        marginTop: 20,
    };

    return (
        <div style={{ padding: 24 }}>
            <div className="page-toolbar">
                <input
                    placeholder={searchPlaceholder}
                    value={searchInput}
                    onChange={(e) => setSearchInput(e.target.value)}
                    className="page-toolbar__field"
                />
                {addLink && (
                    <Link
                        to={addLink}
                        className="page-toolbar__action page-toolbar__action--primary"
                    >
                        + {addLabel || t('common.add')}
                    </Link>
                )}
            </div>

            {error && (
                <div className="paged-list__error" role="alert">
                    {error}
                </div>
            )}

            {loading ? (
                <SkeletonGrid count={skeletonCount} />
            ) : items.length === 0 ? (
                <div className="paged-list__empty" role="status">
                    <div className="paged-list__empty-icon" aria-hidden="true">📭</div>
                    <div>{emptyLabel || t('common.noResults')}</div>
                </div>
            ) : (
                <div style={gridStyle}>{items.map((item, i) => renderItem(item, i))}</div>
            )}

            {totalPages > 1 && (
                <div className="paged-list__pagination" aria-label="pagination">
                    <button
                        type="button"
                        className="paged-list__page-btn"
                        disabled={page <= 1 || loading}
                        onClick={() => setPage((p) => Math.max(1, p - 1))}
                        aria-label={t('common.previousPage')}
                    >
                        <span aria-hidden="true">‹</span>
                    </button>
                    {pages[0] > 1 && (
                        <>
                            <button
                                type="button"
                                className="paged-list__page-btn"
                                onClick={() => setPage(1)}
                            >
                                1
                            </button>
                            {pages[0] > 2 && <span className="paged-list__ellipsis">…</span>}
                        </>
                    )}
                    {pages.map((p) => (
                        <button
                            key={p}
                            type="button"
                            className={`paged-list__page-btn ${
                                p === page ? 'is-active' : ''
                            }`}
                            onClick={() => setPage(p)}
                            disabled={loading}
                        >
                            {p}
                        </button>
                    ))}
                    {pages[pages.length - 1] < totalPages && (
                        <>
                            {pages[pages.length - 1] < totalPages - 1 && (
                                <span className="paged-list__ellipsis">…</span>
                            )}
                            <button
                                type="button"
                                className="paged-list__page-btn"
                                onClick={() => setPage(totalPages)}
                            >
                                {totalPages}
                            </button>
                        </>
                    )}
                    <button
                        type="button"
                        className="paged-list__page-btn"
                        disabled={page >= totalPages || loading}
                        onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                        aria-label={t('common.nextPage')}
                    >
                        <span aria-hidden="true">›</span>
                    </button>
                    <span className="paged-list__meta">
                        {t('common.pageMeta', {
                            defaultValue: 'Page {{page}} of {{totalPages}} · {{total}} total',
                            page,
                            totalPages,
                            total: totalItems,
                        })}
                    </span>
                </div>
            )}
        </div>
    );
}
