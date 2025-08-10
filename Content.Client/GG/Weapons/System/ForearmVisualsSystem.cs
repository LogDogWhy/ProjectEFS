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

    private void InitializeForearmVisuals()
    {
        SubscribeLocalEvent<ForearmVisualsComponent, ComponentInit>(OnForearmVisualsInit);
        SubscribeLocalEvent<ForearmVisualsComponent, AppearanceChangeEvent>(OnForearmVisualsChange);

    }

    private void OnForearmVisualsInit(EntityUid uid, ForearmVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (sprite.LayerMapTryGet(GunVisualLayers.Forearm, out _))
        {
            sprite.LayerSetVisible(GunVisualLayers.Forearm, false);
        }

    }

    private void OnForearmVisualsChange(EntityUid uid, ForearmVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var sprite = args.Sprite;
        var forearm = GetForearmEntity(uid); // Retrieve the forearm entity

        if (!args.AppearanceData.TryGetValue(AmmoVisuals.Forearm, out var forearmAttached) || forearmAttached is not bool attached)
            return;

        if (attached && forearm != null)
        {
            // If a forearm is attached, update the forearm layer's appearance
            if (TryComp<SpriteComponent>(forearm.Value, out var forearmSprite))
            {
                // Get the sprite state of the forearm
                var forearmState = forearmSprite.LayerGetState(0); // Assuming the forearm sprite uses the base layer (0)

                if (forearmState != null)
                {
                    var attachmentStateName = $"attachment-{forearmState}";
                    // Set the gun forearm layer to match the forearm's sprite state
                    if (sprite.LayerMapTryGet(GunVisualLayers.Forearm, out var layer))
                    {
                        sprite.LayerSetState(layer, new RSI.StateId(attachmentStateName));
                        sprite.LayerSetVisible(layer, true);
                    }
                }
            }
        }
        else
        {
            // If no forearm is attached, hide the forearm layer
            if (sprite.LayerMapTryGet(GunVisualLayers.Forearm, out var layer))
            {
                sprite.LayerSetVisible(layer, false);
            }
        }

    }
}

