using System.Linq;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.KillTracking;
using Content.Server.Mind;
using Content.Server.Points;
using Content.Server.RoundEnd;
using Content.Server.Station.Systems;
using Content.Shared.GameTicking.Components;
using Content.Shared.Points;
using Robust.Server.GameObjects;
using Robust.Server.Player;
using Robust.Shared.Utility;
using Content.Shared.Roles;
using Content.Shared.Station;
using Robust.Shared.Prototypes;
using Content.Shared.Clothing;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Robust.Shared.Player;
using Robust.Shared.Map;
using Robust.Shared.Random;
using Content.Server.Spawners.Components;

namespace Content.Server.GameTicking.Rules;

/// <summary>
/// Manages <see cref="DeathMatchRuleComponent"/>
/// </summary>
public sealed class DeathMatchRuleSystem : GameRuleSystem<DeathMatchRuleComponent>
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly MindSystem _mind = default!;
    [Dependency] private readonly PointSystem _point = default!;

    [Dependency] private readonly SharedStationSpawningSystem huita = default!;
    [Dependency] private readonly RespawnRuleSystem _respawn = default!;
    [Dependency] private readonly RoundEndSystem _roundEnd = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly StationSpawningSystem _stationSpawning = default!;
    [Dependency] private readonly TransformSystem _transform = default!;

    [Dependency] private readonly ActorSystem _actors = default!;

    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlayerBeforeSpawnEvent>(OnBeforeSpawn);
        SubscribeLocalEvent<PlayerSpawnCompleteEvent>(OnSpawnComplete);
        SubscribeLocalEvent<KillReportedEvent>(OnKillReported);
        SubscribeLocalEvent<DeathMatchRuleComponent, PlayerPointChangedEvent>(OnPointChanged);
    }

    private void OnBeforeSpawn(PlayerBeforeSpawnEvent ev)
    {
        var query = EntityQueryEnumerator<DeathMatchRuleComponent, RespawnTrackerComponent, PointManagerComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var dm, out var tracker, out var point, out var rule))
        {
            if (!GameTicker.IsGameRuleActive(uid, rule))
                continue;

            var protoMan = IoCManager.Resolve<IPrototypeManager>();

            var newMind = _mind.CreateMind(ev.Player.UserId, ev.Profile.Name);
            _mind.SetUserId(newMind, ev.Player.UserId);

            EntityCoordinates spawnCoordinates = EntityCoordinates.Invalid;

            var spawnPoints = new List<EntityUid>();

            foreach (var entity in EntityManager.EntityQuery<SpawnPointComponent>())
            {
                spawnPoints.Add(entity.Owner);
            }

            var spawn = _random.Pick(spawnPoints);

            var spawnCords = EntityManager.GetComponent<TransformComponent>(spawn).Coordinates;

            var mobMaybe = _stationSpawning.SpawnPlayerMob(spawnCords, null, ev.Profile, ev.Station );



            DebugTools.AssertNotNull(mobMaybe);
            var mob = mobMaybe;

            _mind.TransferTo(newMind, mob);
// GG start
            var highPriorityJob = ev.Profile.JobPriorities.FirstOrDefault(p => p.Value == JobPriority.High).Key;
            var job = protoMan.Index<JobPrototype>(highPriorityJob);
            var jobLoadout = LoadoutSystem.GetJobPrototype(job?.ID);

            if (_prototypeManager.TryIndex(jobLoadout, out RoleLoadoutPrototype? roleProto))
            {
                RoleLoadout? loadout = null;
                ev.Profile?.Loadouts.TryGetValue(jobLoadout, out loadout);

                // Set to default if not present
                if (loadout == null)
                {
                    loadout = new RoleLoadout(jobLoadout);
                    loadout.SetDefault(ev.Profile, _actors.GetSession(mob), _prototypeManager);
                }

                huita.EquipRoleLoadout(mob, loadout, roleProto);
            }

            if (job?.StartingGear != null)
            {
                var startingGear = _prototypeManager.Index<StartingGearPrototype>(job.StartingGear);
                huita.EquipStartingGear(mob, startingGear, raiseEvent: false);
            }

            var gearEquippedEv = new StartingGearEquippedEvent(mob);
            RaiseLocalEvent(mob, ref gearEquippedEv);
// GG end


            EnsureComp<KillTrackerComponent>(mob);
            _respawn.AddToTracker(ev.Player.UserId, (uid, tracker));

            _point.EnsurePlayer(ev.Player.UserId, uid, point);

            ev.Handled = true;
            break;
        }
    }

    private void OnSpawnComplete(PlayerSpawnCompleteEvent ev)
    {
        EnsureComp<KillTrackerComponent>(ev.Mob);
        var query = EntityQueryEnumerator<DeathMatchRuleComponent, RespawnTrackerComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out _, out var tracker, out var rule))
        {
            if (!GameTicker.IsGameRuleActive(uid, rule))
                continue;
            _respawn.AddToTracker((ev.Mob, null), (uid, tracker));
        }
    }

    private void OnKillReported(ref KillReportedEvent ev)
    {
        var query = EntityQueryEnumerator<DeathMatchRuleComponent, PointManagerComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var dm, out var point, out var rule))
        {
            if (!GameTicker.IsGameRuleActive(uid, rule))
                continue;

            // YOU SUICIDED OR GOT THROWN INTO LAVA!
            // WHAT A GIANT FUCKING NERD! LAUGH NOW!
            if (ev.Primary is not KillPlayerSource player)
            {
                _point.AdjustPointValue(ev.Entity, -1, uid, point);
                continue;
            }

            _point.AdjustPointValue(player.PlayerId, 1, uid, point);

            if (ev.Assist is KillPlayerSource assist && dm.Victor == null)
                _point.AdjustPointValue(assist.PlayerId, 1, uid, point);

            // var spawns = EntitySpawnCollection.GetSpawns(dm.RewardSpawns).Cast<string?>().ToList();
            // EntityManager.SpawnEntities(_transform.GetMapCoordinates(ev.Entity), spawns);
        }
    }

    private void OnPointChanged(EntityUid uid, DeathMatchRuleComponent component, ref PlayerPointChangedEvent args)
    {
        if (component.Victor != null)
            return;

        if (args.Points < component.KillCap)
            return;

        component.Victor = args.Player;
        _roundEnd.EndRound(component.RestartDelay);
    }

    protected override void AppendRoundEndText(EntityUid uid, DeathMatchRuleComponent component, GameRuleComponent gameRule, ref RoundEndTextAppendEvent args)
    {
        if (!TryComp<PointManagerComponent>(uid, out var point))
            return;

        if (component.Victor != null && _player.TryGetPlayerData(component.Victor.Value, out var data))
        {
            args.AddLine(Loc.GetString("point-scoreboard-winner", ("player", data.UserName)));
            args.AddLine("");
        }
        args.AddLine(Loc.GetString("point-scoreboard-header"));
        args.AddLine(new FormattedMessage(point.Scoreboard).ToMarkup());
    }
}
