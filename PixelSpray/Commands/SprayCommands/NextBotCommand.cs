using AdminToys;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using Exiled.API.Features;
using MEC;
using Mirror;
using NetworkManagerUtils.Dummies;
using PixelSpray.API;
using PlayerRoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PixelSpray.Commands.SprayCommands
{
    namespace PixelSpray.Commands.SprayCommands
    {
        public class NextBotCommand : ICommand
        {
            public string Command { get; } = "nextbot";
            public string[] Aliases { get; } = { "nb" };
            public string Description { get; } = PixelSprayPlugin.Instance.Translation.SprayCommandDescription;

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                Player player = Player.Get(sender);

                string imageUrl = arguments.ElementAtOrDefault(0);

                if (imageUrl == null)
                {
                    response = "Missing image URL.";
                    return false;
                }

                GameObject gameObject = Object.Instantiate(NetworkManager.singleton.playerPrefab);
                if (!gameObject.TryGetComponent<ReferenceHub>(out var component))
                {
                    response = string.Empty;
                    return false;
                }

                NetworkServer.AddPlayerForConnection(new DummyNetworkConnection(), gameObject);
                Player dummy = Player.Get(gameObject);

                if (dummy == null)
                {
                    response = "Dummy is null";
                    return false;
                }

                _ = SprayManager.AddPlayerSprayByUrl(player, "NextBot", imageUrl, textToy =>
                {
                    if (textToy == null)
                    {
                        Timing.CallDelayed(0f, () =>
                        {
                            player?.SendConsoleMessage(PixelSprayPlugin.Instance.Translation.SprayGeneralError, "");
                        });
                        return;
                    }

                    Timing.CallDelayed(Timing.WaitForOneFrame, () =>
                    {
                        dummy.Role.Set(RoleTypeId.Tutorial);
                        dummy.Teleport(player.Position);
                        dummy.SetFakeScale(new Vector3(0.01f, 0.01f, 0.01f), Player.List);

                        textToy.transform.position = dummy.Position;
                        textToy.transform.rotation = dummy.Rotation;

                        player?.SendConsoleMessage("Created NextBot with sucess.", "green");
                    });
                });

                response = "Creating NextBot in the backgroud.";
                return true;
            }
        }
    }
}
