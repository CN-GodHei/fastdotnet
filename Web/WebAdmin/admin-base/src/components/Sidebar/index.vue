<script setup lang="ts">
import { computed } from 'vue'
import { useUserStore } from '@/stores/user'
import SidebarItem from './SidebarItem.vue'

const userStore = useUserStore()

// 过滤出启用的菜单并按排序字段排序
const enabledMenus = computed(() => {
  return userStore.userMenus
    .filter(menu => menu.isEnabled)
    .sort((a, b) => a.sort - b.sort)
})
</script>

<template>
  <div class="sidebar-container">
    <el-scrollbar wrap-class="scrollbar-wrapper">
      <el-menu
        :default-active="$route.path"
        :collapse="false"
        background-color="#001529"
        text-color="#fff"
        active-text-color="#409eff"
        mode="vertical"
      >
        <sidebar-item 
          v-for="menu in enabledMenus" 
          :key="menu.id" 
          :item="menu" 
          :base-path="menu.path"
        />
      </el-menu>
    </el-scrollbar>
  </div>
</template>

<style lang="scss" scoped>
.sidebar-container {
  .el-menu {
    border: none;
    height: 100%;
    width: 100% !important;
  }
}

.scrollbar-wrapper {
  overflow-x: hidden !important;
}

.el-scrollbar__bar.is-vertical {
  right: 0px;
}

.el-scrollbar {
  height: 100%;
}

.has-logo {
  .el-scrollbar {
    height: calc(100% - 50px);
  }
}
</style>