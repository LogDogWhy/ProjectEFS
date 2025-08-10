using Content.Shared.Antag;
using Robust.Shared.GameStates;
using Content.Shared.StatusIcon;
using Robust.Shared.Prototypes;
using Robust.Shared.Audio;

namespace Content.Shared.GG.CapturePoint;

[RegisterComponent, NetworkedComponent]
public sealed partial class GGBearTeamComponent : Component
{

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<StatusIconPrototype> StatusIcon { get; set; } = "BearFaction";
    public override bool SessionSpecific => true;
}

[RegisterComponent, NetworkedComponent]
public sealed partial class GGUsecTeamComponent : Component
{

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public ProtoId<StatusIconPrototype> StatusIcon { get; set; } = "UsecFaction";
    public override bool SessionSpecific => true;
}

