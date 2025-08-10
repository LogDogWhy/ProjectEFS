using Content.Client.Weapons.Ranged.Components;
using Content.Shared.Rounding;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.GameObjects;
using Content.Shared.Popups;
using Robust.Shared.Audio;
using Robust.Shared.Player;
using Robust.Client.Graphics;

namespace Content.Client.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    private void InitializeSuppressorVisuals()
    {
        SubscribeLocalEvent<SuppressorVisualsComponent, ComponentInit>(OnSuppressorVisualsInit);
        SubscribeLocalEvent<SuppressorVisualsComponent, AppearanceChangeEvent>(OnSuppressorVisualsChange);
    }

    private void OnSuppressorVisualsInit(EntityUid uid, SuppressorVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        // Ensure that the suppressor layer is initially hidden
        if (sprite.LayerMapTryGet(GunVisualLayers.Suppressor, out _))
        {
            sprite.LayerSetVisible(GunVisualLayers.Suppressor, false);
        }
    }

    private void OnSuppressorVisualsChange(EntityUid uid, SuppressorVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var sprite = args.Sprite;
        var sup = GetSuppressorEntity(uid); // Retrieve the suppressor entity

        if (!args.AppearanceData.TryGetValue(AmmoVisuals.Suppressor, out var suppressorAttached) || suppressorAttached is not bool attached)
            return;

        if (attached && sup != null)
        {
            // If a suppressor is attached, update the suppressor layer's appearance
            if (TryComp<SpriteComponent>(sup.Value, out var suppressorSprite))
            {
                // Get the sprite state of the suppressor
                var suppressorState = suppressorSprite.LayerGetState(0); // Assuming the suppressor sprite uses the base layer (0)

                if (suppressorState != null)
                {
                    var attachmentStateName = $"attachment-{suppressorState}";
                    // Set the gun suppressor layer to match the suppressor's sprite state
                    if (sprite.LayerMapTryGet(GunVisualLayers.Suppressor, out var layer))
                    {
                        sprite.LayerSetState(layer, new RSI.StateId(attachmentStateName));
                        sprite.LayerSetVisible(layer, true);
                    }
                }
            }
        }
        else
        {
            // If no suppressor is attached, hide the suppressor layer
            if (sprite.LayerMapTryGet(GunVisualLayers.Suppressor, out var layer))
            {
                sprite.LayerSetVisible(layer, false);
            }
        }
    }
}
