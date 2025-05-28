using Exiled.API.Features;
using PixelSpray.Configuration;
using PixelSpray.Features;
using System;

namespace PixelSpray
{
    public class PixelSprayPlugin : Plugin<Config, Translation>
    {
        public override string Author => "Baggins(@haci33)";
        public override string Name => "PixelSpray";
        public override string Prefix => "pixel_spray";
        public override Version Version => new Version(0, 0, 2);

        public static PixelSprayPlugin Instance;
        public override void OnEnabled()
        {
            Instance = this;
            StaticSpraysHandler.RegisterStaticSpraysFromFolder();
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();
        }
    }
}
