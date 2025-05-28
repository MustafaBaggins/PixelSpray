using AdminToys;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PixelSpray.API;
using PixelSpray.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Networking;

namespace PixelSpray.Commands.StaticSprayCommands
{
    public class StaticSprayCommand : ICommand
    {
        public string Command { get; } = "spray";
        public string[] Aliases { get; } = { "s" };
        public string Description { get; } = PixelSprayPlugin.Instance.Translation.SprayCommandDescription;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission("pixelspray.spray"))
            {
                response = PixelSprayPlugin.Instance.Translation.NoPermission;
                return false;
            }

            string input = arguments.ElementAtOrDefault(0);

            if (!StaticSpraysHandler.StaticSprays.TryGetValue(input, out string convertedImage))
            {
                response = PixelSprayPlugin.Instance.Translation.LabelNotFound;
                return false;
            }

            SprayManager.AddPlayerSprayByConvertedImage(player, input, convertedImage);

            response = $"{PixelSprayPlugin.Instance.Translation.SprayCommandSent} - {input}.";
            return false;
        }
    }
}
