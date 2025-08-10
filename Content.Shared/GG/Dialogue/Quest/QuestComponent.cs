using System.Collections.Generic;
using Robust.Shared.GameObjects;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Shared.GG.Quests
{
    [RegisterComponent]
    public partial class QuestsComponent : Component
    {

        [DataField("activeQuests")]
        private List<string> _activeQuests = new();

        [DataField("completedQuests")]
        private List<string> _completedQuests = new();

        public bool HasQuest(string questId) => _activeQuests.Contains(questId);

        public bool IsQuestCompleted(string questId) => _completedQuests.Contains(questId);

        public bool AddQuest(string questId)
        {
            if (_activeQuests.Contains(questId) || _completedQuests.Contains(questId))
                return false;

            _activeQuests.Add(questId);
            return true;
        }

        public bool CompleteQuest(string questId)
        {
            if (!_activeQuests.Contains(questId))
                return false;

            _activeQuests.Remove(questId);
            _completedQuests.Add(questId);
            return true;
        }

        public List<string> GetActiveQuests()
        {
            return _activeQuests;
        }
        public List<string> GetCompletedQuests()
        {
            return _completedQuests;
        }
    }
}
