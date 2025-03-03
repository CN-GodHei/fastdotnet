<template>
  <div class="plugin-a-container">
    <h1>PluginA 插件页面</h1>
    <el-card class="box-card">
      <template #header>
        <div class="card-header">
          <span>插件功能区</span>
        </div>
      </template>
      <div class="text item">
        这里是PluginA插件的内容区域，可以根据实际需求开发具体功能。
      </div>
    </el-card>
  </div>
</template>

<script setup>
// 可以在这里添加具体的业务逻辑
</script>

<style scoped>
.plugin-a-container {
  padding: 20px;
}

.box-card {
  margin-top: 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
}
</style>