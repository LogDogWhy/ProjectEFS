using Content.Shared.GG.Dialogue;
using Content.Shared.Verbs;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server.GG.Dialogue
{
    public sealed class DialogueSystem : EntitySystem
    {
        [Dependency] private readonly IEntityManager _entities = default!;
        [Dependency] private readonly IPlayerManager _playerManager = default!;


        [Dependency] private readonly IPrototypeManager _proto = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<DialogueComponent, GetVerbsEvent<InteractionVerb>>(AddDialogueVerb);
        }

        private void AddDialogueVerb(EntityUid uid, DialogueComponent component, GetVerbsEvent<InteractionVerb> args)
        {
            if (!args.CanAccess || !args.CanInteract)
                return;

            var verb = new InteractionVerb
            {
                Text = "Поговорить",
                Act = () => StartDialogue(args.User, uid, component)
            };

            args.Verbs.Add(verb);
        }

        private void StartDialogue(EntityUid user, EntityUid target, DialogueComponent comp)
        {
            var proto = _proto.Index<DialoguePrototype>(comp.StartDialogueId);
            if (proto != null)
            {
                Logger.Error("StartDialogue");
                if (_playerManager.TryGetSessionByEntity(user, out var session))
                    RaiseNetworkEvent(new OpenDialogueWindowEvent(_entities.GetNetEntity(user), _entities.GetNetEntity(target), proto), Filter.SinglePlayer(session));
            }
            else
            {
                Logger.Error("not StartDialogue");
            }

        }
    }
}
