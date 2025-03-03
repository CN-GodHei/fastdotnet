<template>
  <el-container class="layout-container">
    <!-- 顶部导航栏 -->
    <el-header height="60px">
      <div class="header-left">
        <el-button :icon="Fold" @click="toggleSidebar" />
        <h2 class="site-title">FastDotnet Admin</h2>
      </div>
      <div class="header-right">
        <el-dropdown trigger="click">
          <div class="user-info">
            <el-avatar :size="32" :src="userStore.avatar" />
            <span>{{ userStore.realName }}</span>
            <el-icon><CaretBottom /></el-icon>
          </div>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item @click="handleLogout">退出登录</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </el-header>

    <el-container>
      <!-- 侧边菜单 -->
      <el-aside :width="isCollapse ? '64px' : '200px'" class="sidebar">
        <el-menu
          :collapse="isCollapse"
          :router="true"
          :default-active="route.path"
          class="menu"
        >
          <template v-for="menu in menuStore.menus" :key="menu.path">
            <template v-if="menu.children && menu.children.length > 0">
              <el-sub-menu :index="menu.path">
                <template #title>
                  <el-icon><component :is="menu.meta.icon" /></el-icon>
                  <span>{{ menu.meta.title }}</span>
                </template>
                <el-menu-item
                  v-for="child in menu.children"
                  :key="child.path"
                  :index="child.path"
                >
                  <el-icon><component :is="child.meta.icon" /></el-icon>
                  <span>{{ child.meta.title }}</span>
                </el-menu-item>
              </el-sub-menu>
            </template>
            <template v-else>
              <el-menu-item :index="menu.path">
                <el-icon><component :is="menu.meta.icon" /></el-icon>
                <span>{{ menu.meta.title }}</span>
              </el-menu-item>
            </template>
          </template>
        </el-menu>
      </el-aside>

      <!-- 主要内容区域 -->
      <el-main>
        <router-view />
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { Fold, CaretBottom } from '@element-plus/icons-vue'
import { useUserStore } from '@/stores/user'
import { useMenuStore } from '@/stores/menu'

const route = useRoute()
const router = useRouter()
const userStore = useUserStore()
const menuStore = useMenuStore()

// 获取菜单数据
const initMenus = async () => {
  await menuStore.getMenus()
}

// 在组件挂载时获取菜单数据
onMounted(() => {
  initMenus()
})

// 侧边栏折叠状态
const isCollapse = ref(false)

// 切换侧边栏
const toggleSidebar = () => {
  isCollapse.value = !isCollapse.value
}

// 退出登录
const handleLogout = () => {
  userStore.logout()
  router.push('/login')
}
</script>

<style lang="scss" scoped>
.layout-container {
  height: 100vh;
  display: flex;
  flex-direction: column;

  :deep(.el-container) {
    flex: 1;
    overflow: hidden;
  }

  :deep(.el-header) {
    background-color: #fff;
    border-bottom: 1px solid #e6e6e6;
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0 20px;

    .header-left {
      display: flex;
      align-items: center;
      gap: 10px;

      .site-title {
        margin: 0;
        font-size: 18px;
        color: #303133;
      }
    }

    .header-right {
      .user-info {
        display: flex;
        align-items: center;
        gap: 8px;
        cursor: pointer;

        span {
          color: #303133;
        }
      }
    }
  }

  :deep(.el-aside) {
    background-color: #304156;
    transition: width 0.3s;
    overflow-x: hidden;

    .menu {
      border-right: none;
      background-color: transparent;

      :deep(.el-menu-item),
      :deep(.el-sub-menu__title) {
        color: #bfcbd9;

        &:hover {
          background-color: #263445;
        }

        &.is-active {
          color: #409eff;
          background-color: #263445;
        }
      }
    }
  }

  :deep(.el-main) {
    background-color: #f0f2f5;
    padding: 20px;
    overflow-y: auto;
    height: 100%;
  }
}
</style>