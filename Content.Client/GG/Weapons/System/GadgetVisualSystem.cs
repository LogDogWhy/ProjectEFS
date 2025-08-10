using Content.Client.Weapons.Ranged.Components;
using Content.Shared.Rounding;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.GameObjects;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Player;

namespace Content.Client.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{

    private void InitializeGadgetVisuals()
    {
        SubscribeLocalEvent<GadgetVisualsComponent, ComponentInit>(OnGadgetVisualsInit);
        SubscribeLocalEvent<GadgetVisualsComponent, AppearanceChangeEvent>(OnGadgetVisualsChange);

    }

    private void OnGadgetVisualsInit(EntityUid uid, GadgetVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (sprite.LayerMapTryGet(GunVisualLayers.Gadget, out _))
        {
            sprite.LayerSetVisible(GunVisualLayers.Gadget, false);
        }

    }

    private void OnGadgetVisualsChange(EntityUid uid, GadgetVisualsComponent component, ref AppearanceChangeEvent args)
    {
        var sprite = args.Sprite;

        if (sprite == null) return;


        if (!args.AppearanceData.TryGetValue(AmmoVisuals.Gadget, out var gadgetAttached) ||
            gadgetAttached is true)
        {
                sprite.LayerSetVisible(GunVisualLayers.Gadget, true);
        }
        else
        {
            sprite.LayerSetVisible(GunVisualLayers.Gadget, false);
        }

        }
    }

