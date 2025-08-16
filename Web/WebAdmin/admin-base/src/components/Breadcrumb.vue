<template>
  <el-breadcrumb class="app-breadcrumb" separator="/">
    <transition-group name="breadcrumb">
      <el-breadcrumb-item v-for="(item, index) in breadcrumbs" :key="item.path">
        <span
          v-if="index === breadcrumbs.length - 1"
          class="no-redirect"
        >{{ item.meta.title }}</span>
        <a v-else @click.prevent="handleLink(item)">{{ item.meta.title }}</a>
      </el-breadcrumb-item>
    </transition-group>
  </el-breadcrumb>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { useRoute, useRouter, RouteLocationMatched } from 'vue-router'

const route = useRoute()
const router = useRouter()

interface BreadcrumbItem {
  path: string
  meta: {
    title: string
  }
}

const breadcrumbs = ref<BreadcrumbItem[]>([])

// 判断是否为首页
const isDashboard = (route: RouteLocationMatched) => {
  const name = route && route.name
  if (!name) {
    return false
  }
  return name.toString().trim().toLocaleLowerCase() === 'Dashboard'.toLocaleLowerCase()
}

// 获取面包屑数据
const getBreadcrumb = () => {
  // 过滤掉没有标题的路由并转换类型
  let matched: BreadcrumbItem[] = route.matched
    .filter(item => item.meta && item.meta.title)
    .map(item => ({
      path: item.path,
      meta: {
        title: item.meta.title as string
      }
    }))
  
  const first = matched[0]

  // 如果不是首页，添加首页作为第一个面包屑
  if (route.matched.length > 0 && !isDashboard(route.matched[0])) {
    matched = [{ path: '/dashboard', meta: { title: '首页' } }, ...matched]
  }

  breadcrumbs.value = matched
}

// 处理链接点击
const handleLink = (item: BreadcrumbItem) => {
  router.push(item.path)
}

// 监听路由变化
watch(
  () => route.path,
  () => {
    getBreadcrumb()
  },
  {
    immediate: true
  }
)
</script>

<style lang="scss" scoped>
.app-breadcrumb {
  display: inline-block;
  font-size: 14px;
  line-height: 50px;
  margin-left: 8px;

  .no-redirect {
    color: #97a8be;
    cursor: text;
  }
}
</style>