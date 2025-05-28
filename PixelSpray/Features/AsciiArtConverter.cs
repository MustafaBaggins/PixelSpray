using Exiled.API.Features;
using PixelSpray.Extensions;
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
        public static readonly List<string> DefaultHtmlCharacter = new List<string> { "█" };
        public static int GrayscaleMode = 0;
        public static string OriginalFontHtmlStartTag = "<b style=\"color:";
        public static string OriginalFontHtmlEndTag = "</b>";

        private class ConverterState
        {
            public int TextTypeCount { get; set; } = -1;
        }

        public static async Task<string> ProcessImageFromUrl(string imageUrl)
        {
            byte[] imageBytes;

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(20);
                imageBytes = await httpClient.GetByteArrayAsync(imageUrl).ConfigureAwait(false);
            }

            return ProcessImageFromBytes(imageBytes);
        }

        public static string ProcessImageFromBytes(byte[] imageBytes)
        {
            string rawHtmlOutput = ConvertImageToHtml(imageBytes);

            if (string.IsNullOrEmpty(rawHtmlOutput))
            {
                throw new Exception("C# based converter did not produce output.");
            }

            return FormatHtmlOutput(rawHtmlOutput);
        }


        private static string ConvertImageToHtml(byte[] imageData)
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
                image.ResizeImage();
                image.SetGrayScaleMode(GrayscaleMode);

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

        private static string NextCharacter(List<string> htmlCharacter, ConverterState state)
        {
            if (htmlCharacter.Count == 1) return htmlCharacter[0];
            state.TextTypeCount++;
            if (state.TextTypeCount >= htmlCharacter.Count) state.TextTypeCount = 0;
            return htmlCharacter[state.TextTypeCount];
        }

        private static string FormatHtmlOutput(string htmlInput)
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