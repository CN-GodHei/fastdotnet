
namespace Fastdotnet.Core.Utils.Extensions
{
    /// <summary>
    /// 提供基于 DataAnnotations 的模型校验扩展方法。
    /// </summary>
    public static class ModelValidationExtensions
    {
        /// <summary>
        /// 验证模型及其所有嵌套属性和集合是否符合其 DataAnnotations 特性的要求。
        /// </summary>
        /// <typeparam name="T">模型类型。</typeparam>
        /// <param name="model">要验证的模型实例。</param>
        /// <param name="internalReturn">如果为 true，当验证失败时将直接抛出 BusinessException 异常；否则返回一个包含错误信息的 ValidationResult 对象。</param>
        /// <returns>一个 ValidationResult 对象，指示验证是否成功以及相关的错误信息。</returns>
        public static ValidationResult IsValid<T>(this T model, bool internalReturn = true) where T : class
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            try
            {
                var validationErrors = new List<FieldValidationInfo>();
                TraverseObject(model, "", validationErrors);

                if (!validationErrors.Any())
                    return new ValidationResult() { IsValid = true };

                var allErrorMessages = validationErrors
                    .SelectMany(v => v.ErrorMessages)
                    .Distinct()
                    .ToList();

                var errorMessage = string.Join("; ", allErrorMessages);

                if (internalReturn)
                {
                    throw new BusinessException(errorMessage);
                }

                return new ValidationResult() { Message = errorMessage };
            }
            catch (Exception e)
            {
                if (internalReturn)
                {
                    if (e is BusinessException)
                    {
                        //throw; // 重新抛出原始的业务异常，以保留堆栈跟踪,不返回原始堆栈，可以记录异常日志
                        throw new BusinessException(e.Message);
                    }
                    else
                    {
                        throw new BusinessException("校验失败。");
                    }

                }
                return new ValidationResult() { Message = $"校验失败: {e.Message}" };
            }
        }

        /// <summary>
        /// 递归遍历对象的所有属性，并根据 ValidationAttribute 进行校验。
        /// </summary>
        private static void TraverseObject(object obj, string path, List<FieldValidationInfo> validationErrors)
        {
            if (obj == null)
                return;

            var objectType = obj.GetType();
            
            // 对实现了 IValidatableObject 接口的对象，执行其自身的 Validate 方法
            if (obj is IValidatableObject validatableObject)
            {
                var context = new ValidationContext(validatableObject, null, null);
                var results = validatableObject.Validate(context);
                foreach (var validationResult in results)
                {
                    if (validationResult != System.ComponentModel.DataAnnotations.ValidationResult.Success)
                    {
                        var fieldInfo = new FieldValidationInfo
                        {
                            Path = path,
                            DisplayName = objectType.Name,
                            ErrorMessages = { validationResult.ErrorMessage }
                        };
                        validationErrors.Add(fieldInfo);
                    }
                }
            }

            foreach (var prop in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!prop.CanRead || prop.GetIndexParameters().Length > 0)
                    continue;

                string currentPath = string.IsNullOrEmpty(path) ? prop.Name : $"{path}.{prop.Name}";
                string displayName = GetDisplayName(prop);
                object value;
                try
                {
                    value = prop.GetValue(obj);
                }
                catch (Exception ex)
                {
                    // 忽略属性访问异常，但可以记录日志
                    Console.WriteLine($"Error accessing property {currentPath}: {ex.Message}");
                    continue;
                }


                // 1. 校验属性上的特性
                var validationAttributes = prop.GetCustomAttributes<ValidationAttribute>(true);
                var fieldInfo = new FieldValidationInfo { Path = currentPath, DisplayName = displayName };
                bool hasErrorOnProperty = false;

                foreach (var attr in validationAttributes)
                {
                    // Skip MaxLength and StringLength validation for bool type
                    if ((attr is MaxLengthAttribute || attr is StringLengthAttribute) && prop.PropertyType == typeof(bool))
                    {
                        continue;
                    }

                    if (!attr.IsValid(value))
                    {
                        hasErrorOnProperty = true;
                        fieldInfo.ErrorMessages.Add(attr.FormatErrorMessage(displayName));
                    }
                }

                if (hasErrorOnProperty)
                {
                    validationErrors.Add(fieldInfo);
                }

                // 2. 递归校验复杂类型和集合
                if (value != null)
                {
                    var propertyType = prop.PropertyType;
                    if (IsCollectionType(propertyType))
                    {
                        ProcessCollection(value as IEnumerable, currentPath, validationErrors);
                    }
                    // 确保不会对简单类型或已校验过的类型进行不必要的递归
                    else if (!IsSimpleType(propertyType))
                    {
                        TraverseObject(value, currentPath, validationErrors);
                    }
                }
            }
        }

        /// <summary>
        /// 处理集合中的元素校验。
        /// </summary>
        private static void ProcessCollection(IEnumerable collection, string path, List<FieldValidationInfo> validationErrors)
        {
            if (collection == null)
                return;

            int index = 0;
            foreach (var item in collection)
            {
                if (item != null && !IsSimpleType(item.GetType()))
                {
                    TraverseObject(item, $"{path}[{index}]", validationErrors);
                }
                index++;
            }
        }

        /// <summary>
        /// 获取属性的显示名称，优先于 DisplayAttribute, 其次 DisplayNameAttribute, 最后是属性名。
        /// </summary>
        private static string GetDisplayName(PropertyInfo prop)
        {
            var displayAttr = prop.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr != null && !string.IsNullOrEmpty(displayAttr.Name))
                return displayAttr.Name;

            var displayNameAttr = prop.GetCustomAttribute<DisplayNameAttribute>();
            if (displayNameAttr != null && !string.IsNullOrEmpty(displayNameAttr.DisplayName))
                return displayNameAttr.DisplayName;

            return prop.Name;
        }

        /// <summary>
        /// 判断一个类型是否为简单类型。
        /// </summary>
        private static bool IsSimpleType(Type type)
        {
            // 考虑可空类型
            var underlyingType = Nullable.GetUnderlyingType(type);
            type = underlyingType ?? type;

            return type.IsPrimitive ||
                   type.IsEnum ||
                   type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(Guid) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(DateTimeOffset);
        }

        /// <summary>
        /// 判断一个类型是否为集合类型（非字符串）。
        /// </summary>
        private static bool IsCollectionType(Type type)
        {
            if (type == typeof(string))
                return false;
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        /// <summary>
        /// 内部类，用于存储字段的验证错误信息。
        /// </summary>
        private class FieldValidationInfo
        {
            public string Path { get; set; }
            public string DisplayName { get; set; }
            public List<string> ErrorMessages { get; set; } = new List<string>();
        }

        /// <summary>
        /// 验证结果的返回类。
        /// </summary>
        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }
    }
}