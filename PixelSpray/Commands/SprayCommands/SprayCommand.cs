using AdminToys;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using Exiled.API.Features;
using Exiled.Permissions.Extensions;
using MEC;
using PixelSpray.API;
using PixelSpray.Features;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

namespace PixelSpray.Commands.SprayCommands
{
    public class SprayCommand : ICommand
    {
        public string Command { get; } = "spray";
        public string[] Aliases { get; } = { "grf", "s" };
        public string Description { get; } = PixelSprayPlugin.Instance.Translation.SprayCommandDescription;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Player.Get(sender);

            if (!player.CheckPermission("pixelspray.spray"))
            {
                response = PixelSprayPlugin.Instance.Translation.NoPermission;
                return false;
            }

            string label = arguments.ElementAtOrDefault(0);
            string imageUrl = arguments.ElementAtOrDefault(1);
            string time = arguments.ElementAtOrDefault(2);

            if (string.IsNullOrWhiteSpace(imageUrl) || string.IsNullOrWhiteSpace(label))
            {
                response = PixelSprayPlugin.Instance.Translation.Usage;
                return false;
            }

            string initialMessage = string.Format(PixelSprayPlugin.Instance.Translation.RequestReceived, imageUrl);

            player?.SendConsoleMessage(initialMessage, "");

            _ = SprayManager.AddPlayerSpray(player, label, imageUrl, null);

            if (!string.IsNullOrWhiteSpace(time))
            {
                if (float.TryParse(time, out float result))
                {
                    Timing.CallPeriodically(result, result, null, () => SprayManager.RemoveSprayByLabel(label));
                }
            }
            response = PixelSprayPlugin.Instance.Translation.SprayRequestBackground;
            return true;
        }
    }
}