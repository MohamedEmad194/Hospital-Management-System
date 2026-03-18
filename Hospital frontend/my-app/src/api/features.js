import apiClient from './client';

export async function fetchFeatures(lang = 'en') {
    const { data } = await apiClient.get('/Features', { params: { lang } });
    return data;
}

export async function fetchFeature(id, lang = 'en') {
    const { data } = await apiClient.get(`/Features/${id}`, { params: { lang } });
    return data;
}

