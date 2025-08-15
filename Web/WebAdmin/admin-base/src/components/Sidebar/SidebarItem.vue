<script setup lang="ts">
import { computed } from 'vue'
import type { PropType } from 'vue'
import { isExternal } from '@/utils/validate'
import type { Menu } from '@/api/menu'
import SidebarItem from './SidebarItem.vue'
import { ElSubMenu, ElMenuItem } from 'element-plus'

const props = defineProps({
  item: {
    type: Object as PropType<Menu>,
    required: true
  },
  basePath: {
    type: String,
    required: true
  }
})

// 判断是否始终显示根菜单
const alwaysShowRootMenu = computed(() => {
  return props.item.type === 'Directory' && props.item.children && props.item.children.length > 0
})

// 显示的子菜单
const showingChildren = computed(() => {
  return props.item.children?.filter((item) => {
    return !(item.type === 'Menu' && item.isExternal && !item.externalUrl)
  }) || []
})

// 是否只有一个子项显示
const onlyOneChild = computed(() => {
  const showingChildrenCount = showingChildren.value.length
  // 如果只有一个子项且没有子菜单，则显示该子项
  if (showingChildrenCount === 1) {
    return showingChildren.value[0]
  }
  // 如果没有子项，则显示自己
  if (showingChildrenCount === 0) {
    return { ...props.item, path: '', noShowingChildren: true }
  }
  return null
})

// 解析路径
const resolvePath = (routePath: string) => {
  if (isExternal(routePath)) {
    return routePath
  }
  if (isExternal(props.basePath)) {
    return props.basePath
  }
  
  // 处理路径连接
  const basePath = props.basePath || ''
  const path = routePath || ''
  
  if (basePath.endsWith('/') && path.startsWith('/')) {
    return basePath + path.substring(1)
  }
  
  if (!basePath.endsWith('/') && !path.startsWith('/')) {
    return basePath + '/' + path
  }
  
  return basePath + path
}
</script>

<template>
  <div v-if="!item.isHidden">
    <template v-if="alwaysShowRootMenu">
      <el-sub-menu :index="resolvePath(item.path)" v-if="item.children && item.children.length > 0">
        <template #title>
          <el-icon v-if="item.icon">
            <component :is="item.icon" />
          </el-icon>
          <span>{{ item.name }}</span>
        </template>
        
        <sidebar-item
          v-for="child in item.children"
          :key="child.id"
          :item="child"
          :base-path="resolvePath(child.path)"
        />
      </el-sub-menu>
    </template>
    
    <template v-else>
      <el-menu-item 
        v-if="onlyOneChild && !onlyOneChild.children" 
        :index="resolvePath(onlyOneChild.path)"
      >
        <el-icon v-if="onlyOneChild.icon">
          <component :is="onlyOneChild.icon" />
        </el-icon>
        <template #title>
          <span>{{ onlyOneChild.name }}</span>
        </template>
      </el-menu-item>
      
      <el-sub-menu 
        v-else 
        :index="resolvePath(item.path)"
        popper-append-to-body
      >
        <template #title>
          <el-icon v-if="item.icon">
            <component :is="item.icon" />
          </el-icon>
          <span>{{ item.name }}</span>
        </template>
        
        <sidebar-item
          v-for="child in item.children"
          :key="child.id"
          :item="child"
          :base-path="resolvePath(child.path)"
        />
      </el-sub-menu>
    </template>
  </div>
</template>

<style lang="scss" scoped>
.el-sub-menu.is-active > .el-sub-menu__title {
  color: #409eff !important;
}

.el-menu-item:hover {
  background-color: #001528 !important;
}

.el-sub-menu__title:hover {
  background-color: #001528 !important;
}
</style>