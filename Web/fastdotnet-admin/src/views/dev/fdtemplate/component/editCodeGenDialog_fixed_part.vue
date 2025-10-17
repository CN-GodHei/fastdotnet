				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="实体名称" prop="entityName" :rules="[{ required: true, message: '实体名称不能为空', trigger: 'blur' }]">
						<el-input v-model="state.ruleForm.entityName" placeholder="请输入实体名称" clearable />
					</el-form-item>
				</el-col>
				<el-col :xs="24" :sm="12" :md="12" :lg="12" :xl="12" class="mb20">
					<el-form-item label="生成菜单" prop="generateMenu">
						<el-radio-group v-model="state.ruleForm.generateMenu">
							<el-radio :value="true">是</el-radio>
							<el-radio :value="false">否</el-radio>
						</el-radio-group>
					</el-form-item>
				</el-col>