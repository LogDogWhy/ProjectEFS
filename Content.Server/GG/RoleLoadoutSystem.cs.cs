using Content.Shared.Preferences.Loadouts.Messages;
using Robust.Shared.Player;
using Content.Shared.Preferences.Loadouts;
public sealed class RoleLoadoutSystem : EntitySystem
{


    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<UpdatePointsMessage>(OnUpdatePoints);
    }

    private void OnUpdatePoints(UpdatePointsMessage msg, EntitySessionEventArgs args)
    {
        // Здесь мы предполагаем, что у нас есть способ получить текущий RoleLoadout для сессии пользователя
        var roleLoadout = GetRoleLoadoutForSession(args.SenderSession);

        if (roleLoadout == null)
            return;

        Logger.InfoS("points", $"Succesful update");

        // var query = EntityQueryEnumerator<PointManagerComponent>();

        // while(query.MoveNext(out var uid, out var point))
        // {
        //     foreach ( var player in point.Points)
        //     {
        //         if (player.Key == args.SenderSession.UserId)
        //         {

        //             roleLoadout.SetPoints(msg.NewPoints + player.Value.Value);
        //             RaiseNetworkEvent(new UpdatePointsMessage(msg.NewPoints), args.SenderSession.ConnectedClient);
        //             Logger.InfoS("points", $"Succesful{player.Value.Value} {msg.NewPoints}");
        //             break;
        //         }


        //     }

        // }

        // Обновляем очки на сервере
        roleLoadout.SetPoints(msg.NewPoints);

        // Отправляем обновленное значение обратно на клиент
        RaiseNetworkEvent(new UpdatePointsMessage(msg.NewPoints), args.SenderSession.ConnectedClient);
    }

    public RoleLoadout? GetRoleLoadoutForSession(ICommonSession session)
    {
        // Реализация метода для получения RoleLoadout для сессии пользователя
        // Это может быть часть сессии пользователя или храниться в отдельном месте
        return null;
    }
}
