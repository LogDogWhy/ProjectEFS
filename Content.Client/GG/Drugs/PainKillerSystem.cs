using Content.Shared.Drugs;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;
using Content.Shared.Alert;
using Content.Shared.GG.Drugs;
using Content.Client.UserInterface.Systems.DamageOverlays;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Systems;
using Robust.Client.UserInterface;

namespace Content.Client.GG.Drugs;

public sealed class PainKillerSystem : EntitySystem
{
    [Dependency] private readonly IUserInterfaceManager _ui = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PainKillerComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<PainKillerComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
    }

    private void OnInit(EntityUid uid, PainKillerComponent component, ComponentInit args)
    {
        var damageUi = _ui.GetUIController<DamageOverlayUiController>();
        damageUi.ClearOverlay();
    }

    private void OnShutdown(EntityUid uid, PainKillerComponent component, ComponentShutdown args)
    {
        var damageUi = _ui.GetUIController<DamageOverlayUiController>();
        damageUi.UpdateOverlays(uid, null, null, null);
    }

    private void OnMobStateChanged(MobStateChangedEvent args)
    {

        if (args.Target != _playerManager.LocalEntity)
            return;
        var targetMobState = EntityManager.GetComponentOrNull<MobStateComponent>(args.Target);

        if (targetMobState == null)
            return;
        var damageUi = _ui.GetUIController<DamageOverlayUiController>();
        if(_mobState.IsDead(args.Target, targetMobState) == true || _mobState.IsIncapacitated(args.Target, targetMobState) == true )
        {

            damageUi.UpdateOverlays(args.Target, null, null, null);
        }
        else
        {
            damageUi.ClearOverlay();
        }

    }


}
