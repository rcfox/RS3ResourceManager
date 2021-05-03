using HarmonyLib;
using UnityModManagerNet;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.IO;
using System;

namespace RS3ResourceManager
{
    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        [Draw("Extract files as they load")] public bool SaveMissingFiles = false;        
        [Draw("Load files from the filesystem (if they exist)")] public bool LoadFiles = true;
        public string LoadFilesOnStartup = "";

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
            return;
        }
    }

    static class Main
    {
        static UnityModManager.ModEntry mod;
        static Settings settings;

        static void Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;
            settings = Settings.Load<Settings>(modEntry);

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnGUI = OnGUI;
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        [HarmonyPatch(typeof(Util))]
        [HarmonyPatch("falloc")]
        static class Util_falloc_Patch
        {
            static void Postfix(ref byte[] __result, string fname)
            {
                if (__result == null)
                {
                    return;
                }

                string fileName = Path.Combine(Path.Combine(mod.Path, "resources"), fname);
                try
                {
                    if (!File.Exists(fileName))
                    {
                        if (!settings.SaveMissingFiles)
                        {
                            return;
                        }
                        mod.Logger.Log(fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        {
                            binWriter.Write(__result);
                        }
                    } else
                    {
                        if (settings.LoadFiles)
                        {
                            __result = File.ReadAllBytes(fileName);
                        }
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.Error(fileName + " -- " + e.ToString());
                }
            }
        }

        [HarmonyPatch(typeof(Util))]
        [HarmonyPatch("fopen")]
        static class Util_fopen_Patch
        {
            static void Postfix(ref Stream __result, string fname)
            {
                if (__result == null)
                {
                    return;
                }
                string fileName = Path.Combine(Path.Combine(mod.Path, "resources"), fname);
                try
                {
                    if (!File.Exists(fileName))
                    {
                        if (!settings.SaveMissingFiles)
                        {
                            return;
                        }
                        mod.Logger.Log(fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        {
                            byte[] buffer = new byte[32768];
                            int read;
                            while ((read = __result.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                binWriter.Write(buffer, 0, read);
                            }
                        }
                        __result.Seek(0, SeekOrigin.Begin);
                    } else
                    {
                        if (settings.LoadFiles)
                        {
                            __result = File.Open(fileName, FileMode.Open);
                        }
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.Error(fileName + " -- " + e.ToString());
                }
            }
        }

        [HarmonyPatch(typeof(GameMain))]
        [HarmonyPatch("Start")]
        static class GameMain_Start_Patch
        {
            static void Postfix()
            {
                string[] fileNames = settings.LoadFilesOnStartup.Split(',');
                foreach (string fileName in fileNames)
                {
                    Util.falloc(fileName);
                }
            }
        }
    }
}