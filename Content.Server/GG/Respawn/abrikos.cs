using Robust.Shared.GameObjects;

namespace Content.Server.GameTicking.Rules.Components
{
    [RegisterComponent]
    public sealed partial class RespawnStateComponent : Component
    {
        public bool? RespawnAccepted;
    }
}
