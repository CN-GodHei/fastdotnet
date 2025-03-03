<template>
  <el-container class="layout-container">
    <el-header class="header">
      <div class="logo">Fastdotnet Admin</div>
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
      <el-aside width="200px">
        <el-menu
          :default-active="activeMenu"
          class="menu"
          router
          unique-opened
        >
          <template v-for="menu in menus" :key="menu.path">
            <el-sub-menu v-if="menu.children" :index="menu.path">
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
            <el-menu-item v-else :index="menu.path">
              <el-icon><component :is="menu.meta.icon" /></el-icon>
              <span>{{ menu.meta.title }}</span>
            </el-menu-item>
          </template>
        </el-menu>
      </el-aside>
      <el-main>
        <router-view />
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
import { CaretBottom } from '@element-plus/icons-vue'

const router = useRouter()
const activeMenu = ref('')
const menus = ref([])
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

<style scoped>
.layout-container {
  height: 100vh;
}

.header {
  background-color: #fff;
  border-bottom: 1px solid #dcdfe6;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
}

.logo {
  font-size: 20px;
  font-weight: bold;
}

.header-right {
  display: flex;
  align-items: center;
}

.user-info {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.menu {
  height: 100%;
  border-right: none;
}

.el-aside {
  background-color: #fff;
  border-right: 1px solid #dcdfe6;
}

.el-main {
  background-color: #f5f7f9;
  padding: 20px;
}
</style>