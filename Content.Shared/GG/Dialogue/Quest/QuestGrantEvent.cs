using Robust.Shared.Serialization;

namespace Content.Shared.GG.Quests
{
    [Serializable, NetSerializable]
    public sealed class QuestGrantEvent : EntityEventArgs
    {
        public string QuestId { get; }

        public NetEntity Player { get; }

        public QuestGrantEvent(NetEntity player, string questId)
        {
            Player = player;
            QuestId = questId;
        }
    }
}
