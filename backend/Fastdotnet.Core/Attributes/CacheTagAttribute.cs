using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Attributes
{
    /// <summary>
    /// 缓存标签属性，用于对缓存项进行分类管理
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class CacheTagAttribute : Attribute
    {
        /// <summary>
        /// 标签名称
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="tag">标签名称</param>
        public CacheTagAttribute(string tag)
        {
            Tag = tag;
        }
    }
}