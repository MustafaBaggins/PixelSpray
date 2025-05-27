using Exiled.API.Features.Core.UserSettings;
using System.Collections.Generic;
using UnityEngine;


namespace PixelSpray.Features
{
    public static class SSMenu
    {
        public static IEnumerable<SettingBase> PixelSpraySettings { get; private set; }

        public static TextInputSetting ImageUrlInput { get; set; }
        public static KeybindSetting SprayKeybind { get; set; }
        public static ButtonSetting SprayButton { get; set; }
        public static void RegisterSettings()
        {
        }
        public static void UnregisterSettings()
        {
            SettingBase.Unregister(settings: PixelSpraySettings);
        }
    }
}
