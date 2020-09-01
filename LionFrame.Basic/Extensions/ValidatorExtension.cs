using System.Text.RegularExpressions;

namespace LionFrame.Basic.Extensions
{
    /// <summary>
    /// 数据验证 矫正 静态扩展
    /// </summary>
    public static class ValidatorExtension
    {
        /// <summary>
        /// 验证是否是电话号码
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsPhoneNumber(this string number)
        {
            return IsTelephone(number) || IsMobilePhone(number);
        }

        /// <summary>
        /// 验证是否是电话号码
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsTelephone(this string number)
        {
            const string regformat = @"^((\+86)|(86))?1(3|4|5|6|7|8|9)\d{9}$";
            var regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(number);
        }

        /// <summary>
        /// 验证是否是手机号
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(this string number)
        {
            const string regformat = @"^(\(\d{3,4}\)|\d{3,4}-|\s)?\d{7,14}$";
            var regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(number);
        }

        /// <summary>
        /// 验证是否是邮箱
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsEmail(this string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                const string regformat = @"^[A-Za-z0-9\u4e00-\u9fa5_-]+@[a-zA-Z0-9_-]+(\.[a-zA-Z0-9_-]+)+$";
                var regex = new Regex(regformat, RegexOptions.IgnoreCase);
                return regex.IsMatch(email);
            }
            return false;
        }
        /// <summary>
        /// 验证是否是纳税人识别码
        /// </summary>
        /// <param name="tax"></param>
        /// <returns></returns>
        public static bool IsTaxpayer(this string tax)
        {
            const string regformat = @"^[A-Za-z0-9]+$";
            var regex = new Regex(regformat);
            return regex.IsMatch(tax);
        }

        /// <summary>
        /// 是否为Ip地址
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsIpAddress(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < 7 || str.Length > 15)
                return false;

            const string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";
            var regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str);
        }

        /// <summary>
        /// 是否为url地址
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static bool ValidateUrlAddress(this string ipAddress)
        {
            Regex validipregex = new Regex(@"^((http|ftp|https)://)(([a-zA-Z0-9\._-]+\.[a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(:[0-9]{1,4})*(/[a-zA-Z0-9\&%_\./-~-]*)?$");
            return (ipAddress != "" && validipregex.IsMatch(ipAddress.Trim())) ? true : false;
        }

        public static bool IsWindowFileName(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return false;
            }
            var items = new[] { "\\", "/", ":", "*", "?", "\"", "<", ">", "|" };
            foreach (var item in items)
            {
                if (str.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// 判断某个数据是否在这个字段值中
        /// </summary>
        /// <param name="num"></param>
        /// <param name="minNum"></param>
        /// <param name="maxNum"></param>
        /// <returns></returns>
        public static bool IsInRange(this int num, int minNum, int maxNum)
        {
            return num >= minNum && num <= maxNum;
        }

        /// <summary>
        /// 正则替换文件名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string RegexFileName(this string filePath)
        {
            Regex regex = new Regex(@"[\/\:\*\?\""\<\>\|\,\\]");
            var matcher = regex.Matches(filePath);
            if (matcher.Count <= 0) return filePath;
            foreach (Match match in matcher)
            {
                filePath = filePath.Replace(match.Value, "");
            }
            return filePath;
        }
    }
}
