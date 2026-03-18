import apiClient from './client';

export async function fetchMedicalRecords() {
    const { data } = await apiClient.get('/MedicalRecords');
    return data;
}

export async function fetchMedicalRecord(id) {
    const { data } = await apiClient.get(`/MedicalRecords/${id}`);
    return data;
}

export async function fetchMedicalRecordsByPatient(patientId) {
    const { data } = await apiClient.get(`/MedicalRecords/patient/${patientId}`);
    return data;
}

export async function fetchMedicalRecordsByDoctor(doctorId) {
    const { data } = await apiClient.get(`/MedicalRecords/doctor/${doctorId}`);
    return data;
}

export async function createMedicalRecord(payload) {
    const { data } = await apiClient.post('/MedicalRecords', payload);
    return data;
}

export async function updateMedicalRecord(id, payload) {
    const { data } = await apiClient.put(`/MedicalRecords/${id}`, payload);
    return data;
}

export async function deleteMedicalRecord(id) {
    await apiClient.delete(`/MedicalRecords/${id}`);
}

