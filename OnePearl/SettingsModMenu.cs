using Kingmaker;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Localization;
using ModMenu.Settings;

namespace OnePearl;

internal class SettingsModMenu
{
    private static readonly string RootKey = "AlterAsc.OnePearl".ToLower();

    public bool AllowLowerLevels => !ModMenu.ModMenu.GetSettingValue<bool>(GetKey("specificlevelonly"));

    internal void Initialize()
    {
        ModMenu.ModMenu.AddSettings(
          SettingsBuilder
            .New(GetKey("title"), CreateString("title", "The One Pearl"))
            .AddToggle(
              Toggle
                .New(GetKey("specificlevelonly"), defaultValue: false, CreateString("specificlevelonly", "Make pearls work only on exact spell level"))
                .WithLongDescription(CreateString("specificlevelonly-desc", "Make pearls work according to tabletop rules, where pearl of " +
                "power can only be used to restore spells of particular level. I.e. pearl of power for 6th spell level can only be used " +
                "to restore spells prepared in 6th level slot. Note that this only applies to how One Pearl works."))
            )
            .AddButton(
              Button.New(
                  CreateString("receiveonepearl-desc", "Adds The One Pearl to inventory"),
                  CreateString("receiveonepearl-btn", "Receive"),
                  () =>
                  {
                      var onePearl = Utils.GetModBlueprint<BlueprintItemEquipmentUsable>("OnePearlItem");
                      Game.Instance.Player.Inventory.Add(onePearl);
                  })
            )
        );
    }

    private static LocalizedString CreateString(string partialKey, string text)
    {
        return CreateStringInner(GetKey(partialKey, "--"), text);
    }

    private static string GetKey(string partialKey, string separator = ".")
    {
        return $"{RootKey}{separator}{partialKey}";
    }

    private static LocalizedString CreateStringInner(string key, string value)
    {
        LocalizedString result = new()
        {
            m_Key = key
        };
        LocalizationManager.CurrentPack.PutString(key, value);
        return result;
    }
}
