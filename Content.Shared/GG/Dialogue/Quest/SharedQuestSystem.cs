using Robust.Shared.GameStates;
using Robust.Shared.Player;
using Content.Shared.Antag;
using Robust.Shared.Utility;
using Content.Shared.GG.Dialogue;
using Robust.Shared.Audio.Systems;

namespace Content.Shared.GG.Quests;

public sealed class QuestsSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    [Dependency] private readonly SharedAudioSystem _audio = default!;
    [Dependency] private readonly ISharedPlayerManager _playerManager = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<QuestGrantEvent>(OnQuestGrant);
        SubscribeLocalEvent<QuestCompleteEvent>(OnQuestComplete);
    }

    private void OnQuestGrant(QuestGrantEvent ev)
    {
        var player = _entityManager.GetEntity(ev.Player);
        if (!EntityManager.TryGetComponent<QuestsComponent>(player, out var questsComponent))
        {
            Logger.Warning($"Entity {player} does not have a QuestsComponent.");
            return;
        }

        if (questsComponent.AddQuest(ev.QuestId))
        {
            if(_playerManager.TryGetSessionByEntity(player, out var session))
                _audio.PlayGlobal("/Audio/GG/Effects/Quest/quest_started.ogg", session);
            Logger.Info($"Quest '{ev.QuestId}' added to {player}.");
        }
        else
        {
            Logger.Info($"Quest '{ev.QuestId}' already exists for {player}.");
        }
    }

    private void OnQuestComplete(QuestCompleteEvent ev)
    {
        var player = _entityManager.GetEntity(ev.Player);

        if (!EntityManager.TryGetComponent<QuestsComponent>(player, out var questsComponent))
        {
            Logger.Warning($"Entity {player} does not have a QuestsComponent.");
            return;
        }

        if (questsComponent.CompleteQuest(ev.QuestId))
        {
            if(_playerManager.TryGetSessionByEntity(player, out var session))
                _audio.PlayGlobal("/Audio/GG/Effects/Quest/quest_finished.ogg", session);
            Logger.Info($"Quest '{ev.QuestId}' completed by {player}.");
        }
        else
        {
            Logger.Info($"Quest '{ev.QuestId}' could not be completed by {player}.");
        }
    }

    public bool QuestsStatus(EntityUid uid, DialogueResponse response)
    {
        Logger.Error("QuestsStatus");
        var player = uid;
        var questsComp = _entityManager.GetComponent<QuestsComponent>(player);
        if (questsComp == null)
        {
            Logger.Error("No comp QuestsStatus");
            return false;
        }
        if (response.QuestCompleteId != null)
        {
            if (questsComp.HasQuest(response.QuestCompleteId))
            {
                Logger.Error("here2");
                var temp = $"{response.QuestCompleteId}_yes";
                if(!temp.EndsWith("_yes"))
                    response.NextDialogueId = response.NextDialogueId + temp;
                return true;
            }
            else
            {
                return false;
            }
        }

        return false;

    }
    public bool QuestsButtons(EntityUid uid, DialogueResponse response)
    {
        var player = uid;
        var questsComp = _entityManager.GetComponent<QuestsComponent>(player);
        if (questsComp == null)
        {
            Logger.Error("No comp QuestsButtons");
            return false;
        }
        if (response.QuestId != null)
        {
            Logger.Error($"Quest : {response.QuestId}");
            if (questsComp.IsQuestCompleted(response.QuestId))
                return false;
            else
                Logger.Error("QuestCompleted");

            if (questsComp.HasQuest(response.QuestId))
                return false;
            else
                Logger.Error("HasQuest");

        }

        if (response.QuestCompleteId != null)
        {
            Logger.Error($"QuestCompleteId : {response.QuestCompleteId}");
            if (questsComp.IsQuestCompleted(response.QuestCompleteId))
                return false;

            if (!questsComp.HasQuest(response.QuestCompleteId))
                return false;

        }

        return true;
    }



}
