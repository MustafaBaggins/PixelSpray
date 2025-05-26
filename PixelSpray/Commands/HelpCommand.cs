using CommandSystem;
using System;

namespace PixelSpray.Commands
{
    public class HelpCommand : ICommand
    {
        public string Command { get; } = "help";
        public string[] Aliases { get; } = { "h" };
        public string Description { get; } = PixelSprayPlugin.Instance.Translation.SprayCommandDescription;

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "pixelspray/ps" +
                "\n\n" +
                "Available Commands:\n" +
                "- ps spray <label> <imageLink> - Sprays a Spray with the specified label and text.\n" +
                "- ps spray <label> <imageLink> <time> - Sprays a temporary Spray.\n" +
                "- ps list - Lists all currently spawned Spray.\n" +
                "- ps remove <id/label/playerNickname/all> - Removes a Spray by its ID, Label or Nickname.\n" +
                "- ps respray <id/label> - Spawns an already existing Spray by its ID or label.\n" +
                "- ps help - Show this help message.";
            return true;
        }
    }
}
