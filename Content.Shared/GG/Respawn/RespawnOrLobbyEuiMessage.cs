using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared.GG.Escapetile;

[Serializable, NetSerializable]
public sealed class RespawnChoiceMessage : EuiMessageBase
{
    public readonly bool Accepted;

    public RespawnChoiceMessage(bool accepted)
    {
        Accepted = accepted;
    }
}
