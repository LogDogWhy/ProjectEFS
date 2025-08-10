using Robust.Shared.Prototypes;

namespace Content.Shared.GG.DayCycle;

[Prototype("GGDayCyclePeriod")]
public sealed class GGDayCyclePeriodPrototype : IPrototype
{
    [IdDataField]
    public string ID { get; } = string.Empty;

    [DataField(required: true)]
    public LocId Name = default!;
}
