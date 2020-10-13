using Newtonsoft.Json;
using System;

namespace LionFrame.Basic.Models
{
    /// <summary>
    /// 验证码结果
    /// </summary>
    public class CaptchaResult
    {
        public string Uuid { get; set; }
#if !DEBUG
        [JsonIgnore]
#endif
        public string Captcha { get; set; }
#if !DEBUG
        [JsonIgnore]
#endif
        public byte[] CaptchaByteData { get; set; }
        public string CaptchaBase64Data => Convert.ToBase64String(CaptchaByteData);
        public DateTime Timestamp { get; set; }
    }
}
