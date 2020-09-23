using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace LionFrame.Basic.Encryptions
{
    /// <summary>
    /// 加密数据 aes 比des 的密码强度高
    /// </summary>
    public static class Cryptography
    {
        /// <summary> 
        /// 秘钥向量 
        /// </summary> 
        private static string Iv => "0102030405060708";
        private static string Secret => "Levy2020";


        #region aes 加密

        /// <summary>
        /// 默认Key加密(Key=Secret)
        /// </summary>
        /// <param name="text">需要加密的明文</param>
        /// <returns>加密的明文文</returns>
        public static string EncryptAES(this string text)
        {
            return EncryptAES(text, Secret);
        }

        /// <summary> 
        /// 加密数据 
        /// </summary> 
        /// <param name="text">需要加密的明文</param> 
        /// <param name="sKey">密钥</param> 
        /// <returns>加密的明文文</returns> 
        public static string EncryptAES(string text, string sKey)
        {
            var encryptData = EncryptAES(text, sKey, Iv);
            return encryptData;
        }

        /// <summary>AES加密</summary>
        /// <param name="text">明文</param>
        /// <param name="key">密钥,长度为16的字符串</param>
        /// <param name="iv">偏移量,长度为16的字符串</param>
        /// <returns>密文</returns>
        public static string EncryptAES(string text, string key, string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.Zeros;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = transform.TransformFinalBlock(plainText, 0, plainText.Length);
            return Convert.ToBase64String(cipherBytes);
        }

        #endregion

        #region  aes 解密

        /// <summary>
        /// 默认Key解密(Key=Secret)
        /// </summary>
        /// <param name="text">要解密的密文</param>
        /// <returns>解密的明文</returns>
        public static string DecryptAES(this string text)
        {
            return DecryptAES(text, Secret);
        }

        /// <summary> 
        /// 解密数据 
        /// </summary> 
        /// <param name="text">要解密的密文</param> 
        /// <param name="sKey">密钥</param> 
        /// <returns>解密的明文</returns> 
        public static string DecryptAES(string text, string sKey)
        {
            var encryptData = DecryptAES(text, sKey, Iv);
            return encryptData;
        }

        /// <summary>AES解密</summary>
        /// <param name="text">密文</param>
        /// <param name="key">密钥,长度为16的字符串</param>
        /// <param name="iv">偏移量,长度为16的字符串</param>
        /// <returns>明文</returns>
        public static string DecryptAES(string text, string key, string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.Zeros;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(text);
            byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;
            System.Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = Encoding.UTF8.GetBytes(iv);
            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }
        #endregion

        #region Base64加密解密
        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        public static string Base64Encrypt(this string input)
        {
            return Base64Encrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符编码</param>
        /// <returns></returns>
        public static string Base64Encrypt(this string input, Encoding encode)
        {
            return Convert.ToBase64String(encode.GetBytes(input));
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="input">需要解密的字符串</param>
        /// <returns></returns>
        public static string Base64Decrypt(this string input)
        {
            return Base64Decrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="input">需要解密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Base64Decrypt(this string input, Encoding encode)
        {
            return encode.GetString(Convert.FromBase64String(input));
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <returns></returns>
        public static string Md5Encrypt(this string input)
        {
            return Md5Encrypt(input, new UTF8Encoding());
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="input">需要加密的字符串</param>
        /// <param name="encode">字符的编码</param>
        /// <returns></returns>
        public static string Md5Encrypt(this string input, Encoding encode)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(encode.GetBytes(input));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            return sb.ToString();
        }

        /// <summary>
        /// MD5对文件流加密
        /// </summary>
        /// <param name="sr"></param>
        /// <returns></returns>
        public static string Md5Encrypt(this Stream stream)
        {
            MD5 md5serv = MD5CryptoServiceProvider.Create();
            byte[] buffer = md5serv.ComputeHash(stream);
            StringBuilder sb = new StringBuilder();
            foreach (byte var in buffer)
                sb.Append(var.ToString("x2"));
            return sb.ToString();
        }

        /// <summary>
        /// MD5加密(返回16位加密串)
        /// </summary>
        /// <param name="input"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static string ToMD5Encrypt16(this string input, Encoding encode)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string result = BitConverter.ToString(md5.ComputeHash(encode.GetBytes(input)), 4, 8);
            result = result.Replace("-", "");
            return result;
        }

        /// <summary>
        /// MD5加密md5(base64)
        /// </summary>
        /// <param name="content"></param>
        /// <param name="secretKey"></param>
        /// <returns></returns>
        private static string Md5Encrypt64(this string content, string secretKey = "")
        {
            var sb = new StringBuilder();
            sb.Append(content);
            sb.Append(secretKey);
            var md5 = MD5.Create(); //实例化一个md5对像
            var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
            //var str = Convert.ToString(bytes);
            var stReplace = BitConverter.ToString(bytes).Replace("-", "");
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Md5加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToMd5(this string str)
        {
            using (var md5 = MD5.Create())
            {
                var inputBytes = Encoding.UTF8.GetBytes(str);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                foreach (var hashByte in hashBytes)
                {
                    sb.Append(hashByte.ToString("X2"));
                }

                return sb.ToString();
            }
        }
        #endregion

        /// <summary>
        /// 进行DES加密
        /// </summary>
        /// <param name="pToEncrypt">需要加密的字符串</param>  
        /// <param name="sKey">密钥   长度不能低于8</param>  
        public static string EncryptDes(string pToEncrypt, string sKey)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] keys = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    keys[i] = Convert.ToByte(sKey.Substring(i, 1), 16);
                }
                byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);
                des.Key = keys;
                des.IV = keys;
                des.Padding = PaddingMode.Zeros;
                des.Mode = CipherMode.CBC;
                MemoryStream ms = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string base64 = Convert.ToBase64String(ms.ToArray());
                ms.Close();
                return base64;
            }
        }

        /// <summary>
        /// 进行DES解密
        /// </summary>
        /// <param name="pToDecrypt">需要解密的字符串</param>
        /// <param name="sKey">密钥  长度不能低于8</param>  
        public static string DecryptDes(string pToDecrypt, string sKey)
        {
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                byte[] keys = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    keys[i] = Convert.ToByte(sKey.Substring(i, 1), 16);
                }
                des.Key = keys;
                des.IV = keys;
                des.Padding = PaddingMode.Zeros;
                des.Mode = CipherMode.CBC;
                MemoryStream ms = new MemoryStream();
                using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(inputByteArray, 0, inputByteArray.Length);
                    cs.FlushFinalBlock();
                    cs.Close();
                }
                string str = Encoding.UTF8.GetString(ms.ToArray());
                ms.Close();
                return str;
            }
        }
    }
}
