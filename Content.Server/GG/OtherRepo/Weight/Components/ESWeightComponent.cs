using Content.Server._ES14.Weight.EntitySystems;
using Content.Shared._ES14.Weight;

namespace Content.Server._ES14.Weight.Components;

[RegisterComponent, Access(typeof(ESWeightSystem))]
public sealed partial class ESWeightComponent : SharedESWeightComponent
{
    [ViewVariables]
    public float Total => ModifiedSelf + InsideWeight;

    [DataField]
    public float Self = 0.05f;

    [ViewVariables]
    public float ModifiedSelf;

    [DataField, ViewVariables(VVAccess.ReadOnly)]
    public float InsideWeight;
}
