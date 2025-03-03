<template>
  <div class="tabs-view-container">
    <el-scrollbar class="tabs-view" ref="scrollbarRef">
      <el-tabs
        v-model="activeTab"
        type="card"
        @tab-click="handleTabClick"
        @tab-remove="handleTabRemove"
      >
        <el-tab-pane
          v-for="tab in visitedViews"
          :key="tab.path"
          :label="tab.meta.title"
          :name="tab.path"
          :closable="tab.path !== '/dashboard'"
        >
        </el-tab-pane>
      </el-tabs>
    </el-scrollbar>
  </div>
</template>

<script setup>
import { ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'

const route = useRoute()
const router = useRouter()

// 访问过的视图
const visitedViews = ref([
  {
    path: '/dashboard',
    meta: { title: '首页' }
  }
])

// 当前激活的标签
const activeTab = ref('/dashboard')

// 监听路由变化
watch(
  () => route.path,
  (newPath) => {
    addView(route)
    activeTab.value = newPath
  }
)

// 添加视图
const addView = (view) => {
  if (view.meta?.title) {
    const isExist = visitedViews.value.some(v => v.path === view.path)
    if (!isExist) {
      visitedViews.value.push({
        path: view.path,
        meta: { ...view.meta }
      })
    }
  }
}

// 处理标签点击
const handleTabClick = (tab) => {
  router.push(tab.props.name)
}

// 处理标签移除
const handleTabRemove = (targetPath) => {
  const tabs = visitedViews.value
  let activePath = activeTab.value
  
  if (activePath === targetPath) {
    tabs.forEach((tab, index) => {
      if (tab.path === targetPath) {
        const nextTab = tabs[index + 1] || tabs[index - 1]
        if (nextTab) {
          activePath = nextTab.path
        }
      }
    })
  }
  
  activeTab.value = activePath
  visitedViews.value = tabs.filter(tab => tab.path !== targetPath)
  router.push(activePath)
}
</script>

<style lang="scss" scoped>
.tabs-view-container {
  height: 32px;
  background: #fff;
  border-bottom: 1px solid #d8dce5;
  box-shadow: 0 1px 3px 0 rgba(0, 0, 0, .12), 0 0 3px 0 rgba(0, 0, 0, .04);

  .tabs-view {
    .el-tabs--card {
      height: 32px;
      box-sizing: border-box;

      .el-tabs__header {
        margin: 0;
        border-bottom: none;

        .el-tabs__nav {
          border: none;
        }

        .el-tabs__item {
          height: 32px;
          line-height: 32px;
          border: none;
          background-color: transparent;
          transition: all 0.3s cubic-bezier(.645, .045, .355, 1);

          &.is-active {
            background-color: #42b983;
            color: #fff;
          }

          &:hover {
            color: #42b983;

            &.is-active {
              color: #fff;
            }
          }
        }
      }
    }
  }
}
</style>