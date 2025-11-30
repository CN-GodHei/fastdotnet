using Fastdotnet.Core.Dtos.System;
using Fastdotnet.Core.Entities.System;
using Fastdotnet.Service.Service;
using SqlSugar;
using System;
using System.Collections.Generic;

// 简单测试代码生成功能
public class TestFrontendGeneration
{
    public static void Main()
    {
        // 创建模拟数据来测试前端代码生成
        var entityName = "TestEntity";
        var configColumns = new List<FdCodeGenConfig>
        {
            new FdCodeGenConfig 
            { 
                PropertyName = "Name", 
                ShowColumnName = "姓名", 
                EffectType = "Input", 
                WhetherAddUpdate = true,
                WhetherTable = true,
                WhetherQuery = true,
                QueryType = "contains"
            },
            new FdCodeGenConfig 
            { 
                PropertyName = "Age", 
                ShowColumnName = "年龄", 
                EffectType = "InputNumber", 
                WhetherAddUpdate = true,
                WhetherTable = true,
                WhetherQuery = true,
                QueryType = "eq"
            },
            new FdCodeGenConfig 
            { 
                PropertyName = "BirthDate", 
                ShowColumnName = "出生日期", 
                EffectType = "Datetime", 
                WhetherAddUpdate = true,
                WhetherTable = true,
                WhetherQuery = true,
                QueryType = "BETWEEN"
            },
            new FdCodeGenConfig 
            { 
                PropertyName = "IsActive", 
                ShowColumnName = "是否激活", 
                EffectType = "Switch", 
                WhetherAddUpdate = true,
                WhetherTable = true,
                WhetherQuery = true,
                QueryType = "eq"
            },
            new FdCodeGenConfig 
            { 
                PropertyName = "Gender", 
                ShowColumnName = "性别", 
                EffectType = "Radio", 
                WhetherAddUpdate = true,
                WhetherTable = true,
                WhetherQuery = true,
                QueryType = "eq"
            },
            new FdCodeGenConfig 
            { 
                PropertyName = "Description", 
                ShowColumnName = "描述", 
                EffectType = "Textarea", 
                WhetherAddUpdate = true,
                WhetherTable = true,
                WhetherQuery = false,
                QueryType = ""
            }
        };
        
        var columns = new List<ColumnInfoDto>
        {
            new ColumnInfoDto { ColumnName = "name", PropertyName = "Name", DataType = "varchar", NetType = "string", Length = 50 },
            new ColumnInfoDto { ColumnName = "age", PropertyName = "Age", DataType = "int", NetType = "int", Length = 0 },
            new ColumnInfoDto { ColumnName = "birth_date", PropertyName = "BirthDate", DataType = "datetime", NetType = "DateTime", Length = 0 },
            new ColumnInfoDto { ColumnName = "is_active", PropertyName = "IsActive", DataType = "bit", NetType = "bool", Length = 0 },
            new ColumnInfoDto { ColumnName = "gender", PropertyName = "Gender", DataType = "varchar", NetType = "string", Length = 10 },
            new ColumnInfoDto { ColumnName = "description", PropertyName = "Description", DataType = "text", NetType = "string", Length = 500 }
        };

        var service = new CodeGenConfigService(null, null); // 模拟服务实例，实际使用中依赖注入会提供
        
        // 注意：因为GenerateFrontendVueContentAsync方法是异步的，而且需要完整的IServiceProvider
        // 我们在这里创建了一个简化的测试方法来验证生成结果
        Console.WriteLine("测试前端代码生成功能 - 不同EffectType对应不同前端组件");
        
        // 测试不同的EffectType生成不同的表单组件
        Console.WriteLine("\n=== 测试表单组件生成 ===");
        foreach (var col in configColumns)
        {
            var component = GetFormComponentByEffectType(col, columns);
            Console.WriteLine($"属性: {col.PropertyName}, EffectType: {col.EffectType}, 生成组件: {GetFirstTag(component)}");
        }
        
        // 测试不同的EffectType生成不同的查询组件
        Console.WriteLine("\n=== 测试查询组件生成 ===");
        foreach (var col in configColumns)
        {
            var component = GetQueryComponentByEffectType(col);
            Console.WriteLine($"属性: {col.PropertyName}, EffectType: {col.EffectType}, 生成查询组件: {GetFirstTag(component)}");
        }
    }

