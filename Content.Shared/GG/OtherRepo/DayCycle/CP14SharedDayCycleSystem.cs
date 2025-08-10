using Content.Shared.GG.DayCycle.Components;
using Content.Shared.Maps;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.GG.DayCycle;

public abstract class GGSharedDayCycleSystem : EntitySystem
{
    private static readonly ProtoId<GGDayCyclePeriodPrototype> DayPeriod = "Day";

    [Dependency] private readonly SharedMapSystem _maps = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefManager = default!;

    /// <summary>
    /// Checks to see if the specified entity is on the map where it's daytime.
    /// </summary>
    /// <param name="target">An entity being tested to see if it is in daylight</param>
    /// <param name="checkRoof">Checks if the tile covers the weather (the only "roof" factor at the moment)</param>
    public bool TryDaylightThere(EntityUid target, bool checkRoof)
    {
        var xform = Transform(target);
        if (!TryComp<GGDayCycleComponent>(xform.MapUid, out var dayCycle))
            return false;

        if (!checkRoof || !TryComp<MapGridComponent>(xform.GridUid, out var mapGrid))
            return dayCycle.CurrentPeriod == DayPeriod;

        var tileRef = _maps.GetTileRef(xform.GridUid.Value, mapGrid, xform.Coordinates);
        var tileDef = (ContentTileDefinition) _tileDefManager[tileRef.Tile.TypeId];

        if (!tileDef.Weather)
            return false;

        return dayCycle.CurrentPeriod == DayPeriod;
    }
}
