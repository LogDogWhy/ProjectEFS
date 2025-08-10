using Content.Shared.GG.Dialogue;
using Content.Shared.GG.Quests;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Server.Audio;
using Content.Shared.Mind;
using Content.Server.Objectives;

namespace Content.Server.GG.Quests;
public sealed class QuestSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    [Dependency] private readonly AudioSystem _audio = default!;

    [Dependency] private readonly IPlayerManager _playerManager = default!;

    [Dependency] private readonly SharedMindSystem _mind = default!;

    [Dependency] private readonly ObjectivesSystem _objectives = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<QuestGrantEvent>(OnQuestGrant);
        SubscribeNetworkEvent<QuestCompleteEvent>(OnQuestComplete);
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
            {
                if (!_mind.TryGetMind(session, out var mindId, out var mind))
                {
                    return;
                }
                _mind.TryAddObjective(mindId, mind, ev.QuestId);
                _audio.PlayGlobal("/Audio/GG/Effects/Quest/quest_started.ogg", session);
            }

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
            {
                if (!_mind.TryGetMind(session, out var mindId, out var mind))
                {
                    return;
                }
                _mind.TryFindObjective((mindId, mind),ev.QuestId, out var obj);
                for(int i = 0; i < mind.Objectives.Capacity; i++)
                {
                    Logger.Error($"for int i = {i}");
                    if(mind.Objectives[i] == obj)
                        _mind.TryRemoveObjective(mindId, mind, i);
                }

                _audio.PlayGlobal("/Audio/GG/Effects/Quest/quest_finished.ogg", session);
            }

            Logger.Info($"Quest '{ev.QuestId}' completed by {player}.");
        }
        else
        {
            Logger.Info($"Quest '{ev.QuestId}' could not be completed by {player}.");
        }
    }
}
