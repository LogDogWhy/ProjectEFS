using Content.Server.Chat.Managers;
using Content.Server.Database.Migrations.Postgres;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Station.Systems;
using Content.Shared.Chat;
using Content.Shared.GameTicking.Components;
using Content.Shared.Interaction.Events;
using Content.Shared.Mind;
using Content.Shared.Mobs;
using Content.Shared.Players;
using Robust.Server.Player;
using Robust.Shared.Console;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Content.Server.EUI;
using Content.Server.GG.EscapeTile;
using System.Threading.Tasks;
using Robust.Shared.Player;
using Robust.Server.Audio;
using Content.Server.Access.Systems;
using Content.Server.GameTicking.Rules.Components;
using Microsoft.EntityFrameworkCore.Metadata;


namespace Content.Server.GameTicking.Rules;

/// <summary>
/// This handles logic and interactions related to <see cref="RespawnDeadRuleComponent"/>
/// </summary>
public sealed class RespawnRuleSystem : GameRuleSystem<RespawnDeadRuleComponent>
{

    [Dependency] private readonly IConsoleHost _console = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly StationSystem _station = default!;
    [Dependency] private readonly EuiManager _euiManager = default!;
    [Dependency] private readonly SharedMindSystem _mind = default!;

    [Dependency] private readonly AudioSystem _audio = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<SuicideEvent>(OnSuicide);
        SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (_station.GetStations().FirstOrNull() is not { } station)
            return;

        foreach (var tracker in EntityQuery<RespawnTrackerComponent>())
        {
            foreach (var (player, time) in tracker.RespawnQueue)
            {
                if (_timing.CurTime < time)
                    continue;

                if (!_playerManager.TryGetSessionById(player, out var session))
                    continue;

                if (session.GetMind() is { } mind && TryComp<MindComponent>(mind, out var mindComp) && mindComp.OwnedEntity.HasValue)
                    QueueDel(mindComp.OwnedEntity.Value);
                    {
                        _audio.PlayGlobal("/Audio/GG/Effects/death.ogg", session);
                        GameTicker.MakeJoinGame(session, station,  silent: true);
                    }

                tracker.RespawnQueue.Remove(player);
            }
        }
    }

    private void OnSuicide(SuicideEvent ev)
    {
        if (!TryComp<ActorComponent>(ev.Victim, out var actor))
           return;

        var query = EntityQueryEnumerator<RespawnTrackerComponent>();
        while (query.MoveNext(out _, out var respawn))
        {
            if (respawn.Players.Remove(actor.PlayerSession.UserId))
                QueueDel(ev.Victim);
        }
    }

    private void OnMobStateChanged(MobStateChangedEvent args)
    {
        if (args.NewMobState != MobState.Dead)
            return;

        if (!TryComp<ActorComponent>(args.Target, out var actor))
            return;

        if (!_playerManager.TryGetSessionById(actor.PlayerSession.UserId, out var session))
            return;

        if(!_mind.TryGetMind(actor.PlayerSession.UserId, out var mind) && mind == null)
            return;

        var query = EntityQueryEnumerator<RespawnDeadRuleComponent, RespawnTrackerComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var respawnRule, out var tracker, out var rule))
        {
            if (!GameTicker.IsGameRuleActive(uid, rule))
                continue;

            if (respawnRule.SendTolobby)
            {
                GameTicker.Respawn(actor.PlayerSession);
                return;
            }


            if (respawnRule.AlwaysRespawnDead)
                AddToTracker(actor.PlayerSession.UserId, (uid, tracker));
            if (RespawnPlayer((args.Target, actor), (uid, tracker)))
                break;
        }

    }


    public void RemovePlayerFromRespawnQueue(ICommonSession player)
    {
        foreach (var tracker in EntityQuery<RespawnTrackerComponent>())
        {
            if (!_playerManager.TryGetSessionById(player.UserId, out var session))
                    continue;

                if (session.GetMind() is { } mind && TryComp<MindComponent>(mind, out var mindComp) && mindComp.OwnedEntity.HasValue)
                    QueueDel(mindComp.OwnedEntity.Value);
            tracker.RespawnQueue.Remove(player.UserId);
        }
    }
    /// <summary>
    /// Attempts to directly respawn a player, skipping the lobby screen.
    /// </summary>
    public bool RespawnPlayer(Entity<ActorComponent> player, Entity<RespawnTrackerComponent> respawnTracker)
    {
        if (!respawnTracker.Comp.Players.Contains(player.Comp.PlayerSession.UserId) || respawnTracker.Comp.RespawnQueue.ContainsKey(player.Comp.PlayerSession.UserId))
            return false;

        if (respawnTracker.Comp.RespawnDelay == TimeSpan.Zero)
        {
            if (_station.GetStations().FirstOrNull() is not { } station)
                return false;

            if (respawnTracker.Comp.DeleteBody)
                QueueDel(player);
            GameTicker.MakeJoinGame(player.Comp.PlayerSession, station, silent: true);
            return false;
        }

        var msg = Loc.GetString("rule-respawn-in-seconds", ("second", respawnTracker.Comp.RespawnDelay.TotalSeconds));
        var wrappedMsg = Loc.GetString("chat-manager-server-wrap-message", ("message", msg));
        _chatManager.ChatMessageToOne(ChatChannel.Server, msg, wrappedMsg, respawnTracker, false, player.Comp.PlayerSession.Channel, Color.LimeGreen);

        respawnTracker.Comp.RespawnQueue[player.Comp.PlayerSession.UserId] = _timing.CurTime + respawnTracker.Comp.RespawnDelay;

        return true;
    }

    /// <summary>
    /// Adds a given player to the respawn tracker, ensuring that they are respawned if they die.
    public void AddToTracker(Entity<ActorComponent?> player, Entity<RespawnTrackerComponent?> respawnTracker)
    {
        if (!Resolve(respawnTracker, ref respawnTracker.Comp) || !Resolve(player, ref player.Comp, false))
            return;

        AddToTracker(player.Comp.PlayerSession.UserId, (respawnTracker, respawnTracker.Comp));
    }

    /// <summary>
    /// Adds a given player to the respawn tracker, ensuring that they are respawned if they die.
    /// </summary>
    public void AddToTracker(NetUserId id, Entity<RespawnTrackerComponent> tracker)
    {
        tracker.Comp.Players.Add(id);
    }
}
