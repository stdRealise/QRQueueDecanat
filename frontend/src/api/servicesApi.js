import instance from './axios';

export async function getServices() {
    const response = await instance.get('/api/services')
    return response.data
}