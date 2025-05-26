using CommandSystem;
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

        private readonly AsciiArtConverter _converter = new AsciiArtConverter();

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

            Task.Run(async () =>
            {
                try
                {
                    string SprayFullCommand = await _converter.ProcessImageFromUrlAsync(imageUrl);

                    Timing.CallDelayed(0f, () =>
                    {
                        if (string.IsNullOrEmpty(SprayFullCommand))
                        {
                            player?.SendConsoleMessage(PixelSprayPlugin.Instance.Translation.SprayEmptyResult, "");
                            return;
                        }

                        SprayManager.AddPlayerSpray(player, label, SprayFullCommand);
                    });
                }
                catch (HttpRequestException httpEx)
                {
                    Timing.CallDelayed(0f, () =>
                    {
                        player?.SendConsoleMessage(string.Format(PixelSprayPlugin.Instance.Translation.ImageDownloadError, httpEx.Message?.ToString() ?? "Unknown"), "");
                    });
                }
                catch (Exception)
                {
                    Timing.CallDelayed(0f, () =>
                    {
                        player?.SendConsoleMessage(PixelSprayPlugin.Instance.Translation.SprayGeneralError, "");
                    });
                }
            });


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