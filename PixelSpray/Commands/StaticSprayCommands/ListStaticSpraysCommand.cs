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

namespace PixelSpray.Commands.StaticSprayCommands
{
    public class StaticListSprayCommand : ICommand
    {
        public string Command { get; } = "list";
        public string[] Aliases { get; } = { "l", "ls" };
        public string Description { get; } = PixelSprayPlugin.Instance.Translation.SprayCommandDescription;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission("pixelspray.static"))
            {
                response = PixelSprayPlugin.Instance.Translation.NoPermission;
                return false;
            }

            string sprays = string.Empty;

            foreach (KeyValuePair<string, string> kvp in StaticSpraysHandler.StaticSprays)
            {
                sprays += $"Spray: {kvp.Key}" + "\n";
            }

            response = $"Currently Available Sprays:\n{sprays}";
            return true;
        }
    }
}
