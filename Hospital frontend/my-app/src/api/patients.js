import apiClient from './client';

export async function fetchPatients() {
    const { data } = await apiClient.get('/Patients');
    return data;
}

export async function fetchPatient(id) {
    const { data } = await apiClient.get(`/Patients/${id}`);
    return data;
}

export async function searchPatients(searchTerm) {
    const { data } = await apiClient.get('/Patients/search', { params: { searchTerm } });
    return data;
}

export async function createPatient(payload) {
    const { data } = await apiClient.post('/Patients', payload);
    return data;
}

export async function updatePatient(id, payload) {
    const { data } = await apiClient.put(`/Patients/${id}`, payload);
    return data;
}

export async function deletePatient(id) {
    await apiClient.delete(`/Patients/${id}`);
}


