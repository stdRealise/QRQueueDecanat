<script setup>
import ServicesIconL from '@/components/icons/ServicesLineIcon.vue'
import ServicesIcon from '@/components/icons/ServicesIcon.vue'
import TicketIconL from '@/components/icons/TicketLineIcon.vue'
import TicketIcon from '@/components/icons/TicketIcon.vue'

const navigationItems = [
  {
    route: '/student/services',
    label: 'Услуги',
    iconActive: ServicesIcon,
    iconInactive: ServicesIconL
  },
  {
    route: '/student/ticket',
    label: 'Талон',
    iconActive: TicketIcon,
    iconInactive: TicketIconL
  }
]
</script>

<template>
  <nav class="bottom-nav">
    <RouterLink v-for="item in navigationItems"
      :key="item.route" :to="item.route"
      v-slot="{ isActive }" class="nav-link">
      <component :is="isActive ? item.iconActive : item.iconInactive"
        class="nav-icon"/>
      {{ item.label }}
    </RouterLink>
  </nav>
</template>

<style scoped>
.bottom-nav {
  width: 100%;
  max-width: 480px;
  position: fixed;
  bottom: 0;
  left: 50%;
  transform: translateX(-50%);
  background: var(--color-surface);
  display: flex;
  align-items: stretch;
  border-top: 1px solid var(--color-border-light);
  padding-bottom: 20px;
}
.nav-link {
  padding: 6px;
  color: var(--color-primary);
  text-decoration: none;
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  gap: 4px;
}
.nav-icon {
  width: 24px;
  height: auto;
}
</style>
