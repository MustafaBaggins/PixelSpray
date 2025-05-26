using CommandSystem;
using Exiled.API.Features;
using Exiled.Permissions.Extensions; 
using System;
using System.Linq;
using System.Net.Http; 
using System.Threading.Tasks;
using MEC;

namespace PixelSpray
{
    [CommandHandler(typeof(ClientCommandHandler))]
    public class Spray : ICommand
    {
        public string Command { get; } = "spray";
        public string[] Aliases { get; } = { "grf" };
        public string Description { get; } = Class1.Instance.Translation.SprayCommandDescription;

        private readonly AsciiArtConverter _converter = new AsciiArtConverter();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            var t = Class1.Instance.Translation;
            Player player = Player.Get(sender);

            if (!player.CheckPermission("pixel_spray_spray"))
            {
                response = t.NoPermission;
                return false;
            }

            string imageUrl = arguments.ElementAtOrDefault(0);
            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                response = t.Usage;
                return false;
            }

            string initialMessage = string.Format(t.RequestReceived, imageUrl);
            if (player != null) player.SendConsoleMessage(initialMessage, "green");


            Task.Run(async () =>
            {
                try
                {
                    
                    string graffitiFullCommand = await _converter.ProcessImageFromUrlAsync(imageUrl);

                    Timing.CallDelayed(0f, () =>
                    {
                        if (string.IsNullOrEmpty(graffitiFullCommand))
                        {
                            if (player != null) player.SendConsoleMessage(t.GraffitiEmptyResult, "red");
                            return;
                        }

                        try
                        {
                            
                            foreach (var command in RemoteAdmin.CommandProcessor.RemoteAdminCommandHandler.AllCommands)
                            {
                                if (command.Command == "spawntoy" || command.Command == "spawntoy text")
                                {
                                    command.Execute(new ArraySegment<string>(graffitiFullCommand.Split(' ')), sender, out string commandResponse);
                                    return;
                                }
                            }

                            string successMsg = t.GraffitiCommandSent;
                            if (player != null) player.SendConsoleMessage(successMsg, "green");
                        }
                        catch (Exception exSpawn)
                        {
                            if (player != null) player.SendConsoleMessage(t.GraffitiCommandError, "red");
                        }
                    });
                }
                catch (HttpRequestException httpEx)
                {
                    Timing.CallDelayed(0f, () =>
                    {
                        if (player != null) player.SendConsoleMessage(string.Format(t.ImageDownloadError, httpEx.Message?.ToString() ?? "Unknown"), "red");
                    });
                }
                catch (Exception ex)
                {
                    Timing.CallDelayed(0f, () =>
                    {
                        if (player != null) player.SendConsoleMessage(t.GraffitiGeneralError, "red");
                    });
                }
            });

            response = t.GraffitiRequestBackground;
            return true;
        }
    }
}