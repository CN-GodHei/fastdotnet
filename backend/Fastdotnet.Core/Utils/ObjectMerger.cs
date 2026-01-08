using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fastdotnet.Core.Utils
{
    // 对象通用合并工具类
    public static class ObjectMerger
    {
        //public static T Merge<T>(T target, T source) where T : class
        //{
        //    if (target == null) return source;
        //    if (source == null) return target;

        //    var targetJson = JObject.FromObject(target);
        //    var sourceJson = JObject.FromObject(source);

        //    targetJson.Merge(sourceJson, new JsonMergeSettings
        //    {
        //        MergeArrayHandling = MergeArrayHandling.Replace,
        //        MergeNullValueHandling = MergeNullValueHandling.Ignore
        //    });

        //    return targetJson.ToObject<T>();
        //}

        public static T ApplyOverrides<T>(T defaults, T overrides) where T : class, new()
        {
            if (overrides == null) return defaults;
            if (defaults == null) return overrides;

            // 先深拷贝 defaults
            var result = JsonConvert.DeserializeObject<T>(
                JsonConvert.SerializeObject(defaults)
            );

            // 用 overrides 中的非 null 值去更新 result
            JsonConvert.PopulateObject(
                JsonConvert.SerializeObject(overrides),
                result,
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                }
            );

            return result;
        }
    }
}
