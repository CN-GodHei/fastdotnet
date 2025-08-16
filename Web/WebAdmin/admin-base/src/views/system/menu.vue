<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { ElMessage, ElMessageBox } from 'element-plus'
import type { Menu } from '@/api/menu'
import { getAllMenus, getMenuById, createMenu, updateMenu, deleteMenu } from '@/api/menu'

// 菜单表格数据
const menuList = ref<Menu[]>([])
const loading = ref(false)

// 表单相关
const dialogVisible = ref(false)
const formLoading = ref(false)
const dialogTitle = ref('')

// 菜单表单数据
const menuForm = ref({
  Id: 0,
  Name: '',
  Code: '',
  Path: '',
  Icon: '',
  ParentCode: null as string | null,
  Sort: 0,
  Type: 1, // 1: 目录, 2: 菜单
  Module: '',
  Category: 'Admin',
  IsExternal: false,
  ExternalUrl: '',
  IsEnabled: true,
  PermissionCode: '',
  Children: [] as Menu[],
  CreateTime: '',
  UpdateTime: null,
  IsDeleted: false,
  DeleteTime: null
})

// 表单引用
const menuFormRef = ref()

// 表单验证规则
const formRules = {
  Name: [{ required: true, message: '请输入菜单名称', trigger: 'blur' }],
  Path: [{ required: true, message: '请输入菜单路径', trigger: 'blur' }],
  Category: [{ required: true, message: '请选择菜单分类', trigger: 'change' }]
}

// 所有菜单选项（用于父级菜单选择）
const menuOptions = ref<Menu[]>([])

// 获取菜单列表
const loadMenus = async () => {
  loading.value = true
  try {
    const response = await getAllMenus()
    menuList.value = response.data || []
    // 构建菜单选项用于父级选择
    buildMenuOptions(menuList.value)
  } catch (error) {
    ElMessage.error('获取菜单列表失败')
    console.error(error)
  } finally {
    loading.value = false
  }
}

// 构建菜单选项
const buildMenuOptions = (menus: Menu[], prefix = ''): Menu[] => {
  const options: Menu[] = []
  menus.forEach(menu => {
    options.push({
      ...menu,
      Name: prefix + menu.Name
    })
    
    if (menu.Children && menu.Children.length > 0) {
      const childrenOptions = buildMenuOptions(menu.Children, prefix + '　')
      options.push(...childrenOptions)
    }
  })
  menuOptions.value = options
  return options
}

// 新增菜单
const handleAdd = (parentMenu?: Menu) => {
  resetForm()
  if (parentMenu) {
    menuForm.value.ParentCode = parentMenu.Code 
    menuForm.value.Category = parentMenu.Category
  }
  dialogTitle.value = '新增菜单'
  dialogVisible.value = true
}

// 编辑菜单
const handleEdit = async (menu: Menu) => {
  console.log('编辑菜单', menu)
  try {
    formLoading.value = true
    const response = await getMenuById(menu.Id)
    menuForm.value = { ...response.data }
    dialogTitle.value = '编辑菜单'
    dialogVisible.value = true
  } catch (error) {
    ElMessage.error('获取菜单详情失败')
    console.error(error)
  } finally {
    formLoading.value = false
  }
}

// 删除菜单
const handleDelete = async (menu: Menu) => {
  try {
    await ElMessageBox.confirm(`确定要删除菜单"${menu.Name}"吗？`, '提示', {
      type: 'warning'
    })
    
    await deleteMenu(menu.Id)
    ElMessage.success('删除成功')
    loadMenus()
  } catch (error) {
    if (error !== 'cancel') {
      ElMessage.error('删除失败')
      console.error(error)
    }
  }
}

// 保存菜单
const saveMenu = async () => {
  formLoading.value = true
  try {
    if (menuForm.value.Id > 0) {
      // 更新
      await updateMenu(menuForm.value.Id, menuForm.value)
      ElMessage.success('更新成功')
    } else {
      // 新增
      await createMenu(menuForm.value)
      ElMessage.success('新增成功')
    }
    
    dialogVisible.value = false
    loadMenus()
  } catch (error) {
    ElMessage.error('保存失败')
    console.error(error)
  } finally {
    formLoading.value = false
  }
}

// 重置表单
const resetForm = () => {
  menuForm.value = {
    Id: 0,
    Name: '',
    Code: '',
    Path: '',
    Icon: '',
    ParentCode: null,
    Sort: 0,
    Type: 1,
    Module: '',
    Category: 'Admin',
    IsExternal: false,
    ExternalUrl: '',
    IsEnabled: true,
    PermissionCode: '',
    Children: [],
    CreateTime: '',
    UpdateTime: null,
    IsDeleted: false,
    DeleteTime: null
  }
}

