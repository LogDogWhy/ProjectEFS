using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using System.Linq;

namespace Content.Shared.Preferences.Loadouts.Effects;

public sealed partial class PointsCostLoadoutEffect : LoadoutEffect
{
    [DataField(required: true)]
    public int Cost = 1;

    public override bool Validate(
        HumanoidCharacterProfile profile,
        RoleLoadout loadout,
        LoadoutPrototype proto,
        ICommonSession? session,
        IDependencyCollection collection,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = null;
        var protoManager = collection.Resolve<IPrototypeManager>();

        if (!protoManager.TryIndex(loadout.Role, out var roleProto) || roleProto.Points == null)
        {
            return true;
        }

        // Проверяем, если текущий предмет выбран, то разрешаем его оставаться выбранным, даже если очков недостаточно
        if (loadout.SelectedLoadouts.Values.Any(group => group.Any(item => item.Prototype == proto.ID)))
        {
            return true;
        }
        // Блокируем предметы, на которые не хватает очков
        if (loadout.Points < Cost)
        {
            reason = FormattedMessage.FromUnformatted(Loc.GetString("loadouts-points-restriction"));
            return false; // Блокируем предмет, так как очков недостаточно
        }

        return true; // Разрешаем выбор предмета, так как очков достаточно
    }

    public override void Apply(RoleLoadout loadout)
    {
        // Вычитаем очки только в том случае, если их достаточно
        if (loadout.Points >= Cost)
        {
            loadout.DeductPoints(Cost);
        }
    }
}
