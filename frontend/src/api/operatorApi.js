import instance from './axios'

export async function startOperatorSession(counterNumber) {
  const response = await instance.post(
    '/api/operator/session', 
    { 
      counterNumber
    })
  return response.data
}

export async function closeOperatorSession() {
  await instance.delete(`/api/operator/session`)
}

export async function getOperatorWorkspace() {
  const response = await instance.get(`/api/operator/workspace`)
  return response.data
}

export async function callNextTicket() {
  const response = await instance.post(
    `/api/operator/tickets/call-next`)
  return response.data
}

export async function startTicket(ticketId) {
  const response = await instance.post(
    `/api/operator/tickets/${ticketId}/start`)
  return response.data
}

export async function completeTicket(ticketId) {
  const response = await instance.post(
    `/api/operator/tickets/${ticketId}/complete`)
  return response.data
}

export async function skipTicket(ticketId) {
  const response = await instance.post(
    `/api/operator/tickets/${ticketId}/skip`)
  return response.data
}
export async function getOperatorHistory() {
  const response = await instance.get(
    `/api/operator/tickets/history`)
  return response.data
}
export async function getOperatorServices() {
  const response = await instance.get(`/api/operator/settings/services`)
  return response.data
}

export async function updateOperatorServices(serviceIds) {
  await instance.put(`/api/operator/settings/services`, 
    { 
      serviceIds 
    })
}

