<template>
  <div v-if="!item.meta?.hidden">
    <template
      v-if="
        hasOneShowingChild(item.children, item) &&
        (!onlyOneChild.children || onlyOneChild.noShowingChildren) &&
        !item.meta?.alwaysShow
      "
    >
      <app-link v-if="onlyOneChild.meta" :to="resolvePath(onlyOneChild.path)">
        <el-menu-item
          :index="resolvePath(onlyOneChild.path)"
          :class="{ 'submenu-title-noDropdown': !isNest }"
        >
          <el-icon>
            <component :is="onlyOneChild.meta.icon || (item.meta && item.meta.icon)" />
          </el-icon>
          <template #title>
            <span>{{ onlyOneChild.meta.title }}</span>
          </template>
        </el-menu-item>
      </app-link>
    </template>

    <el-sub-menu v-else ref="subMenu" :index="resolvePath(item.path)" popper-append-to-body>
      <template #title>
        <el-icon v-if="item.meta && item.meta.icon">
          <component :is="item.meta.icon" />
        </el-icon>
        <span>{{ item.meta?.title }}</span>
      </template>

      <sidebar-item
        v-for="child in item.children"
        :key="child.path"
        :is-nest="true"
        :item="child"
        :base-path="resolvePath(child.path)"
        class="nest-menu"
      />
    </el-sub-menu>
  </div>
</template>

<script setup lang="ts">
import { ref, reactive } from 'vue'
import AppLink from './Link.vue'
import { isExternal } from '@/utils/validate'
import path from 'path-browserify'

interface Props {
  item: any
  isNest?: boolean
  basePath: string
}

const props = withDefaults(defineProps<Props>(), {
  isNest: false,
  basePath: ''
})

const onlyOneChild = ref()

// 判断是否只有一个显示的子菜单
const hasOneShowingChild = (children: any[] = [], parent: any) => {
  const showingChildren = children.filter((item: any) => {
    if (item.meta?.hidden) {
      return false
    } else {
      // 临时设置
      onlyOneChild.value = item
      return true
    }
  })

  // 只有一个子菜单时显示
  if (showingChildren.length === 1) {
    return true
  }

  // 没有子菜单时显示父级
  if (showingChildren.length === 0) {
    onlyOneChild.value = { ...parent, path: '', noShowingChildren: true }
    return true
  }

  return false
}

// 解析路径
const resolvePath = (routePath: string) => {
  if (isExternal(routePath)) {
    return routePath
  }
  if (isExternal(props.basePath)) {
    return props.basePath
  }

  return path.resolve(props.basePath, routePath)
}
</script>