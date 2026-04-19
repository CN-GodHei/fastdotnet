<template>
  <div class="home-container">
    <el-card>
      <template #header>
        <div class="card-header">
          <span>{{ displayName }} - 应用端</span>
        </div>
      </template>
      
      <div class="content">
        <p>这是 {{ displayName }} 插件的应用端页面。</p>
        <p>Plugin ID: {{ pluginId }}</p>
      </div>
    </el-card>
  </div>
</template>

<script setup lang="ts">
// 这些值会在模板生成时被替换
const pluginId = '11375910391972869'
const displayName = '演示插件'
</script>

<style scoped>
.home-container {
  padding: 20px;
}

.card-header {
  font-size: 18px;
  font-weight: bold;
}

.content {
  line-height: 2;
}
</style>
