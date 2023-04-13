using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;

namespace TAO.Photos.Functions
{
    public static class PhotosResizer
    {
        [FunctionName("PhotosResizer")]
        public static async Task Run([BlobTrigger("photos/{name}", Connection = Literals.Literals.StorageConnectionString)] Stream myBlob,
            [Blob("photos-small/{name}", FileAccess.Write, Connection = Literals.Literals.StorageConnectionString)] Stream imageSmall,
            [Blob("photos-medium/{name}", FileAccess.Write, Connection = Literals.Literals.StorageConnectionString)] Stream imageMedium,
            ILogger logger)
        {
            logger?.LogInformation("Resizing...");

            try
            {
                using var msMedium = CreateMemoryStream(myBlob, ImageSize.Medium);
                await msMedium.CopyToAsync(imageMedium);

                using var msSmall = CreateMemoryStream(myBlob, ImageSize.Small);
                await msSmall.CopyToAsync(imageSmall);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
            }
            finally
            {
                imageSmall.Close();
                imageMedium.Close();
            }
        }

        private static MemoryStream CreateMemoryStream(Stream image, ImageSize imageSize)
        {
            var ms = new MemoryStream();
            var img = Image.FromStream(image);
            var desiredWidth = imageSize == ImageSize.Medium ? img.Width / 2 :
                                                        img.Width / 4;
            var ratio = (decimal)desiredWidth / img.Width;
            var resized = ResizeImage(img, desiredWidth, (int)Math.Floor((img.Height * ratio)));
            resized.Save(ms, ImageFormat.Jpeg);
            ms.Position = 0;
            return ms;
        }

        private static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private enum ImageSize
        {
            Medium,
            Small
        }
    }
}
