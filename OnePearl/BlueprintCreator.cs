using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Items;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Designers.Mechanics.EquipmentEnchants;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.Localization;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.FactLogic;
using OnePearl.Components;
using System.Collections.Generic;
using System.Linq;

namespace OnePearl;
internal static class BlueprintCreator
{
    internal static void CreateBlueprints()
    {
        var exquisitePearlIcon = Utils.GetBlueprint<BlueprintItem>("f682126f69da1ea479bf1ddf1d775d97").m_Icon;
        BlueprintUnitFactReference[] abilities = new BlueprintUnitFactReference[9];

        BlueprintAbilityResource[] resources = new BlueprintAbilityResource[9];

        var normalPearls = new List<BlueprintItemEquipmentUsable>() {
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("edab1a1ee8654810aca64df16dd87aae"),
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("f548bd21606344f0963c0ae374946d39"),
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("d2d4b585db9f43698d183437f20bd8de"),
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("54cbc6480de54d49b277d2686a98b4fb"),
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("3f3aa97c9a1c46d5a8b3a3c22a5727db"),
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("8103f79802eb4ec59c373829ff5907a1"),
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("778b25032e7343ba856326bf7f3dfeab"),
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("d0552e5dd29d49568cebfb8fd2335486"),
            Utils.GetBlueprint<BlueprintItemEquipmentUsable>("4e98e92f49024c529cb2afa01fc63b0e"),
        };

        // resources
        for (int i = 1; i <= 9; i++)
        {
            var resource = Utils.CreateBlueprint<BlueprintAbilityResource>($"OnePearlRestoreResource{i}", bp =>
            {
                bp.m_Min = 0;
                bp.m_Max = 99;
            });
            resources[i - 1] = resource;
        }
        var resourceRefArray = resources.Select(x => x.ToReference<BlueprintScriptableObjectReference>()).ToArray();

        var nullString = new LocalizedString();

        // restore abilities
        for (int i = 1; i <= 9; i++)
        {
            var ability = Utils.CreateBlueprint<BlueprintAbility>($"OnePearlRestoreAbility{i}", bp =>
            {
                bp.m_Icon = exquisitePearlIcon;
                bp.m_DisplayName = normalPearls[i - 1].m_DisplayNameText;
                bp.m_Description = normalPearls[i - 1].m_DescriptionText;
                bp.LocalizedSavingThrow = nullString;
                bp.LocalizedDuration = nullString;
                bp.AddComponent<AbilityResourceLogic>(c =>
                {
                    c.m_RequiredResource = resources[i - 1].ToReference<BlueprintAbilityResourceReference>();
                    c.m_IsSpendResource = true;
                });
                bp.AddComponent<AbilityRestoreFixedLevelSpellSlot>(c =>
                {
                    c.TrackedResources = resourceRefArray;
                    c.SpellLevel = i;
                });
                bp.ActionBarAutoFillIgnored = true;
                bp.Hidden = true;
                bp.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Standard;
                bp.Type = AbilityType.Supernatural;
                bp.Range = AbilityRange.Personal;
                bp.CanTargetSelf = true;
                bp.Animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Self;
            });
            abilities[i - 1] = ability.ToReference<BlueprintUnitFactReference>();
        }

        // feature granting resources and abilities
        var onePearlFeature = Utils.CreateBlueprint<BlueprintFeature>("$OnePearlFeature", bp =>
        {
            bp.AddComponent<AddFacts>(c =>
            {
                c.m_Facts = abilities;
            });
            for (int i = 0; i < resources.Length; i++)
            {
                bp.AddComponent<AddAbilityResources>(c =>
                {
                    c.RestoreAmount = false;
                    c.m_Resource = resources[i].ToReference<BlueprintAbilityResourceReference>();
                });
            }
            bp.AddComponent<AddRestTrigger>(c =>
            {
                c.Action = new()
                {
                    Actions = [
                        new UpdateOnePearlResourcesAction
                        {
                            TrackedResources = resourceRefArray,
                            RaiseEvent = false,
                            TakeMaxCharges = true
                        }
                    ]
                };
            });
            bp.AddComponent<OnePearlResourceUpdateHandler>(c =>
            {
                c.TrackedResources = resourceRefArray;
            });
            bp.HideInUI = true;
            bp.HideInCharacterSheetAndLevelUp = true;
            bp.IsClassFeature = false;
        });

        // enchnantment that grants feature above
        var ench = Utils.CreateBlueprint<BlueprintEquipmentEnchantment>("OnePearlEnchantment", bp =>
        {
            bp.m_EnchantName = nullString;
            bp.m_Description = nullString;
            bp.m_Prefix = nullString;
            bp.m_Suffix = nullString;
            bp.m_IdentifyDC = 0;
            bp.m_EnchantmentCost = 0;
            bp.AddComponent<AddOnePearlFeatureEquipment>(c =>
            {
                c.m_Feature = onePearlFeature.ToReference<BlueprintFeatureReference>();
                c.TrackedResources = resourceRefArray;
            });
        });

        var updResources = Utils.CreateBlueprint<BlueprintAbility>($"OnePearlResourceUpdater", bp =>
        {
            bp.m_Icon = exquisitePearlIcon;
            bp.m_DisplayName = Utils.CreateLocalizedString($"{bp.name}Name", $"One Pearl - Update resources");
            bp.m_Description = Utils.CreateLocalizedString($"{bp.name}Description", $"Activate One Pearl to match its resources to pearls in inventory");
            bp.LocalizedSavingThrow = nullString;
            bp.LocalizedDuration = nullString;
            bp.AddComponent<AbilityEffectRunAction>(c =>
            {
                c.Actions = new()
                {
                    Actions = [
                        new UpdateOnePearlResourcesAction
                        {
                            TrackedResources = resourceRefArray,
                            TakeMaxCharges = false
                        }
                    ]
                };
            });
            bp.ActionType = Kingmaker.UnitLogic.Commands.Base.UnitCommand.CommandType.Free;
            bp.Type = AbilityType.Supernatural;
            bp.Range = AbilityRange.Personal;
            bp.CanTargetSelf = true;
            bp.Animation = Kingmaker.Visual.Animation.Kingmaker.Actions.UnitAnimationActionCastSpell.CastAnimationStyle.Immediate;
        });

        // the item
        var item = Utils.CreateBlueprint<BlueprintItemEquipmentUsable>("OnePearlItem", bp =>
        {
            bp.m_Icon = exquisitePearlIcon;
            bp.m_DisplayNameText = Utils.CreateLocalizedString($"{bp.name}Name", $"The One Pearl");
            bp.m_DescriptionText = Utils.CreateLocalizedString($"{bp.name}Description", $"The pearl of power that combines powers of all other pearls that owner possesses.");
            bp.m_FlavorText = nullString;
            bp.m_NonIdentifiedDescriptionText = nullString;
            bp.m_NonIdentifiedNameText = nullString;
            bp.m_Cost = 0;
            bp.m_Weight = 0;
            bp.SpendCharges = false;
            bp.m_Ability = updResources.ToReference<BlueprintAbilityReference>();
            bp.Type = UsableItemType.Other;
            bp.m_MiscellaneousType = BlueprintItem.MiscellaneousItemType.None;
            bp.m_Destructible = false;
            bp.m_Enchantments = [
                ench.ToReference<BlueprintEquipmentEnchantmentReference>()
            ];
            bp.m_BeltItemPrefab = normalPearls[0].m_BeltItemPrefab;
            bp.m_ShardItem = normalPearls[0].m_ShardItem;
            bp.m_EquipmentEntity = normalPearls[0].m_EquipmentEntity;
            bp.m_EquipmentEntityAlternatives = [];
        });
    }
}
