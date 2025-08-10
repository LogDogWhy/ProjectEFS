using Content.Shared.Actions;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom;

namespace Content.Server.HealthScanner
{
    [RegisterComponent, AutoGenerateComponentPause]
    public partial class PlayerHealthAnalyzerComponent : Component
    {
        [DataField]
        public EntProtoId Action = "ActionActivateHealthAnalyzer";

        [DataField]
        public EntityUid? ActionEntity;

        [DataField(customTypeSerializer: typeof(TimeOffsetSerializer))]
        [AutoPausedField]
        public TimeSpan NextUpdate = TimeSpan.Zero;

        /// <summary>
        /// The delay between patient health updates
        /// </summary>
        [DataField]
        public TimeSpan UpdateInterval = TimeSpan.FromSeconds(1);

        /// <summary>
        /// How long it takes to scan someone.
        /// </summary>
        [DataField]
        public TimeSpan ScanDelay = TimeSpan.FromSeconds(0.8);
    }
}
