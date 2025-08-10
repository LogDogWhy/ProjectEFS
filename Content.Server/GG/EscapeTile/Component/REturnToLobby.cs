using Content.Server.EUI;
using Content.Shared.Eui;
using Content.Shared.GG.Escapetile;
using Content.Shared.Mind;
using Robust.Shared.Player;
using Content.Shared.Mind;
using Content.Shared.Players;
using Robust.Server.Player;
using Content.Server.GameTicking;
using Content.Shared.Preferences.Loadouts.Messages;
using Content.Shared.Preferences.Loadouts;
using Robust.Shared.Prototypes;
using Content.Shared.Points;
using Content.Server.Points;

namespace Content.Server.GG.EscapeTile;

public sealed class RespawnEui : BaseEui
{

    [Dependency] private readonly RoleLoadoutSystem _roleLoadoutSystem = default!;

    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly PointSystem _point = default!;
    [Dependency] private readonly IEntityManager _entMan = default!;
    private readonly ICommonSession _session;

    private bool _onDeath;

    private readonly GGEscapeTileComponent _component;

    public RespawnEui(ICommonSession session, GGEscapeTileComponent component, bool Ondeath = false)
    {
        _session = session;
        _component = component;
        _onDeath = Ondeath;
    }

    public override void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (msg is not RespawnChoiceMessage choice ||
            !choice.Accepted)
        {
            _component.Opened = false;

            Close();
            return;
        }

            var sysMan = IoCManager.Resolve<IEntitySystemManager>();
            var mind = sysMan.GetEntitySystem<SharedMindSystem>();
            var ticker = sysMan.GetEntitySystem<GameTicker>();
            _component.Opened = false;

            mind.WipeMind(_session.Data.ContentData()?.Mind);
            ticker.Respawn(_session);

        Close();
    }
}
