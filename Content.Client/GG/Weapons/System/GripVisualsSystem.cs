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

    private void InitializeGripVisuals()
    {
        SubscribeLocalEvent<GripVisualsComponent, ComponentInit>(OnGripVisualsInit);
        SubscribeLocalEvent<GripVisualsComponent, AppearanceChangeEvent>(OnGripVisualsChange);

    }

    private void OnGripVisualsInit(EntityUid uid, GripVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (sprite.LayerMapTryGet(GunVisualLayers.Grip, out _))
        {
            sprite.LayerSetVisible(GunVisualLayers.Grip, false);
        }

    }

    private void OnGripVisualsChange(EntityUid uid, GripVisualsComponent component, ref AppearanceChangeEvent args)
    {
if (args.Sprite == null)
            return;

        var sprite = args.Sprite;
        var grip = GetGripEntity(uid); // Retrieve the grip entity

        if (!args.AppearanceData.TryGetValue(AmmoVisuals.Grip, out var gripAttached) || gripAttached is not bool attached)
            return;

        if (attached && grip != null)
        {
            // If a grip is attached, update the grip layer's appearance
            if (TryComp<SpriteComponent>(grip.Value, out var gripSprite))
            {
                // Get the sprite state of the grip
                var gripState = gripSprite.LayerGetState(0); // Assuming the grip sprite uses the base layer (0)

                if (gripState != null)
                {
                    var attachmentStateName = $"attachment-{gripState}";
                    // Set the gun grip layer to match the grip's sprite state
                    if (sprite.LayerMapTryGet(GunVisualLayers.Grip, out var layer))
                    {
                        sprite.LayerSetState(layer, new RSI.StateId(attachmentStateName));
                        sprite.LayerSetVisible(layer, true);
                    }
                }
            }
        }
        else
        {
            // If no grip is attached, hide the grip layer
            if (sprite.LayerMapTryGet(GunVisualLayers.Grip, out var layer))
            {
                sprite.LayerSetVisible(layer, false);
            }
        }

    }
}

