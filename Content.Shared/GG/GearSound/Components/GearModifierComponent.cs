// File: Content.Shared.Weapons/WeaponCarryComponent.cs

using Robust.Shared.GameObjects;
using Robust.Shared.Audio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.GameStates;

namespace Content.Shared.Movement.Components;

/// <summary>
/// Changes footstep sound
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class WeaponCarryComponent : Component
{
    [DataField(required: true), AutoNetworkedField]
    public SoundSpecifier CarrySoundCollection = default!;
}
