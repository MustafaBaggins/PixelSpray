using System;
using Exiled.API.Features;
namespace PixelSpray
{
    public class Class1 : Plugin<Config, Translations>
    {
        public override string Author => "Baggins(@haci33)";
        public override string Name => "PixelSpray";
        public override Version Version => new Version(1, 0, 0);

        public static Class1 Instance;
        public override void OnEnabled()
        {
            Instance = this;
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;
            base.OnDisabled();
        }
    }
}
