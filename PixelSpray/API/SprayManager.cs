using AdminToys;
using Exiled.API.Features;
using LabApi.Features.Wrappers;
using MEC;
using Mirror;
using PixelSpray.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Emit;
using System.Threading.Tasks;
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

        public async static Task AddPlayerSprayByUrl(Player player, string label, string imageUrl, Action<TextToy> callback)
        {
            string image = await ConvertImage(imageUrl);

            TextToy textToy = AddPlayerSprayByConvertedImage(player, label, image);
            callback?.Invoke(textToy);
            return;



            Timing.CallDelayed(0f, () =>
            {
                TextToy textToy = AddPlayerSprayByConvertedImage(player, label, image);
                Log.Info(textToy == null);
            });

            if (textToy != null)
            {
                callback?.Invoke(textToy);
                return;
            }

            callback?.Invoke(null);
        }

        public static TextToy AddPlayerSprayByConvertedImage(Player player, string label, string image)
        {
            if (image.IsEmpty())
            {
                return null;
            }

            return SpawnTextToy(player, label, image);
        }

        public static bool RemoveSprayById(uint sprayId)
        {
            if (!NetworkUtils.SpawnedNetIds.TryGetValue(sprayId, out var value) || !value.TryGetComponent<AdminToyBase>(out var component))
            {
                return false;
            }

            if (PlayerSprays.ContainsKey(sprayId))
            {
                PlayerSprays.Remove(sprayId);
            }

            List<string> keysToRemove = SpraysLabel.Where(kvp => kvp.Value == sprayId).Select(kvp => kvp.Key).ToList();

            foreach (string key in keysToRemove)
            {
                SpraysLabel.Remove(key);
            }

            NetworkServer.Destroy(component.gameObject);
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

        private static TextToy SpawnTextToy(Player player, string label, string image)
        {
            TextToy textToy = null;
            foreach (GameObject value in NetworkClient.prefabs.Values)
            {
                if (value.TryGetComponent<TextToy>(out textToy))
                {
                    textToy = UnityEngine.Object.Instantiate(textToy);
                    textToy.TextFormat = image;
                    textToy.OnSpawned(player.ReferenceHub, new ArraySegment<string>(image.Split(' ')));
                    player?.SendConsoleMessage(PixelSprayPlugin.Instance.Translation.SprayCommandSent, "");

                    if (!PlayerSprays.ContainsKey(textToy.netId))
                    {
                        PlayerSprays[textToy.netId] = player;
                    }
                    int labelNameIndex = 0;

                    while (SpraysLabel.ContainsKey(label))
                    {
                        labelNameIndex++;
                        label = $"{label}({labelNameIndex})";
                    }

                    if (!SpraysLabel.ContainsKey(label))
                    {
                        SpraysLabel[label] = textToy.netId;
                    }
                    return textToy;
                }
            }
            return textToy;
        }

        public static async Task<string> ConvertImage(string imageUrl)
        {
            try
            {
                string SprayFullCommand = await AsciiArtConverter.ProcessImageFromUrl(imageUrl);
                return SprayFullCommand;
            }
            catch (HttpRequestException httpEx)
            {
                Log.Warn(string.Format(PixelSprayPlugin.Instance.Translation.ImageDownloadError, httpEx.Message?.ToString() ?? "Unknown"));
                return string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
