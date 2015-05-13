using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace weixin
{
   public  class ImageOpreate
    {
        /// <summary>
        /// 压缩图片
        /// </summary>
        /// <param name="imageStream">输入</param>
        /// <param name="quality">压缩质量</param>
        /// <param name="maxKb">最大大小（单位kb）只供参考</param>
        public static Stream CompressImage(Stream imageStream, int quality =100, int maxKb = 2000)
        {
            if (imageStream.Length > maxKb * 1024)
            {
                if (imageStream.CanSeek)
                {
                    imageStream.Seek(0, SeekOrigin.Begin);
                }
                var bitmapImage = new BitmapImage();
                bitmapImage.SetSource(imageStream);
                var writeableBitmap = new WriteableBitmap(bitmapImage);
                var tempStreamm = new MemoryStream();
                var size = GetSize(bitmapImage.PixelWidth, bitmapImage.PixelHeight);
                writeableBitmap.SaveJpeg(tempStreamm, (int)size.Width, (int)size.Height, 0, quality);
                return tempStreamm;
            }
            return imageStream;
        }


        private static Size GetSize(Size size)
        {
            return GetSize(size.Width, size.Height);
        }


        private static Size GetSize(double width, double height)
        {
            if (width > height)
            {
                if (width > 2000)
                {
                    return new Size(2000, Convert.ToInt32(height * 2000/ width));
                }
                return new Size(width, height);
            }
            else
            {
                if (height > 2000)
                {
                    return new Size(Convert.ToInt32(width * 2000 / height), 2000);
                }
                return new Size(width, height);
            }
        }
    }
}
