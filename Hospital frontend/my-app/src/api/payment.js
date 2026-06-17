import apiClient, { API_BASE_URL } from './client';

export async function createStripeSession(billId, amount, currency = 'EGP') {
    const response = await fetch(`${API_BASE_URL}/api/Payment/create-stripe-session`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({
            billId: parseInt(billId),
            amount: amount,
            currency: currency
        })
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to create Stripe session');
    }

    return await response.json();
}

export async function createPayPalOrder(billId, amount, currency = 'EGP') {
    const response = await fetch(`${API_BASE_URL}/api/Payment/create-paypal-order`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({
            billId: parseInt(billId),
            amount: amount,
            currency: currency
        })
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to create PayPal order');
    }

    return await response.json();
}

export async function createPaymobOrder(billId, amount, currency = 'EGP') {
    const response = await fetch(`${API_BASE_URL}/api/Payment/create-paymob-order`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: JSON.stringify({
            billId: parseInt(billId),
            amount: amount,
            currency: currency
        })
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'Failed to create Paymob order');
    }

    return await response.json();
}

export async function processPaymentCallback(callbackData) {
    const { data } = await apiClient.post('/Payment/callback', callbackData);
    return data;
}

