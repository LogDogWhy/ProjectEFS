using System.Linq;
using Content.Shared.Body.Systems;
using Content.Shared.Clothing.Components;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Content.Shared.Station;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Content.Shared.Preferences.Loadouts.Effects;

namespace Content.Shared.Clothing;

/// <summary>
/// Assigns a loadout to an entity based on the RoleLoadout prototype
/// </summary>
public sealed class LoadoutSystem : EntitySystem
{
    // Shared so we can predict it for placement manager.

    [Dependency] private readonly ActorSystem _actors = default!;
    [Dependency] private readonly SharedStationSpawningSystem _station = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    [Dependency] private readonly ILogManager _logManager = default!;

    private ISawmill _sawmill = default!;
    public override void Initialize()
    {
        base.Initialize();
        _sawmill = _logManager.GetSawmill("LoadoutSystem");
        // Wait until the character has all their organs before we give them their loadout
        SubscribeLocalEvent<LoadoutComponent, MapInitEvent>(OnMapInit, after: [typeof(SharedBodySystem)]);
    }

    public static string GetJobPrototype(string? loadout)
    {
        if (string.IsNullOrEmpty(loadout))
            return string.Empty;

        return "Job" + loadout;
    }

    /// <summary>
    /// Tries to get the first entity prototype for operations such as sprite drawing.
    /// </summary>
    public EntProtoId? GetFirstOrNull(LoadoutPrototype loadout)
    {
        if (!_protoMan.TryIndex(loadout.Equipment, out var gear))
            return null;

        // Приоритетное извлечение идентификатора из рюкзака (или другого слота хранения)
        foreach (var storageSlot in gear.Storage)
        {
            if (storageSlot.Key == "back" && storageSlot.Value.Count > 0)
            {
                return storageSlot.Value.First(); // Возвращаем идентификатор первого предмета из рюкзака
            }
        }

        // Определение общего количества предметов
        var count = gear.Equipment.Count + gear.Inhand.Count + gear.Storage.Values.Sum(x => x.Count);

        if (count == 1)
        {
            // Если есть только один предмет в экипировке
            if (gear.Equipment.Count == 1 && _protoMan.TryIndex<EntityPrototype>(gear.Equipment.Values.First(), out var proto))
            {
                return proto.ID;
            }

            // Если есть только один предмет в руках
            if (gear.Inhand.Count == 1 && _protoMan.TryIndex<EntityPrototype>(gear.Inhand[0], out proto))
            {
                return proto.ID;
            }

            // Проверка слотов хранения
            foreach (var ents in gear.Storage.Values)
            {
                foreach (var ent in ents)
                {
                    return ent; // Возвращаем первый найденный предмет
                }
            }
        }

        return null;
    }


    /// <summary>
    /// Tries to get the name of a loadout.
    /// </summary>
    public string GetName(LoadoutPrototype loadout)
    {
        if (!_protoMan.TryIndex(loadout.Equipment, out var gear))
            return Loc.GetString("loadout-unknown");

        string name = string.Empty;
        int? itemCost = null;

        // Приоритетное извлечение имени из рюкзака (или другого слота хранения)
        foreach (var storageSlot in gear.Storage)
        {
            if (storageSlot.Key == "back" && storageSlot.Value.Count > 0)
            {
                var firstItemId = storageSlot.Value.First();
                if (_protoMan.TryIndex<EntityPrototype>(firstItemId, out var proto))
                {
                    name = proto.Name;
                    itemCost = GetCostFromLoadout(loadout);
                    break;
                }
            }
        }

        // Определение общего количества предметов
        var count = gear.Equipment.Count + gear.Storage.Values.Sum(o => o.Count) + gear.Inhand.Count;

        if (string.IsNullOrEmpty(name) && count == 1)
        {
            if (gear.Equipment.Count == 1 && _protoMan.TryIndex<EntityPrototype>(gear.Equipment.Values.First(), out var proto))
            {
                name = proto.Name;
                itemCost = GetCostFromLoadout(loadout);
            }

            if (gear.Inhand.Count == 1 && _protoMan.TryIndex<EntityPrototype>(gear.Inhand[0], out proto))
            {
                name = proto.Name;
                itemCost = GetCostFromLoadout(loadout);
            }

            foreach (var values in gear.Storage.Values)
            {
                if (values.Count != 1)
                    continue;

                if (_protoMan.TryIndex<EntityPrototype>(values[0], out proto))
                {
                    name = proto.Name;
                    itemCost = GetCostFromLoadout(loadout);
                    break;
                }
            }
        }

        // Если предмет имеет цену, добавляем ее справа от имени
        if (itemCost.HasValue && itemCost.Value > 0)
        {
            name += $" ({itemCost.Value} pts)";
        }

        return string.IsNullOrEmpty(name) ? Loc.GetString($"loadout-{loadout.ID}") : name;
    }

    // Дополнительный метод для извлечения стоимости из LoadoutPrototype
    private int? GetCostFromLoadout(LoadoutPrototype loadout)
    {
        var pointsCostEffect = loadout.Effects.OfType<PointsCostLoadoutEffect>().FirstOrDefault();
        return pointsCostEffect?.Cost;
    }


    private void OnMapInit(EntityUid uid, LoadoutComponent component, MapInitEvent args)
    {
        // Use starting gear if specified
        if (component.StartingGear != null)
        {
            var gear = _protoMan.Index(_random.Pick(component.StartingGear));
            _station.EquipStartingGear(uid, gear);
            return;
        }

        if (component.RoleLoadout == null)
            return;
        
        // ...otherwise equip from role loadout
        var id = _random.Pick(component.RoleLoadout);
        var proto = _protoMan.Index(id);
        var loadout = new RoleLoadout(id);
        loadout.SetDefault(GetProfile(uid), _actors.GetSession(uid), _protoMan, true);
        _station.EquipRoleLoadout(uid, loadout, proto);
    }

    public HumanoidCharacterProfile GetProfile(EntityUid? uid)
    {
        if (TryComp(uid, out HumanoidAppearanceComponent? appearance))
        {
            return HumanoidCharacterProfile.DefaultWithSpecies(appearance.Species);
        }

        return HumanoidCharacterProfile.Random();
    }
}
