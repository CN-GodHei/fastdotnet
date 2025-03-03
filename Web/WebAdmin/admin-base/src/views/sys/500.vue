<template>
  <div class="error-container">
    <h1>500</h1>
    <p>抱歉，服务器出现了一些问题</p>
    <el-button type="primary" @click="$router.push('/')">返回首页</el-button>
  </div>
</template>

<script setup>
</script>

<style scoped>
.error-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100vh;
}

h1 {
  font-size: 72px;
  margin-bottom: 20px;
  color: #909399;
}

p {
  font-size: 20px;
  color: #606266;
  margin-bottom: 30px;
}
</style>