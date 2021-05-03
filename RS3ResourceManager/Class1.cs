using HarmonyLib;
using UnityModManagerNet;
using System.Reflection;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.IO;
using System;

namespace RS3ResourceManager
{
    static class Main
    {
        static UnityModManager.ModEntry mod = null;

        static void Load(UnityModManager.ModEntry modEntry)
        {
            mod = modEntry;

            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
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

                string fileName = Path.Combine(Path.Combine(mod.Path, "falloc"), fname);
                try
                {
                    if (!File.Exists(fileName))
                    {
                        mod.Logger.Log(fileName);
                        Directory.CreateDirectory(Path.GetDirectoryName(fileName));
                        using (BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
                        {
                            binWriter.Write(__result);
                        }
                    } else
                    {
                        __result = File.ReadAllBytes(fileName);
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
                string fileName = Path.Combine(Path.Combine(mod.Path, "fopen"), fname);
                try
                {
                    if (!File.Exists(fileName))
                    {
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
                        __result = File.Open(fileName, FileMode.Open);
                    }
                }
                catch (Exception e)
                {
                    mod.Logger.Error(fileName + " -- " + e.ToString());
                }
            }
        }
    }
}