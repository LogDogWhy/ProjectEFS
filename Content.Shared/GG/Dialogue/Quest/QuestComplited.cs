using Robust.Shared.Serialization;

namespace Content.Shared.GG.Quests
{
    [Serializable, NetSerializable]
    public sealed class QuestCompleteEvent : EntityEventArgs
    {
        public string QuestId { get; }
        public NetEntity Player { get; }

        public QuestCompleteEvent(NetEntity player, string questId)
        {
            Player = player;
            QuestId = questId;
        }
    }
}
