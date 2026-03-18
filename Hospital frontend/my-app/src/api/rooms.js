import apiClient from './client';

export async function fetchRooms() {
    const { data } = await apiClient.get('/Rooms');
    return data;
}

export async function createRoom(payload) {
    const { data } = await apiClient.post('/Rooms', payload);
    return data;
}

const roomsApi = { fetchRooms, createRoom };

export default roomsApi;
