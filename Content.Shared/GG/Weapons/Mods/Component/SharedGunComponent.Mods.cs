using Content.Shared.Weapons.Ranged.Components;
using Robust.Shared.Audio;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Whitelist;
using Content.Shared.Tag;
using Robust.Shared.Prototypes;
using Content.Shared.Mobs;
using Robust.Shared.GameStates;
using Robust.Shared.Utility;

namespace Content.Shared.Weapons.Ranged;

/// <summary>
/// Just change sound
/// </summary>
[RegisterComponent, Virtual]
public partial class ScopeComponent : Component
{

}

[RegisterComponent, Virtual]
public partial class StockComponent : Component
{

}

[RegisterComponent, Virtual]
public partial class GripComponent : Component
{

}

[RegisterComponent, Virtual]
public partial class ForearmComponent : Component
{


}

[RegisterComponent, Virtual]
public partial class ReceiverComponent : Component
{


}

[RegisterComponent, Virtual]
public partial class GadgetComponent : Component
{

}


[RegisterComponent, Virtual, AutoGenerateComponentState, NetworkedComponent]
public partial class AttachmentComponent: Component
{
    // Slot and his Id
    [DataField("newSlot")]
    public string? NewSlot;

    [DataField]
    public bool Swap = false;

    [DataField]
    public bool Sil = false;

    // Slot Tags
    [DataField]
    public List<ProtoId<TagPrototype>>? Tags;

    //Modifiers
    [DataField]
    public Angle MinAngle = Angle.FromDegrees(0);

    [DataField]
    public Angle MaxAngle = Angle.FromDegrees(0);

    [DataField]
    public Angle AngleIncrease = Angle.FromDegrees(0);

    [DataField]
    public Angle AngleDecay = Angle.FromDegrees(0);

    [DataField]
    [AutoNetworkedField]
    public float FireRate = 0f;

    [DataField, AutoNetworkedField]
    public float CameraRecoilScalar = 0f;

}

[ByRefEvent]
public record struct AttachmentExamineEvent(FormattedMessage Msg);
