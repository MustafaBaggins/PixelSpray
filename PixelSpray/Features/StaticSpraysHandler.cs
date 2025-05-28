using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PixelSpray.Features
{
    public class StaticSpraysHandler
    {
        public static Dictionary<string, string> StaticSprays { get; private set; } = new Dictionary<string, string>();

        public static async Task RegisterStaticSprayFromUrl(string label, string imageUrl)
        {
            if (string.IsNullOrEmpty(label) || string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentException("Label and image URL cannot be null or empty.");
            }
            string convertedImage = await AsciiArtConverter.ProcessImageFromUrl(imageUrl);
            if (!StaticSprays.ContainsKey(label))
            {
                StaticSprays[label] = imageUrl; 
            }
        }

        public static void RegisterStaticSpraysFromFolder()
        {
            string configPath = LoaderPlugin.Config.ConfigType == ConfigType.Default ? Paths.Configs : Paths.IndividualConfigs;
            string imagePath = Path.Combine(configPath, PixelSprayPlugin.Instance.Prefix, "Images");

            if (!Directory.Exists(imagePath))
            {
                Directory.CreateDirectory(imagePath);
            }

            foreach (string file in Directory.GetFiles(imagePath, "*.png"))
            {
                string label = Path.GetFileNameWithoutExtension(file);
                byte[] imageBytes = File.ReadAllBytes(file);

                string formatedImage = AsciiArtConverter.ProcessImageFromBytes(imageBytes);

                if (!StaticSprays.ContainsKey(label))
                {
                    StaticSprays.Add(label, formatedImage);
                }
            }
        }
    }
}
