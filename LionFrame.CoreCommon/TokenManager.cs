using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using LogHelper = LionFrame.Basic.LogHelper;

namespace LionFrame.CoreCommon
{
    /// <summary>
    /// token管理验证类
    /// </summary>
    public class TokenManager
    {
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="tokenStr">需要签名的数据  </param>
        /// <param name="expireHour">默认3天过期</param>
        /// <returns>返回token字符串</returns>
        public static string GenerateToken(string tokenStr, int expireHour = 3 * 24) //3天过期
        {
            var key1 = new SymmetricSecurityKey(Convert.FromBase64String(LionWeb.Configuration["Jwt:Key"]));
            var cred = new SigningCredentials(key1, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim("sid",tokenStr),
                //new Claim(ClaimTypes.Name,name), //示例  可使用ClaimTypes中的类型
            };
            var token = new JwtSecurityToken(
                issuer: LionWeb.Configuration["Jwt:Issuer"],//签发者
                notBefore: DateTime.Now,//token不能早于这个时间使用
                expires: DateTime.Now.AddHours(expireHour),//添加过期时间
                claims: claims,//签名数据
                signingCredentials: cred//签名
                );
            //解决一个不知什么问题的PII什么异常
            IdentityModelEventSource.ShowPII = true;
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// 得到Token中的验证消息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ValidateToken(string token, out DateTime dateTime)
        {
            dateTime = DateTime.Now;
            var principal = GetPrincipal(token, out dateTime);

            if (principal == null)
                return default(string);

            ClaimsIdentity identity = null;
            try
            {
                identity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return null;
            }
            //identity.FindFirst(ClaimTypes.Name).Value;
            return identity.FindFirst("sid").Value;
        }

        /// <summary>
        /// 从Token中得到ClaimsPrincipal对象
        /// </summary>
        /// <param name="token"></param>
        /// <param name="dateTime">返回过期时间</param>
        /// <returns></returns>
        private static ClaimsPrincipal GetPrincipal(string token, out DateTime dateTime)
        {
            try
            {
                dateTime = DateTime.Now;
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);

                if (jwtToken == null)
                    return null;

                var key = Convert.FromBase64String(LionWeb.Configuration["Jwt:Key"]);

                var parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = true,//验证创建该令牌的发布者
                    ValidateLifetime = true,//检查令牌是否未过期，以及发行者的签名密钥是否有效
                    ValidateAudience = false,//确保令牌的接收者有权接收它
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidIssuer = LionWeb.Configuration["Jwt:Issuer"]//验证创建该令牌的发布者
                };
                //验证token 
                var principal = tokenHandler.ValidateToken(token, parameters, out var securityToken);
                //若开始时间大于当前时间 或结束时间小于当前时间 则返回空
                if (securityToken.ValidFrom.ToLocalTime() > DateTime.Now || securityToken.ValidTo.ToLocalTime() < DateTime.Now)
                {
                    dateTime = DateTime.Now;
                    return null;
                }
                dateTime = securityToken.ValidTo.ToLocalTime();//返回Token结束时间
                return principal;
            }
            catch (Exception e)
            {
                dateTime = DateTime.Now;
                LogHelper.Logger.Fatal(e, "Token验证失败");
                return null;
            }
        }
    }
}
