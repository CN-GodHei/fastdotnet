<script setup lang="ts">
import { onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import Sidebar from '@/components/Sidebar/index.vue'
import Navbar from '@/components/Navbar/index.vue'
import Breadcrumb from '@/components/Breadcrumb.vue'
import TabsView from '@/components/TabsView.vue'

const userStore = useUserStore()

onMounted(async () => {
  // 加载用户菜单
  await userStore.loadUserMenus()
})
</script>

<template>
  <div class="app-wrapper">
    <!-- 侧边栏 -->
    <div class="sidebar-container">
      <Sidebar />
    </div>
    
    <div class="main-container">
      <!-- 顶部导航栏 -->
      <Navbar />
      
      <!-- 面包屑导航 -->
      <Breadcrumb />
      
      <!-- 标签页 -->
      <TabsView />
      
      <!-- 主要内容区域 -->
      <div class="app-main">
        <router-view />
      </div>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.app-wrapper {
  display: flex;
  height: 100vh;
  width: 100%;
  position: relative;
}

.sidebar-container {
  width: 210px;
  background-color: #001529;
  transition: width 0.28s;
  flex-shrink: 0;
  height: 100%;
}

.main-container {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.app-main {
  flex: 1;
  overflow: auto;
  padding: 20px;
  background-color: #f0f2f5;
}
</style>