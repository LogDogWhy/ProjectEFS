using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.GG.Dialogue
{
    [DataDefinition]
    [Serializable]
    public partial class DialogueResponse
    {
        [DataField("text")]
        public string Text { get; set; } = default!; // Текст ответа

        [DataField("nextDialogueId")]
        public string? NextDialogueId { get; set; } // Id следующего диалога (может быть null)

        [DataField("questId", required: false)]
        public string? QuestId { get; set; } // Для выдачи квеста

        [DataField("questCompleteId", required: false)]
        public string? QuestCompleteId { get; set; } // Для завершения квеста
    }

}
