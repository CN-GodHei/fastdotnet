<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

const goBack = () => {
  router.go(-1)
}

const goHome = () => {
  router.push('/')
}
</script>

<template>
  <div class="not-found">
    <div class="content">
      <h1>404</h1>
      <p>抱歉，您访问的页面不存在</p>
      <div class="actions">
        <el-button type="primary" @click="goBack">返回上一页</el-button>
        <el-button @click="goHome">回到首页</el-button>
      </div>
    </div>
  </div>
</template>

<style scoped lang="scss">
.not-found {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100vh;
  background-color: #f5f5f5;
  
  .content {
    text-align: center;
    
    h1 {
      font-size: 72px;
      color: #409eff;
      margin-bottom: 20px;
    }
    
    p {
      font-size: 20px;
      color: #666;
      margin-bottom: 30px;
    }
    
    .actions {
      button {
        margin: 0 10px;
      }
    }
  }
}
</style>