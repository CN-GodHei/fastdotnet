<template>
  <section class="app-main">
    <router-view v-slot="{ Component }">
      <transition name="fade-transform" mode="out-in">
        <keep-alive>
          <component :is="Component" />
        </keep-alive>
      </transition>
    </router-view>
    <div id="subapp-viewport"></div>
  </section>
</template>

<script setup lang="ts">
// 主内容区域组件
</script>

<style lang="scss" scoped>
.app-main {
  min-height: calc(100vh - 50px);
  width: 100%;
  position: relative;
  overflow: hidden;
  padding: 20px;
  background-color: #f0f2f5;
}

#subapp-viewport {
  width: 100%;
  height: calc(100vh - 90px);
  margin-top: 20px;
}
</style>