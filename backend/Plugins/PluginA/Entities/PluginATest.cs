using Fastdotnet.Core.Dtos.Base;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Entities
{
    [SugarTable("plugina_test", "插件A演示测试")]
    public class PluginATest : BaseEntity
    {
        /// <summary>
        /// 名称
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar(50)", IsNullable = false)]
        public string Name { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar(500)", IsNullable = true)]
        public string Description { get; set; }

        /// <summary>
        /// 测试值
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int TestValue { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 创建者
        /// </summary>
        [SugarColumn(ColumnDataType = "nvarchar(50)", IsNullable = true)]
        public string Creator { get; set; }
    }
}
