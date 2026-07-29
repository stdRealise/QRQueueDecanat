import instance from './axios';

export async function getPanel() {
  const response = await instance.get('/api/panel')
  return response.data
}