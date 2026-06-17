import apiClient from './client';

export async function fetchDoctors() {
    const { data } = await apiClient.get('/Doctors');
    return data;
}

export async function fetchDoctorsPaged({ page = 1, pageSize = 20, search = '', signal } = {}) {
    const { data } = await apiClient.get('/Doctors/paged', {
        params: { page, pageSize, search: search || undefined },
        signal,
    });
    return data;
}

export async function searchDoctors(searchTerm) {
    const { data } = await apiClient.get('/Doctors/search', { params: { searchTerm } });
    return data;
}

export async function fetchDoctor(id) {
    const { data } = await apiClient.get(`/Doctors/${id}`);
    return data;
}

export async function createDoctor(payload) {
    const { data } = await apiClient.post('/Doctors', payload);
    return data;
}

export async function updateDoctor(id, payload) {
    const { data } = await apiClient.put(`/Doctors/${id}`, payload);
    return data;
}

export async function deleteDoctor(id) {
    await apiClient.delete(`/Doctors/${id}`);
}


