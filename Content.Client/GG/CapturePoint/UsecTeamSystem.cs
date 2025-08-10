
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;
using Content.Shared.GG.CapturePoint;

namespace Content.Client.GG.CapturePoint;

public sealed class GGUsecTeamSystem : SharedGGUsecTeamSystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GGUsecTeamComponent, GetStatusIconsEvent>(GetUsecIcon);
    }

    private void GetUsecIcon(Entity<GGUsecTeamComponent> ent, ref GetStatusIconsEvent args)
    {
        if (HasComp<GGBearTeamComponent>(ent))
            return;

        if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
            args.StatusIcons.Add(iconPrototype);
    }

}
