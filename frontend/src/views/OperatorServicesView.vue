<script setup>
import { computed, onMounted, ref } from 'vue'
import { getOperatorServices, updateOperatorServices } from '@/api/operatorApi'
import { iconService } from '@/components/icons/iconService'

const services = ref([])
const initialServiceIds = ref([])
const isLoading = ref(true)
const isSaving = ref(false)
const errorMessage = ref('')
const successMessage = ref('')

const selectedServiceIds = computed(() => {
  return services.value
    .filter(service => service.isSelected)
    .map(service => service.id)
})

const hasChanges = computed(() => {
  const current = selectedServiceIds.value
  const initial = initialServiceIds.value
  if (current.length !== initial.length) return true
  const initialSet = new Set(initial)
  return current.some(id => !initialSet.has(id))
})

async function loadServices() {
  isLoading.value = true
  errorMessage.value = ''
  successMessage.value = ''
  try {
    services.value = await getOperatorServices()
    initialServiceIds.value = [...selectedServiceIds.value]
  } catch (error) {
    console.error('Loading services error:', error)
    const status = error.response?.status
    if (status === 403) {
      errorMessage.value =
         'У пользователя нет доступа к выбору услуг.'
    } else {
      errorMessage.value = 'Не удалось загрузить услуги.'
    }
  } finally {
    isLoading.value = false
  }
}

function toggleService(service) {
  if (isSaving.value) {
    return
  }
  service.isSelected = !service.isSelected
  successMessage.value = ''
  errorMessage.value = ''
}

async function saveServices() {
  if (isSaving.value || !hasChanges.value) {
    return
  }
  isSaving.value = true
  errorMessage.value = ''
  successMessage.value = ''
  try {
    await updateOperatorServices(selectedServiceIds.value)
    initialServiceIds.value = [...selectedServiceIds.value]
    successMessage.value = 'Услуги сохранены.'
  } catch (error) {
    console.error('Saving services error:', error)
    const status = error.response?.status
    if (status === 403) {
      errorMessage.value = 'У пользователя нет доступа к выбору услуг.'
    } else {
      errorMessage.value = 'Не удалось сохранить услуги.'
    }
  } finally {
    isSaving.value = false
  }
}

onMounted(loadServices)
</script>

<template>
  <div class="operator-services">
    <section class="services-section">
      <div class="section-header">
        <div>
          <h1>Мои услуги</h1>
          <p>Выбрано: {{ selectedServiceIds.length }}</p>
        </div>
        <button class="button-app" type="button"
          :disabled="!hasChanges || isSaving"
          @click="saveServices">
          {{ isSaving ? 'Сохранение...' : 'Сохранить' }}
        </button>
      </div>
      <p v-if="isLoading" class="state-message">
        Загрузка услуг...
      </p>
      <template v-else>
        <p v-if="errorMessage" class="error-message">
          {{ errorMessage }}
        </p>
        <template v-else>
          <p v-if="successMessage" class="success-message">
            {{ successMessage }}
          </p>
          <div v-if="services.length" class="services-grid">
            <button v-for="service in services"
              :key="service.id" class="service-card"
              :class="{ 'service-card_selected': service.isSelected }" 
              type="button" @click="toggleService(service)">
              <div class="service-icon">
                <component v-if="iconService[service.icon]"
                  :is="iconService[service.icon]"/>
                <div v-else>
                  {{ service.prefix }}
                </div>
              </div>
              <div class="service-name">
                {{ service.name }}
              </div>
              <div class="service-checkbox"
                :class="{ 'service-checkbox_checked': service.isSelected }">
                <div v-if="service.isSelected">✓</div>
              </div>
            </button>
          </div>
          <p v-else>
            Доступные услуги не найдены.
          </p>
        </template>
      </template>
    </section>
  </div>
</template>

<style scoped>
.operator-services {
  width: 100%;
}

.services-section {
  width: 100%;
  padding: 20px;
  border-radius: 15px;
  background: var(--color-surface);
  box-shadow: var(--shadow-card);
}

.section-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  gap: 20px;
}

.services-grid {
  padding-top: 18px;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 10px;
}

.service-card {
  min-height: 70px;
  padding: 12px;
  display: grid;
  grid-template-columns: auto 1fr auto;
  align-items: center;
  gap: 10px;
  border: 1px solid var(--color-primary);
  border-radius: 10px;
  background: var(--color-surface);
  text-align: left;
  cursor: pointer;
}

.service-card:hover {
  background: var(--color-hover-element);
}

.service-card_selected {
  background: var(--color-selected-element);
  box-shadow: 0 2px 6px rgba(38, 71, 150, 0.2);
}

.service-icon {
  width: 26px;
  height: 26px;
  display: inline-flex;
  justify-content: center;
  align-items: center;
  color: var(--color-primary);
}

.service-name {
  line-height: 1.25;
}

.service-checkbox {
  width: 18px;
  height: 18px;
  display: inline-flex;
  justify-content: center;
  align-items: center;
  border: 1px solid var(--color-primary);
  border-radius: 4px;
  color: var(--color-text-light);
  font-weight: 700;
}
.service-checkbox_checked {
  background: var(--color-primary);
}

.error-message {
  color: var(--color-danger);
}
.success-message {
  color: var(--color-success);
}
.state-message {
  text-align: center;
}

@media (max-width: 670px) {
  .services-grid {
    grid-template-columns: 1fr;
  }
}
</style>