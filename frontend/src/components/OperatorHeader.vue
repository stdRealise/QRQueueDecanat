<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { closeOperatorSession } from '@/api/operatorApi'
import susuLogo from '@/assets/susu_logo.png'

const router = useRouter()

const storedCounterNumber = Number(
  localStorage.getItem('counterNumber'))

const counterNumber =
  Number.isInteger(storedCounterNumber) &&
  storedCounterNumber > 0
    ? storedCounterNumber : null

const isPresent = ref(true)
const isLoggingOut = ref(false)
const logoutError = ref('')

function togglePresence() {
  isPresent.value = !isPresent.value
}

function clearOperatorData() {
  sessionStorage.removeItem('accessToken')
  sessionStorage.removeItem('authUser')
}

async function redirectToLogin() {
  clearOperatorData()
  await router.replace('/operator/start')
}

async function logout() {
  if (isLoggingOut.value) {
    return
  }
  isLoggingOut.value = true
  logoutError.value = ''
  try {
    await closeOperatorSession()
    await redirectToLogin()
  } catch (error) {
    console.error('Ошибка завершения сессии:', error)
    const status = error.response?.status
    if (status === 401 || status === 404) {
      await redirectToLogin()
      return
    }
    logoutError.value =
      error.response?.data?.detail ??
      error.response?.data?.message ??
      'Не удалось завершить рабочую сессию.'
  } finally {
    isLoggingOut.value = false
  }
}
</script>

<template>
  <header>
    <div class="operator-header">
      <div class="header-content">
        <img :src="susuLogo" alt="ЮУрГУ" class="logo" />
        <div class="header-text">
          <div class="app-name">Электронная очередь</div>
          <p>Деканат ЮУрГУ</p>
        </div>
        <div class="status">
          <template v-if="counterNumber">
            Окно {{ counterNumber }}
          </template>
          <template v-else>
            Окно не определено
          </template>
        </div>
        <button type="button" class="operator-status" :class="{ active: isPresent }"
          @click="togglePresence">
          <div class="status-switch">
            <div class="status-switch-dot" />
          </div>
          <div class="status-text">
            {{ isPresent ? 'На месте' : 'Перерыв' }}
          </div>
        </button>
        <button type="button" class="logout-button"
          :disabled="isLoggingOut" @click="logout">
          <div>Выход</div>
        </button>
      </div>
      <nav class="operator-navigation">
        <RouterLink to="/operator/queue">
          Очередь
        </RouterLink>
        <RouterLink to="/operator/history">
          История
        </RouterLink>
        <RouterLink to="/operator/services">
          Услуги
        </RouterLink>
      </nav>
    </div>
  </header>
</template>

<style scoped>
.operator-header {
  padding: 15px 40px;
}
.header-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
  box-sizing: border-box;
}
.header-text {
  display: flex;
  flex-direction: column;
}
.app-name {
  font-size: 32px;
  font-weight: bold;
}
.logo {
  width: 130px;
  height: auto;
}
.status {
  display: inline-flex;
  padding: 7px 14px;
  align-items: center;
  gap: 10px;
  border-radius: 10px;
  border: 1px solid var(--color-text-light);
}
.logout-button {
  padding: 7px 14px;
  color: var(--color-text);
}
.operator-status {
  padding: 0;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 10px;
  color: var(--color-text-light);
  background: transparent;
  border: none;
  cursor: pointer;
}
.status-switch {
  position: relative;
  width: 40px;
  height: 20px;
  display: block;
  border: 1px solid #ffffff;
  border-radius: 999px;
  background-color: #dbe2ef;
  box-sizing: border-box;
  transition: background-color 0.2s ease;
}
.status-switch-dot {
  position: absolute;
  top: 2px;
  left: 2px;
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background-color: #8d97aa;
}
.operator-status.active .status-switch-dot {
  background-color: var(--color-success);
  transform: translateX(19px);
}
.status-switch-dot {  
  width: 14px;
  height: 14px;
  border-radius: 50%;
  background-color: #8d97aa;
  transition:
    transform 0.2s ease,
    background-color 0.2s ease;
}
.operator-navigation {
  padding: 10px 20px;
  display: flex;
  gap: 30px;
  background: var(--color-primary);
}
.operator-navigation a {
  padding: 0px 5px;
  border-bottom: 2px solid transparent;
  color: var(--color-text-light);
  text-decoration: none;
  opacity: 0.75;
}
.operator-navigation a:hover {
  opacity: 1;
}
.operator-navigation a.router-link-active {
  border-bottom-color: var(--color-text-light);
  opacity: 1;
}
</style>
