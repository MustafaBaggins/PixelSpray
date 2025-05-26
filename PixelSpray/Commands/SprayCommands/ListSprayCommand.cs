using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PixelSpray.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PixelSpray.Commands.SprayCommands
{
    public class ListSprayCommand : ICommand
    {
        public string Command { get; } = "list";
        public string[] Aliases { get; } = { "l", "ls" };
        public string Description { get; } = PixelSprayPlugin.Instance.Translation.SprayCommandDescription;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission("pixelspray.list"))
            {
                response = PixelSprayPlugin.Instance.Translation.NoPermission;
                return false;
            }

            string sprays = string.Empty;

            foreach (KeyValuePair<uint, Player> kvp in SprayManager.PlayerSprays)
            {
                string label = SprayManager.SpraysLabel.Where(kvp1 => kvp1.Value == kvp.Key).Select(kvp1 => kvp1.Key).First();
                sprays += string.Format(PixelSprayPlugin.Instance.Translation.SprayInformation, kvp.Key, label, kvp.Value.Nickname) + "\n";
            }

            response = $"{PixelSprayPlugin.Instance.Translation.SprayListHeader}\n{sprays}";
            return true;
        }
    }
}