using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace AnimationLibrary
{
    public static class AnimationTextureExporter
    {
        private static Image source = null;

        /// <summary>
        /// 判断文件是否为图片
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsImageFile(string file)
        {
            Image img = Image.FromFile(file);
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Jpeg)) { return true; }
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Png)) { return true; }
            if (img.RawFormat.Equals(System.Drawing.Imaging.ImageFormat.Bmp)) { return true; }

            return false;
        }

        /// <summary>
        /// 裁剪图片
        /// </summary>
        /// <param name="file"></param>
        /// <param name="x1"></param>
        /// <param name="y2"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <returns></returns>
        public static Bitmap TextureSlicing(string file, int x1, int y2, int x2, int y1)
        {
            source = Image.FromFile(file);

            var w = x2 - x1;
            var h = y1 - y2;
            Bitmap output = new Bitmap(w, h);
            Graphics graphic = Graphics.FromImage(output);
            graphic.PageUnit = GraphicsUnit.Pixel;
            graphic.DrawImage(source, new RectangleF(0, 0, w, h), new RectangleF(x1, source.Height - y1, w, h), GraphicsUnit.Pixel);
            source.Dispose();
            return output;
        }
    }
}
