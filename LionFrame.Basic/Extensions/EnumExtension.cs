using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace LionFrame.Basic.Extensions
{
    /// <summary>
    /// 枚举静态扩展
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取DisplayAttribute上指定的Name
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDisplay(this Enum value)
        {
            var type = value.GetType();
            // 获取常数名称
            var name = value.ToString();
            // 获取常数访问权限
            var field = type.GetField(name);

            if (!field.IsDefined(typeof(DisplayAttribute), true))
            {
                return name;
            }
            var desc = (DisplayAttribute)field.GetCustomAttribute(typeof(DisplayAttribute));
            return desc.Name;
        }
        /// <summary>
        /// 获取DisplayAttribute上指定的Name
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal">defVal</param>
        /// <returns></returns>
        public static string GetDisplayName(this Enum value, string defVal = "")
        {
            var type = value.GetType();
            // 获取常数名称
            var name = value.ToString();
            // 获取常数访问权限
            var field = type.GetField(name);

            if (!field.IsDefined(typeof(DisplayAttribute), true))
            {
                return defVal == "" ? name : defVal;
            }
            var desc = (DisplayAttribute)field.GetCustomAttribute(typeof(DisplayAttribute));
            return desc.Name;
        }

        /// <summary>
        /// 获取DisplayAttribute上指定的Desc
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defVal">defVal</param>
        /// <returns></returns>
        public static string GetDisplayDesc(this Enum value, string defVal = "")
        {
            var type = value.GetType();
            // 获取常数名称
            var name = value.ToString();
            // 获取常数访问权限
            var field = type.GetField(name);

            if (!field.IsDefined(typeof(DisplayAttribute), true))
            {
                return defVal == "" ? name : defVal;
            }
            var desc = (DisplayAttribute)field.GetCustomAttribute(typeof(DisplayAttribute));
            return desc.Description;
        }

        /// <summary>
        /// 得到某个字段上的attribute
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Attribute GetAttribute<T>(this Enum value) where T : Attribute
        {
            var type = value.GetType();
            // 获取常数名称
            var name = value.ToString();
            // 获取常数访问权限
            var field = type.GetField(name);

            if (!field.IsDefined(typeof(T), true))
            {
                return default;
            }
            var attribute = (T)field.GetCustomAttribute(typeof(T), true);
            return attribute;
        }

        /// <summary>
        ///  获取DescriptionAttribute上指定的Description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum value)
        {
            return GetEnumDescription(value);
        }
        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="t"></param>
        /// <param name="defaultDesc">不输则返回枚举的名字</param>
        /// <returns></returns>
        public static string GetEnumDescription(this Enum t, string defaultDesc = "")
        {
            var type = t.GetType();
            // 获取常数名称
            var name = t.ToString();
            // 获取常数访问权限
            FieldInfo field = type.GetField(name);

            if (!field.IsDefined(typeof(DescriptionAttribute), true))
            {
                return defaultDesc == "" ? type.Name : defaultDesc;
            }
            var desc = (DescriptionAttribute)field.GetCustomAttribute(typeof(DescriptionAttribute));
            return desc.Description;
        }

        /// <summary>
        ///  转化为枚举类型，转化不成功为默认值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int value, T defVal = default(T)) where T : struct
        {
            T objEnum;
            Type typeEnum = typeof(T);
            if (string.IsNullOrEmpty(value.ToString())) return defVal;
            if (!Enum.TryParse<T>(value.ToString(), out objEnum) || !Enum.IsDefined(typeEnum, objEnum))
            {
                objEnum = defVal;
            }
            return objEnum;
        }
    }
}
