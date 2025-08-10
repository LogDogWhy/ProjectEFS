namespace Content.Shared.GG.WorldEdge;

/// <summary>
/// when colliding with a player, starts a timer to remove him from the round.
/// </summary>
[RegisterComponent, Access(typeof(GGSharedWorldEdgeSystem))]
public sealed partial class GGWorldRemovePendingComponent : Component
{
    [DataField]
    public TimeSpan RemoveTime;

    [DataField]
    public Entity<GGWorldBoundingComponent>? Bounding;
}
