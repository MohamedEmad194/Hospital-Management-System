import apiClient from './client';

export async function fetchNursingUnits() {
    try {
        console.log('🔄 Fetching nursing units from /NursingUnits...');
        const response = await apiClient.get('/NursingUnits');
        console.log('✅ Nursing units fetched successfully:', {
            count: Array.isArray(response.data) ? response.data.length : 'unknown',
            data: response.data
        });
        return response.data;
    } catch (error) {
        console.error('❌ Error fetching nursing units:', {
            message: error?.message,
            response: error?.response?.data,
            status: error?.response?.status,
            url: error?.config?.url
        });
        throw error;
    }
}

export async function searchNursingUnits(searchTerm) {
    try {
        const response = await apiClient.get(`/NursingUnits/search?searchTerm=${encodeURIComponent(searchTerm)}`);
        return response.data;
    } catch (error) {
        console.error('Error searching nursing units:', error);
        throw error;
    }
}

export async function fetchNursingUnitById(id) {
    try {
        const response = await apiClient.get(`/NursingUnits/${id}`);
        return response.data;
    } catch (error) {
        console.error('Error fetching nursing unit:', error);
        throw error;
    }
}

