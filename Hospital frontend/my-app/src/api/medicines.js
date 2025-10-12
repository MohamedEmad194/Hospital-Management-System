import apiClient from './client';

export async function fetchMedicines() {
    const { data } = await apiClient.get('/Medicines');
    return data;
}

export async function createMedicine(payload) {
    const { data } = await apiClient.post('/Medicines', payload);
    return data;
}


