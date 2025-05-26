using Exiled.API.Interfaces;


namespace PixelSpray.Configuration
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        public int DefaultImageWidth { get; set; } = 50;
    }
}