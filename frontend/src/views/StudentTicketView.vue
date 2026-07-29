<script setup>
import { computed, onMounted, ref } from 'vue'
import { getTicket, cancelTicket } from '@/api/ticketsApi'
import { formatDateTime } from '@/utils/dateTime'
import { currentTicket, clearCurrentTicket, isTicketActive } from '@/state/studentState'
import ArrowIcon from '@/components/icons/ArrowIcon.vue'
import CalendarIcon from '@/components/icons/CalendarIcon.vue'
import CancelIcon from '@/components/icons/CancelIcon.vue'
import ClockIcon from '@/components/icons/ClockIcon.vue'
import CounterIcon from '@/components/icons/CounterIcon.vue'
import InfoIcon from '@/components/icons/InfoIcon.vue'
import QueueIcon from '@/components/icons/QueueIcon.vue'
import RefreshIcon from '@/components/icons/RefreshIcon.vue'

const ticket = currentTicket
const isLoading = ref(true)
const isUpdating = ref(false)
const isCancelling = ref(false)
const errorMessage = ref('')
const canCancel = computed(() => {
  return (ticket.value?.statusName === 'waiting' ||
    ticket.value?.statusName === 'called')
})
const isCalled = computed(() => {
  return ticket.value?.statusName === 'called'
})
const isCompleted = computed(() => {
  return ticket.value?.statusName === 'completed'
})
const isSkipped = computed(() => {
  return ticket.value?.statusName === 'skipped'
})
const isCanceled = computed(() => {
  return ticket.value?.statusName === 'canceled'
})
function getStoredTicketId() {
  const storedId = localStorage.getItem('currentTicketId')
  if (!storedId) {
    return null
  }
  return storedId
}
async function loadTicket() {
  if (isUpdating.value) {
    return
  }
  const ticketId = getStoredTicketId()
  if (!ticketId) {
    clearCurrentTicket()
    isLoading.value = false
    return
  }
  isUpdating.value = true
  errorMessage.value = ''
  try {
    ticket.value = await getTicket(ticketId)
  } catch (error) {
    console.error('Getting ticket error:', error)
    if (error.response?.status === 404) {
      localStorage.removeItem('currentTicketId')
      clearCurrentTicket()
      errorMessage.value = 'Талон не найден.'
    } else {
      errorMessage.value = 'Не удалось загрузить талон.'
    }
  } finally {
    isLoading.value = false
    isUpdating.value = false
  }
}

async function handleCancelTicket() {
  const ticketId = getStoredTicketId()
  if (!ticketId || !canCancel.value ||
    isCancelling.value || isUpdating.value
  ) {
    return
  }
  isCancelling.value = true
  errorMessage.value = ''
  try {
    await cancelTicket(ticketId)
    localStorage.removeItem('currentTicketId')
    clearCurrentTicket()
  } catch (error) {
    console.error('Canceling ticket error:', error)
    if (error.response?.status === 409) {
      await loadTicket()
      errorMessage.value =
        'Статус талона уже изменился. Данные обновлены.'
    } else {
      errorMessage.value = 'Не удалось отменить талон.'
    }
  } finally {
    isCancelling.value = false
  }
}
onMounted(loadTicket)
</script>

