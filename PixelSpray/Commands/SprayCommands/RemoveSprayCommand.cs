using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PixelSpray.API;
using System;
using System.Linq;

namespace PixelSpray.Commands.SprayCommands
{
    public class RemoveSprayCommand : ICommand
    {
        public string Command { get; } = "remove";
        public string[] Aliases { get; } = { "clean", "cl", "rm", "c" };
        public string Description { get; } = PixelSprayPlugin.Instance.Translation.SprayCommandDescription;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission("pixelspray.remove"))
            {
                response = PixelSprayPlugin.Instance.Translation.NoPermission;
                return false;
            }

            string input = arguments.ElementAtOrDefault(0);

            if (string.IsNullOrWhiteSpace(input))
            {
                response = PixelSprayPlugin.Instance.Translation.Usage;
                return false;
            }

            if (input.ToLower() == "all")
            {
                foreach (uint id in SprayManager.PlayerSprays.Keys.ToList())
                {
                    SprayManager.RemoveSprayById(id);
                }
                response = string.Format(PixelSprayPlugin.Instance.Translation.RemovedSprays, "All");
                return true;
            }

            if (uint.TryParse(input, out uint result))
            {
                if (!SprayManager.RemoveSprayById(result))
                {
                    response = PixelSprayPlugin.Instance.Translation.IdNotFound;
                    return false;
                }
                response = string.Format(PixelSprayPlugin.Instance.Translation.RemovedSprays, "1");
                return true;
            }

            if (!Player.TryGet(input, out Player p))
            {
                if (SprayManager.RemoveSprayByLabel(input))
                {
                    response = string.Format(PixelSprayPlugin.Instance.Translation.RemovedSprays, "1"); ;
                    return true;
                }
            }

            int spraysRemoved = SprayManager.RemoveSprayByPlayer(p);

            response = string.Format(PixelSprayPlugin.Instance.Translation.RemovedSprays, spraysRemoved);
            return true;
        }
    }
}