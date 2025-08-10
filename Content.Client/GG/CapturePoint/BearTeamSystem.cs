
using Content.Shared.StatusIcon.Components;
using Robust.Shared.Prototypes;
using Content.Shared.GG.CapturePoint;

namespace Content.Client.GG.CapturePoint;

public sealed class GGBearTeamSystem : SharedGGBearTeamSystem
{
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GGBearTeamComponent, GetStatusIconsEvent>(GetBearIcon);
    }
    private void GetBearIcon(Entity<GGBearTeamComponent> ent, ref GetStatusIconsEvent args)
    {
        if (HasComp<GGUsecTeamComponent>(ent))
            return;

        if (_prototype.TryIndex(ent.Comp.StatusIcon, out var iconPrototype))
        {
            if(iconPrototype == null)
            {
                Logger.WarningS("status-icon", $"Missing or invalid StatusIconPrototype: {ent.Comp.StatusIcon}");
            }
            else
                args.StatusIcons.Add(iconPrototype);


        }
    }


}
