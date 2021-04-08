using LionFrame.Basic.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace LionFrame.Basic
{
    /// <summary>
    /// 图形验证码工具类
    /// </summary>
    public static class CaptchaHelper
    {
        const string Letters = "2346789ABCDEFGHJKLMNPRTUVWXYZ";

        /// <summary>
        /// 生成带字母的验证码 -- 去除了混淆字母
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateCaptchaCode(int length = 4)
        {
            Random rand = new Random();
            int maxRand = Letters.Length - 1;

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                int index = rand.Next(maxRand);
                sb.Append(Letters[index]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 生成指定位数的随机数字码
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateRandomNumber(int length)
        {
            Random random = new Random();
            StringBuilder sbMsgCode = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sbMsgCode.Append(random.Next(0, 9));
            }

            return sbMsgCode.ToString();
        }

        /// <summary>
        /// 创建验证码图片
        /// </summary>
        /// <param name="captcha"></param>
        /// <returns></returns>
        public static CaptchaResult CreateVerifyCodeImage(string captcha)
        {
            int codeW = 80;
            int codeH = 30;
            int fontSize = 16;
            Color[] color = { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.DarkBlue, Color.CornflowerBlue };
            string[] font = { "Times New Roman" };
           
            //生成验证码字符串
            Random rnd = new Random(Guid.NewGuid().GetHashCode());

            //创建画布
            Bitmap bmp = new Bitmap(codeW, codeH);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.Linen);
 
            //画噪线
            for (int i = 0; i < 3; i++)
            {
                int x1 = rnd.Next(codeW);
                int y1 = rnd.Next(codeH);
                int x2 = rnd.Next(codeW);
                int y2 = rnd.Next(codeH);
 
                Color clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            //画验证码
            for (int i = 0; i < captcha.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, fontSize);
                Color clr = color[rnd.Next(color.Length)];
                g.DrawString(captcha[i].ToString(), ft, new SolidBrush(clr), (float)i * 18, (float)0);
            }
            //将验证码写入图片内存流中，以image/png格式输出
            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                return new CaptchaResult
                {
                    Captcha = captcha,
                    CaptchaBase64Data = ms.ToArray(),
                    Timestamp = DateTime.Now
                };
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                ms.Close();
                ms.Dispose();
                g.Dispose();
                bmp.Dispose();
            }
        }

        #region 绘制验证码

        ///// <summary>
        ///// 绘制验证码
        ///// </summary>
        //public static CaptchaResult CreateImage(string captcha, int letterWidth = 16, int letterHeight = 20)
        //{
        //    Random rand = new Random(Guid.NewGuid().GetHashCode());

        //    int intImageWidth = captcha.Length * letterWidth;
        //    Bitmap image = new Bitmap(intImageWidth, letterHeight);
        //    Graphics g = Graphics.FromImage(image);
        //    g.Clear(Color.White);
        //    for (int i = 0; i < 4; i++)
        //    {
        //        int x1 = rand.Next(image.Width - 1);
        //        int x2 = rand.Next(image.Width - 1);
        //        int y1 = rand.Next(image.Height - 1);
        //        int y2 = rand.Next(image.Height - 1);
        //        g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
        //    }

        //    int _x = -12;
        //    Font[] fonts = {
        //                    new Font(new FontFamily("Times New Roman"),10 +rand.Next(1),System.Drawing.FontStyle.Regular),
        //                    new Font(new FontFamily("Georgia"), 10 + rand.Next(1),System.Drawing.FontStyle.Regular),
        //                    new Font(new FontFamily("Arial"), 10 + rand.Next(1),System.Drawing.FontStyle.Regular),
        //                    new Font(new FontFamily("Comic Sans MS"), 10 + rand.Next(1),System.Drawing.FontStyle.Regular)
        //                    };
        //    for (int int_index = 0; int_index < captcha.Length; int_index++)
        //    {
        //        _x += rand.Next(12, 16);
        //        var _y = rand.Next(-2, 2);
        //        string str_char = captcha.Substring(int_index, 1);
        //        str_char = rand.Next(1) == 1 ? str_char.ToLower() : str_char.ToUpper();
        //        Brush newBrush = new SolidBrush(GetRandomColor());
        //        Point thePos = new Point(_x, _y);
        //        g.DrawString(str_char, fonts[rand.Next(fonts.Length - 1)], newBrush, thePos);
        //    }
        //    for (int i = 0; i < 10; i++)
        //    {
        //        int x = rand.Next(image.Width - 1);
        //        int y = rand.Next(image.Height - 1);
        //        image.SetPixel(x, y, Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)));
        //    }
        //    image = TwistImage(image, true, rand.Next(1, 3), rand.Next(4, 6));
        //    g.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, intImageWidth - 1, (letterHeight - 1));
        //    MemoryStream ms = new MemoryStream();

        //    image.Save(ms, ImageFormat.Png);

        //    return new CaptchaResult { Captcha = captcha, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };
        //}

        ///// <summary>
        ///// 字体随机颜色
        ///// </summary>
        //public static Color GetRandomColor()
        //{
        //    var randomNum = new Random(Guid.NewGuid().GetHashCode());
        //    int intRed = randomNum.Next(180);
        //    int intGreen = randomNum.Next(180);
        //    int intBlue = (intRed + intGreen > 300) ? 0 : 400 - intRed - intGreen;
        //    intBlue = (intBlue > 255) ? 255 : intBlue;
        //    return Color.FromArgb(intRed, intGreen, intBlue);
        //}

        ///// <summary>
        ///// 正弦曲线Wave扭曲图片
        ///// </summary>
        ///// <param name="srcBmp">图片路径</param>
        ///// <param name="bXDir">如果扭曲则选择为True</param>
        ///// <param name="dMultipleValue">波形的幅度倍数，越大扭曲的程度越高,一般为3</param>
        ///// <param name="dPhase">波形的起始相位,取值区间[0-2*PI)</param>
        //public static Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultipleValue, double dPhase)
        //{
        //    double PI = 6.283185307179586476925286766559;
        //    Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
        //    Graphics graph = Graphics.FromImage(destBmp);
        //    graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
        //    graph.Dispose();
        //    double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
        //    for (int i = 0; i < destBmp.Width; i++)
        //    {
        //        for (int j = 0; j < destBmp.Height; j++)
        //        {
        //            double dx = 0;
        //            dx = bXDir ? (PI * (double)j) / dBaseAxisLen : (PI * (double)i) / dBaseAxisLen;
        //            dx += dPhase;
        //            double dy = Math.Sin(dx);
        //            int nOldX = bXDir ? i + (int)(dy * dMultipleValue) : i;
        //            int nOldY = bXDir ? j : j + (int)(dy * dMultipleValue);

        //            Color color = srcBmp.GetPixel(i, j);
        //            if (nOldX >= 0 && nOldX < destBmp.Width
        //             && nOldY >= 0 && nOldY < destBmp.Height)
        //            {
        //                destBmp.SetPixel(nOldX, nOldY, color);
        //            }
        //        }
        //    }
        //    srcBmp.Dispose();
        //    return destBmp;
        //}
        #endregion
    }
}
