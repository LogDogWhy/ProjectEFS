using Content.Shared.Examine;
using Content.Shared.Interaction.Events;
using Content.Shared.Nutrition.Components;
using Content.Shared.Verbs;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared.Weapons.Ranged.Events;
using Robust.Shared.Containers;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Content.Shared.Weapons.Ranged.Systems;

namespace Content.Shared.Weapons.Ranged.Systems;

public abstract partial class SharedGunSystem
{
    protected const string SuppressorSlot = "gun_suppressor";

    private Dictionary<string, float>? m_slot_barrel = new();
    [Dependency] private readonly IEntityManager _entityManager = default!;

    [Dependency] private readonly SharedAttachmentSystem _attachmentSystem = default!;

    protected virtual void InitializeSuppressor()
    {

        SubscribeLocalEvent<SuppressorComponent, EntInsertedIntoContainerMessage>(OnSuppressorSlotChange);
        SubscribeLocalEvent<SuppressorComponent, EntRemovedFromContainerMessage>(OnSuppressorSlotChange);
    }

    protected EntityUid? GetSuppressorEntity(EntityUid uid)
    {

        if (!Containers.TryGetContainer(uid, SuppressorSlot, out var container) ||
            container is not ContainerSlot slot)
        {
            if (TryComp<GunComponent>(uid, out var gun) && gun != null)
            {

                gun.SoundGunshot = gun.SoundLoudGunshot;
            }
            return null;
        }

        if(TryComp<AttachmentComponent>(slot.ContainedEntity, out var dic))
        {
            m_slot_barrel = _attachmentSystem.GetAllStats(dic);
        }

        if(dic != null && dic.Sil == true)
        {
            if (TryComp<GunComponent>(uid, out var gun) && gun != null)
            {
                gun.SoundGunshot = gun.SoundSilGunshot;
            }

        }





        return slot.ContainedEntity;
    }

    protected virtual void OnSuppressorSlotChange(EntityUid uid, SuppressorComponent component, ContainerModifiedMessage args)
    {
        if (SuppressorSlot != args.Container.ID)
            return;

        if (!TryComp<AppearanceComponent>(uid, out var appearance))
            return;

        var sup = GetSuppressorEntity(uid);



        if (sup != null)
        {
            if (TryComp<GunComponent>(uid, out var gun) && gun != null)
            {
                if(m_slot_barrel != null)
                    _attachmentSystem.TryModifyStats(gun, m_slot_barrel);

            if(TryComp<AttachmentComponent>(sup, out var dic) && dic.Sil == true)
            {
                gun.SoundGunshot = gun.SoundSilGunshot;
            }
                RefreshModifiers(uid);
            }
            Appearance.SetData(uid, AmmoVisuals.Suppressor, true, appearance);

        }
        else
        {
            if (_entityManager.TryGetComponent(uid, out GunComponent? gun))
            {
                if(m_slot_barrel != null)
                    _attachmentSystem.TryUnmodifyStats(gun, m_slot_barrel);
                gun.SoundGunshot = gun.SoundLoudGunshot;

                RefreshModifiers(uid);
            }
            Appearance.SetData(uid, AmmoVisuals.Suppressor, false, appearance);
            return;
        }
    }

}




