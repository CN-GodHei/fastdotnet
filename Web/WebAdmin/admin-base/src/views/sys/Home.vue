<template>
  <el-container class="layout-container">
    <el-header class="header">
      <div class="logo">
        <el-icon class="fold-btn" @click="isCollapse = !isCollapse">
          <component :is="isCollapse ? 'Expand' : 'Fold'" />
        </el-icon>
        <span :class="{ 'hidden': isCollapse }">Fastdotnet Admin</span>
      </div>
      <div class="header-right">
        <el-dropdown>
          <span class="user-info">
            {{ userInfo.realName }}
            <el-icon><CaretBottom /></el-icon>
          </span>
          <template #dropdown>
            <el-dropdown-menu>
              <el-dropdown-item @click="handleLogout">退出登录</el-dropdown-item>
            </el-dropdown-menu>
          </template>
        </el-dropdown>
      </div>
    </el-header>
    <el-container>
      <el-aside :width="isCollapse ? '64px' : '200px'">
        <el-menu
          :default-active="activeMenu"
          class="menu"
          router
          unique-opened
          :collapse="isCollapse"
          :collapse-transition="false"
        >
          <template v-for="menu in menus" :key="menu.path">
            <el-sub-menu v-if="menu.children" :index="menu.path">
              <template #title>
                <el-icon><component :is="menu.meta.icon" /></el-icon>
                <span>{{ menu.meta.title }}</span>
              </template>
              <el-menu-item-group>
                <el-menu-item
                  v-for="child in menu.children"
                  :key="child.path"
                  :index="child.path"
                >
                  <el-icon><component :is="child.meta.icon" /></el-icon>
                  <span>{{ child.meta.title }}</span>
                </el-menu-item>
              </el-menu-item-group>
            </el-sub-menu>
            <el-menu-item v-else :index="menu.path">
              <el-icon><component :is="menu.meta.icon" /></el-icon>
              <span>{{ menu.meta.title }}</span>
            </el-menu-item>
          </template>
        </el-menu>
      </el-aside>
      <el-main>
        <Breadcrumb />
        <TabsView />
        <router-view />
        <div id="subapp-viewport"></div>
        <div id="subapp-viewport"></div>
      </el-main>
    </el-container>
  </el-container>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import axios from 'axios'
import { ElMessage } from 'element-plus'
import { CaretBottom, Fold, Expand } from '@element-plus/icons-vue'
import Breadcrumb from '@/components/Breadcrumb.vue'
import TabsView from '@/components/TabsView.vue'

const router = useRouter()
const activeMenu = ref('')
const menus = ref([])
const isCollapse = ref(false)
const userInfo = ref({
  realName: ''
})

// 获取用户信息
const getUserInfo = async () => {
  try {
    const res = await axios.get('/api/user/info')
    if (res.data.code === 200) {
      userInfo.value = res.data.data
    }
  } catch (error) {
    console.error('获取用户信息失败', error)
  }
}

// 获取菜单数据
const getMenus = async () => {
  try {
    const res = await axios.get('/api/user/menus')
    if (res.data.code === 200) {
      menus.value = res.data.data
    }
  } catch (error) {
    console.error('获取菜单失败', error)
  }
}

// 退出登录
const handleLogout = () => {
  localStorage.removeItem('token')
  router.push('/login')
  ElMessage.success('已退出登录')
}

onMounted(() => {
  getUserInfo()
  getMenus()
})
</script>

<style lang="scss" scoped>
.layout-container {
  height: 100vh;
  .header {
    display: flex;
    align-items: center;
    justify-content: space-between;
    background-color: #304156;
    color: #fff;
    padding: 0;
    height: 50px;
    .logo {
      display: flex;
      align-items: center;
      font-size: 20px;
      font-weight: bold;
      .fold-btn {
        padding: 0 15px;
        cursor: pointer;
        font-size: 16px;
        height: 50px;
        display: flex;
        align-items: center;
        &:hover {
          background: rgba(0, 0, 0, 0.1);
        }
      }
      span {
        transition: opacity 0.3s;
        padding-right: 15px;
        &.hidden {
          width: 0;
          padding-right: 0;
          opacity: 0;
        }
      }
    }
    .header-right {
      margin-right: 15px;
      .user-info {
        cursor: pointer;
        display: flex;
        align-items: center;
        gap: 4px;
      }
    }
  }
  .menu {
    height: 100%;
    border-right: none;
  }
}
</style>