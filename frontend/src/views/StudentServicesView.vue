<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import { getServices } from '@/api/servicesApi'
import { createTicket } from '@/api/ticketsApi'
import { currentTicket, restoreCurrentTicket,
  hasActiveTicket } from '@/state/studentState'
import { iconService } from '@/components/icons/iconService'
import TicketIcon from '@/components/icons/TicketIcon.vue'

const router = useRouter()
const services = ref([])
const selectedServiceId = ref(null)
const isLoading = ref(true)
const isCreatingTicket = ref(false)
const errorMessage = ref('')

async function loadPage() {
  isLoading.value = true
  errorMessage.value = ''
  try {
    await restoreCurrentTicket()
    services.value = await getServices()
    selectedServiceId.value = null
  } catch (error) {
    console.error('Student services loading error:',
      error)
    errorMessage.value = 
      'Не удалось загрузить страницу услуг.'
  } finally {
    isLoading.value = false
  }
}
const selectedService = computed(() => {
  return services.value.find(service => service.id === selectedServiceId.value)
})
function selectService(serviceId) {
  selectedServiceId.value = serviceId
}
async function handleCreateTicket() {
  if (!selectedService.value || isCreatingTicket.value||
    hasActiveTicket.value) {
    return
  }
  isCreatingTicket.value = true
  errorMessage.value = ''
  try {
    const createdTicket = await createTicket(
      selectedService.value.id)
    localStorage.setItem('currentTicketId', String(createdTicket.id))  
  currentTicket.value = createdTicket
    await router.push('/student/ticket')
  } catch (error) {
    console.error('Creating ticket error:', error)
    errorMessage.value = 'Не удалось создать талон.'
  } finally {
    isCreatingTicket.value = false
  }
}
onMounted(loadPage)
</script>

<template>
  <div class="services-page">
    <h1>Выберите услугу</h1>
    <p v-if="isLoading">Загрузка услуг...</p>
    <p v-else-if="errorMessage" class="error-message">
      {{ errorMessage }}
    </p>
    <p v-else-if="services.length === 0" class="state-message">
      Сейчас нет доступных услуг.
    </p>
    <div v-else class="services-grid">
      <button v-for="service in services" :key="service.id"
        class="service-card" :class="{ selected: selectedServiceId === service.id }"
        type="button" @click="selectService(service.id)">
        <div class="service-icon">
          <component
            v-if="iconService[service.iconKey]"
            :is="iconService[service.iconKey]" />
        </div>
        <span class="service-name">
          {{ service.name }}
        </span>
      </button>
    </div>
    <button class="button-app" type="button"
      :disabled="!selectedServiceId || isCreatingTicket || hasActiveTicket"
      @click="handleCreateTicket">
      <TicketIcon />
      <div>
        {{ hasActiveTicket ? 'Талон уже получен' : isCreatingTicket ?
          'Получение...' : 'Получить талон' }}
      </div>
    </button>
  </div>
</template>

<style scoped>
.services-page {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px; 
}
.services-grid {
  width: 100%;
  display: grid;
  grid-template-columns: repeat(2,minmax(0,1fr));
  gap: 12px 10px;
}
.service-card {
  padding: 15px 10px;
  background-color: var(--color-surface); 
  display: flex;
  align-items: center;
  gap: 10px;
  align-self: stretch;
  border-radius: 15px;
  border: none;
  box-shadow: var(--shadow-card);
}
.service-icon {
  color: var(--color-primary);
}
.button-app {
  width: 100%;
}
.service-card.selected {
  background-color: var(--color-selected-element);
}
@media (max-width: 350px) {
  .services-grid {
    grid-template-columns: 1fr;
  }
}
</style>