<script setup>
import { computed } from 'vue'
import { currentTicket, workingWindowsCount } from '@/state/studentState'
import susuLogo from '@/assets/susu_logo.png'

function getWorkingWindowsText(count) {
  const lastDigit = count % 10
  if (lastDigit === 1) {
    return `${count} окно работает`
  }
  if (lastDigit >= 2 && lastDigit <= 4) {
    return `${count} окна работают`
  }
  return `${count} окон работают`
}

const headerStatus = computed(() => {
  const statusName = currentTicket.value?.statusName
  switch (statusName) {
    case 'waiting':
      return {
        text: 'Вы в очереди',
        className: 'status_waiting',
        showDot: true
      }
    case 'called':
      return {
        text: 'Вас вызывают',
        className: 'status_called',
        showDot: false
      }
    case 'serving':
      return {
        text: 'Талон обслуживается',
        className: 'status_serving',
        showDot: true
      }
    case 'completed':
      return {
        text: 'Обслуживание завершено',
        className: 'status_completed',
        showDot: false
      }
    case 'skipped':
      return {
        text: 'Талон пропущен',
        className: 'status_skipped',
        showDot: false
      }
    default:
      return {
        text: getWorkingWindowsText(workingWindowsCount.value),
        className: 'status_windows',
        showDot: true
      }
  }
})
</script>

<template>
  <header>
    <div class="student-header">
      <div class="app-name">Электронная очередь</div>
      <div class="header-content">
        <div class="header-text">
          <p>Деканат ЮУрГУ</p>
          <div class="status" :class="headerStatus.className">
            <div v-if="headerStatus.showDot" class="status-dot"
              :class="{ 'status-dot_active': currentTicket ||
                workingWindowsCount > 0 }" />
            <div>{{ headerStatus.text }}</div>
          </div>
        </div>
        <img :src="susuLogo" alt="ЮУрГУ" class="logo" />
      </div>
    </div>
  </header>
</template>

<style scoped>
.student-header {
  padding: 15px 20px;
  display: flex;
  flex-direction: column;
  gap: 5px;
  color: var(--color-text-light);
}
.app-name {
  font-size: 25px;
  font-weight: bold;
}
.header-content {
  display: flex;
  align-items: center;
  justify-content: space-between;
}
.header-text {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 10px;
}
.logo {
  width: 80px;
  height: auto;
}
.status {
  display: inline-flex;
  padding: 5px 10px;
  align-items: center;
  gap: 10px;
  border-radius: 10px;
  border: 1px solid;
  background: transparent;
}
.status-dot {
  width: 9px;
  height: 9px;
  border-radius: 50%;
  background: var(--color-disabled);
}
.status-dot_active {
  background: var(--color-success);
}
.status_waiting {
  border-color: var(--color-success);
  color: var(--color-success);
  background: transparent;
}
.status_called {
  background: var(--color-danger);
  color: var(--color-text-light);
  border: none;
}
</style>
