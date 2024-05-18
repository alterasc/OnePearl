using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.PubSubSystem;
using Kingmaker.UnitLogic;
using System.Linq;

namespace OnePearl.Components;

/// <summary>
/// Handler to update resources of other party members that have the pearl
/// </summary>
[TypeId("d85ff479d02844baaaf61a9158618601")]
public class OnePearlResourceUpdateHandler : UnitFactComponentDelegate, IUnitAbilityResourceHandler
{
    public BlueprintScriptableObjectReference[] TrackedResources;
    void IUnitAbilityResourceHandler.HandleAbilityResourceChange(UnitEntityData unit, UnitAbilityResource resource, int oldAmount)
    {
        if (unit == Owner) return;
        if (!TrackedResources.Contains(resource.Blueprint.ToReference<BlueprintScriptableObjectReference>())) return;
        var thisUnitResource = Owner.Resources.GetResource(resource.Blueprint);
        if (thisUnitResource == null) return;
        thisUnitResource.Amount = resource.Amount;
    }
}