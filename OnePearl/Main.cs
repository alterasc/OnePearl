using HarmonyLib;
using Kingmaker.Blueprints.JsonSystem;
using System;
using System.Reflection;
using UnityModManagerNet;

namespace OnePearl;

#if DEBUG
[EnableReloading]
#endif
static class Main
{
    internal static Harmony HarmonyInstance;
    internal static UnityModManager.ModEntry.ModLogger log;
    internal static Guid ModNamespaceGuid;
    internal static SettingsModMenu Settings;

    static bool Load(UnityModManager.ModEntry modEntry)
    {
        log = modEntry.Logger;
        ModNamespaceGuid = NamespaceGuidUtils.CreateV5Guid(NamespaceGuidUtils.UrlNamespace, modEntry.Info.Id);
#if DEBUG
        modEntry.OnUnload = OnUnload;
#endif
        modEntry.OnGUI = OnGUI;
        HarmonyInstance = new Harmony(modEntry.Info.Id);
        HarmonyInstance.PatchAll(Assembly.GetExecutingAssembly());
        return true;
    }

    static void OnGUI(UnityModManager.ModEntry modEntry)
    {

    }

#if DEBUG
    static bool OnUnload(UnityModManager.ModEntry modEntry)
    {
        HarmonyInstance.UnpatchAll(modEntry.Info.Id);
        return true;
    }
#endif

    [HarmonyPatch(typeof(BlueprintsCache), nameof(BlueprintsCache.Init))]
    internal static class BlueprintInitPatch
    {
        private static bool _initiailized;

        [HarmonyPostfix]
        public static void Postfix()
        {
            if (_initiailized) return;
            _initiailized = true;
            Settings = new SettingsModMenu();
            Settings.Initialize();
            BlueprintCreator.CreateBlueprints();
            log.Log("Added The One Pearl");
        }
    }
}