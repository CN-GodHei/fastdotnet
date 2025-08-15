<template>
  <div class="dashboard-container">
    <el-card class="welcome-card">
      <template #header>
        <div class="card-header">
          <span>欢迎使用 Fastdotnet Admin</span>
        </div>
      </template>
      <div class="welcome-content">
        <p>这是一个基于 Vue3 + TypeScript + Vite + qiankun 的现代化管理平台</p>
        <p>您可以在这里管理您的系统和插件</p>
      </div>
    </el-card>

    <el-row :gutter="20" class="stats-row">
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-item">
            <el-icon class="stat-icon" color="#409EFF"><User /></el-icon>
            <div class="stat-info">
              <div class="stat-value">120</div>
              <div class="stat-label">用户数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-item">
            <el-icon class="stat-icon" color="#67C23A"><Tickets /></el-icon>
            <div class="stat-info">
              <div class="stat-value">36</div>
              <div class="stat-label">插件数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-item">
            <el-icon class="stat-icon" color="#E6A23C"><Document /></el-icon>
            <div class="stat-info">
              <div class="stat-value">240</div>
              <div class="stat-label">文档数</div>
            </div>
          </div>
        </el-card>
      </el-col>
      <el-col :span="6">
        <el-card class="stat-card">
          <div class="stat-item">
            <el-icon class="stat-icon" color="#F56C6C"><Warning /></el-icon>
            <div class="stat-info">
              <div class="stat-value">5</div>
              <div class="stat-label">警告数</div>
            </div>
          </div>
        </el-card>
      </el-col>
    </el-row>

    <el-card class="info-card">
      <template #header>
        <div class="card-header">
          <span>系统信息</span>
        </div>
      </template>
      <el-descriptions :column="2" border>
        <el-descriptions-item label="框架版本">Fastdotnet v1.0</el-descriptions-item>
        <el-descriptions-item label="Vue版本">Vue 3.3</el-descriptions-item>
        <el-descriptions-item label="Element Plus">v2.4</el-descriptions-item>
        <el-descriptions-item label="qiankun">v2.10</el-descriptions-item>
        <el-descriptions-item label="服务器时间">{{ currentTime }}</el-descriptions-item>
        <el-descriptions-item label="运行环境">Windows 10</el-descriptions-item>
      </el-descriptions>
    </el-card>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onBeforeUnmount } from 'vue'
import { User, Tickets, Document, Warning } from '@element-plus/icons-vue'

// 当前时间
const currentTime = ref(new Date().toLocaleString())

// 更新时间的定时器
let timer: NodeJS.Timeout | null = null

// 更新当前时间
const updateTime = () => {
  currentTime.value = new Date().toLocaleString()
}

onMounted(() => {
  timer = setInterval(updateTime, 1000)
})

onBeforeUnmount(() => {
  if (timer) {
    clearInterval(timer)
  }
})
</script>

<style lang="scss" scoped>
.dashboard-container {
  padding: 20px;
  background-color: #f0f2f5;

  .welcome-card {
    margin-bottom: 20px;

    .card-header {
      font-size: 18px;
      font-weight: bold;
    }

    .welcome-content {
      p {
        font-size: 16px;
        line-height: 1.6;
        color: #666;
      }
    }
  }

  .stats-row {
    margin-bottom: 20px;

    .stat-card {
      .stat-item {
        display: flex;
        align-items: center;

        .stat-icon {
          font-size: 36px;
          margin-right: 15px;
        }

        .stat-info {
          .stat-value {
            font-size: 24px;
            font-weight: bold;
          }

          .stat-label {
            font-size: 14px;
            color: #999;
          }
        }
      }
    }
  }

  .info-card {
    .card-header {
      font-size: 18px;
      font-weight: bold;
    }
  }
}
</style>