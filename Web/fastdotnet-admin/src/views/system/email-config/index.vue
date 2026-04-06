<template>
	<div class="email-config-container">
		<el-card shadow="hover">
			<template #header>
				<div class="card-header">
					<el-icon size="18" style="margin-right: 8px; vertical-align: middle"><ele-Message /></el-icon>
					<span>邮件配置</span>
				</div>
			</template>

			<el-form :model="formData" ref="formRef" label-width="120px" v-loading="loading">
				<el-row :gutter="20">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="SMTP服务器" prop="Host" :rules="[{ required: true, message: '请输入SMTP服务器地址', trigger: 'blur' }]">
							<el-input 
								v-model="formData.Host" 
								placeholder="例如: smtp.qq.com" 
								clearable 
							/>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="端口号" prop="Port" :rules="[
							{ required: true, message: '请输入端口号', trigger: 'blur' },
							{ type: 'number', min: 1, max: 65535, message: '端口号范围1-65535', trigger: 'blur' }
						]">
							<el-input-number 
								v-model="formData.Port" 
								:min="1" 
								:max="65535" 
								style="width: 100%"
								placeholder="例如: 587"
							/>
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="用户名" prop="Username" :rules="[{ required: true, message: '请输入用户名', trigger: 'blur' }]">
							<el-input 
								v-model="formData.Username" 
								placeholder="请输入邮箱账号" 
								clearable 
							/>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="密码/授权码" prop="Password" :rules="[{ required: true, message: '请输入密码或授权码', trigger: 'blur' }]">
							<el-input 
								v-model="formData.Password" 
								type="password"
								placeholder="请输入密码或授权码" 
								show-password
								clearable 
							/>
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="发件人邮箱" prop="SenderEmail" :rules="[
							{ required: true, message: '请输入发件人邮箱', trigger: 'blur' },
							{ type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
						]">
							<el-input 
								v-model="formData.SenderEmail" 
								placeholder="例如: noreply@example.com" 
								clearable 
							/>
						</el-form-item>
					</el-col>
					<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
						<el-form-item label="发件人名称" prop="SenderName" :rules="[{ required: true, message: '请输入发件人名称', trigger: 'blur' }]">
							<el-input 
								v-model="formData.SenderName" 
								placeholder="例如: 系统通知" 
								clearable 
							/>
						</el-form-item>
					</el-col>
				</el-row>

				<el-row :gutter="20">
					<el-col :xs="24" :sm="24" :md="24" :lg="24" :xl="24" class="mb20">
						<el-form-item label="启用SSL" prop="EnableSsl">
							<el-switch 
								v-model="formData.EnableSsl" 
								active-text="开启" 
								inactive-text="关闭"
							/>
							<el-text type="info" size="small" style="margin-left: 10px">
								建议使用SSL加密连接以提高安全性
							</el-text>
						</el-form-item>
					</el-col>
				</el-row>

				<el-row>
					<el-col :span="24" style="text-align: center">
						<el-button type="primary" @click="handleSubmit" :loading="submitting">
							<el-icon style="margin-right: 5px"><ele-Check /></el-icon>
							保存配置
						</el-button>
						<el-button @click="handleReset">
							<el-icon style="margin-right: 5px"><ele-Refresh /></el-icon>
							重置
						</el-button>
						<el-button type="success" @click="handleTest">
							<el-icon style="margin-right: 5px"><ele-Send /></el-icon>
							测试发送
						</el-button>
					</el-col>
				</el-row>
			</el-form>
		</el-card>

		<!-- 测试发送邮件对话框 -->
		<el-dialog 
			v-model="testDialog.visible" 
			title="测试邮件发送" 
			width="500px"
			:close-on-click-modal="false"
		>
			<el-form :model="testDialog.formData" label-width="100px">
				<el-form-item label="收件人邮箱" :rules="[
					{ required: true, message: '请输入收件人邮箱', trigger: 'blur' },
					{ type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
				]">
					<el-input 
						v-model="testDialog.formData.toEmail" 
						placeholder="请输入测试收件人邮箱" 
						clearable 
					/>
				</el-form-item>
				<el-form-item label="测试主题">
					<el-input 
						v-model="testDialog.formData.subject" 
						placeholder="邮件主题" 
						clearable 
					/>
				</el-form-item>
				<el-form-item label="测试内容">
					<el-input 
						v-model="testDialog.formData.body" 
						type="textarea"
						:rows="4"
						placeholder="邮件内容" 
					/>
				</el-form-item>
			</el-form>
			<template #footer>
				<span class="dialog-footer">
					<el-button @click="testDialog.visible = false">取 消</el-button>
					<el-button type="primary" @click="sendTestEmail" :loading="testDialog.sending">
						发 送
					</el-button>
				</span>
			</template>
		</el-dialog>
	</div>
