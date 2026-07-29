<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { getOperatorWorkspace, callNextTicket, startTicket,
  completeTicket, skipTicket } from '@/api/operatorApi'
import { createQueueConnection } from '@/utils/queueConnection'
import { formatTime } from '@/utils/dateTime'
import { getTicketStatus } from '@/utils/ticketStatuses'

const workspace = ref(null)
const isLoading = ref(true)
const isProcessing = ref(false)
const errorMessage = ref('')

const currentTicket = computed(() => {
  return workspace.value?.currentTicket
})

const waitingTickets = computed(() => {
  return workspace.value?.waitingTickets ?? []
})

async function loadWorkspace() {
  const isInitialLoading =
    workspace.value === null

  if (isInitialLoading) {
    isLoading.value = true
  }
  errorMessage.value = ''
  try {
    workspace.value = await getOperatorWorkspace()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Не удалось загрузить очередь.'
  } finally {
    if (isInitialLoading) {
      isLoading.value = false
    }
  }
}

async function runAction(action) {
  if (isProcessing.value) {
    return
  }
  isProcessing.value = true
  errorMessage.value = ''
  try {
    await action()
    await loadWorkspace()
  } catch (error) {
    console.error(error)
    errorMessage.value = 'Не удалось выполнить действие.'
  } finally {
    isProcessing.value = false
  }
}

async function callTicket() {
  await runAction(() => callNextTicket())
}

async function startCurrentTicket() {
  const ticket = currentTicket.value
  if (!ticket) {
    return
  }
  await runAction(() => startTicket(ticket.id))
}

async function completeCurrentTicket() {
  const ticket = currentTicket.value
  if (!ticket) {
    return
  }
  await runAction(() => completeTicket(ticket.id))
}

async function skipCurrentTicket() {
  const ticket = currentTicket.value
  if (!ticket) {
    return
  }
  await runAction(() => skipTicket(ticket.id))
}

const hubConnection = createQueueConnection()
async function connectToOperatorHub() {
  hubConnection.on('QueueChanged', loadWorkspace)
  hubConnection.onreconnected(async () => {
    try {
      await hubConnection.invoke('SubscribeToOperators')
      await loadWorkspace()
    } catch (error) {
      console.error('Operator resubscription error: ', error)
    }
  })
  try {
    await hubConnection.start()
    await hubConnection.invoke('SubscribeToOperators')
  } catch (error) {
    console.error('Failed to connect panel to SignalR:', error)
  }
}

onMounted(async () => {
  await loadWorkspace()
  await connectToOperatorHub()
})
onUnmounted(() => {
  hubConnection.stop().catch(error => {
    console.error('SignalR panel stop error:', error)
  })
})
</script>

<template>
  <div class="operator-workspace">
    <p v-if="isLoading">
      Загрузка очереди...
    </p>
    <template v-else>
      <p v-if="errorMessage" class="error-message"> 
        {{ errorMessage }}</p>
      <template v-if="workspace">
        <section class="workspace-section">
          <template v-if="currentTicket">
            <h1>
              Талон {{ currentTicket.displayNumber }}
            </h1>
            <div>{{ currentTicket.serviceName }}</div>
            <div>Статус: 
              <div class="status-badge" 
                :class="getTicketStatus(currentTicket.statusName).className">
                {{ getTicketStatus(currentTicket.statusName).label }}
              </div>
            </div>
            <div>Создан: {{ formatTime(currentTicket.createdAt) }}</div>
          </template>

          <template v-else>
            <h1>Очередь</h1>
            <table v-if="waitingTickets.length">
              <thead>
                <tr>
                  <th>Талон</th>
                  <th>Услуга</th>
                  <th>Время</th>
                </tr>
              </thead>
              <tbody>
                <tr v-for="ticket in waitingTickets" :key="ticket.id">
                  <td>{{ ticket.displayNumber }}</td>
                  <td>{{ ticket.serviceName }}</td>
                  <td>{{ formatTime(ticket.createdAt) }}</td>
                </tr>
              </tbody>
            </table>
            <p v-else>
              В очереди нет талонов.
            </p>
          </template>
        </section>

        <section class="workspace-section">
          <h2>Действия</h2>
          <div class="actions">
            <template v-if="!currentTicket">
              <button type="button" class="button-app"
                :disabled="isProcessing || waitingTickets.length === 0"
                @click="callTicket">
                {{ isProcessing ? 'Вызов...' : 'Вызвать' }}
              </button>
            </template>
            <template v-else-if="currentTicket.statusName === 'called'">
              <button type="button" class="button-app"
                :disabled="isProcessing" @click="startCurrentTicket">
                Начать обслуживание
              </button>
              <button type="button" class="button-contour"
                :disabled="isProcessing" @click="skipCurrentTicket">
                Пропустить
              </button>
            </template>
            <button v-else-if="currentTicket.statusName === 'serving'"
              type="button" class="button-app"
              :disabled="isProcessing" @click="completeCurrentTicket">
              {{ isProcessing ? 'Завершение...' : 'Завершить обслуживание' }}
            </button>
            <p v-else class="actions-empty">
              Для текущего состояния действия недоступны.
            </p>
          </div>
        </section>
      </template>
    </template>
  </div>
</template>

<style scoped>
.operator-workspace {
  width: 100%;
  display: grid;
  grid-template-columns: minmax(0, 2fr) minmax(280px, 1fr);
  gap: 20px;
}

.workspace-section {
  padding: 20px;
  border-radius: 15px;
  background: var(--color-surface);
  box-shadow: var(--shadow-card);
}

.actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.button-app,
.button-contour {
  min-height: 44px;
  padding: 10px 18px;
  border-radius: 10px;
  cursor: pointer;
}

.actions-empty {
  color: var(--color-empty);
}

.button-contour {
  border: 1px solid var(--color-primary);
  background: var(--color-surface);
  color: var(--color-primary);
}

button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.error-message {
  color: var(--color-danger);
}
</style>