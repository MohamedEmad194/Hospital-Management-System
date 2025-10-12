import apiClient from './client';

export async function fetchAppointments() {
    const { data } = await apiClient.get('/Appointments');
    return data;
}

export async function fetchAppointment(id) {
    const { data } = await apiClient.get(`/Appointments/${id}`);
    return data;
}

export async function searchAppointments(params) {
    const { data } = await apiClient.get('/Appointments/search', { params });
    return data;
}

export async function availableSlots(doctorId, date) {
    const { data } = await apiClient.get(`/Appointments/available-slots/${doctorId}/${date}`);
    return data;
}

export async function createAppointment(payload) {
    const { data } = await apiClient.post('/Appointments', payload);
    return data;
}

export async function updateAppointment(id, payload) {
    const { data } = await apiClient.put(`/Appointments/${id}`, payload);
    return data;
}

export async function cancelAppointment(id) {
    const { data } = await apiClient.put(`/Appointments/${id}/cancel`);
    return data;
}

export async function completeAppointment(id, payload) {
    const { data } = await apiClient.put(`/Appointments/${id}/complete`, payload);
    return data;
}

export async function deleteAppointment(id) {
    await apiClient.delete(`/Appointments/${id}`);
}


