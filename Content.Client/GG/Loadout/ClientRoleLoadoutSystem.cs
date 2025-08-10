using Content.Shared.Preferences.Loadouts.Messages;
using Content.Shared.Preferences.Loadouts;

public sealed class ClientRoleLoadoutSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<UpdatePointsMessage>(OnUpdatePointsMessage);
    }

    private void OnUpdatePointsMessage(UpdatePointsMessage msg)
    {
        // Здесь мы предполагаем, что у нас есть способ получить текущий RoleLoadout
        var roleLoadout = GetClientRoleLoadout();

        if (roleLoadout == null)
            return;
        // Обновляем очки на клиенте
        roleLoadout.SetPoints(msg.NewPoints);
    }

    private RoleLoadout? GetClientRoleLoadout()
    {
        // Реализация метода для получения RoleLoadout на клиенте
        return null;
    }
}
