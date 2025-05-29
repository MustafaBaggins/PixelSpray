using AdminToys;
using CommandSystem;
using CommandSystem.Commands.RemoteAdmin.Dummies;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Features.DamageHandlers;
using MEC;
using Mirror;
using NetworkManagerUtils.Dummies;
using PixelSpray.API;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using PlayerStatsSystem;
using RemoteAdmin;
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
                    Timing.CallDelayed(0f, () =>
                    {
                        dummy.Role.Set(RoleTypeId.Tutorial);
                        dummy.Teleport(player.Position + new Vector3(10f, 0));
                        dummy.SetFakeScale(new Vector3(0.01f, 0.01f, 0.01f), Player.List);

                        textToy.transform.position = dummy.Position;
                        textToy.transform.rotation = dummy.Rotation;

                        player?.SendConsoleMessage("Created NextBot with sucess.", "green");
                        if (dummy.ReferenceHub.TryGetComponent<PlayerFollower>(out var component))
                        {
                            Object.Destroy(component);
                            Log.Info(component + " was destroyed.");
                        }
                        else
                        {
                            dummy.ReferenceHub.gameObject.AddComponent<PlayerFollower>().Init(player.ReferenceHub, minDistance: 0, speed: 40);
                            dummy.IsGodModeEnabled = true;
                            dummy.EnableEffect<Scp207>(240, 999999, true);
                        }
                        Timing.RunCoroutine(FollowPlayer(dummy, player, textToy));
                        Timing.RunCoroutine(CheckIfPlayerIsNear(dummy, player));
                    });
                });

                response = "Creating NextBot in the backgroud.";
                return true;
            }
            private IEnumerator<float> FollowPlayer(Player dummy, Player player, TextToy toy)
            {
                {
                    Vector3 offset = new Vector3(0f, 0f, 0f);
                    while (dummy != null && toy != null && player.IsAlive)
                    {
                        toy.transform.position = dummy.Position + offset;
                        toy.transform.rotation = dummy.Rotation;
                        if ((player.Position - dummy.Position).sqrMagnitude <= 0.01f)
                        {
                            PlayerStatsSystem.DamageHandlerBase nextBotDamage = new CustomReasonDamageHandler("Killed by the next bot!!");
                            DamageHandler damage = new DamageHandler(player, nextBotDamage);
                            damage.StartVelocity = player.Transform.rotation * new Vector3(-20, 10, 0);
                            player.Kill(damage);
                        }
                        yield return Timing.WaitForOneFrame;
                    }
                    if (!player.IsAlive)
                    {
                        if (dummy.ReferenceHub.TryGetComponent<PlayerFollower>(out var component))
                        {
                            Object.Destroy(component);
                        }
                        NetworkServer.Destroy(dummy.GameObject);
                        SprayManager.RemoveSprayByPlayer(player);
                        yield break;
                    }
                }
            }

            private IEnumerator<float> CheckIfPlayerIsNear(Player dummy, Player player)
            {
                if ((player.Position - dummy.Position).sqrMagnitude <= 0.01f)
                {
                    PlayerStatsSystem.DamageHandlerBase nextBotDamage = new CustomReasonDamageHandler("Killed by the next bot!!");
                    DamageHandler damage = new DamageHandler(player, nextBotDamage);
                    damage.StartVelocity = player.Transform.rotation * new Vector3(-20, 10, 0);
                    player.Kill(damage);
                }
                yield return Timing.WaitForOneFrame;
            }
        }
    }
}
