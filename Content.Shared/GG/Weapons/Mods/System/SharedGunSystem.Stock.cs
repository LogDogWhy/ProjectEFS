using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Nutrition.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Containers;
using Robust.Shared.Audio;
using Robust.Shared.Player;

namespace Content.Shared.Weapons.Ranged.Systems;

public abstract partial class SharedGunSystem
{

    private Dictionary<string, float>? m_slot_stock= new();
    protected const string StockSlot = "gun_stock";

    protected virtual void InitializeStock()
    {
        SubscribeLocalEvent<StockComponent, EntInsertedIntoContainerMessage>(OnStockSlotChange);
        SubscribeLocalEvent<StockComponent, EntRemovedFromContainerMessage>(OnStockSlotChange);
    }

    protected EntityUid? GetStockEntity(EntityUid uid)
    {
        if (!Containers.TryGetContainer(uid, StockSlot, out var container) ||
            container is not ContainerSlot slot)
        {
            return null;
        }

        if(TryComp<AttachmentComponent>(slot.ContainedEntity, out var dic) )
        {
            m_slot_stock = _attachmentSystem.GetAllStats(dic);
        }

        return slot.ContainedEntity;
    }

    protected virtual void OnStockSlotChange(EntityUid uid, StockComponent component, ContainerModifiedMessage args)
    {
        if (StockSlot != args.Container.ID)
            return;

        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        var stock = GetStockEntity(uid);

        if (stock != null)
        {
             if (TryComp<GunComponent>(uid, out var gun) && gun != null)
             {
                if(m_slot_stock != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_stock);
                 RefreshModifiers(uid);
             }
            Appearance.SetData(uid, AmmoVisuals.Stock, true, appearance);

        }
        else
        {
             if (_entityManager.TryGetComponent(uid, out GunComponent? gun))
             {
                if(m_slot_stock != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_stock);
                 RefreshModifiers(uid);
             }
            Appearance.SetData(uid, AmmoVisuals.Stock, false, appearance);
            return;
        }
    }

}




