using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Brew.HacPack
{
    public static class ControlIcons
    {
        public static void generate(string Icon, string OutDir)
        {
            if(!File.Exists(Icon)) return;
            if(!Directory.Exists(OutDir)) return;
            Bitmap basebmp = new Bitmap(Icon);
            Bitmap dest = new Bitmap(256, 256);
            Rectangle rec = new Rectangle(0, 0, 256, 256);
            if(basebmp.Height != 256 || basebmp.Width != 256)
            {
                dest.SetResolution(basebmp.HorizontalResolution, basebmp.VerticalResolution);
                Graphics gfx = Graphics.FromImage(dest);
                gfx.CompositingMode = CompositingMode.SourceCopy;
                gfx.CompositingQuality = CompositingQuality.HighQuality;
                gfx.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gfx.SmoothingMode = SmoothingMode.HighQuality;
                gfx.PixelOffsetMode = PixelOffsetMode.HighQuality;
                ImageAttributes attr = new ImageAttributes();
                attr.SetWrapMode(WrapMode.TileFlipXY);
                gfx.DrawImage(basebmp, rec, 0, 0, basebmp.Width, basebmp.Height, GraphicsUnit.Pixel, attr);
            }
            else
            {
                Graphics gfx = Graphics.FromImage(dest);
                gfx.DrawImage(basebmp, rec);
            }
            dest.Save(OutDir + "\\icon_AmericanEnglish.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_BritishEnglish.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Japanese.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_French.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_German.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_LatinAmericanSpanish.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Spanish.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Italian.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Dutch.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_CanadianFrench.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Portuguese.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Russian.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Korean.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Taiwanese.dat", ImageFormat.Jpeg);
            dest.Save(OutDir + "\\icon_Chinese.dat", ImageFormat.Jpeg);
        }
    }
}
