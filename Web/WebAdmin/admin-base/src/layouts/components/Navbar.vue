<template>
  <div class="navbar">
    <div class="left-menu">
      <el-icon class="menu-icon" @click="toggleSideBar">
        <Expand />
      </el-icon>
    </div>
    <div class="right-menu">
      <el-dropdown class="avatar-container" trigger="click">
        <div class="avatar-wrapper">
          <el-avatar :size="40" src="https://cube.elemecdn.com/0/88/03b0d39583f48206768a7534e55bcpng.png" />
          <span class="user-name">Admin</span>
        </div>
        <template #dropdown>
          <el-dropdown-menu class="user-dropdown">
            <el-dropdown-item @click="logout">退出登录</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </div>
  </div>
</template>

<script setup lang="ts">
import { Expand } from '@element-plus/icons-vue'

// 切换侧边栏
const toggleSideBar = () => {
  // TODO: 实现侧边栏切换逻辑
  console.log('切换侧边栏')
}

// 退出登录
const logout = () => {
  // TODO: 实现退出登录逻辑
  console.log('退出登录')
}
</script>

<style lang="scss" scoped>
.navbar {
  height: 50px;
  overflow: hidden;
  position: relative;
  background: #fff;
  box-shadow: 0 1px 4px rgba(0, 21, 41, 0.08);
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;

  .left-menu {
    display: flex;
    align-items: center;

    .menu-icon {
      font-size: 20px;
      cursor: pointer;
    }
  }

  .right-menu {
    display: flex;
    align-items: center;

    .avatar-container {
      cursor: pointer;

      .avatar-wrapper {
        position: relative;
        display: flex;
        align-items: center;

        .user-name {
          margin-left: 10px;
          font-size: 14px;
        }
      }
    }
  }
}
</style>