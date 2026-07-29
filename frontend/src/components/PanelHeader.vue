<script setup>
import { computed, onMounted, onUnmounted, ref } from 'vue'
import { formatDate, formatTime } from '@/utils/dateTime'
import susuLogo from '@/assets/susu_logo.png'
import QueueIcon from '@/components/icons/QueueIcon.vue'
import ClockIcon from '@/components/icons/ClockIcon.vue'
import CounterIcon from '@/components/icons/CounterIcon.vue'

defineProps({
  waitingCount: {
    type: Number, default: 0
  },
  servingCount: {
    type: Number, default: 0
  },
  averageWaitingMinutes: {
    type: Number, default: 0
  }
})

let timerId = null
const now = ref(new Date())
const currentTime = computed(() => formatTime(now.value))
const currentDate = computed(() => formatDate(now.value))
onMounted(() => {
  timerId = window.setInterval(() => {
    now.value = new Date()
  }, 30000)
})
onUnmounted(() => {
  if (timerId !== null) {
    window.clearInterval(timerId)
    timerId = null
  }
})
</script>

<template>
  <header>
    <div class="panel-header">
      <img :src="susuLogo" alt="ЮУрГУ" class="logo" />
      <div class="header-text">
        <div class="app-name">Электронная очередь</div>
        <p>Деканат ЮУрГУ</p>
      </div>
      <div class="panel-stats">
        <div class="stat-item">
          <QueueIcon />
          <div class="stat-name">Ожидают</div>
          <div class="stat-info">{{ waitingCount }}</div>
        </div>
        <div class="stat-item">
          <ClockIcon />
          <div class="stat-name">Обслуживание</div>
          <div class="stat-info">{{ servingCount }}</div>
        </div>
        <div class="stat-item">
          <CounterIcon />
          <div class="stat-name">Среднее ожидание</div>
          <div class="stat-info">{{ averageWaitingMinutes }}</div>
        </div>
      </div>
      <div class="time-element">
        <span class="time-info">{{ currentTime }}</span>
        <span class="date-info">{{ currentDate }}</span>
      </div>
    </div>
  </header>
</template>

<style scoped>
.panel-header {
  padding: 20px;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 20px;
}
.header-text {
  display: flex;
  flex-direction: column;
  gap: 5px;
}
.app-name {
  font-size: 35px;
  font-weight: bold; 
}
.logo {
  width: 130px;
  height: auto;
}
.panel-stats {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  border-radius: 8px;
  background: var(--color-surface);
  color: var(--color-primary);
}
.stat-item {
  padding: 9px 12px;
  display: flex;
  flex-direction: column;
  gap: 5px;
  align-items: center;
  justify-content: center;
  border-right: 1px solid var(--color-border-medium);
  text-align: center;
}
.stat-item:last-child {
  border-right: none;
}
.stat-name {
  font-size: 20px;
}
.stat-info {
  font-weight: bold;
  font-size: 25px;
}
.time-element {
  display: flex;
  align-items: center;
  justify-content: center;
  flex-direction: column;
}
.time-info {
  font-size: 40px;
  font-weight: bold;
}
.date-info {
  font-size: 20px;
}
</style>
