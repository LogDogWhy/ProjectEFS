using Content.Client.Weapons.Ranged.Systems;

namespace Content.Client.Weapons.Ranged.Components;


[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class GripVisualsComponent : Component
{
    [DataField("gripState")] public string? GripState;
}

[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class ScopeVisualsComponent : Component
{
    [DataField("scopeState")] public string? ScopeState;
}

[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class StockVisualsComponent : Component
{
    [DataField("stockState")] public string? StockState;
}

[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class ForearmVisualsComponent : Component
{
    [DataField("forearmState")] public string? ForearmState;
}

[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class GadgetVisualsComponent : Component
{
    [DataField("gadgetState")] public string? GadgetState;
}

[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class SuppressorVisualsComponent : Component
{
    [DataField("suppressorState")] public string? SuppressorState;
}

[RegisterComponent, Access(typeof(GunSystem))]
public sealed partial class ReceiverVisualsComponent : Component
{
    [DataField("receiverState")] public string? ReceiverState;
}
