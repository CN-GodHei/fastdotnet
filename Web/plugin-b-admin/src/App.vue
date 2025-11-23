<template>
  <div id="app">
    <router-view v-slot="{ Component }">
      <keep-alive>
        <component :is="Component" />
      </keep-alive>
    </router-view>
  </div>
</template>

<script setup lang="ts">
// 在子应用中也可以使用主应用传来的 keep-alive 信息
import { getMenuInfo } from './main'

// 可以通过 getMenuInfo() 获取主应用传递的菜单信息，包括是否需要 keep-alive
const menuInfo = getMenuInfo()
//console.log('Plugin A menu info:', menuInfo)
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>