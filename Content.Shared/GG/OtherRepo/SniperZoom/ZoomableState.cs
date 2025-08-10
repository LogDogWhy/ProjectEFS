using System;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared._Sunrise.SniperZoom
{
    /// <summary>
    /// State for networked syncing of the ZoomableGunComponent
    /// </summary>
    [Serializable, NetSerializable]
    public sealed class ZoomableGunComponentState : ComponentState
    {
        public bool Enabled { get; }

        public ZoomableGunComponentState(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
