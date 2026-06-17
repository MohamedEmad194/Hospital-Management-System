import React, { useCallback, useEffect, useRef, useState } from 'react';
import { useTranslation } from 'react-i18next';
import { analyzeXRayImage, fetchXRayAiStatus } from '../api/xrayAi';
import './XRayAnalyzer.css';

const ACCEPT = 'image/png,image/jpeg,image/webp,image/bmp';

export default function XRayAnalyzer() {
    const { t, i18n } = useTranslation();
    const isAr = i18n.language === 'ar';
    const inputRef = useRef(null);

    const [status, setStatus] = useState(null);
    const [file, setFile] = useState(null);
    const [preview, setPreview] = useState(null);
    const [prompt, setPrompt] = useState('');
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [report, setReport] = useState('');
    const [dragOver, setDragOver] = useState(false);

    useEffect(() => {
        let cancelled = false;
        (async () => {
            try {
                const { data } = await fetchXRayAiStatus();
                if (!cancelled) setStatus(data);
            } catch {
                if (!cancelled) {
                    setStatus({
                        success: false,
                        serviceReachable: false,
                        message: t('xrayAi.statusOffline'),
                    });
                }
            }
        })();
        return () => {
            cancelled = true;
        };
    }, [t]);

    const pickFile = useCallback(
        (f) => {
            if (!f?.type?.startsWith('image/')) {
                setError(t('xrayAi.errors.notImage'));
                return;
            }
            setError('');
            setFile(f);
            setReport('');
            if (preview) URL.revokeObjectURL(preview);
            setPreview(URL.createObjectURL(f));
        },
        [preview, t]
    );

    const onDrop = (e) => {
        e.preventDefault();
        setDragOver(false);
        const f = e.dataTransfer.files?.[0];
        if (f) pickFile(f);
    };

    const runAnalyze = async () => {
        if (!file) {
            setError(t('xrayAi.errors.noFile'));
            return;
        }
        if (!status?.serviceReachable) {
            setError(t('xrayAi.errors.serviceDown'));
            return;
        }
        setLoading(true);
        setError('');
        setReport('');
        try {
            const { data } = await analyzeXRayImage(file, prompt);
            if (data.success && data.report) {
                setReport(data.report);
            } else {
                setError(data.message || t('xrayAi.errors.generic'));
            }
        } catch (err) {
            const msg =
                err?.response?.data?.message ||
                err?.message ||
                t('xrayAi.errors.generic');
            setError(msg);
        } finally {
            setLoading(false);
        }
    };

    const clear = () => {
        setFile(null);
        setError('');
        setReport('');
        if (preview) URL.revokeObjectURL(preview);
        setPreview(null);
        if (inputRef.current) inputRef.current.value = '';
    };

    const statusClass =
        status?.serviceReachable && status?.success
            ? 'xray-status xray-status--ok'
            : 'xray-status xray-status--warn';

    return (
        <div className="xray-page" dir={isAr ? 'rtl' : 'ltr'}>
            <div className="xray-inner">
                <header className="xray-hero">
                    <h1>{t('xrayAi.title')}</h1>
                    <p>{t('xrayAi.subtitle')}</p>
                    <div className="xray-disclaimer">{t('xrayAi.disclaimer')}</div>
                </header>

                <div className="xray-card">
                    <div className={statusClass}>
                        {status?.serviceReachable
                            ? status.modelLoaded
                                ? t('xrayAi.statusReady')
                                : t('xrayAi.statusReachable')
                            : t('xrayAi.statusOffline')}
                        {status?.modelId ? ` · ${status.modelId}` : ''}
                    </div>

                    <div
                        className={`xray-drop ${dragOver ? 'is-dragover' : ''}`}
                        onDragOver={(e) => {
                            e.preventDefault();
                            setDragOver(true);
                        }}
                        onDragLeave={() => setDragOver(false)}
                        onDrop={onDrop}
                        onClick={() => inputRef.current?.click()}
                        role="button"
                        tabIndex={0}
                        onKeyDown={(e) => e.key === 'Enter' && inputRef.current?.click()}
                    >
                        <input
                            ref={inputRef}
                            type="file"
                            accept={ACCEPT}
                            hidden
                            onChange={(e) => pickFile(e.target.files?.[0])}
                        />
                        <strong>{t('xrayAi.dropTitle')}</strong>
                        <div style={{ marginTop: 8, opacity: 0.85 }}>{t('xrayAi.dropHint')}</div>
                        {preview ? (
                            <img src={preview} alt="" className="xray-preview" />
                        ) : null}
                    </div>

                    <label className="xray-label" htmlFor="xray-prompt">
                        {t('xrayAi.promptLabel')}
                    </label>
                    <textarea
                        id="xray-prompt"
                        className="xray-textarea"
                        value={prompt}
                        onChange={(e) => setPrompt(e.target.value)}
                        placeholder={t('xrayAi.promptPlaceholder')}
                    />

                    <div className="xray-actions">
                        <button
                            type="button"
                            className="xray-btn xray-btn--primary"
                            disabled={loading || !file || !status?.serviceReachable}
                            onClick={runAnalyze}
                        >
                            {loading ? t('xrayAi.analyzing') : t('xrayAi.analyze')}
                        </button>
                        <button type="button" className="xray-btn xray-btn--ghost" disabled={loading} onClick={clear}>
                            {t('xrayAi.clear')}
                        </button>
                    </div>

                    {error ? <div className="xray-error">{error}</div> : null}
                    {report ? (
                        <>
                            <div className="xray-label">{t('xrayAi.reportTitle')}</div>
                            <div className="xray-report">{report}</div>
                        </>
                    ) : loading ? (
                        <p style={{ marginTop: 16, opacity: 0.8 }}>{t('xrayAi.waitModel')}</p>
                    ) : null}
                </div>
            </div>
        </div>
    );
}
