using System.Collections.Generic;
using System.IO;
using AssetBundles;
using HarmonyLib;
using PavonisInteractive.TerraInvicta.Modding;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

namespace TerraInvictaSprites
{
    public class SpriteLoader
    {
        public static bool enabled;

        public static UnityModManager.ModEntry mod;

        public static Dictionary<string, Sprite> additionalSprites;
        
        

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();
            
            Debug.Log("SpriteLoader loaded");

            return true;
        }

        [HarmonyPatch(typeof(AssetBundleManager), nameof(AssetBundleManager.Initialize))]
        public class ManagerInitializePatch
        {
            static bool Prefix(AssetBundleManager __instance)
            {
               
                additionalSprites = new Dictionary<string, Sprite>();
                Debug.Log("[Sprite Loader]:---------------START----------------");
                for (int l = 0; l < ModManager.ModDirectories.Count; l++)
                {
                    string dpath = Path.Combine(Directory.GetCurrentDirectory(), ModManager.ModDirectories[l],"sprites");
                    Debug.Log($"[Sprite Loader]: checking mod dir {dpath}");
                    if (File.Exists(Path.Combine(dpath, "sprites.txt")))
                    {

                        string sjson = File.ReadAllText(Path.Combine(dpath, "sprites.txt"));

                        var sprite_list = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SpriteDefinition>>(sjson);

                        foreach (var spriteDefinition in sprite_list)
                        {
                            if (!additionalSprites.ContainsKey(spriteDefinition.Name))
                            {
                                if (File.Exists(Path.Combine(dpath, spriteDefinition.Path)))
                                {
                                    Debug.Log(
                                        $"[Sprite Loader]: Loading sprite {spriteDefinition.Name} from {spriteDefinition.Path}");
                                    byte[] file_data = File.ReadAllBytes(Path.Combine(dpath, spriteDefinition.Path));
                                    Texture2D t2d = new Texture2D(2, 2);
                                    if (t2d.LoadImage(file_data))
                                    {
                                        Sprite spr = Sprite.Create(t2d,
                                            new Rect(0f, 0f, (float)t2d.width, (float)t2d.height),
                                            new Vector2(0.5f, 0.5f));
                                        additionalSprites.Add(spriteDefinition.Name,spr);
                                    }
                                    else
                                    {
                                        Debug.Log($"[Sprite Loader]: Failed to load image {spriteDefinition.Path}");
                                    }
                                }
                                else
                                {
                                    Debug.Log(
                                        $"[Sprite Loader]: Sprite {spriteDefinition.Name} not found, path {spriteDefinition.Path}");
                                }
                            }
                            else
                            {
                                Debug.Log($"[Sprite Loader]: Sprite with name {spriteDefinition.Name} was already loaded");
                            }
                        }

                        
                    }
                }
                Debug.Log("[Sprite Loader]:---------------FINISH----------------");

                return true;
            }
        }

        [HarmonyPatch(typeof(AssetLoader), nameof(AssetLoader.LoadAssetForImageAssignment))]
        public class LoadAssetForImageAssignment_Patch
        {
            static bool Prefix(AssetLoader __instance, string asset, Image imageToAssign)
            {
                if (!additionalSprites.ContainsKey(asset))
                {
                    return true;
                }

                imageToAssign.sprite = additionalSprites[asset];
                return false;
            }
        }
    }
}