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
    protected const string ScopeSlot = "gun_scope";

    private Dictionary<string, float>? m_slot_scope = new();
    protected virtual void InitializeScope()
    {
        SubscribeLocalEvent<ScopeComponent, EntInsertedIntoContainerMessage>(OnScopeSlotChange);
        SubscribeLocalEvent<ScopeComponent, EntRemovedFromContainerMessage>(OnScopeSlotChange);
    }

    protected EntityUid? GetScopeEntity(EntityUid uid)
    {
        if (!Containers.TryGetContainer(uid, ScopeSlot, out var container) ||
            container is not ContainerSlot slot)
        {
            return null;
        }

        if(TryComp<AttachmentComponent>(slot.ContainedEntity, out var dic) )
        {
            m_slot_scope = _attachmentSystem.GetAllStats(dic);
        }

        return slot.ContainedEntity;
    }

    protected virtual void OnScopeSlotChange(EntityUid uid, ScopeComponent component, ContainerModifiedMessage args)
    {
        if (ScopeSlot != args.Container.ID)
            return;

        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        var scope = GetScopeEntity(uid);

        if (scope != null)
        {
             if (TryComp<GunComponent>(uid, out var gun) && gun != null)
             {
                if(m_slot_scope != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_scope);
                 RefreshModifiers(uid);
             }
            Appearance.SetData(uid, AmmoVisuals.Scope, true, appearance);

        }
        else
        {
             if (_entityManager.TryGetComponent(uid, out GunComponent? gun))
             {
                if(m_slot_scope != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_scope);
                 RefreshModifiers(uid);
             }
            Appearance.SetData(uid, AmmoVisuals.Scope, false, appearance);
            return;
        }
    }

}




