using Content.Server.GG.EscapeTile;
using Content.Server.Explosion.EntitySystems;
using Content.Shared.Popups;
using Content.Shared.StepTrigger.Systems;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Content.Shared.Explosion.Components;
using Robust.Server.GameObjects;
using Content.Server.EUI;
using Content.Shared.Mind;
using Content.Server.GG.EscapeTile;
using Robust.Server.Player;
using Content.Shared.Preferences.Loadouts.Messages;
using Content.Shared.Preferences.Loadouts;

namespace Content.Server.GG.EscapeTile;

public sealed class GGEscapeTileSystem : EntitySystem
{
    [Dependency] private readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    [Dependency] private readonly TriggerSystem _trigger = default!;


    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly EuiManager _euiManager = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;

    private BaseEui penis = default!;


    public override void Initialize()
    {
        SubscribeLocalEvent<GGEscapeTileComponent, StepTriggeredOnEvent>(HandleStepOnTriggered);
        SubscribeLocalEvent<GGEscapeTileComponent, StepTriggeredOffEvent>(HandleStepOffTriggered);

        SubscribeLocalEvent<GGEscapeTileComponent, StepTriggerAttemptEvent>(HandleStepTriggerAttempt);
    }

    private void HandleStepOnTriggered(EntityUid uid, GGEscapeTileComponent component, ref StepTriggeredOnEvent args)
    {
        if (_playerManager.TryGetSessionByEntity(args.Tripper, out var session) && !component.Opened)
        {
            component.Opened = true;
            penis = new RespawnEui(session, component);
            _euiManager.OpenEui(penis, session);
        }

    }

    private void HandleStepOffTriggered(EntityUid uid, GGEscapeTileComponent component, ref StepTriggeredOffEvent args)
    {
        _trigger.Trigger(uid, args.Tripper);
        component.Opened = false;
        _euiManager.CloseEui(penis);

    }

    private static void HandleStepTriggerAttempt(EntityUid uid, GGEscapeTileComponent component, ref StepTriggerAttemptEvent args)
    {
        args.Continue = true;
    }

}
