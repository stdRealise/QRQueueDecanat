<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { getPanel } from '@/api/panelApi'
import { createQueueConnection } from '@/utils/queueConnection'
import CounterIcon from '@/components/icons/CounterIcon.vue'
import InfoIcon from '@/components/icons/InfoIcon.vue'
import PanelHeader from '@/components/PanelHeader.vue'

const panel = ref(null)
const isUpdating = ref(false)
const errorMessage = ref('')
const calls = computed(() => {
  return panel.value?.calls ?? []
})
const waitingNumbers = computed(() => {
  return panel.value?.waitingNumbers ?? []
})
async function loadPanel() {
  if (isUpdating.value) {
    return
  }
  isUpdating.value = true
  try {
    panel.value = await getPanel()
    errorMessage.value = ''
  } catch (error) {
    console.error('Panel loading error:', error)
    errorMessage.value = 'Не удалось загрузить табло.'
  } finally {
    isUpdating.value = false
  }
}

const hubConnection = createQueueConnection()

async function connectToPanelHub() {
  hubConnection.on('QueueChanged', loadPanel)
  hubConnection.onreconnected(async () => {
    try {
      await hubConnection.invoke('SubscribeToPanel')
      await loadPanel()
    } catch (error) {
      console.error('Panel resubscription error:', error)
    }
  })
  try {
    await hubConnection.start()
    await hubConnection.invoke('SubscribeToPanel')
  } catch (error) {
    console.error('Failed to connect panel to SignalR:', error)
  }
}

onMounted(async () => {
  await connectToPanelHub()
  await loadPanel()
})
onUnmounted(() => {
  hubConnection.stop().catch(error => {
    console.error('SignalR panel stop error:', error)
  })
})
</script>

<template>
  <div class="panel-page">
    <PanelHeader
      :waiting-count="panel?.waitingCount ?? 0"
      :serving-count="panel?.servingCount ?? 0"
      :average-waiting-minutes="
        panel?.averageWaitingMinutes ?? 0 "
    />
    <main class="panel-main">
      <p v-if="errorMessage" class="error-message">
        {{ errorMessage }}
      </p>

      <div class="calls-section">
        <div class="ticket-grid">
          <div v-for="ticket in calls"
            :key="ticket.id" class="ticket-call">
            <div class="ticket-number">
              {{ ticket.displayNumber }}
            </div>
            <div class="ticket-counter">
              <CounterIcon />
              Окно {{ ticket.counterNumber }}
            </div>
          </div>
        </div>
        <div class="waiting-list">
          <div v-for="number in waitingNumbers"
            :key="number" class="waiting-number">
            {{ number }}
          </div>
        </div>
      </div>
      <div class="panel-notification">
        <InfoIcon />
        <div class="notification-text">
          <strong>Следите за номером талона и номером окна</strong>
          <p>Получить талон можно по QR-коду</p>
        </div>
      </div>
    </main>
  </div>
</template>

<style scoped>
.panel-page {
  min-height: 100vh;
  background: var(--color-background);
}
.panel-main {
  padding: 25px;
}
.calls-section {
  display: grid;
  grid-template-columns: minmax(0, 1fr) 280px;
  gap: 60px;
}
.ticket-grid {
  display: grid;
  grid-template-columns: repeat(4, minmax(210px, 1fr));
  gap: 12px;
}
.ticket-call {
  min-height: 125px;
  display: flex;
  flex-direction: column;
  border: 1px solid #83d79a;
  background: #ebffe9;
  border-radius: 8px;
}
.ticket-number {
  flex: 1;
  padding: 12px;
  display: flex;
  justify-content: center;
  align-items: center;
  color: var(--color-primary);
  font-size: 52px;
  font-weight: bold;
}
.ticket-counter {
  min-height: 53px;
  padding: 8px;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 12px;
  border-top: 1px solid var(--color-border-medium);
  font-size: 32px;
  font-weight: bold;
  color: var(--color-primary);
}
.waiting-list {
  width: 100%;
  border: 1px solid var(--color-border-strong);
  border-radius: 8px;
  overflow: hidden;
  background: var(--color-surface);
}
.waiting-number {
  padding: 14px 20px;
  border-bottom: 1px solid var(--color-border-medium);
  color: var(--color-primary);
  font-size: 28px;
  font-weight: bold;
}
.waiting-number:last-child {
  border-bottom: none;
}
.panel-notification {
  position: fixed;
  right: 25px;
  bottom: 10px;
  left: 25px;
  margin-bottom: 10px;
  padding: 12px 24px;
  display: flex;
  align-items: center;
  gap: 20px;
  border-radius: 20px;
  background: var(--color-primary);
  color: var(--color-text-light);
}
.notification-text {
  display: flex;
  flex-direction: column;
}
.error-message {
  color: var(--color-danger);
}
</style>