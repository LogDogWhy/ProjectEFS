using System.Collections.Generic;
using Robust.Shared.Prototypes;
using Content.Shared.GG.Dialogue;

namespace Content.Shared.GG.Dialogue
{
    [Serializable]
    [Prototype("dialogue")] // Указываем тип прототипа
    public sealed class DialoguePrototype : IPrototype // Реализуем интерфейс IPrototype
    {

        [IdDataField]
        public string ID { get; set; } = default!; // Уникальный идентификатор диалога

        [DataField("narratorText")]
        public string NarratorText { get; set; } = default!; // Текст рассказчика

        [DataField("responses")]
        public List<DialogueResponse> Responses { get; set; } = new(); // Ответы пользователя
    }

}
