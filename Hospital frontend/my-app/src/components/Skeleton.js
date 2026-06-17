import React from 'react';

export function Skeleton({ width = '100%', height = 16, radius = 8, style }) {
    return (
        <span
            className="hms-skeleton"
            style={{
                display: 'inline-block',
                width,
                height,
                borderRadius: radius,
                ...style,
            }}
            aria-hidden="true"
        />
    );
}

export function SkeletonCard({ rows = 4 }) {
    return (
        <div className="hms-skeleton-card">
            <Skeleton width={50} height={50} radius={12} style={{ marginBottom: 16 }} />
            <Skeleton width="70%" height={20} style={{ marginBottom: 12 }} />
            {Array.from({ length: rows }).map((_, idx) => (
                <div key={idx} style={{ marginBottom: 10 }}>
                    <Skeleton width="40%" height={10} style={{ marginBottom: 6 }} />
                    <Skeleton width="80%" height={14} />
                </div>
            ))}
        </div>
    );
}

export function SkeletonGrid({ count = 6, rows = 4 }) {
    return (
        <div
            style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))',
                gap: 20,
                marginTop: 20,
            }}
        >
            {Array.from({ length: count }).map((_, idx) => (
                <SkeletonCard key={idx} rows={rows} />
            ))}
        </div>
    );
}

export function SkeletonStats({ count = 8 }) {
    return (
        <div
            style={{
                display: 'grid',
                gridTemplateColumns: 'repeat(auto-fit, minmax(250px, 1fr))',
                gap: 30,
            }}
        >
            {Array.from({ length: count }).map((_, idx) => (
                <div key={idx} className="hms-skeleton-card" style={{ minHeight: 130 }}>
                    <div
                        style={{
                            display: 'flex',
                            justifyContent: 'space-between',
                            alignItems: 'center',
                            marginBottom: 18,
                        }}
                    >
                        <Skeleton width={60} height={60} radius={14} />
                        <Skeleton width={80} height={28} />
                    </div>
                    <Skeleton width="60%" height={16} />
                </div>
            ))}
        </div>
    );
}

export default Skeleton;
