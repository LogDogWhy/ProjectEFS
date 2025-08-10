using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Nutrition.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Containers;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Content.Shared.Whitelist;
using Robust.Shared.Prototypes;
using Content.Shared.Containers.ItemSlots;
using Content.Shared.Tag;
using Linguini.Bundle.Resolver;

namespace Content.Shared.Weapons.Ranged.Systems;

public abstract partial class SharedGunSystem
{
    protected const string ReceiverSlot = "gun_receiver";

    private Dictionary<string, float>? m_slot_receiver = new();

    private List<ProtoId<TagPrototype>> _tagsSlot_receiver = new();

    private ItemSlot? _NewScope;
    private ItemSlot? _TempScope;

    private bool check = false;

    private string? _scope_name;

    private bool blacklist_receiver = false;



    protected virtual void InitializeReceiver()
    {
        SubscribeLocalEvent<ReceiverComponent, EntInsertedIntoContainerMessage>(OnReceiverSlotChange);
        SubscribeLocalEvent<ReceiverComponent, EntRemovedFromContainerMessage>(OnReceiverSlotChange);
    }

    protected EntityUid? GetReceiverEntity(EntityUid uid)
    {
        if (!Containers.TryGetContainer(uid, ReceiverSlot, out var container) ||
            container is not ContainerSlot slot)
        {
            return null;
        }

        if(TryComp<AttachmentComponent>(slot.ContainedEntity, out var dic) )
        {
            m_slot_receiver = _attachmentSystem.GetAllStats(dic);
            if (dic.Tags != null)
                _tagsSlot_receiver = dic.Tags;
            if (dic.NewSlot != null)
                _scope_name = dic.NewSlot;
            else
                _scope_name = null;
            blacklist_receiver = dic.Swap;

        }

        return slot.ContainedEntity;
    }

    protected virtual void OnReceiverSlotChange(EntityUid uid, ReceiverComponent component, ContainerModifiedMessage args)
    {
        if (ReceiverSlot != args.Container.ID)
            return;

        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        var receiver = GetReceiverEntity(uid);
        ItemSlotsSystem.TryGetSlot(uid, "gun_scope", out var slot);
        if(check == false)
        {
            _TempScope = slot;
            if (_TempScope != null &&
                _TempScope.Whitelist != null &&
                _TempScope.Whitelist.Tags != null)
                check = true;
        }

        if (receiver != null)
        {
             if (TryComp<GunComponent>(uid, out var gun) && gun != null)
             {
                if(m_slot_receiver != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_receiver);
                 RefreshModifiers(uid);
             }

            if(_scope_name != null)
            {
                _NewScope = new()
                {
                    Whitelist = new EntityWhitelist
                    {
                        Tags = _tagsSlot_receiver,
                    }

                };
                ItemSlotsSystem.AddItemSlot(uid, _scope_name, _NewScope);
            }
            else
            {
                slot = _TempScope;
            }


            Appearance.SetData(uid, AmmoVisuals.Receiver, true, appearance);

        }
        else
        {
            if (_entityManager.TryGetComponent(uid, out GunComponent? gun))
            {
                if(m_slot_receiver != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_receiver);
                 RefreshModifiers(uid);
            }

            if (slot != null)
            {
                ItemSlotsSystem.TryEjectToHands(uid, slot, uid);
                if(TryComp<AttachmentComponent>(receiver, out var dic) && dic.Swap == true)
                {
                    ItemSlotsSystem.RemoveItemSlot(uid, slot);
                }

                if (_scope_name != null && _TempScope != null)
                    ItemSlotsSystem.AddItemSlot(uid, _scope_name, _TempScope);

            }


            Appearance.SetData(uid, AmmoVisuals.Receiver, false, appearance);
            return;
        }
    }

}




