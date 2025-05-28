using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;

namespace PixelSpray.Extensions
{
    public static class ImageExtensions
    {
        public static void ResizeImage(this Image<Rgba32> image)
        {
            int originalWidth = image.Width;
            int originalHeight = image.Height;

            if (originalWidth == 0 || originalHeight == 0)
            {
                throw new InvalidOperationException("The width or height of the image cannot be zero.");
            }

            int newHeightBasedOnWidth = (int)Math.Round((double)originalHeight * PixelSprayPlugin.Instance.Config.DefaultImageWidth / originalWidth);

            if (newHeightBasedOnWidth == 0 && originalHeight > 0)
            {
                newHeightBasedOnWidth = 1;
            }

            image.Mutate(x => x.Resize(PixelSprayPlugin.Instance.Config.DefaultImageWidth, newHeightBasedOnWidth == 0 ? 1 : newHeightBasedOnWidth));

            double aspectRatioScaleY = "other" == "ie" ? 0.65 : 0.43;

            int finalHeight = (int)Math.Round(image.Height * aspectRatioScaleY);

            if (finalHeight == 0 && image.Height > 0)
            {
                finalHeight = 1;
            }

            if (image.Width > 0 && finalHeight > 0)
            {
                image.Mutate(x => x.Resize(image.Width, finalHeight));
            }
                
            if (finalHeight == 0)
            {
                image.Mutate(x => x.Resize(image.Width, 1));
            }
        }

        public static void SetGrayScaleMode(this Image<Rgba32> image, int grayscale)
        {
            switch (grayscale)
            {
                case 1:
                    image.Mutate(x => x.Grayscale());
                    break;
                case 2:
                    image.Mutate(x => x.Grayscale().BinaryThreshold(0.5f));
                    break;
                default:
                    break;
            }
        }
    }
}
