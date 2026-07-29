import { computed, ref } from 'vue'
import { getTicket } from '@/api/ticketsApi'

export const currentTicket = ref(null)
export const workingWindowsCount = ref(8)

const activeTicketStatuses = new Set([
  'waiting', 'called', 'serving'
])

export function isTicketActive(ticket) {
  return activeTicketStatuses.has(
    ticket?.statusName,
  )
}

export const hasActiveTicket = computed(() => {
  return isTicketActive(currentTicket.value)
})

export function clearCurrentTicket() {
  currentTicket.value = null
}

export async function restoreCurrentTicket() {
  const ticketId = localStorage.getItem('currentTicketId')
  if (!ticketId) {
    clearCurrentTicket()
    return null
  }
  try {
    const ticket = await getTicket(ticketId)
    currentTicket.value = ticket
    return ticket
  } catch (error) {
    if (error.response?.status === 404) {
      localStorage.removeItem('currentTicketId')
      clearCurrentTicket()
      return null
    }
    throw error
  }
}