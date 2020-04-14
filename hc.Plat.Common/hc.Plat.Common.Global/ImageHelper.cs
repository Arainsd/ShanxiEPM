using System;
using System.Collections;

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace hc.Plat.Common.Global
{
    /// <summary>
    /// 图片帮助类
    /// </summary>
    public class ImageHelper
    {
        /// <summary>
        /// 生成缩略图方式
        /// </summary>
        public enum ImageCutMode
        {
            /// <summary>
            /// 指定高宽缩放(可能变形)
            /// </summary>
            Hw,

            /// <summary>
            /// 指定宽，高按比例
            /// </summary>
            W,

            /// <summary>
            /// 指定高，宽按比例
            /// </summary>
            H,

            /// <summary>
            /// 指定高宽裁剪(不变形)
            /// </summary>
            Cut
        }

        /// <summary>
        /// 判断是否是图片
        /// </summary>
        /// <param name="fileExt">文件后缀名</param>
        /// <returns></returns>
        public static bool IsImage(string fileExt)
        {
            ArrayList al = new ArrayList { "bmp", "jpeg", "jpg", "gif", "png" };

            if (fileExt.StartsWith("."))
            {
                fileExt = fileExt.Substring(1);
            }

            return al.Contains(fileExt.ToLower());
        }

        #region 生成缩略图

        /// <summary>
        /// 生成缩略图
        /// </summary>
        /// <param name="originalImagePath">原始图片路径(绝对路径)</param>
        /// <param name="thumbnailPath">缩略图存储路径(绝对路径)</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="mode">生成缩略图方式</param>
        public static void MakeThumbnailImage(string originalImagePath, string thumbnailPath, int width, int height, ImageCutMode mode)
        {
            Image originalImage = Image.FromFile(originalImagePath);
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode)
            {
                case ImageCutMode.Hw:
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    else
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    break;
                case ImageCutMode.W:
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case ImageCutMode.H:
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case ImageCutMode.Cut:
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片
            Bitmap bitmap = new Bitmap(towidth, toheight);
            try
            {
                //新建一个画板
                Graphics g = Graphics.FromImage(bitmap);
                //设置高质量插值法
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                //设置高质量,低速度呈现平滑程度
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                //清空画布并以透明背景色填充
                g.Clear(Color.White);
                //g.Clear(Color.Transparent);
                //在指定位置并且按指定大小绘制原图片的指定部分
                g.DrawImage(originalImage, new Rectangle(0, 0, towidth, toheight), new Rectangle(x, y, ow, oh), GraphicsUnit.Pixel);

                SaveImage(bitmap, thumbnailPath, GetCodecInfo("image/" + GetFormat(thumbnailPath).ToString().ToLower()));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
            }
        }

        #endregion

        #region 压缩图片

        
        /// <summary>
        /// 压缩图片(等比例压缩)
        /// </summary>
        /// <param name="oldFile">原图片地址(绝对路径)</param>
        /// <param name="newFile">压缩后保存图片地址(绝对路径)</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="maxSize">压缩后图片的最大大小</param>
        /// <returns></returns>
        public static void CompressImage(string oldFile, string newFile, int flag = 90, int maxSize = 1000)
        {
            Image oldImage = Image.FromFile(oldFile);
            CompressImage(oldImage, newFile, oldImage.Width / 2, oldImage.Height / 2, flag, maxSize);
        }

        /// <summary>
        /// 按指定大小压缩图片
        /// </summary>
        /// <param name="oldFile">原图片地址(绝对路径)</param>
        /// <param name="newFile">压缩后保存图片地址(绝对路径)</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="maxSize">压缩后图片的最大大小</param>
        public static void CompressImageSize(string oldFile, string newFile, int width, int height, int flag = 90, int maxSize = 1000)
        {
            Image oldImage = Image.FromFile(oldFile);
            CompressImage(oldImage, newFile, width, height, flag, maxSize);
        }

        /// <summary>
        /// 按指定大小压缩图片
        /// </summary>
        /// <param name="oldImage">原图片</param>
        /// <param name="newFile">压缩后保存图片地址(绝对路径)</param>
        /// <param name="width">指定宽度</param>
        /// <param name="height">指定高度</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="maxSize">压缩后图片的最大大小</param>
        private static void CompressImage(Image oldImage, string newFile, int width, int height, int flag = 90, int maxSize = 1000)
        {
            ImageFormat tFormat = oldImage.RawFormat;

            if (width > oldImage.Width)
            {
                width = oldImage.Width / 2;
            }
            if (height > oldImage.Height)
            {
                height = oldImage.Height / 2;
            }

            int newWidth = width;
            int newHeight = height;
            int tempWidth;
            int tempHeight;

            //按比例缩放
            Size newSize = new Size(oldImage.Width, oldImage.Height);
            if (newSize.Width > newHeight || newSize.Width > newWidth)
            {
                if ((newSize.Width * newHeight) > (newSize.Width * newWidth))
                {
                    tempWidth = newWidth;
                    tempHeight = (newWidth * newSize.Height) / newSize.Width;
                }
                else
                {
                    tempHeight = newHeight;
                    tempWidth = (newSize.Width * newHeight) / newSize.Height;
                }
            }
            else
            {
                tempWidth = newSize.Width;
                tempHeight = newSize.Height;
            }

            Bitmap ob = new Bitmap(newWidth, newHeight);
            Graphics g = Graphics.FromImage(ob);

            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(oldImage, new Rectangle((newWidth - tempWidth) / 2, (newHeight - tempHeight) / 2, tempWidth, tempHeight), 0, 0, oldImage.Width, oldImage.Height, GraphicsUnit.Pixel);

            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                ImageCodecInfo[] arrayIci = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegIcIinfo = null;
                for (int x = 0; x < arrayIci.Length; x++)
                {
                    if (arrayIci[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegIcIinfo = arrayIci[x];
                        break;
                    }
                }
                if (jpegIcIinfo != null)
                {
                    ob.Save(newFile, jpegIcIinfo, ep);//dFile是压缩后的新路径
                    FileInfo fi = new FileInfo(newFile);
                    if (fi.Length > 1024 * maxSize)
                    {
                        flag = flag - 10;
                        CompressImage(oldImage, newFile, width, height, flag, maxSize);
                    }
                }
                else
                {
                    ob.Save(newFile, tFormat);
                }
            }
            finally
            {
                oldImage.Dispose();
                ob.Dispose();
            }
        }


        #endregion


        /// <summary>
        /// 得到图片格式
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <returns></returns>
        public static ImageFormat GetFormat(string name)
        {
            string ext = name.Substring(name.LastIndexOf(".", StringComparison.Ordinal) + 1);
            switch (ext.ToLower())
            {
                case "jpg":
                case "jpeg":
                    return ImageFormat.Jpeg;
                case "bmp":
                    return ImageFormat.Bmp;
                case "png":
                    return ImageFormat.Png;
                case "gif":
                    return ImageFormat.Gif;
                default:
                    return ImageFormat.Jpeg;
            }
        }



        /// <summary>
        /// 获取图像编码解码器的所有相关信息
        /// </summary>
        /// <param name="mimeType">包含编码解码器的多用途网际邮件扩充协议 (MIME) 类型的字符串</param>
        /// <returns>返回图像编码解码器的所有相关信息</returns>
        private static ImageCodecInfo GetCodecInfo(string mimeType)
        {
            ImageCodecInfo[] codecInfo = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in codecInfo)
            {
                if (ici.MimeType == mimeType)
                    return ici;
            }
            return null;
        }


        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="image">Image 对象</param>
        /// <param name="savePath">保存路径</param>
        /// <param name="ici">指定格式的编解码参数</param>
        private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
        {
            //设置 原图片 对象的 EncoderParameters 对象
            EncoderParameters parameters = new EncoderParameters(1);
            parameters.Param[0] = new EncoderParameter(Encoder.Quality, ((long)100));
            image.Save(savePath, ici, parameters);
            parameters.Dispose();
        }

    }
}
