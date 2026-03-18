import apiClient from './client';

export async function fetchBills() {
    const { data } = await apiClient.get('/Bills');
    return data;
}

export async function fetchBill(id) {
    const { data } = await apiClient.get(`/Bills/${id}`);
    return data;
}

export async function searchBills(params) {
    const { data } = await apiClient.get('/Bills/search', { params });
    return data;
}

export async function createBill(payload) {
    const { data } = await apiClient.post('/Bills', payload);
    return data;
}

export async function updateBill(id, payload) {
    const { data } = await apiClient.put(`/Bills/${id}`, payload);
    return data;
}

export async function overdueBills() {
    const { data } = await apiClient.get('/Bills/overdue');
    return data;
}

export async function outstandingAmount() {
    const { data } = await apiClient.get('/Bills/outstanding-amount');
    return data;
}

export async function payBill(id, payload) {
    const { data } = await apiClient.post(`/Bills/${id}/payment`, payload);
    return data;
}

export async function deleteBill(id) {
    await apiClient.delete(`/Bills/${id}`);
}


