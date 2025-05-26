using AdminToys;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Networking;
using Player = Exiled.API.Features.Player;
using TextToy = AdminToys.TextToy;

namespace PixelSpray.API
{
    public class SprayManager
    {
        public static Dictionary<uint, Player> PlayerSprays = new Dictionary<uint, Player>();
        public static Dictionary<string, uint> SpraysLabel = new Dictionary<string, uint>();

        public static void AddPlayerSpray(Player player, string label, string image)
        {
            foreach (GameObject value in NetworkClient.prefabs.Values)
            {
                if (value.TryGetComponent<TextToy>(out var textToy))
                {
                    textToy = UnityEngine.Object.Instantiate(textToy);
                    textToy.TextFormat = image;
                    textToy.OnSpawned(player.ReferenceHub, new ArraySegment<string>(image.Split(' ')));
                    player?.SendConsoleMessage(PixelSprayPlugin.Instance.Translation.SprayCommandSent, "");
                    if (!PlayerSprays.ContainsKey(textToy.netId))
                    {
                        PlayerSprays[textToy.netId] = player;
                    }
                    if (!SpraysLabel.ContainsKey(label))
                    {
                        SpraysLabel[label] = textToy.netId;
                        return;
                    }
                    int labelNameIndex = 0;
                    while (SpraysLabel.ContainsKey(label))
                    {
                        labelNameIndex++;
                        label = $"{label}({labelNameIndex})";
                    }
                    SpraysLabel[label] = textToy.netId;
                }
            }
        }

        public static bool RemoveSprayById(uint sprayId)
        {
            if (!NetworkUtils.SpawnedNetIds.TryGetValue(sprayId, out var value) || !value.TryGetComponent<AdminToyBase>(out var component))
            {
                return false;
            }
            NetworkServer.Destroy(component.gameObject);
            if (PlayerSprays.ContainsKey(sprayId))
            {
                PlayerSprays.Remove(sprayId);
            }
            List<string> keysToRemove = SpraysLabel.Where(kvp => kvp.Value == sprayId).Select(kvp => kvp.Key).ToList();
            foreach (string key in keysToRemove)
            {
                SpraysLabel.Remove(key);
            }
            return true;
        }

        public static bool RemoveSprayByLabel(string label)
        {
            if (!SpraysLabel.TryGetValue(label, out uint value))
            {
                return false;
            }
            RemoveSprayById(value);
            return true;
        }

        public static int RemoveSprayByPlayer(Player player)
        {
            List<uint> ids = PlayerSprays.Where(kvp => kvp.Value == player).Select(kvp => kvp.Key).ToList();
            foreach (uint id in ids)
            {
                RemoveSprayById(id);
            }
            return ids.Count;
        }
    }
}
