using Content.Shared.Drugs;
using Content.Shared.GG.Drugs;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Player;
using Content.Shared.Mobs.Systems;
using Content.Shared.FixedPoint;

namespace Content.Client.GG.Drugs;

public sealed class TunnelVisionOverlaySystem : EntitySystem
{
    private ISawmill _sawmill = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;


    private TunnelVisionOverlay _overlay = default!;

    public static string TunnelVisionKey = "TunnelVision";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<TunnelVisionComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<TunnelVisionComponent, ComponentShutdown>(OnShutdown);

        SubscribeLocalEvent<TunnelVisionComponent, LocalPlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<TunnelVisionComponent, LocalPlayerDetachedEvent>(OnPlayerDetached);

        _overlay = new();
    }

    private void OnPlayerAttached(EntityUid uid, TunnelVisionComponent component, LocalPlayerAttachedEvent args)
    {
        _overlayMan.AddOverlay(_overlay);
    }

    private void OnPlayerDetached(EntityUid uid, TunnelVisionComponent component, LocalPlayerDetachedEvent args)
    {
        _overlay.TunnelLevel = 0;
        _overlay.TimeTicker = 0;
        _overlayMan.RemoveOverlay(_overlay);
    }

    private void OnInit(EntityUid uid, TunnelVisionComponent component, ComponentInit args)
    {
        if (_player.LocalEntity == uid)
            _overlayMan.AddOverlay(_overlay);
    }

    private void OnShutdown(EntityUid uid, TunnelVisionComponent component, ComponentShutdown args)
    {
        if (_player.LocalEntity == uid)
        {
            _overlay.TunnelLevel = 0;
            _overlay.TimeTicker = 0;
            _overlayMan.RemoveOverlay(_overlay);
        }
    }

}
