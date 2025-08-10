using Content.Shared.GG.DayCycle;
using Content.Shared.GG.DayCycle.Components;
using Content.Shared.Random.Rules;
using Robust.Shared.Prototypes;

namespace Content.Shared.GG.Random.Rules;

/// <summary>
/// Checks whether there is a time of day on the current map, and whether the current time of day corresponds to the specified periods.
/// </summary>
public sealed partial class GGTimePeriod : RulesRule
{
    [DataField] private List<ProtoId<GGDayCyclePeriodPrototype>> Periods = new();

    public override bool Check(EntityManager entManager, EntityUid uid)
    {
        var transform = entManager.System<SharedTransformSystem>();

        var map = transform.GetMap(uid);
        return entManager.TryGetComponent<GGDayCycleComponent>(map, out var dayCycle) && Periods.Contains(dayCycle.CurrentPeriod);
    }
}
