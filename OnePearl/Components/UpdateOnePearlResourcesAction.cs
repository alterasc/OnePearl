using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Mechanics.Actions;

namespace OnePearl.Components;

/// <summary>
/// Context action that updates pearl resources to match resources of common pearls
/// </summary>
[TypeId("1c20490f6d86456a9c9a7c8275beb120")]
public class UpdateOnePearlResourcesAction : ContextAction
{
    public BlueprintScriptableObjectReference[] TrackedResources;

    public bool RaiseEvent;

    public bool TakeMaxCharges;
    public override string GetCaption()
    {
        return "Restore One Pearl resources";
    }

    public override void RunAction()
    {
        var unit = Target.Unit;
        var totals = PearlUtils.PearlTotal(PearlUtils.CollectPearls(unit.Inventory, TakeMaxCharges), Main.Settings.AllowLowerLevels);
        PearlUtils.UpdateResources(unit, TrackedResources, totals, true);
    }
}
