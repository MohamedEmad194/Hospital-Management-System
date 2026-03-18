import React, { useState, useEffect, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { fetchBill, payBill } from '../api/bills';
import { createStripeSession, createPayPalOrder, createPaymobOrder } from '../api/payment';
import { AuthContext } from '../context/AuthContext';
import { useTranslation } from 'react-i18next';

export default function Payment() {
    const { id } = useParams();
    const navigate = useNavigate();
    const { t, i18n } = useTranslation();
    const { user } = useContext(AuthContext);
    const [bill, setBill] = useState(null);
    const [loading, setLoading] = useState(true);
    const [processing, setProcessing] = useState(false);
    const [error, setError] = useState('');
    const [paymentMethod, setPaymentMethod] = useState('card');
    const [paymentAmount, setPaymentAmount] = useState(0);

    const isArabic = i18n.language === 'ar';

    useEffect(() => {
        loadBill();
    }, [id]);

    const loadBill = async () => {
        try {
            setLoading(true);
            const billData = await fetchBill(id);
            setBill(billData);
            setPaymentAmount(billData.remainingAmount || billData.totalAmount);
        } catch (err) {
            setError(err?.response?.data?.message || 'فشل تحميل بيانات الفاتورة');
        } finally {
            setLoading(false);
        }
    };

    const handlePayment = async () => {
        if (!bill) return;

        if (paymentAmount <= 0 || paymentAmount > bill.remainingAmount) {
            setError('المبلغ غير صحيح');
            return;
        }

        setProcessing(true);
        setError('');

        try {
            // Process payment based on selected method
            if (paymentMethod === 'card' || paymentMethod === 'stripe') {
                // Redirect to Stripe payment gateway
                await initiateStripePayment();
            } else if (paymentMethod === 'paypal') {
                // Redirect to PayPal payment gateway
                await initiatePayPalPayment();
            } else if (paymentMethod === 'paymob') {
                // Redirect to Paymob payment gateway
                await initiatePaymobPayment();
            } else {
                // Direct payment (cash, bank transfer, etc.)
                await processDirectPayment();
            }
        } catch (err) {
            setError(err?.response?.data?.message || 'فشل معالجة الدفع');
            setProcessing(false);
        }
    };

    const initiateStripePayment = async () => {
        try {
            const data = await createStripeSession(id, paymentAmount, 'EGP');
            if (data.sessionId && data.checkoutUrl) {
                // Redirect to Stripe Checkout
                window.location.href = data.checkoutUrl;
            } else {
                throw new Error('فشل إنشاء جلسة الدفع');
            }
        } catch (err) {
            throw err;
        }
    };

    const initiatePayPalPayment = async () => {
        try {
            const data = await createPayPalOrder(id, paymentAmount, 'EGP');
            if (data.approvalUrl) {
                // Redirect to PayPal
                window.location.href = data.approvalUrl;
            } else {
                throw new Error('فشل إنشاء طلب الدفع');
            }
        } catch (err) {
            throw err;
        }
    };

    const initiatePaymobPayment = async () => {
        try {
            const data = await createPaymobOrder(id, paymentAmount, 'EGP');
            if (data.iframeUrl) {
                // Show Paymob iframe or redirect
                window.location.href = data.iframeUrl;
            } else {
                throw new Error('فشل إنشاء طلب الدفع');
            }
        } catch (err) {
            throw err;
        }
    };

    const processDirectPayment = async () => {
        try {
            await payBill(id, {
                amount: paymentAmount,
                paymentMethod: paymentMethod,
                notes: `دفع مباشر - ${paymentMethod}`
            });
            
            alert('تم الدفع بنجاح');
            navigate('/bills');
        } catch (err) {
            throw err;
        }
    };

    if (loading) {
        return (
            <div style={{ 
                display: 'flex', 
                justifyContent: 'center', 
                alignItems: 'center', 
                minHeight: '50vh' 
            }}>
                <div style={{ fontSize: '1.2rem' }}>جاري التحميل...</div>
            </div>
        );
    }

    if (!bill) {
        return (
            <div style={{ 
                display: 'flex', 
                justifyContent: 'center', 
                alignItems: 'center', 
                minHeight: '50vh',
                flexDirection: 'column',
                gap: 20
            }}>
                <div style={{ fontSize: '1.2rem', color: '#e53e3e' }}>الفاتورة غير موجودة</div>
                <button 
                    onClick={() => navigate('/bills')}
                    style={{
                        padding: '10px 20px',
                        background: '#667eea',
                        color: 'white',
                        border: 'none',
                        borderRadius: '8px',
                        cursor: 'pointer'
                    }}
                >
                    العودة للفواتير
                </button>
            </div>
        );
    }

    return (
        <div style={{
            maxWidth: 800,
            margin: '40px auto',
            padding: '0 20px'
        }}>
            <div style={{
                background: 'white',
                borderRadius: '16px',
                padding: '32px',
                boxShadow: '0 4px 20px rgba(0,0,0,0.1)'
            }}>
                <h1 style={{
                    fontSize: '2rem',
                    fontWeight: 700,
                    marginBottom: '32px',
                    color: '#1a202c',
                    textAlign: isArabic ? 'right' : 'left'
                }}>
                    {isArabic ? 'دفع الفاتورة' : 'Pay Bill'}
                </h1>

                {/* Bill Summary */}
                <div style={{
                    background: '#f7fafc',
                    borderRadius: '12px',
                    padding: '24px',
                    marginBottom: '32px'
                }}>
                    <h2 style={{
                        fontSize: '1.25rem',
                        fontWeight: 600,
                        marginBottom: '20px',
                        color: '#2d3748',
                        textAlign: isArabic ? 'right' : 'left'
                    }}>
                        {isArabic ? 'ملخص الفاتورة' : 'Bill Summary'}
                    </h2>
                    <div style={{
                        display: 'grid',
                        gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))',
                        gap: '16px'
                    }}>
                        <div>
                            <div style={{ color: '#718096', fontSize: '0.9rem', marginBottom: '4px' }}>
                                {isArabic ? 'رقم الفاتورة' : 'Bill Number'}
                            </div>
                            <div style={{ fontWeight: 600, color: '#2d3748' }}>{bill.billNumber}</div>
                        </div>
                        <div>
                            <div style={{ color: '#718096', fontSize: '0.9rem', marginBottom: '4px' }}>
                                {isArabic ? 'المبلغ الإجمالي' : 'Total Amount'}
                            </div>
                            <div style={{ fontWeight: 600, color: '#2d3748' }}>
                                {bill.totalAmount?.toFixed(2)} {isArabic ? 'جنيه' : 'EGP'}
                            </div>
                        </div>
                        <div>
                            <div style={{ color: '#718096', fontSize: '0.9rem', marginBottom: '4px' }}>
                                {isArabic ? 'المبلغ المدفوع' : 'Paid Amount'}
                            </div>
                            <div style={{ fontWeight: 600, color: '#2d3748' }}>
                                {bill.paidAmount?.toFixed(2) || 0} {isArabic ? 'جنيه' : 'EGP'}
                            </div>
                        </div>
                        <div>
                            <div style={{ color: '#718096', fontSize: '0.9rem', marginBottom: '4px' }}>
                                {isArabic ? 'المبلغ المتبقي' : 'Remaining Amount'}
                            </div>
                            <div style={{ fontWeight: 600, color: '#e53e3e' }}>
                                {bill.remainingAmount?.toFixed(2) || bill.totalAmount?.toFixed(2)} {isArabic ? 'جنيه' : 'EGP'}
                            </div>
                        </div>
                    </div>
                </div>

                {/* Payment Amount */}
                <div style={{ marginBottom: '24px' }}>
                    <label style={{
                        display: 'block',
                        marginBottom: '8px',
                        fontWeight: 600,
                        color: '#2d3748',
                        textAlign: isArabic ? 'right' : 'left'
                    }}>
                        {isArabic ? 'المبلغ المراد دفعه' : 'Payment Amount'}
                    </label>
                    <input
                        type="number"
                        value={paymentAmount}
                        onChange={(e) => setPaymentAmount(parseFloat(e.target.value) || 0)}
                        min="0"
                        max={bill.remainingAmount}
                        step="0.01"
                        style={{
                            width: '100%',
                            padding: '12px 16px',
                            borderRadius: '8px',
                            border: '2px solid #e2e8f0',
                            fontSize: '1rem',
                            textAlign: isArabic ? 'right' : 'left'
                        }}
                    />
                    <div style={{
                        marginTop: '8px',
                        fontSize: '0.875rem',
                        color: '#718096',
                        textAlign: isArabic ? 'right' : 'left'
                    }}>
                        {isArabic ? 'الحد الأقصى:' : 'Maximum:'} {bill.remainingAmount?.toFixed(2)} {isArabic ? 'جنيه' : 'EGP'}
                    </div>
                </div>

                {/* Payment Method */}
                <div style={{ marginBottom: '32px' }}>
                    <label style={{
                        display: 'block',
                        marginBottom: '16px',
                        fontWeight: 600,
                        color: '#2d3748',
                        textAlign: isArabic ? 'right' : 'left'
                    }}>
                        {isArabic ? 'طريقة الدفع' : 'Payment Method'}
                    </label>
                    <div style={{
                        display: 'grid',
                        gridTemplateColumns: 'repeat(auto-fit, minmax(150px, 1fr))',
                        gap: '12px'
                    }}>
                        {[
                            { value: 'stripe', label: isArabic ? 'بطاقة ائتمانية (Stripe)' : 'Credit Card (Stripe)', icon: '💳' },
                            { value: 'paypal', label: isArabic ? 'PayPal' : 'PayPal', icon: '🔵' },
                            { value: 'paymob', label: isArabic ? 'Paymob' : 'Paymob', icon: '💳' },
                            { value: 'cash', label: isArabic ? 'نقدي' : 'Cash', icon: '💵' },
                            { value: 'bank', label: isArabic ? 'تحويل بنكي' : 'Bank Transfer', icon: '🏦' }
                        ].map((method) => (
                            <button
                                key={method.value}
                                onClick={() => setPaymentMethod(method.value)}
                                style={{
                                    padding: '16px',
                                    borderRadius: '12px',
                                    border: paymentMethod === method.value ? '2px solid #667eea' : '2px solid #e2e8f0',
                                    background: paymentMethod === method.value ? '#f0f4ff' : 'white',
                                    cursor: 'pointer',
                                    transition: 'all 0.2s',
                                    textAlign: 'center',
                                    display: 'flex',
                                    flexDirection: 'column',
                                    alignItems: 'center',
                                    gap: '8px'
                                }}
                                onMouseEnter={(e) => {
                                    if (paymentMethod !== method.value) {
                                        e.target.style.borderColor = '#cbd5e0';
                                    }
                                }}
                                onMouseLeave={(e) => {
                                    if (paymentMethod !== method.value) {
                                        e.target.style.borderColor = '#e2e8f0';
                                    }
                                }}
                            >
                                <span style={{ fontSize: '2rem' }}>{method.icon}</span>
                                <span style={{
                                    fontSize: '0.9rem',
                                    fontWeight: paymentMethod === method.value ? 600 : 500,
                                    color: paymentMethod === method.value ? '#667eea' : '#4a5568'
                                }}>
                                    {method.label}
                                </span>
                            </button>
                        ))}
                    </div>
                </div>

                {error && (
                    <div style={{
                        padding: '16px',
                        borderRadius: '8px',
                        background: '#fed7d7',
                        color: '#c53030',
                        marginBottom: '24px',
                        textAlign: isArabic ? 'right' : 'left'
                    }}>
                        {error}
                    </div>
                )}

                {/* Action Buttons */}
                <div style={{
                    display: 'flex',
                    gap: '12px',
                    justifyContent: isArabic ? 'flex-end' : 'flex-start',
                    flexWrap: 'wrap'
                }}>
                    <button
                        onClick={() => navigate('/bills')}
                        style={{
                            padding: '12px 24px',
                            borderRadius: '8px',
                            border: '2px solid #e2e8f0',
                            background: 'white',
                            color: '#4a5568',
                            fontWeight: 600,
                            cursor: 'pointer',
                            transition: 'all 0.2s'
                        }}
                        onMouseEnter={(e) => {
                            e.target.style.borderColor = '#cbd5e0';
                            e.target.style.background = '#f7fafc';
                        }}
                        onMouseLeave={(e) => {
                            e.target.style.borderColor = '#e2e8f0';
                            e.target.style.background = 'white';
                        }}
                    >
                        {isArabic ? 'إلغاء' : 'Cancel'}
                    </button>
                    <button
                        onClick={handlePayment}
                        disabled={processing || paymentAmount <= 0}
                        style={{
                            padding: '12px 24px',
                            borderRadius: '8px',
                            border: 'none',
                            background: processing || paymentAmount <= 0 ? '#cbd5e0' : 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
                            color: 'white',
                            fontWeight: 600,
                            cursor: processing || paymentAmount <= 0 ? 'not-allowed' : 'pointer',
                            transition: 'all 0.2s',
                            minWidth: '150px'
                        }}
                        onMouseEnter={(e) => {
                            if (!processing && paymentAmount > 0) {
                                e.target.style.transform = 'translateY(-2px)';
                                e.target.style.boxShadow = '0 4px 12px rgba(102, 126, 234, 0.4)';
                            }
                        }}
                        onMouseLeave={(e) => {
                            e.target.style.transform = 'translateY(0)';
                            e.target.style.boxShadow = 'none';
                        }}
                    >
                        {processing 
                            ? (isArabic ? 'جاري المعالجة...' : 'Processing...')
                            : (isArabic ? 'دفع الآن' : 'Pay Now')
                        }
                    </button>
                </div>
            </div>
        </div>
    );
}

