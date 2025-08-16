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
  return props.item.Type === 1 && props.item.Children && props.item.Children.length > 0
})

// 显示的子菜单
const showingChildren = computed(() => {
  return props.item.Children?.filter((item) => {
    // 过滤掉外部链接但没有URL的菜单项
    return !(item.Type === 2 && item.IsExternal && !item.ExternalUrl)
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
    return { ...props.item, Path: '', noShowingChildren: true }
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
  <div v-if="!item.IsDeleted">
    <template v-if="alwaysShowRootMenu && item.Children && item.Children.length > 0">
      <el-sub-menu :index="resolvePath(item.Path)">
        <template #title>
          <el-icon v-if="item.Icon">
            <component :is="item.Icon" />
          </el-icon>
          <span>{{ item.Name }}</span>
        </template>
        
        <sidebar-item
          v-for="child in item.Children"
          :key="child.Id"
          :item="child"
          :base-path="resolvePath(child.Path)"
        />
      </el-sub-menu>
    </template>
    
    <template v-else>
      <el-menu-item 
        v-if="onlyOneChild && !onlyOneChild.Children" 
        :index="resolvePath(onlyOneChild.Path)"
      >
        <el-icon v-if="onlyOneChild.Icon">
          <component :is="onlyOneChild.Icon" />
        </el-icon>
        <template #title>
          <span>{{ onlyOneChild.Name }}</span>
        </template>
      </el-menu-item>
      
      <el-sub-menu 
        v-else 
        :index="resolvePath(item.Path)"
        popper-append-to-body
      >
        <template #title>
          <el-icon v-if="item.Icon">
            <component :is="item.Icon" />
          </el-icon>
          <span>{{ item.Name }}</span>
        </template>
        
        <sidebar-item
          v-for="child in item.Children"
          :key="child.Id"
          :item="child"
          :base-path="resolvePath(child.Path)"
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