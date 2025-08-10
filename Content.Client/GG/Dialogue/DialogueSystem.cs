using Content.Shared.GG.Dialogue;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Content.Shared.GG.Quests;
using Robust.Client.Player;
using Robust.Client.UserInterface.CustomControls;
using System.Numerics;
using System.Numerics;
using Robust.Client.Graphics;
using Robust.Shared.Maths;
using SixLabors.ImageSharp.Processing.Processors.Overlays;

namespace Content.Client.GG.Dialogue
{
    public sealed class DialogueSystem : EntitySystem
    {
        [Dependency] private readonly IPlayerManager _playerManager = default!;
        private DialogueWindow? _window;

        [Dependency] private readonly QuestsSystem _questsSystem = default!;
        [Dependency] private readonly IEntityManager _entityManager = default!;

        public override void Initialize()
        {
            base.Initialize();
            SubscribeNetworkEvent<OpenDialogueWindowEvent>(OnOpenDialogueWindow);
        }

        private void OnOpenDialogueWindow(OpenDialogueWindowEvent ev)
        {

            if (_window == null)
            {
                _window = new DialogueWindow(ev.Player, ev.Narrator,ev.Dialogue);
                _window.OnClose += () => _window = null;
            }

            _window.OpenCentered();
        }

        public bool QuestCheckButtons(NetEntity _player, DialogueResponse response)
        {
            var player = _entityManager.GetEntity(_player);
            return _questsSystem.QuestsButtons(player, response);
        }
        public bool QuestCheckStatus(NetEntity _player, DialogueResponse response)
        {
            var player = _entityManager.GetEntity(_player);
            return _questsSystem.QuestsStatus(player, response);
        }


        public void QuestGrant(NetEntity player, string questId)
        {
            Logger.Warning($"Try send.");
            var questGrantEvent = new QuestGrantEvent(player, questId!);
            RaiseLocalEvent(questGrantEvent);
        }

        public void QuestComplete(NetEntity player, string questId)
        {
            Logger.Warning($"Try send.");
            var questCompleteEvent = new QuestCompleteEvent(player, questId!);
            RaiseLocalEvent(questCompleteEvent);
        }
    }


}