// 格式化菜单类型显示
const formatMenuType = (type:string) => {
  return type === 1 ? '目录' : '菜单'
}

// 格式化状态显示
const formatStatus = (isEnabled: boolean) => {
  return isEnabled ? '启用' : '禁用'
}

onMounted(() => {
  loadMenus()
})
</script>

<template>
  <div class="app-container">
    <div class="header">
      <el-button type="primary" @click="handleAdd()">新增菜单</el-button>
    </div>
    
    <el-table
      v-loading="loading"
      :data="menuList"
      row-key="Id"
      default-expand-all
      :tree-props="{ children: 'Children', hasChildren: 'hasChildren' }"
      border
    >
      <el-table-column prop="Name" label="菜单名称" width="200" />
      <el-table-column prop="Path" label="路径" width="150" />
      <el-table-column prop="Type" label="类型" width="80">
        <template #default="{ row }">
          {{ formatMenuType(row.Type) }}
        </template>
      </el-table-column>
      <el-table-column prop="Category" label="分类" width="100" />
      <el-table-column prop="Sort" label="排序" width="80" />
      <el-table-column prop="IsEnabled" label="状态" width="80">
        <template #default="{ row }">
          <el-tag :type="row.IsEnabled ? 'success' : 'danger'">
            {{ formatStatus(row.IsEnabled) }}
          </el-tag>
        </template>
      </el-table-column>
      <el-table-column prop="PermissionCode" label="权限标识" width="150" />
      <el-table-column prop="CreateTime" label="创建时间" width="160" />
      <el-table-column label="操作" width="200" fixed="right">
        <template #default="{ row }">
          <el-button type="primary" link @click="handleAdd(row)">新增</el-button>
          <el-button type="primary" link @click="handleEdit(row)">编辑</el-button>
          <el-button type="danger" link @click="handleDelete(row)">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    
    <!-- 菜单表单对话框 -->
    <el-dialog
      v-model="dialogVisible"
      :title="dialogTitle"
      width="600px"
      :close-on-click-modal="false"
    >
      <el-form
        ref="menuFormRef"
        :model="menuForm"
        :rules="formRules"
        label-width="100px"
        v-loading="formLoading"
      >
        <el-form-item label="菜单名称" prop="Name">
          <el-input v-model="menuForm.Name" placeholder="请输入菜单名称" />
        </el-form-item>
        
        <el-form-item label="菜单类型" prop="Type">
          <el-radio-group v-model="menuForm.Type">
            <el-radio :label="1">目录</el-radio>
            <el-radio :label="2">菜单</el-radio>
          </el-radio-group>
        </el-form-item>
        
        <el-form-item label="菜单分类" prop="Category">
          <el-radio-group v-model="menuForm.Category">
            <el-radio label="Admin">管理端</el-radio>
            <el-radio label="App">客户端</el-radio>
          </el-radio-group>
        </el-form-item>
        
        <el-form-item label="父级菜单" prop="ParentCode">
          <el-select v-model="menuForm.ParentCode" clearable placeholder="请选择父级菜单">
            <el-option
              v-for="item in menuOptions"
              :key="item.Id"
              :label="item.Name"
              :value="item.Id"
            />
          </el-select>
        </el-form-item>
        
        <el-form-item label="菜单路径" prop="Path">
          <el-input v-model="menuForm.Path" placeholder="请输入菜单路径" />
        </el-form-item>
        
        <el-form-item label="是否外链" prop="IsExternal">
          <el-switch v-model="menuForm.IsExternal" />
        </el-form-item>
        
        <el-form-item v-if="menuForm.IsExternal" label="外链地址" prop="ExternalUrl">
          <el-input v-model="menuForm.ExternalUrl" placeholder="请输入外链地址" />
        </el-form-item>
        
        <el-form-item label="菜单图标" prop="Icon">
          <el-input v-model="menuForm.Icon" placeholder="请输入菜单图标" />
        </el-form-item>
        
        <el-form-item label="权限标识" prop="PermissionCode">
          <el-input v-model="menuForm.PermissionCode" placeholder="请输入权限标识" />
        </el-form-item>
        
        <el-form-item label="排序" prop="Sort">
          <el-input-number v-model="menuForm.Sort" :min="0" />
        </el-form-item>
        
        <el-form-item label="状态" prop="IsEnabled">
          <el-switch v-model="menuForm.IsEnabled" active-text="启用" inactive-text="禁用" />
        </el-form-item>
      </el-form>
      
      <template #footer>
        <el-button @click="dialogVisible = false">取 消</el-button>
        <el-button type="primary" @click="saveMenu" :loading="formLoading">确 定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style lang="scss" scoped>
.app-container {
  padding: 20px;
}

.header {
  margin-bottom: 20px;
}

.el-table {
  margin-top: 10px;
}
</style>





