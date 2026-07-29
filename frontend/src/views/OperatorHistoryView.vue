<script setup>
import { onMounted, ref } from 'vue'
import { getOperatorHistory } from '@/api/operatorApi'
import { formatDateTime, formatTime } from '@/utils/dateTime'
import { getTicketStatus } from '@/utils/ticketStatuses'

const tickets = ref([])
const isLoading = ref(true)
const errorMessage = ref('')

async function loadHistory() {
  isLoading.value = true
  errorMessage.value = ''
  try {
    tickets.value = await getOperatorHistory()
  } catch (error) {
    console.error('Loading history error:', error)
    const status = error.response?.status
    if (status === 403) {
      errorMessage.value = 'У пользователя нет доступа к истории.'
    } else {
      errorMessage.value = 'Не удалось загрузить историю.'
    }
  } finally {
    isLoading.value = false
  }
}

onMounted(loadHistory)
</script>

<template>
  <section class="history-section">
    <div class="section-header">
      <div>
        <h1>История</h1>
        <p>Обработано талонов: {{ tickets.length }}</p>
      </div>
    </div>
    <p v-if="isLoading" class="state-message">
      Загрузка истории...
    </p>
    <template v-else>
      <p v-if="errorMessage" class="error-message">
        {{ errorMessage }}
      </p>
      <template v-else>
        <div v-if="tickets.length" class="table-wrapper">
          <table>
            <thead>
              <tr>
                <th>Талон</th>
                <th>Услуга</th>
                <th>Статус</th>
                <th>Создан</th>
                <th>Начало</th>
                <th>Завершение</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="ticket in tickets" :key="ticket.id">
                <td class="ticket-number">
                  {{ ticket.displayNumber }}
                </td>
                <td>
                  {{ ticket.serviceName }}
                </td>
                <td>
                  <div class="status-badge"
                    :class="getTicketStatus(
                      ticket.statusName).className">
                    {{ getTicketStatus(ticket.statusName).label }}
                  </div>
                </td>
                <td>
                  {{ formatDateTime(ticket.createdAt) }}
                </td>
                <td>
                  {{ formatTime(ticket.startedAt) }}
                </td>
                <td>
                  {{ formatTime(ticket.endedAt) }}
                </td>
              </tr>
            </tbody>
          </table>
        </div>
        <div v-else class="empty-history">
          <p>
            История пуста. Завершённые и пропущенные талоны появятся здесь.
          </p>
        </div>
      </template>
    </template>
  </section>
</template>

<style scoped>
.history-section {
  width: 100%;
  padding: 20px;
  border-radius: 15px;
  background: var(--color-surface);
  box-shadow: var(--shadow-card);
}

.section-header {
  margin-bottom: 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
}

.table-wrapper {
  width: 100%;
  overflow-x: auto;
}

.ticket-number {
  color: var(--color-primary);
  font-weight: bold;
}

.empty-history {
  padding: 50px 20px;
  text-align: center;
  color: var(--color-empty);
}

.state-message,
.error-message {
  text-align: center;
}

.error-message {
  color: var(--color-danger);
}
</style>