using System;
using System.Text;
using System.Text.RegularExpressions;

namespace LionFrame.Basic.Extensions
{
    /// <summary>
    /// 字符串静态扩展
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 字符串是否为Null、空字符串组成。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// 字符串 不为Null、空字符串组成。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNotNullOrEmpty(this string str)
        {
            return !string.IsNullOrEmpty(str);
        }
        /// <summary>
        /// 字符串是否为Null、空字符串或仅由空白字符组成。
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// 添加判断添加
        /// </summary>
        /// <param name="str"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string If(this string str, bool condition)
        {
            return condition ? str : string.Empty;
        }

        /// <summary>
        /// 从字符串的开头得到一个字符串的子串
        /// len参数不能大于给定字符串的长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Left(this string str, int len)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length < len)
                throw new ArgumentException("len参数不能大于给定字符串的长度");

            return str.Substring(0, len);
        }

        /// <summary>
        /// 从字符串的末尾得到一个字符串的子串
        /// len参数不能大于给定字符串的长度
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Right(this string str, int len)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length < len)
                throw new ArgumentException("len参数不能大于给定字符串的长度");

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// 
        /// len参数大于给定字符串是返回原字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string MaxLeft(this string str, int len)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length < len)
                return str;

            return str.Substring(0, len);
        }

        /// <summary>
        /// 从指定位置截取字符串，如果小于指定位置，则返回空字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Sub(this string str, int len)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length < len)
                return string.Empty;

            return str.Substring(len);
        }

        /// <summary>
        /// 从字符串的末尾得到一个字符串的子串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string MaxRight(this string str, int len)
        {
            if (str == null)
                throw new ArgumentNullException("str");

            if (str.Length < len)
                return str;

            return str.Substring(str.Length - len, len);
        }

        /// <summary>
        /// 是否是数字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDouble(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return double.TryParse(str, out double number);
        }

        #region 类型转换
        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T">类型的枚举</typeparam>
        /// <param name="value">字符串值转换</param>
        /// <returns></returns>
        public static T ToEnum<T>(this string value)
           where T : struct
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return (T)Enum.Parse(typeof(T), value);
        }

        /// <summary>
        /// 字符串转枚举
        /// </summary>
        /// <typeparam name="T">类型的枚举</typeparam>
        /// <param name="value">字符串值转换</param>
        /// <param name="ignoreCase">忽略大小写</param>
        /// <returns>Returns enum object</returns>
        public static T ToEnum<T>(this string value, bool ignoreCase)
            where T : struct
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return (T)Enum.Parse(typeof(T), value, ignoreCase);
        }

        public static int ToInt32(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Convert.ToInt32(str);
        }

        public static bool ToBoolean(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Convert.ToBoolean(str);
        }

        public static DateTime ToDateTime(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Convert.ToDateTime(str);
        }

        public static decimal ToDecimal(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Convert.ToDecimal(str);
        }

        public static double ToDouble(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Convert.ToDouble(str);
        }

        public static float ToFloat(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Convert.ToSingle(str);
        }

        /// <summary>
        /// 如果不是数值 则返回默认值
        /// </summary>
        /// <param name="str"></param>
        /// <param name="defaultNumber"></param>
        /// <returns></returns>
        public static float? ToFloatOrDefault(this string str, float? defaultNumber = 0)
        {
            if (float.TryParse(str?.Trim(), out float number))
            {
                return number;
            }
            return defaultNumber;
        }

        /// <summary>
        /// Bytes转String
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string BytesToString(this byte[] byteArray)
        {
            if (byteArray == null)
                throw new ArgumentNullException(nameof(byteArray));
            return Encoding.Default.GetString(byteArray);
        }

        /// <summary>
        /// String转Bytes
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ToBytes(this string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str));
            return Encoding.Default.GetBytes(str);
        }
        #endregion

        ///   <summary>
        ///   去除HTML标记
        ///   </summary>
        ///   <param   name="strHtml">包括HTML的源码   </param>
        ///   <returns>已经去除后的文字</returns>
        public static string StripHTML(string strHtml, bool keepEnter = false)
        {
            string[] aryReg ={
                                @"<script[^>]*?>.*?</script>",
                                @"<style[^>]*?>.*?</style>",
                                @"<(.[^>]*)>",//@"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[""'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>", 
                                @"&(quot|#34);",
                                @"&(amp|#38);",
                                @"&(lt|#60);",
                                @"&(gt|#62);",
                                @"&(nbsp|#160);",
                                @"&(iexcl|#161);",
                                @"&(cent|#162);",
                                @"&(pound|#163);",
                                @"&(copy|#169);",
                                @"&#(\d+);",
                                @"-->",
                                @"<!--.*\n" ,
                                "<.+?>",
                                @"\u000a",
                                @"\u0009",
                                @"\u000d",
                                @"\\u007f",
                                @"\u007f",
                                @"\u003ca",
                                @"\u003e",
                                @"\u003c",
                                @"\r",
                                @"\n",
                                @"\b",
                                @"\t",
                                @"\b"
                             };

            string[] aryRep = {
                                "",
                                "",
                                "",
                                "\"",
                                "&",
                                "<",
                                ">",
                                " ",
                                "\xa1",//chr(161), 
                                "\xa2",//chr(162), 
                                "\xa3",//chr(163), 
                                "\xa9",//chr(169), 
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                "",
                                ""
                              };

            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput.Replace("<", "");
            strOutput.Replace(">", "");
            if (!keepEnter)
            {
                strOutput.Replace("\r\n", "");
            }
            strOutput = strOutput.Trim();
            return strOutput;
        }

        /// <summary>
        /// 得到字符串的Byte
        /// </summary>
        /// <param name="str"></param>
        /// <returns>Byte</returns>
        public static byte[] GetBytes(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return default(byte[]);
            return Encoding.UTF8.GetBytes(str);
        }

        public static bool ToBool(this string str, bool defaultValue = false)
        {
            bool.TryParse(str, out defaultValue);
            return defaultValue;
        }

        public static short ToShort(this string str, short defaultValue = 0)
        {
            short.TryParse(str, out defaultValue);
            return defaultValue;
        }

        public static int ToInt(this string str, int defaultValue = 0)
        {
            int.TryParse(str, out defaultValue);
            return defaultValue;
        }

        public static long ToLong(this string str, long defaultValue = 0)
        {
            long.TryParse(str, out defaultValue);
            return defaultValue;
        }

        public static double ToDouble(this string str, double defaultValue = 0)
        {
            double.TryParse(str, out defaultValue);
            return defaultValue;
        }

        /// <summary>
        /// 比较是否相等，忽略大小写
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string a, string b)
        {
            if (a != null)
                return a.Equals(b, StringComparison.CurrentCultureIgnoreCase);
            return b == null;
        }
    }
}
