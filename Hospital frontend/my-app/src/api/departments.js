import apiClient from './client';

export async function fetchDepartments() {
    const { data } = await apiClient.get('/Departments');
    return data;
}

export async function fetchDepartment(id) {
    const { data } = await apiClient.get(`/Departments/${id}`);
    return data;
}

export async function createDepartment(payload) {
    const { data } = await apiClient.post('/Departments', payload);
    return data;
}

export async function updateDepartment(id, payload) {
    const { data } = await apiClient.put(`/Departments/${id}`, payload);
    return data;
}

export async function deleteDepartment(id) {
    await apiClient.delete(`/Departments/${id}`);
}


