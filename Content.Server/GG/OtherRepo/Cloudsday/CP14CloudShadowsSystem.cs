using Content.Shared.GG.DayCycle.Components;
using Robust.Shared.Random;

namespace Content.Server.GG.DayCycle;

public sealed class GGCloudShadowsSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<GGCloudShadowsComponent, MapInitEvent>(OnMapInit);
    }

    private void OnMapInit(Entity<GGCloudShadowsComponent> entity, ref MapInitEvent args)
    {
        entity.Comp.CloudSpeed = _random.NextVector2(-entity.Comp.MaxSpeed, entity.Comp.MaxSpeed);
    }
}
