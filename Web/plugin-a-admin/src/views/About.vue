<!-- src/views/About.vue -->
<template>
  <div class="about" style="background-color: #fffbe6; padding: 50px; border: 2px solid #ffe58f;">
    <h1 style="font-size: 48px; color: #faad14;">Plugin A - ABOUT PAGE</h1>
    <p style="font-size: 24px;">这里是插件A的 **关于** 页面。</p>
    <p>This page provides more information about Plugin A.</p>
    <p>Current Route: {{ $route.path }}</p>
  </div>
</template>

<script setup lang="ts">
// Plugin A About View
</script>

<style scoped>
.about {
  text-align: center;
  border-radius: 8px;
}
</style>
