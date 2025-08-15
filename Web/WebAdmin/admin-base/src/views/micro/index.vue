<script setup lang="ts">
import { onMounted, onUnmounted } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()
const microAppContainerId = 'subapp-viewport'

onMounted(() => {
  // 微前端应用将在该容器中加载
  console.log('Micro app container mounted')
})

onUnmounted(() => {
  // 清理工作
  console.log('Micro app container unmounted')
})
</script>

<template>
  <div class="micro-app-container">
    <div :id="microAppContainerId" class="micro-app-wrapper"></div>
  </div>
</template>

<style lang="scss" scoped>
.micro-app-container {
  width: 100%;
  height: 100%;
  
  .micro-app-wrapper {
    width: 100%;
    height: 100%;
  }
}
</style>