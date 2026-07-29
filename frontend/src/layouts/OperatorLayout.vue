<script setup>
import OperatorHeader from '@/components/OperatorHeader.vue'
</script>

<template>
  <div class="operator-page">
    <OperatorHeader />
    <main class="operator-main">
      <RouterView />
    </main>
  </div>
</template>

<style scoped>
.operator-page {
  width: 100%;
  min-height: 100vh;
  background: var(--color-surface);
  display: flex;
  flex-direction: column;
}
.operator-main {
  flex: 1;
  width: 100%;
  padding: 20px;
  display: flex;
  flex-direction: column;
}
</style>