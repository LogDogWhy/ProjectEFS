using Robust.Shared.Serialization;

namespace Content.Shared.Preferences.Loadouts.Messages
{
    [Serializable, NetSerializable]
    public sealed class UpdatePointsMessage : EntityEventArgs
    {
        public int NewPoints { get; }

        public UpdatePointsMessage(int newPoints)
        {
            NewPoints = newPoints;
        }
    }
}
