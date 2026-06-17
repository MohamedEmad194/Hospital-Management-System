import apiClient from './client';

export function fetchXRayAiStatus() {
    return apiClient.get('/XRayAi/status');
}

export function analyzeXRayImage(file, prompt) {
    const formData = new FormData();
    formData.append('file', file);
    if (prompt?.trim()) formData.append('prompt', prompt.trim());
    return apiClient.post('/XRayAi/analyze', formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
        timeout: 600000,
    });
}
