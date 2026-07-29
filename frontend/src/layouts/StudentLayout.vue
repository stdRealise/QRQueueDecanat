<script setup>
import { onMounted, onUnmounted, watch, ref } from 'vue'
import StudentHeader from '@/components/StudentHeader.vue'
import StudentBottom from '@/components/StudentBottom.vue'
import { currentTicket, restoreCurrentTicket } from '@/state/studentState'
import { createQueueConnection } from '@/utils/queueConnection'

const hubConnection = createQueueConnection()
const isConnected = ref(false)

async function subscribeToCurrentTicket() {
  if (!isConnected.value) {
    return
  }
  const ticketId = localStorage.getItem('currentTicketId')
  if (!ticketId) {
    return
  }
  await hubConnection.invoke('SubscribeToTicket', ticketId)
}

async function handleTicketChanged(changedTicketId) {
  const storedTicketId = localStorage.getItem('currentTicketId')
  if (!storedTicketId || String(changedTicketId) !== storedTicketId) {
    return
  }
  try {
    await restoreCurrentTicket()
  } catch (error) {
    console.error('Current ticket updating error:', error)
  }
}

async function connectToStudentHub() {
  hubConnection.on('TicketChanged', handleTicketChanged)
  hubConnection.onreconnected(async () => {
    isConnected.value = true
    try {
      await subscribeToCurrentTicket()
      await restoreCurrentTicket()
    } catch (error) {
      console.error('Student resubscription error:', error)
    }
  })
  hubConnection.onclose(() => {
    isConnected.value = false
  })
  try {
    await hubConnection.start()
    isConnected.value = true
    await subscribeToCurrentTicket()
  } catch (error) {
    isConnected.value = false
    console.error('Failed to connect student to SignalR:', error)
  }
}
watch(
  () => currentTicket.value?.id,
  async ticketId => {
    if (!ticketId || !isConnected.value) {
      return
    }
    try {
      await hubConnection.invoke('SubscribeToTicket', String(ticketId))
    } catch (error) {
      console.error('Ticket subscription error:', error)
    }
  }
)

onMounted(async () => {
  await connectToStudentHub()
})

onUnmounted(() => {
  isConnected.value = false
  hubConnection.off('TicketChanged', handleTicketChanged)
  hubConnection.stop().catch(error => {
    console.error('Student SignalR stop error:', error)
  })
})
</script>

<template>
  <div class="student-wrapper">
    <div class="student-page">
      <StudentHeader />
      <main class="student-main">
        <RouterView />
      </main>
      <StudentBottom />
    </div>
  </div>
</template>

<style scoped>
  .student-wrapper {
    width: 100%;
    min-height: 100dvh;
    display: flex;
    justify-content: center;
    background-color: var(--color-background);
  }
  .student-page {
    width: 100%; 
    max-width: 480px;
    background: var(--color-surface);
    display: flex;
    flex-direction: column;
    font-size: 16px;
  }
  .student-main {
    max-width: 520px;
    margin-inline: auto;
    padding: 10px 20px 100px;
    display: flex;
    flex-direction: column;
  }
</style>