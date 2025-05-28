using CommandSystem;
using PixelSpray.Commands.SprayCommands.PixelSpray.Commands.SprayCommands;
using PixelSpray.Commands.SprayCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PixelSpray.Commands.StaticSprayCommands;

namespace PixelSpray.Commands.ParentCommands
{
    
    [CommandHandler(typeof(ClientCommandHandler))]
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class ParentStaticSprayCommand : ParentCommand
    {
        public override string Command => "spraystatic";

        public override string[] Aliases => new[] { "sst" };

        public override string Description => PixelSprayPlugin.Instance.Translation.SprayCommandDescription;

        public ParentStaticSprayCommand() => LoadGeneratedCommands();

        public override void LoadGeneratedCommands()
        {
            RegisterCommand(new StaticSprayCommand());
            RegisterCommand(new StaticListSprayCommand());
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = PixelSprayPlugin.Instance.Translation.Usage;
            return false;
        }
    }
    
}
