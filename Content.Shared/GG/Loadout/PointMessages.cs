using Robust.Shared.GameObjects;
using Robust.Shared.Serialization;

namespace Content.Shared.GG.Loadout
{
    [Serializable, NetSerializable]
    public sealed class MsgRequestLoadoutPoints : EntityEventArgs
    {
        // Это сообщение используется для запроса очков игрока
    }

    [Serializable, NetSerializable]
    public sealed class MsgLoadoutPointsResponse : EntityEventArgs
    {
        // Это сообщение используется для отправки очков игроку
        public int Points { get; set; }
    }
}
