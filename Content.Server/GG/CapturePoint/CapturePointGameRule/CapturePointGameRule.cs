using Content.Shared.FixedPoint;
using Content.Shared.Roles;
using Content.Shared.Storage;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.GG.GameTicking.Rules.Components;

[RegisterComponent]
public sealed partial class CapturePointGameRuleComponent : Component
{
    [DataField("pointsCap"), ViewVariables(VVAccess.ReadWrite)]
    public FixedPoint2 PointsCap = 500;

    public float BearTeamPoints = 0;
    public float UsecTeamPoints = 0;

    [DataField("victor")]
    public string? Victor;
}

