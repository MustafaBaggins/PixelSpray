using Exiled.API.Features;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PixelSpray.Features
{
    public class AsciiArtConverter
    {
        private static readonly List<string> DefaultHtmlCharacter = new List<string> { "█" };
        private const string BrowserTypeForAspectRatio = "other";
        private const int GrayscaleMode = 0;
        private const string OriginalFontHtmlStartTag = "<b style=\"color:";
        private const string OriginalFontHtmlEndTag = "</b>";

        private class ConverterState
        {
            public int TextTypeCount { get; set; } = -1;
        }

        private static string NextCharacter(List<string> htmlCharacter, ConverterState state)
        {
            if (htmlCharacter.Count == 1) return htmlCharacter[0];
            state.TextTypeCount++;
            if (state.TextTypeCount >= htmlCharacter.Count) state.TextTypeCount = 0;
            return htmlCharacter[state.TextTypeCount];
        }

        public async Task<string> ProcessImageFromUrlAsync(string imageUrl)
        {
            byte[] imageBytes;

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(20);
                imageBytes = await httpClient.GetByteArrayAsync(imageUrl).ConfigureAwait(false);
            }

            string rawHtmlOutput = ConvertImageToHtml(imageBytes);
            if (string.IsNullOrEmpty(rawHtmlOutput))
                throw new Exception("C# based converter did not produce output.");



            string finalResult = FormatHtmlOutput(rawHtmlOutput);

            return finalResult;
        }

        private string ConvertImageToHtml(byte[] imageData)
        {
            StringBuilder htmlBuilder = new StringBuilder();
            ConverterState state = new ConverterState();
            Image<Rgba32> image;
            try
            {
                image = Image.Load<Rgba32>(imageData);
            }
            catch (Exception ex)
            {
                Log.Error($"AsciiArtConverter Error (ConvertImageToHtml - Image.Load): {ex.Message}\n{ex.StackTrace}");
                throw;
            }

            using (image)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                if (originalWidth == 0 || originalHeight == 0)
                    throw new InvalidOperationException("The width or height of the image cannot be zero.");

                int newHeightBasedOnWidth = (int)Math.Round((double)originalHeight * PixelSprayPlugin.Instance.Config.DefaultImageWidth / originalWidth);
                if (newHeightBasedOnWidth == 0 && originalHeight > 0) newHeightBasedOnWidth = 1;

                image.Mutate(x => x.Resize(PixelSprayPlugin.Instance.Config.DefaultImageWidth, newHeightBasedOnWidth == 0 ? 1 : newHeightBasedOnWidth));

                double aspectRatioScaleY = BrowserTypeForAspectRatio == "ie" ? 0.65 : 0.43;
                int finalHeight = (int)Math.Round(image.Height * aspectRatioScaleY);
                if (finalHeight == 0 && image.Height > 0) finalHeight = 1;

                if (image.Width > 0 && finalHeight > 0)
                    image.Mutate(x => x.Resize(image.Width, finalHeight));
                else if (finalHeight == 0)
                {

                    image.Mutate(x => x.Resize(image.Width, 1));
                }

                if (GrayscaleMode == 1) image.Mutate(x => x.Grayscale());
                else if (GrayscaleMode == 2) image.Mutate(x => x.Grayscale().BinaryThreshold(0.5f));

                int width = image.Width;
                int height = image.Height;
                if (width == 0 || height == 0)
                    throw new InvalidOperationException("Image dimensions became zero after processing.");

                Rgba32? oldColour = null;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Rgba32 currentColour = image[x, y];
                        if (oldColour == null || currentColour.R != oldColour.Value.R || currentColour.G != oldColour.Value.G || currentColour.B != oldColour.Value.B)
                        {
                            string htmlColor = $"#{currentColour.R:x2}{currentColour.G:x2}{currentColour.B:x2}";
                            if (x != 0 && oldColour != null)
                                htmlBuilder.Append(OriginalFontHtmlEndTag);
                            htmlBuilder.Append($"{OriginalFontHtmlStartTag}{htmlColor}\">{NextCharacter(DefaultHtmlCharacter, state)}");
                        }
                        else
                        {
                            htmlBuilder.Append(NextCharacter(DefaultHtmlCharacter, state));
                        }
                        oldColour = currentColour;
                    }
                    if (oldColour != null)
                        htmlBuilder.Append(OriginalFontHtmlEndTag);
                    htmlBuilder.AppendLine();
                    oldColour = null;
                }
            }
            return htmlBuilder.ToString();
        }

        private string FormatHtmlOutput(string htmlInput)
        {
            string text = htmlInput;
            text = text.Replace(OriginalFontHtmlStartTag, "<color=");
            text = text.Replace(OriginalFontHtmlEndTag, "</color>");
            text = text.Replace("\"", "");
            string[] lines = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            string joinedLinesContent = string.Join("\\n", lines);
            string finalPrefix = "spawntoy text <size=0.75><line-height=100%><mspace=0.56em>";
            string finalTextCommand = finalPrefix + joinedLinesContent;
            return finalTextCommand;
        }
    }
}