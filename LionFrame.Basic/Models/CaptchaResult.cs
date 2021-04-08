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
        public byte[] CaptchaBase64Data { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
