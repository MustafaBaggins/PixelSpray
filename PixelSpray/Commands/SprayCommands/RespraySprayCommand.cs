using AdminToys;
using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using PixelSpray.API;
using System;
using System.Linq;
using Utils.Networking;

namespace PixelSpray.Commands.SprayCommands
{
    public class RespraySprayCommand : ICommand
    {
        public string Command { get; } = "respray";
        public string[] Aliases { get; } = { "r", "recreate" };
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

            if (!uint.TryParse(input, out uint id))
            {
                if (!SprayManager.SpraysLabel.TryGetValue(input, out id))
                {
                    response = PixelSprayPlugin.Instance.Translation.LabelNotFound;
                    return false;
                }
            }

            if (!NetworkUtils.SpawnedNetIds.TryGetValue(id, out var value) || !value.TryGetComponent<TextToy>(out var component))
            {
                response = PixelSprayPlugin.Instance.Translation.IdNotFound;
                return false;
            }

            string label = SprayManager.SpraysLabel.Where(kvp1 => kvp1.Value == id).Select(kvp1 => kvp1.Key).First();

            _ = SprayManager.AddPlayerSprayByUrl(player, label, component.TextFormat, null);

            response = $"{PixelSprayPlugin.Instance.Translation.SprayCommandSent} - {input}.";
            return false;
        }
    }
}