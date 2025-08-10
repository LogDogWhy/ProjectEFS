using Robust.Shared.GameStates;
using Robust.Shared.Player;
using Content.Shared.Antag;

namespace Content.Shared.GG.CapturePoint;

public abstract class SharedGGBearTeamSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        // Keep this subscription to handle state attempts for GGBearTeamComponent.
        SubscribeLocalEvent<GGBearTeamComponent, ComponentGetStateAttemptEvent>(OnBearCompGetStateAttempt);

        // Only subscribe to GGBearTeamComponent startup for DirtyBearComps.
        SubscribeLocalEvent<GGBearTeamComponent, ComponentStartup>(DirtyBearComps);

        // Remove redundant subscription for ShowAntagIconsComponent.
        // Antag-specific logic should be handled elsewhere if needed.
    }

    private void OnBearCompGetStateAttempt(EntityUid uid, GGBearTeamComponent comp, ref ComponentGetStateAttemptEvent args)
    {
        args.Cancelled = !CanGetState(args.Player);
    }

    private bool CanGetState(ICommonSession? player)
    {
        // Allow state retrieval if player has GGBearTeamComponent or ShowAntagIconsComponent.
        if (player?.AttachedEntity is not { } uid)
            return true;

        return HasComp<GGBearTeamComponent>(uid);
    }

    private void DirtyBearComps(EntityUid someUid, GGBearTeamComponent someComp, ComponentStartup ev)
    {
        // Iterate over all entities with GGBearTeamComponent and mark them dirty.
        var bearComps = AllEntityQuery<GGBearTeamComponent>();
        while (bearComps.MoveNext(out var uid, out var comp))
        {
            Dirty(uid, comp);
        }
    }
}
