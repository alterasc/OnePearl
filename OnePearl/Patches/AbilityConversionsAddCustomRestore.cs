using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities;
using OnePearl.Components;
using System.Collections.Generic;

namespace OnePearl.Patches;

/// <summary>
/// Patches ability conversions collection method to include 
/// new AbilityRestoreFixedLevelSpellSlot
/// </summary>
[HarmonyPatch(typeof(AbilityData), nameof(AbilityData.GetConversions))]
internal static class AbilityConversionsAddCustomRestore
{
    [HarmonyPostfix]
    public static void Postfix(AbilityData __instance, ref IEnumerable<AbilityData> __result)
    {
        if (__instance.SpellSlot == null || __instance.Spellbook == null)
        {
            return;
        }
        List<AbilityData> tmpList = null;
        foreach (var ability in __instance.Caster.Abilities)
        {
            var spellLevel = __instance.SpellLevel;
            var restoreComponent = ability.Blueprint.GetComponent<AbilityRestoreFixedLevelSpellSlot>();
            if (restoreComponent != null && restoreComponent.SpellLevel == spellLevel)
            {
                AbilityData.AddAbilityUnique(ref tmpList, new AbilityData(ability)
                {
                    ParamSpellSlot = __instance.SpellSlot
                });
            }
        }
        if (tmpList != null)
        {
            if (__result is not List<AbilityData>)
            {
                __result = tmpList;
            }
            else
            {
                (__result as List<AbilityData>).AddRange(tmpList);
            }
        }
    }
}