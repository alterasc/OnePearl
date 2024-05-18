using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;

namespace OnePearl.Components;

[TypeId("3fc64c9a90bc4d699618244850570727")]
public class AbilityRestoreFixedLevelSpellSlot : AbilityApplyEffect, IAbilityRequiredParameters, IAbilityRestriction
{
    public int SpellLevel;
    public BlueprintScriptableObjectReference[] TrackedResources;
    AbilityParameter IAbilityRequiredParameters.RequiredParameters => AbilityParameter.SpellSlot;

    public override void Apply(AbilityExecutionContext context, TargetWrapper target)
    {
        if (context.Ability.ParamSpellSlot == null
            || context.Ability.ParamSpellSlot.SpellShell == null
            || context.Ability.ParamSpellSlot.SpellShell.Spellbook == null
            || context.Ability.ParamSpellSlot.SpellLevel != SpellLevel)
        {
            return;
        }

        var allowLowerSlots = Main.Settings.AllowLowerLevels;

        SpellSlot notAvailableSpellSlot = GetNotAvailableSpellSlot(context.Ability.ParamSpellSlot.SpellShell);
        if (notAvailableSpellSlot != null)
        {
            var resourceSpent = PearlUtils.TrySpendPearlResource(context.MaybeOwner, SpellLevel, !allowLowerSlots, out var pearlTotals);
            if (resourceSpent)
            {
                notAvailableSpellSlot.Available = true;
            }
            if (allowLowerSlots)
            {
                var totals = PearlUtils.PearlTotal(pearlTotals, allowLowerSlots);
                PearlUtils.UpdateResources(context.MaybeOwner, TrackedResources, totals, false, SpellLevel - 1);
            }
        }
    }

    bool IAbilityRestriction.IsAbilityRestrictionPassed(AbilityData ability)
    {
        AbilityData abilityData = ability.ParamSpellSlot?.SpellShell;
        Spellbook spellbook = abilityData?.Spellbook;
        if (abilityData == null || spellbook == null || abilityData.SpellLevel != SpellLevel)
        {
            return false;
        }

        return GetNotAvailableSpellSlot(abilityData) != null;
    }

    string IAbilityRestriction.GetAbilityRestrictionUIText()
    {
        return "";
    }

    public static SpellSlot GetNotAvailableSpellSlot(AbilityData ability)
    {
        if (ability.Spellbook == null)
        {
            return null;
        }

        foreach (SpellSlot memorizedSpellSlot in ability.Spellbook.GetMemorizedSpellSlots(ability.SpellLevel))
        {
            if (!memorizedSpellSlot.Available && memorizedSpellSlot.SpellShell == ability)
            {
                return memorizedSpellSlot;
            }
        }

        return null;
    }
}
