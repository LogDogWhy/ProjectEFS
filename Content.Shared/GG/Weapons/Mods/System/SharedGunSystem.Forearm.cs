using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Nutrition.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Containers;
using Content.Shared.Tag;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;


namespace Content.Shared.Weapons.Ranged.Systems;

public abstract partial class SharedGunSystem
{
    protected const string ForearmSlot = "gun_forearm";

    [Dependency] private readonly TagSystem _tagSystem = default!;

    [Dependency] protected readonly ItemSlotsSystem ItemSlotsSystem = default!;

    private string? _slot_name;

    private List<ProtoId<TagPrototype>> _tagsSlot_forearm = new();
    private ItemSlot? _NewGrip;
    private Dictionary<string, float>? m_slot_forearm = new();


    protected virtual void InitializeForearm()
    {
        SubscribeLocalEvent<ForearmComponent, EntInsertedIntoContainerMessage>(OnForearmSlotChange);
        SubscribeLocalEvent<ForearmComponent, EntRemovedFromContainerMessage>(OnForearmSlotChange);
    }

    protected EntityUid? GetForearmEntity(EntityUid uid)
    {
        if (!Containers.TryGetContainer(uid, ForearmSlot, out var container) ||
            container is not ContainerSlot slot)
        {
            return null;
        }

        if(TryComp<AttachmentComponent>(slot.ContainedEntity, out var dic) )
        {
            m_slot_forearm = _attachmentSystem.GetAllStats(dic);
            if (dic.Tags != null)
                _tagsSlot_forearm = dic.Tags;
            if (dic.NewSlot != null)
                _slot_name = dic.NewSlot;
            else
                _slot_name = null;

        }

        return slot.ContainedEntity;
    }

    protected virtual void OnForearmSlotChange(EntityUid uid, ForearmComponent component, ContainerModifiedMessage args)
    {
        if (ForearmSlot != args.Container.ID)
            return;

        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        var forearm = GetForearmEntity(uid);

        if (forearm != null)
        {
            if (TryComp<GunComponent>(uid, out var gun) && gun != null)
            {
                if(m_slot_forearm != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_forearm);
                RefreshModifiers(uid);
            }

            if(_slot_name != null)
            {
                _NewGrip = new()
                {
                    Whitelist = new EntityWhitelist
                    {
                        Tags = _tagsSlot_forearm,
                    }

                };
                ItemSlotsSystem.AddItemSlot(uid, _slot_name, _NewGrip);
            }


            Appearance.SetData(uid, AmmoVisuals.Forearm, true, appearance);

        }
        else
        {
            if (_entityManager.TryGetComponent(uid, out GunComponent? gun))
            {
                if(m_slot_forearm != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_forearm);
                 RefreshModifiers(uid);
            }

            ItemSlotsSystem.TryGetSlot(uid, "gun_grip", out var slot);
            if (slot != null)
            {
                ItemSlotsSystem.TryEjectToHands(uid, slot, uid);
                ItemSlotsSystem.RemoveItemSlot(uid, slot);
            }


            if (_NewGrip != null)
            {
                ItemSlotsSystem.TryEjectToHands(uid, _NewGrip, uid);
                ItemSlotsSystem.RemoveItemSlot(uid, _NewGrip);
            }


            Appearance.SetData(uid, AmmoVisuals.Forearm, false, appearance);
            return;
        }
    }

}




