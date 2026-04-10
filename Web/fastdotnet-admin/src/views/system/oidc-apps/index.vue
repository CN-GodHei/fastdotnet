<template>
    <div class="oidc-apps-container">
        <el-card class="box-card">
            <template #header>
                <div class="card-header">
                    <span>OIDC 应用管理</span>
                    <el-button type="primary" @click="handleCreate">创建应用</el-button>
                </div>
            </template>

            <el-table :data="apps" style="width: 100%" v-loading="loading">
                <el-table-column prop="DisplayName" label="应用名称" />
                <el-table-column prop="ClientId" label="AppId" width="250" />
                <el-table-column prop="ClientType" label="类型" width="120" />
                <el-table-column label="操作" width="200">
                    <template #default="scope">
                        <el-button size="small" :disabled="scope.row.ClientType !== 'confidential'"
                            @click="handleResetSecret(scope.row)">重置密钥</el-button>
                        <el-button size="small" type="danger" @click="handleDelete(scope.row)">删除</el-button>
                    </template>
                </el-table-column>
            </el-table>
        </el-card>

        <!-- 创建应用对话框 -->
        <el-dialog v-model="dialogVisible" title="创建 OIDC 应用" width="500px">
            <el-form :model="form" label-width="100px">
                <el-form-item label="应用名称">
                    <el-input v-model="form.displayName" placeholder="请输入应用名称" />
                </el-form-item>
                <el-form-item label="客户端类型">
                    <el-select v-model="form.clientType" placeholder="请选择">
                        <el-option label="机密 (Confidential)" value="confidential" />
                        <el-option label="公开 (Public)" value="public" />
                    </el-select>
                </el-form-item>
                <el-form-item label="重定向 URI">
                    <div style="display: flex; gap: 10px;">
                        <el-input v-model="redirectUriInput" placeholder="请输入回调地址" @keyup.enter="addRedirectUri" />
                        <el-button type="primary" @click="addRedirectUri">添加</el-button>
                    </div>
                    <div class="uri-list" style="margin-top: 10px;">
                        <el-tag v-for="(uri, index) in form.redirectUris" :key="index" closable
                            @close="removeRedirectUri(index)">
                            {{ uri }}
                        </el-tag>
                    </div>
                </el-form-item>
            </el-form>
            <template #footer>
                <span class="dialog-footer">
                    <el-button @click="dialogVisible = false">取消</el-button>
                    <el-button type="primary" @click="submitForm">确定</el-button>
                </span>
            </template>
        </el-dialog>
    </div>
</template>

<script setup lang="ts">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage, ElMessageBox } from 'element-plus';
import {
    getApiOidcApp,
    postApiOidcApp,
    postApiOidcAppClientIdResetSecret,
    deleteApiOidcAppClientId
} from '@/api/fd-system-api-admin/OidcApp';

const apps = ref([]);
const loading = ref(false);
const dialogVisible = ref(false);
const redirectUriInput = ref('');

const form = reactive({
    displayName: '',
    clientType: 'confidential',
    redirectUris: [] as string[],
    postLogoutRedirectUris: [] as string[],
    grantTypes: ['authorization_code'],
    scopes: ['openid', 'profile']
});

const fetchApps = async () => {
    loading.value = true;
    try {
        const res = await getApiOidcApp();
        // 兼容不同的返回结构：有些接口直接返回数组，有些包装在 Data 字段中
        apps.value = Array.isArray(res) ? res : (res.Data || []);
    } catch (error) {
        ElMessage.error('获取应用列表失败');
    } finally {
        loading.value = false;
    }
};

const handleCreate = () => {
    form.displayName = '';
    form.clientType = 'confidential';
    form.redirectUris = [];
    form.postLogoutRedirectUris = [];
    redirectUriInput.value = '';
    dialogVisible.value = true;
};

const addRedirectUri = () => {
    if (redirectUriInput.value && !form.redirectUris.includes(redirectUriInput.value)) {
        form.redirectUris.push(redirectUriInput.value);
        redirectUriInput.value = '';
    }
};

const removeRedirectUri = (index: number) => {
    form.redirectUris.splice(index, 1);
};

const submitForm = async () => {
    try {
        await postApiOidcApp(form as any);
        ElMessage.success('创建成功');
        dialogVisible.value = false;
        fetchApps();
    } catch (error) {
        ElMessage.error('创建失败');
    }
};

const handleResetSecret = async (row: any) => {
    try {
        // 注意：这里使用 row.ClientId (大写 C)
        const res = await postApiOidcAppClientIdResetSecret({ clientId: row.ClientId });
        ElMessageBox.alert(`新的 Client Secret: ${res.clientSecret || res.Data?.clientSecret}`, '重置成功', {
            confirmButtonText: '确定'
        });
    } catch (error) {
        ElMessage.error('重置密钥失败');
    }
};

const handleDelete = (row: any) => {
    ElMessageBox.confirm('确定要删除该应用吗？', '警告', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
    }).then(async () => {
        try {
            // 注意：这里使用 row.ClientId (大写 C)
            var result = await deleteApiOidcAppClientId({ clientId: row.ClientId });
            if (result === true) {

                ElMessage.success('删除成功');
            }
            fetchApps();
        } catch (error) {
            ElMessage.error('删除失败');
        }
    });
};

onMounted(() => {
    fetchApps();
});
</script>

<style scoped>
.card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
}

.uri-list {
    margin-top: 10px;
}

.uri-list .el-tag {
    margin-right: 10px;
    margin-bottom: 5px;
}
</style>
