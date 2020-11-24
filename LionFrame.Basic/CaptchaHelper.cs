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

        #region 绘制验证码

        /// <summary>
        /// 绘制验证码
        /// </summary>
        public static CaptchaResult CreateImage(string captcha, int letterWidth = 16, int letterHeight = 20)
        {
            Random rand = new Random(Guid.NewGuid().GetHashCode());

            int intImageWidth = captcha.Length * letterWidth;
            Bitmap image = new Bitmap(intImageWidth, letterHeight);
            Graphics g = Graphics.FromImage(image);
            g.Clear(Color.White);
            for (int i = 0; i < 4; i++)
            {
                int x1 = rand.Next(image.Width - 1);
                int x2 = rand.Next(image.Width - 1);
                int y1 = rand.Next(image.Height - 1);
                int y2 = rand.Next(image.Height - 1);
                g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
            }

            int _x = -12;
            Font[] fonts = {
                            new Font(new FontFamily("Times New Roman"),10 +rand.Next(1),System.Drawing.FontStyle.Regular),
                            new Font(new FontFamily("Georgia"), 10 + rand.Next(1),System.Drawing.FontStyle.Regular),
                            new Font(new FontFamily("Arial"), 10 + rand.Next(1),System.Drawing.FontStyle.Regular),
                            new Font(new FontFamily("Comic Sans MS"), 10 + rand.Next(1),System.Drawing.FontStyle.Regular)
                            };
            for (int int_index = 0; int_index < captcha.Length; int_index++)
            {
                _x += rand.Next(12, 16);
                var _y = rand.Next(-2, 2);
                string str_char = captcha.Substring(int_index, 1);
                str_char = rand.Next(1) == 1 ? str_char.ToLower() : str_char.ToUpper();
                Brush newBrush = new SolidBrush(GetRandomColor());
                Point thePos = new Point(_x, _y);
                g.DrawString(str_char, fonts[rand.Next(fonts.Length - 1)], newBrush, thePos);
            }
            for (int i = 0; i < 10; i++)
            {
                int x = rand.Next(image.Width - 1);
                int y = rand.Next(image.Height - 1);
                image.SetPixel(x, y, Color.FromArgb(rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255)));
            }
            image = TwistImage(image, true, rand.Next(1, 3), rand.Next(4, 6));
            g.DrawRectangle(new Pen(Color.LightGray, 1), 0, 0, intImageWidth - 1, (letterHeight - 1));
            MemoryStream ms = new MemoryStream();

            image.Save(ms, ImageFormat.Png);

            return new CaptchaResult { Captcha = captcha, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };
        }

        /// <summary>
        /// 字体随机颜色
        /// </summary>
        public static Color GetRandomColor()
        {
            var randomNum = new Random(Guid.NewGuid().GetHashCode());
            int intRed = randomNum.Next(180);
            int intGreen = randomNum.Next(180);
            int intBlue = (intRed + intGreen > 300) ? 0 : 400 - intRed - intGreen;
            intBlue = (intBlue > 255) ? 255 : intBlue;
            return Color.FromArgb(intRed, intGreen, intBlue);
        }

        /// <summary>
        /// 正弦曲线Wave扭曲图片
        /// </summary>
        /// <param name="srcBmp">图片路径</param>
        /// <param name="bXDir">如果扭曲则选择为True</param>
        /// <param name="dMultipleValue">波形的幅度倍数，越大扭曲的程度越高,一般为3</param>
        /// <param name="dPhase">波形的起始相位,取值区间[0-2*PI)</param>
        public static Bitmap TwistImage(Bitmap srcBmp, bool bXDir, double dMultipleValue, double dPhase)
        {
            double PI = 6.283185307179586476925286766559;
            Bitmap destBmp = new Bitmap(srcBmp.Width, srcBmp.Height);
            Graphics graph = Graphics.FromImage(destBmp);
            graph.FillRectangle(new SolidBrush(Color.White), 0, 0, destBmp.Width, destBmp.Height);
            graph.Dispose();
            double dBaseAxisLen = bXDir ? (double)destBmp.Height : (double)destBmp.Width;
            for (int i = 0; i < destBmp.Width; i++)
            {
                for (int j = 0; j < destBmp.Height; j++)
                {
                    double dx = 0;
                    dx = bXDir ? (PI * (double)j) / dBaseAxisLen : (PI * (double)i) / dBaseAxisLen;
                    dx += dPhase;
                    double dy = Math.Sin(dx);
                    int nOldX = bXDir ? i + (int)(dy * dMultipleValue) : i;
                    int nOldY = bXDir ? j : j + (int)(dy * dMultipleValue);

                    Color color = srcBmp.GetPixel(i, j);
                    if (nOldX >= 0 && nOldX < destBmp.Width
                     && nOldY >= 0 && nOldY < destBmp.Height)
                    {
                        destBmp.SetPixel(nOldX, nOldY, color);
                    }
                }
            }
            srcBmp.Dispose();
            return destBmp;
        }
        #endregion

        /// <summary>
        /// 得到验证码
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="captchaCode"></param>
        /// <returns></returns>
        public static CaptchaResult GenerateCaptcha(int width, int height, string captchaCode)
        {
            using (Bitmap baseMap = new Bitmap(width, height))
            {
                using (Graphics graph = Graphics.FromImage(baseMap))
                {
                    Random rand = new Random();

                    graph.Clear(GetRandomLightColor());

                    DrawCaptchaCode();
                    DrawDisorderLine();
                    AdjustRippleEffect();

                    MemoryStream ms = new MemoryStream();

                    baseMap.Save(ms, ImageFormat.Png);

                    return new CaptchaResult { Captcha = captchaCode, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };

                    int GetFontSize(int imageWidth, int captchCodeCount)
                    {
                        var averageSize = imageWidth / captchCodeCount;

                        return Convert.ToInt32(averageSize);
                    }

                    Color GetRandomDeepColor()
                    {
                        int redlow = 160, greenLow = 100, blueLow = 160;
                        return Color.FromArgb(rand.Next(redlow), rand.Next(greenLow), rand.Next(blueLow));
                    }

                    Color GetRandomLightColor()
                    {
                        int low = 180, high = 255;

                        int nRend = rand.Next(high) % (high - low) + low;
                        int nGreen = rand.Next(high) % (high - low) + low;
                        int nBlue = rand.Next(high) % (high - low) + low;

                        return Color.FromArgb(nRend, nGreen, nBlue);
                    }

                    void DrawCaptchaCode()
                    {
                        SolidBrush fontBrush = new SolidBrush(Color.Black);
                        int fontSize = GetFontSize(width, captchaCode.Length);
                        Font font = new Font(FontFamily.GenericSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                        for (int i = 0; i < captchaCode.Length; i++)
                        {
                            fontBrush.Color = GetRandomDeepColor();

                            int shiftPx = fontSize / 6;

                            //float x = i * fontSize + rand.Next(-shiftPx, shiftPx) + rand.Next(-shiftPx, shiftPx);
                            float x = i * fontSize + rand.Next(-shiftPx, shiftPx) / 2;
                            //int maxY = height - fontSize;
                            int maxY = height - fontSize * 2;
                            if (maxY < 0)
                            {
                                maxY = 0;
                            }
                            float y = rand.Next(0, maxY);

                            graph.DrawString(captchaCode[i].ToString(), font, fontBrush, x, y);
                        }
                    }

                    void DrawDisorderLine()
                    {
                        Pen linePen = new Pen(new SolidBrush(Color.Black), 2);
                        for (int i = 0; i < rand.Next(2, 6); i++)
                        //for (int i = 0; i < 2; i++)
                        {
                            linePen.Color = GetRandomDeepColor();

                            Point startPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                            Point endPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                            graph.DrawLine(linePen, startPoint, endPoint);
                        }
                    }

                    void AdjustRippleEffect()
                    {
                        short nWave = 6;
                        int nWidth = baseMap.Width;
                        int nHeight = baseMap.Height;

                        Point[,] pt = new Point[nWidth, nHeight];

                        for (int x = 0; x < nWidth; ++x)
                        {
                            for (int y = 0; y < nHeight; ++y)
                            {
                                var xo = nWave * Math.Sin(2.0 * 3.1415 * y / 128.0);
                                var yo = nWave * Math.Cos(2.0 * 3.1415 * x / 128.0);

                                var newX = x + xo;
                                var newY = y + yo;

                                if (newX > 0 && newX < nWidth)
                                {
                                    pt[x, y].X = (int)newX;
                                }
                                else
                                {
                                    pt[x, y].X = 0;
                                }


                                if (newY > 0 && newY < nHeight)
                                {
                                    pt[x, y].Y = (int)newY;
                                }
                                else
                                {
                                    pt[x, y].Y = 0;
                                }
                            }
                        }

                        Bitmap bSrc = (Bitmap)baseMap.Clone();

                        BitmapData bitmapData = baseMap.LockBits(new Rectangle(0, 0, baseMap.Width, baseMap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                        BitmapData bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                        int scanline = bitmapData.Stride;

                        IntPtr scan0 = bitmapData.Scan0;
                        IntPtr srcScan0 = bmSrc.Scan0;

                        unsafe
                        {
                            byte* p = (byte*)(void*)scan0;
                            byte* pSrc = (byte*)(void*)srcScan0;

                            int nOffset = bitmapData.Stride - baseMap.Width * 3;

                            for (int y = 0; y < nHeight; ++y)
                            {
                                for (int x = 0; x < nWidth; ++x)
                                {
                                    var xOffset = pt[x, y].X;
                                    var yOffset = pt[x, y].Y;

                                    if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                                    {
                                        if (pSrc != null)
                                        {
                                            p[0] = pSrc[yOffset * scanline + xOffset * 3];
                                            p[1] = pSrc[yOffset * scanline + xOffset * 3 + 1];
                                            p[2] = pSrc[yOffset * scanline + xOffset * 3 + 2];
                                        }
                                    }

                                    p += 3;
                                }
                                p += nOffset;
                            }
                        }

                        baseMap.UnlockBits(bitmapData);
                        bSrc.UnlockBits(bmSrc);
                        bSrc.Dispose();
                    }
                }
            }
        }
    }
}
