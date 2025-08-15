<script setup lang="ts">
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useUserStore } from '@/stores/user'
import { ElMessage } from 'element-plus'

const userStore = useUserStore()
const router = useRouter()

// 登录表单数据
const loginForm = ref({
  username: 'admin',
  password: '123456'
})

const loading = ref(false)
const redirect = ref('')

// 监听路由变化获取redirect参数
router.afterEach(to => {
  redirect.value = (to.query.redirect as string) || ''
})

// 用户登录
const handleLogin = async () => {
  if (!loginForm.value.username || !loginForm.value.password) {
    ElMessage.error('请输入用户名和密码')
    return
  }

  loading.value = true
  try {
    await userStore.login(loginForm.value.username, loginForm.value.password)
    ElMessage.success('登录成功')
    // 跳转到首页或重定向页面
    router.push({ path: redirect.value || '/' })
  } catch (error: any) {
    console.error('登录失败:', error)
    ElMessage.error(error.message || '登录失败，请检查用户名和密码')
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="login-container">
    <div class="login-form">
      <div class="title-container">
        <h3 class="title">Fastdotnet 管理系统</h3>
      </div>
      
      <el-form ref="loginFormRef" :model="loginForm" class="form">
        <el-form-item prop="username">
          <el-input
            v-model="loginForm.username"
            placeholder="用户名"
            type="text"
            tabindex="1"
            autocomplete="off"
          >
            <template #prefix>
              <el-icon><User /></el-icon>
            </template>
          </el-input>
        </el-form-item>
        
        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            placeholder="密码"
            type="password"
            tabindex="2"
            autocomplete="off"
            @keyup.enter="handleLogin"
          >
            <template #prefix>
              <el-icon><Lock /></el-icon>
            </template>
          </el-input>
        </el-form-item>
        
        <el-button
          :loading="loading"
          type="primary"
          style="width: 100%; margin-bottom: 30px"
          @click.prevent="handleLogin"
        >
          登录
        </el-button>
      </el-form>
    </div>
  </div>
</template>

<style lang="scss" scoped>
.login-container {
  min-height: 100vh;
  width: 100%;
  background-color: #2d3a4b;
  overflow: hidden;
  display: flex;
  align-items: center;
  justify-content: center;
  
  .login-form {
    width: 520px;
    max-width: 100%;
    padding: 160px 35px 0;
    margin: 0 auto;
    overflow: hidden;
    background: #fff;
    border-radius: 8px;
    box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
  }
  
  .title-container {
    position: relative;
    
    .title {
      font-size: 26px;
      color: #2d3a4b;
      margin: 0px auto 40px auto;
      text-align: center;
      font-weight: bold;
    }
  }
  
  .form {
    padding: 0 20px;
  }
  
  @media screen and (max-width: 768px) {
    .login-form {
      width: 90%;
      padding: 60px 20px 0;
    }
  }
}
</style>