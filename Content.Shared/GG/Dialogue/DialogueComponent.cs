using Robust.Shared.GameObjects;

namespace Content.Shared.GG.Dialogue
{
    [RegisterComponent]
    public partial class DialogueComponent : Component
    {

        [DataField("startDialogueId")]
        public string StartDialogueId { get; set; } = "start"; // Начальный Id диалога
    }
}
