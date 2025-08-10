using Robust.Shared.GameStates;
using Robust.Shared.Player;
using Content.Shared.Antag;

namespace Content.Shared.GG.CapturePoint;

public abstract class SharedGGUsecTeamSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        // Keep this subscription to handle state attempts for GGUsecTeamComponent.
        SubscribeLocalEvent<GGUsecTeamComponent, ComponentGetStateAttemptEvent>(OnUsecCompGetStateAttempt);

        // Only subscribe to GGUsecTeamComponent startup for DirtyUsecComps.
        SubscribeLocalEvent<GGUsecTeamComponent, ComponentStartup>(DirtyUsecComps);

        // Remove redundant subscription for ShowAntagIconsComponent.
        // Antag-specific logic should be handled elsewhere if needed.
    }

    private void OnUsecCompGetStateAttempt(EntityUid uid, GGUsecTeamComponent comp, ref ComponentGetStateAttemptEvent args)
    {
        args.Cancelled = !CanGetState(args.Player);
    }

    private bool CanGetState(ICommonSession? player)
    {
        // Allow state retrieval if player has GGUsecTeamComponent or ShowAntagIconsComponent.
        if (player?.AttachedEntity is not { } uid)
            return true;

        return HasComp<GGUsecTeamComponent>(uid);
    }

    private void DirtyUsecComps(EntityUid someUid, GGUsecTeamComponent someComp, ComponentStartup ev)
    {
        // Iterate over all entities with GGUsecTeamComponent and mark them dirty.
        var usecComps = AllEntityQuery<GGUsecTeamComponent>();
        while (usecComps.MoveNext(out var uid, out var comp))
        {
            Dirty(uid, comp);
        }
    }
}