</template>

<script lang="ts" setup name="FdEmailConfig">
import { ref, reactive, onMounted } from 'vue';
import { ElMessage } from 'element-plus';
import * as FdEmailConfigApi from '@/api/fd-system-api-admin/FdEmailConfig';

const formRef = ref();
const loading = ref(false);
const submitting = ref(false);

// 表单数据
const formData = reactive<APIModel.FdUpdateEmailConfigDto>({
	Host: '',
	Port: 587,
	Username: '',
	Password: '',
	SenderEmail: '',
	SenderName: '',
	EnableSsl: true,
});

// 测试对话框
const testDialog = reactive({
	visible: false,
	sending: false,
	formData: {
		toEmail: '',
		subject: '测试邮件',
		body: '这是一封测试邮件，用于验证邮件配置是否正确。',
	}
});

// 加载配置
const loadConfig = async () => {
	loading.value = true;
	try {
		const config = await FdEmailConfigApi.getApiAdminFdEmailConfigGetConfig();
		if (config) {
			Object.assign(formData, config);
		}
	} catch (error: any) {
		ElMessage.error(error.message || '获取配置失败');
	} finally {
		loading.value = false;
	}
};

// 提交表单
const handleSubmit = async () => {
	if (!formRef.value) return;
	
	await formRef.value.validate(async (valid: boolean) => {
		if (!valid) return;
		
		submitting.value = true;
		try {
			await FdEmailConfigApi.postApiAdminFdEmailConfigUpdateConfig(formData);
			ElMessage.success('配置保存成功');
			// 重新加载配置
			await loadConfig();
		} catch (error: any) {
			ElMessage.error(error.message || '保存配置失败');
		} finally {
			submitting.value = false;
		}
	});
};

// 重置表单
const handleReset = () => {
	if (formRef.value) {
		formRef.value.resetFields();
	}
	loadConfig();
};

// 打开测试对话框
const handleTest = () => {
	testDialog.visible = true;
	testDialog.formData = {
		toEmail: '',
		subject: '测试邮件',
		body: '这是一封测试邮件，用于验证邮件配置是否正确。',
	};
};

// 发送测试邮件
const sendTestEmail = async () => {
	if (!testDialog.formData.toEmail) {
		ElMessage.warning('请输入收件人邮箱');
		return;
	}

	testDialog.sending = true;
	try {
		await FdEmailConfigApi.postApiAdminFdEmailConfigTestSend({
			ToEmail: testDialog.formData.toEmail,
			Subject: testDialog.formData.subject,
			Body: testDialog.formData.body,
		});
		ElMessage.success('测试邮件发送成功，请检查收件箱');
		testDialog.visible = false;
	} catch (error: any) {
		ElMessage.error(error.message || '发送测试邮件失败');
	} finally {
		testDialog.sending = false;
	}
};

onMounted(() => {
	loadConfig();
});
</script>

<style scoped lang="scss">
.email-config-container {
	padding: 20px;

	.card-header {
		display: flex;
		align-items: center;
		font-size: 16px;
		font-weight: 500;
	}

	.mb20 {
		margin-bottom: 20px;
	}

	.el-form-item {
		margin-bottom: 22px;
	}
}
</style>
