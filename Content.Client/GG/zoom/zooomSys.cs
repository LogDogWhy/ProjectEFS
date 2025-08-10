using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared._Sunrise.SniperZoom;
using Robust.Shared.Enums;
using Content.Client.Viewport;
using Robust.Client.Input;
using Content.Client._Sunrise.SniperZoom;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;
using Content.Shared.Hands.Components;
using Content.Shared.Hands;

namespace Content.Client._Sunrise.SniperZoom;

public sealed class ZoomableGunSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly TransformSystem _transformSystem = default!;
    [Dependency] private readonly IInputManager _inputManager = default!;
    [Dependency] private readonly IEyeManager _eyeManager = default!;
    [Dependency] private readonly IOverlayManager _overlayManager = default!;

    private SniperZoomOverlay? _sniperZoomOverlay;

    public override void Initialize()
    {
        base.Initialize();
        _sniperZoomOverlay = new SniperZoomOverlay(_entityManager, _playerManager, _prototypeManager, _transformSystem, _inputManager, _eyeManager);

        SubscribeLocalEvent<ZoomableGunComponent, ComponentStartup>(OnComponentStartup);
        SubscribeLocalEvent<ZoomableGunComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<ZoomableGunComponent, ComponentHandleState>(OnHandleState);

        SubscribeLocalEvent<ZoomableGunComponent, HandSelectedEvent>(OnHandSelected);
        SubscribeLocalEvent<ZoomableGunComponent, HandDeselectedEvent>(OnHandDeselected);
    }

    private void OnComponentStartup(EntityUid uid, ZoomableGunComponent component, ComponentStartup args)
    {
        if (component.Enabled && IsHeldByPlayer(uid))
        {
            Logger.InfoS("zoomable-gun-system", "Component startup detected with zoom enabled, adding overlay.");
            AddOverlay();
        }
    }

    private void OnComponentShutdown(EntityUid uid, ZoomableGunComponent component, ComponentShutdown args)
    {
        if (IsHeldByPlayer(uid))
        {
            Logger.InfoS("zoomable-gun-system", "Component shutdown detected, removing overlay if applicable.");
            RemoveOverlay();
        }
    }

    private void OnHandleState(EntityUid uid, ZoomableGunComponent component, ref ComponentHandleState args)
    {
        if (args.Current is not ZoomableGunComponentState state)
            return;

        component.Enabled = state.Enabled;

        if (component.Enabled && IsHeldByPlayer(uid))
        {
            Logger.InfoS("zoomable-gun-system", "Zoom enabled state detected, adding overlay.");
            AddOverlay();
        }
        else if (!component.Enabled && IsHeldByPlayer(uid))
        {
            Logger.InfoS("zoomable-gun-system", "Zoom disabled state detected, removing overlay.");
            RemoveOverlay();
        }
    }

    private void OnHandSelected(EntityUid uid, ZoomableGunComponent component, HandSelectedEvent args)
    {
        if (component.Enabled && args.User == _playerManager.LocalPlayer?.ControlledEntity)
        {
            Logger.InfoS("zoomable-gun-system", "Weapon selected with zoom enabled, adding overlay.");
            AddOverlay();
        }
    }

    private void OnHandDeselected(EntityUid uid, ZoomableGunComponent component, HandDeselectedEvent args)
    {
        if (args.User == _playerManager.LocalPlayer?.ControlledEntity)
        {
            Logger.InfoS("zoomable-gun-system", "Weapon deselected, removing overlay.");
            RemoveOverlay();
        }
    }

    private void AddOverlay()
    {
        if (_sniperZoomOverlay != null && !_overlayManager.HasOverlay(typeof(SniperZoomOverlay)))
        {
            Logger.InfoS("zoomable-gun-system", "Adding sniper zoom overlay.");
            _overlayManager.AddOverlay(_sniperZoomOverlay);
        }
    }

    private void RemoveOverlay()
    {
        if (_sniperZoomOverlay != null && _overlayManager.HasOverlay(typeof(SniperZoomOverlay)))
        {
            Logger.InfoS("zoomable-gun-system", "Removing sniper zoom overlay.");
            _overlayManager.RemoveOverlay(_sniperZoomOverlay);
        }
    }

private bool IsHeldByPlayer(EntityUid uid)
{
    var player = _playerManager.LocalPlayer?.ControlledEntity;
    if (player == null)
        return false;

    if (_entityManager.TryGetComponent(player.Value, out HandsComponent? hands))
    {
        return hands.ActiveHandEntity == uid;
    }

    return false;
}

}
