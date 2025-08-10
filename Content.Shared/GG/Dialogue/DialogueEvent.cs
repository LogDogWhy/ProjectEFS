using Robust.Shared.Serialization;

namespace Content.Shared.GG.Dialogue
{
    [Serializable, NetSerializable]
    public sealed class OpenDialogueWindowEvent : EntityEventArgs
    {
        public OpenDialogueWindowEvent(NetEntity player, NetEntity narrator, DialoguePrototype dialogue)
        {
            Narrator = narrator;
            Dialogue = dialogue;
            Player = player;
        }

        public NetEntity Player { get; }
        public NetEntity Narrator { get; }
        public DialoguePrototype Dialogue { get; }
    }
}