    // 复制服务中的方法实现进行测试
    private static string GetFormComponentByEffectType(FdCodeGenConfig fdCodeGenConfig, List<ColumnInfoDto> columns)
    {
        var effectType = fdCodeGenConfig.EffectType?.ToLower() ?? "input";
        var showColumnName = fdCodeGenConfig.ShowColumnName ?? fdCodeGenConfig.PropertyName;

        return effectType switch
        {
            "select" or "dictselector" => $@"<el-select v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}""  clearable style=""width: 100%"">
                    <el-option label=""选项1"" value=""1"" />
                    <el-option label=""选项2"" value=""2"" />
                </el-select>",
            "datetime" or "date" or "time" => $@"<el-date-picker v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" type=""datetime"" placeholder=""请选择{showColumnName}""  style=""width: 100%"" value-format=""YYYY-MM-DD HH:mm:ss"" />",
            "input-number" => $@"<el-input-number v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}""  style=""width: 100%"" />",
            "switch" => $@"<el-switch v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" />",
            "radio" => $@"<el-radio-group v-model=""state.formData.{fdCodeGenConfig.PropertyName}"">
                    <el-radio label=""1"">是</el-radio>
                    <el-radio label=""0"">否</el-radio>
                </el-radio-group>",
            "checkbox" => $@"<el-checkbox-group v-model=""state.formData.{fdCodeGenConfig.PropertyName}"">
                    <el-checkbox label=""1"">选项1</el-checkbox>
                    <el-checkbox label=""2"">选项2</el-checkbox>
                </el-checkbox-group>",
            "textarea" => $@"<el-input v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}""  type=""textarea"" rows=""4"" />",
            "password" => $@"<el-input v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}""  type=""password"" show-password />",
            "upload" => $@"<el-upload
                    v-model:file-list=""state.formData.{fdCodeGenConfig.PropertyName}""
                    class=""upload-demo""
                    action=""/api/upload""
                    multiple
                    :limit=""3"">
                    <el-button size=""small"" type=""primary"">点击上传</el-button>
                    <template #tip>
                        <div class=""el-upload__tip"">支持多文件上传，最多3个文件</div>
                    </template>
                </el-upload>",
            _ => $@"<el-input v-model=""state.formData.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}""  clearable />"
        };
    }
    
    // 复制服务中的方法实现进行测试
    private static string GetQueryComponentByEffectType(FdCodeGenConfig fdCodeGenConfig)
    {
        var effectType = fdCodeGenConfig.EffectType?.ToLower() ?? "input";
        var showColumnName = fdCodeGenConfig.ShowColumnName ?? fdCodeGenConfig.PropertyName;

        return effectType switch
        {
            "select" or "dictselector" => $@"<el-select v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"">
                    <el-option label=""选项1"" value=""1"" />
                    <el-option label=""选项2"" value=""2"" />
                </el-select>",
            "datetime" or "date" or "time" => $@"<el-date-picker v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" type=""datetime"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"" />",
            "input-number" => $@"<el-input-number v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" style=""width: 150px"" />",
            "switch" => $@"<el-switch v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" />",
            "radio" => $@"<el-radio-group v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"">
                    <el-radio label=""1"">是</el-radio>
                    <el-radio label=""0"">否</el-radio>
                </el-radio-group>",
            "checkbox" => $@"<el-checkbox-group v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请选择{showColumnName}"" clearable style=""width: 150px"">
                    <el-checkbox label=""1"">选项1</el-checkbox>
                    <el-checkbox label=""2"">选项2</el-checkbox>
                </el-checkbox-group>",
            _ => $@"<el-input v-model=""state.queryParams.{fdCodeGenConfig.PropertyName}"" placeholder=""请输入{showColumnName}"" clearable style=""width: 150px"" />"
        };
    }

    // 提取组件的第一个标签名
    private static string GetFirstTag(string component)
    {
        var start = component.IndexOf('<');
        if (start >= 0)
        {
            var end = component.IndexOf(' ', start);
            if (end < 0) end = component.IndexOf('>', start);
            if (end > start)
            {
                return component.Substring(start, end - start);
            }
        }
        return "unknown";
    }
}