<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { loginOperator } from '@/api/authApi'
import { startOperatorSession } from '@/api/operatorApi'
import { getCounterNumber } from '@/services/operatorStorage'

const router = useRouter()

const username = ref('')
const password = ref('')
const isStarting = ref(false)
const errorMessage = ref('')

async function startSession() {
  if (isStarting.value || !username.value ||
    !password.value) {
    return
  }
  const counterNumber = getCounterNumber()
  if (counterNumber === null) {
    errorMessage.value = 'Рабочее место не настроено.'
    return
  }
  isStarting.value = true
  errorMessage.value = ''
  let loginCompleted = false
  try {
    const authentication = await loginOperator(
      username.value, password.value)
    sessionStorage.setItem('accessToken', authentication.accessToken)
    sessionStorage.setItem('authUser', JSON.stringify(authentication.user))
    loginCompleted = true
    await startOperatorSession(counterNumber)
    await router.push('/operator/queue')
  } catch (error) {
    console.error('Operator login error:', error)
    if (loginCompleted) {
      sessionStorage.removeItem('accessToken')
      sessionStorage.removeItem('authUser')
    }
    const status = error.response?.status
    if (status === 401) {
      errorMessage.value =
        'Неверный логин или пароль.'
    } else if (status === 409) {
      errorMessage.value =
        'Окно занято или оператор уже работает в другом окне.'
    } else {
      errorMessage.value = 'Не удалось войти и открыть сессию.'
    }
  } finally {
    isStarting.value = false
  }
}
</script>

<template>
  <section class="session-start">
    <h1>Вход</h1>
    <p class="session-description">
      Введите данные
    </p>
    <label>
      <p>Логин</p>
      <input v-model.trim="username" type="text" 
        autocomplete="username" placeholder="Введите логин" />
    </label>
    <label>
      <p>Пароль</p>
      <input v-model="password" type="password"
        autocomplete="current-password" placeholder="Введите пароль" />
    </label>
    <p v-if="errorMessage" class="error-message">
      {{ errorMessage }}
    </p>
    <button type="button" class="button-app"
      :disabled="isStarting || !username || !password"
      @click="startSession">
      {{ isStarting ? 'Вход...' : 'Начать работу'
      }}
    </button>
  </section>
</template>

<style scoped>
.session-start {
  width: min(100%, 430px);
  margin: auto;
  padding: 28px;
  display: flex;
  flex-direction: column;
  gap: 15px;
  border-radius: 15px;
  background: var(--color-surface);
  box-shadow: var(--shadow-card);
}

.session-start h1,
.session-description {
  text-align: center;
}

label {
  display: flex;
  flex-direction: column;
  gap: 7px;
}

input {
  min-height: 46px;
  padding: 10px 12px;
  border: 1px solid var(--color-border-medium);
  border-radius: 9px;
  background: var(--color-surface);
  font: inherit;
}
input:focus {
  border-color: var(--color-primary);
  outline: 2px solid rgba(38, 71, 150, 0.15);
}

.error-message {
  color: var(--color-danger);
  text-align: center;
}
</style>