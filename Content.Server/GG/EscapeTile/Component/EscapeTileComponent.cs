using Content.Shared.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;


namespace Content.Server.GG.EscapeTile
{
    [RegisterComponent, AutoGenerateComponentPause]
    public partial class GGEscapeTileComponent : Component
    {

        [DataField]
        public float EscapeTime = 5f;

        public bool Opened;

    }
}
