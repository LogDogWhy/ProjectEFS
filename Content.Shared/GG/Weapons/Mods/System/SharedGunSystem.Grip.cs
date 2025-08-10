using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Nutrition.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Containers;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Content.Shared.Containers.ItemSlots;

namespace Content.Shared.Weapons.Ranged.Systems;



public abstract partial class SharedGunSystem
{
    private Dictionary<string, float>? m_slot_grip = new();
    protected const string GripSlot = "gun_grip";

    protected virtual void InitializeGrip()
    {
        SubscribeLocalEvent<GripComponent, EntInsertedIntoContainerMessage>(OnGripSlotChange);
        SubscribeLocalEvent<GripComponent, EntRemovedFromContainerMessage>(OnGripSlotChange);
    }

    protected EntityUid? GetGripEntity(EntityUid uid)
    {
        if (!Containers.TryGetContainer(uid, GripSlot, out var container) ||
            container is not ContainerSlot slot)
        {
            return null;
        }
        if(TryComp<AttachmentComponent>(slot.ContainedEntity, out var dic) )
        {
            m_slot_grip = _attachmentSystem.GetAllStats(dic);
        }

        return slot.ContainedEntity;
    }

    protected virtual void OnGripSlotChange(EntityUid uid, GripComponent component, ContainerModifiedMessage args)
    {
        if (GripSlot != args.Container.ID)
            return;

        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        var grip = GetGripEntity(uid);

        if (grip != null)
        {
             if (TryComp<GunComponent>(uid, out var gun) && gun != null)
             {
                if(m_slot_grip != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_grip);
                 RefreshModifiers(uid);
             }
            Appearance.SetData(uid, AmmoVisuals.Grip, true, appearance);

        }
        else
        {
             if (_entityManager.TryGetComponent(uid, out GunComponent? gun))
             {
                if(m_slot_grip != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_grip);
                 RefreshModifiers(uid);
             }
            Appearance.SetData(uid, AmmoVisuals.Grip, false, appearance);
            return;
        }
    }

}




