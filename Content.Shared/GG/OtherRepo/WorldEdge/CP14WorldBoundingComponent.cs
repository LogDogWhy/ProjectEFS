namespace Content.Shared.GG.WorldEdge;

/// <summary>
/// when colliding with a player, starts a timer to remove him from the round.
/// </summary>
[RegisterComponent, Access(typeof(GGSharedWorldEdgeSystem))]
public sealed partial class GGWorldBoundingComponent : Component
{
    [DataField]
    public TimeSpan ReturnTime = TimeSpan.FromSeconds(15f);

    [DataField]
    public float Range = 0f;
}
