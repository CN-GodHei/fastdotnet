using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PluginA.Dto
{
    public class PluginAUserExtensionDto
    {
        /// <summary>
        /// 关联的用户ID
        /// </summary>
        [SugarColumn]
        public string FdAppUserId { get; set; }

        /// <summary>
        /// 用户偏好设置
        /// </summary>
        [SugarColumn]
        public string Preferences { get; set; }

        /// <summary>
        /// 用户积分
        /// </summary>
        [SugarColumn]
        public int Points { get; set; }
    }

    public class CreatePluginAUserExtensionDto
    {
        /// <summary>
        /// 关联的用户ID
        /// </summary>
        //[SugarColumn]
        //public string FdAppUserId { get; set; }

        /// <summary>
        /// 用户偏好设置
        /// </summary>
        [SugarColumn]
        public string Preferences { get; set; }

        /// <summary>
        /// 用户积分
        /// </summary>
        [SugarColumn]
        public int Points { get; set; }
    }

    public class UpdatePluginAUserExtensionDto
    {
        [SugarColumn]
        public string Id { get; set; }

        /// <summary>
        /// 关联的用户ID
        /// </summary>
        //[SugarColumn]
        //public string FdAppUserId { get; set; }

        /// <summary>
        /// 用户偏好设置
        /// </summary>
        [SugarColumn]
        public string Preferences { get; set; }

        /// <summary>
        /// 用户积分
        /// </summary>
        [SugarColumn]
        public int Points { get; set; }
    }
}
