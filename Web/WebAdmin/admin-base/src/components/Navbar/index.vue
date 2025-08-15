<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '@/stores/user'
import { useRouter } from 'vue-router'

const userStore = useUserStore()
const router = useRouter()
const isFullscreen = ref(false)

// 用户退出登录
const handleLogout = async () => {
  userStore.logout()
  router.push('/login')
}

// 切换全屏
const toggleFullscreen = () => {
  if (!document.fullscreenElement) {
    document.documentElement.requestFullscreen()
    isFullscreen.value = true
  } else {
    if (document.exitFullscreen) {
      document.exitFullscreen()
      isFullscreen.value = false
    }
  }
}
</script>

<template>
  <div class="navbar">
    <div class="left-menu">
      <el-button class="menu-toggle" icon="Expand" circle text />
    </div>
    
    <div class="right-menu">
      <!-- 全屏切换 -->
      <el-button 
        class="fullscreen-button" 
        :icon="isFullscreen ? 'BottomLeft' : 'FullScreen'" 
        circle 
        text 
        @click="toggleFullscreen"
      />
      
      <!-- 用户信息下拉菜单 -->
      <el-dropdown class="user-dropdown">
        <div class="user-info">
          <el-avatar :size="30" icon="User" />
          <span class="user-name">Admin User</span>
        </div>
        
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item @click="handleLogout">
              <el-icon><SwitchButton /></el-icon>
              退出登录
            </el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.navbar {
  height: 50px;
  background-color: #fff;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 20px;
}

.left-menu {
  display: flex;
  align-items: center;
}

.right-menu {
  display: flex;
  align-items: center;
  gap: 15px;
}

.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
  
  .user-name {
    margin-left: 10px;
    font-size: 14px;
  }
}
</style>