<template>
  <div class="ticket-page">
    <p v-if="errorMessage" class="error-message">
      {{ errorMessage }}
    </p>
    <p v-if="isLoading" class="state-message">
      Загрузка талона...
    </p>
    <template v-else>
      <section v-if="ticket && isCompleted">
        <h1>Обслуживание завершено</h1>
      </section>
      <section v-else-if="ticket && isSkipped">
        <h1>Талон пропущен</h1>
        <p>Получите новый талон при необходимости.</p>
      </section>
      <section v-else-if="ticket && isCanceled">
        <h1>Талон отменён</h1>
      </section>
      <section v-else-if="ticket">
        <template v-if="isCalled">
          <h1 class="called-element">
            {{ ticket.displayNumber }}
          </h1>
          <h2>{{ ticket.serviceName }}</h2>
          <div class="called-element">
            ОКНО {{ ticket.counterNumber ?? '—' }}
          </div>
        </template>
        <template v-else>
          <h1>Талон {{ ticket.displayNumber }}</h1>
          <h2>{{ ticket.serviceName }}</h2>
          <div class="ticket-info">
            <div class="ticket-info-row">
              <CalendarIcon />
              <p>Создан:</p>
              <p>{{ formatDateTime(ticket.createdAt) }}</p>
            </div>
            <div class="ticket-info-row">
              <QueueIcon />
              <p>Перед вами:</p>
            </div>
            <div class="ticket-info-row">
              <ClockIcon /> 
              <p>Ожидание:</p>
            </div>
            <div class="ticket-info-row">
              <CounterIcon />
              <p>Окна:</p>
            </div>
          </div>
        </template>
      </section>
      <section v-else>
        <h1>Талон не создан</h1>
        <p>
          Выберите услугу, чтобы получить талон.
        </p>
      </section>
      <template v-if="ticket && isTicketActive(ticket)">
        <div class="student-notification">
          <InfoIcon />
          <div class="notification-text">
            <template v-if="isCalled">
              <strong>Подойдите в течение 2 минут</strong> 
              <p>при опоздании талон отменяется.</p>
            </template>
            <template v-else>
              <strong>Следите за вызовом</strong> 
              <p>на телефоне и на табло в деканате.</p>
            </template>
          </div>
        </div>
        <button v-if="isCalled" class="button_contour button_danger" type="button" 
            :disabled="!canCancel || isUpdating || isCancelling" @click="handleCancelTicket">
            <CancelIcon />
            <div>
              {{ isCancelling ? 'Отмена...' : 'Не могу подойти' }}
            </div>
          </button>
        <div v-else class="ticket-actions">
          <button class="button_contour" type="button" 
            :disabled="isUpdating || isCancelling" @click="loadTicket">
            <RefreshIcon />
            <div> Обновить </div>
          </button>
          <button class="button_contour button_danger" type="button" 
            :disabled="!canCancel || isUpdating || isCancelling" @click="handleCancelTicket">
            <CancelIcon />
            <div>
              {{ isCancelling ? 'Отмена...' : 'Отменить' }}
            </div>
          </button>
        </div>
      </template>
    </template>
    <RouterLink to="/student/services" class="button-app">
      <ArrowIcon />
      Вернуться к услугам
    </RouterLink>
  </div>
</template>

<style scoped>
.ticket-page {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
}
section {
  width: 100%;
  padding: 20px;
  display: flex;
  flex-direction: column;
  align-items: center;
  border-radius: 15px;
  box-shadow: var(--shadow-card);
}
.ticket-info {
  width: 100%;
}
.ticket-info-row {
  display: flex;
  align-items: center;
  padding: 10px;
  gap: 10px;
  border-bottom: 1px solid #b8b8b8;
}
.ticket-info-row:last-child {
  border-bottom: none;
}
.ticket-actions {
  width: 100%;
  display: grid;
  grid-template-columns: repeat(2, minmax(0, 1fr));
  gap: 8px;
}
.button_contour {
  width: 100%;
  display: inline-flex;
  justify-content: center;
  align-items: center;
  border: 1px solid var(--color-primary);
  background: var(--color-surface);
  color: var(--color-primary);
  padding: 15px;
  gap: 10px;
}
.button_contour:active {
  background-color: var(--color-selected-element);
}
.button_danger {
  border-color: var(--color-danger);
  color: var(--color-danger);
}
.student-notification {
  width: 100%;
  display: flex;
  padding: 8px;
  justify-content: center;
  align-items: center;
  gap: 10px;
  border-radius: 10px;
  border: 1px solid var(--color-primary);
  background: var(--color-notification);
  color: var(--color-primary);
}
.notification-text {
  display: flex;
  flex-direction: column;
}
.called-element {
  color: var(--color-danger);
  font-weight: bold;
  font-size: 35px;
}
.button-app {
  width: 100%;
}
a {
  border-radius: 15px;
}
</style>