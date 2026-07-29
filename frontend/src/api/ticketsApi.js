import instance from './axios';

export async function createTicket(serviceId) {
  const response = await instance.post('/api/tickets',
    { 
      serviceId 
    })
  return response.data
}

export async function getTicket(ticketId) {
  const response = await instance.get(`/api/tickets/${ticketId}`)
  return response.data
}

export async function cancelTicket(ticketId) {
  const response = await instance.post(`/api/tickets/${ticketId}/cancel`)
  return response.data
}