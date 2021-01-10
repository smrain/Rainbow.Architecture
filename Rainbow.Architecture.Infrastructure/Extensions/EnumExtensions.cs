using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Rainbow.Architecture.Infrastructure.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 获取枚举值的Description
        /// </summary>
        public static string GetDescription<T>(this T value) where T : struct
        {
            string result = value.ToString();
            Type type = typeof(T);
            FieldInfo info = type.GetField(value.ToString());
            var attributes = info.GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attributes != null && attributes.FirstOrDefault() != null)
            {
                result = (attributes.First() as DescriptionAttribute).Description;
            }

            return result;
        }

        /// <summary>
        /// 根据Description获取枚举值
        /// </summary>
        public static T GetValueByDescription<T>(this string description) where T : struct
        {
            Type type = typeof(T);
            foreach (var field in type.GetFields())
            {
                if (field.Name == description)
                {
                    return (T)field.GetValue(null);
                }

                var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attributes != null && attributes.FirstOrDefault() != null)
                {
                    if (attributes.First().Description == description)
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            return default(T);
        }

        /// <summary>
        /// 根据枚举内容获取枚举值
        /// </summary>
        public static T GetValue<T>(this string value) where T : struct
        {
            if (Enum.TryParse(value, true, out T result))
            {
                return result;
            }
            return default(T);
        }
    }
}
