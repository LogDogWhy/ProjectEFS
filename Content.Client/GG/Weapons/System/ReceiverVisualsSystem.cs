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

    private void InitializeReceiverVisuals()
    {
        SubscribeLocalEvent<ReceiverVisualsComponent, ComponentInit>(OnReceiverVisualsInit);
        SubscribeLocalEvent<ReceiverVisualsComponent, AppearanceChangeEvent>(OnReceiverVisualsChange);

    }

    private void OnReceiverVisualsInit(EntityUid uid, ReceiverVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (sprite.LayerMapTryGet(GunVisualLayers.Receiver, out _))
        {
            sprite.LayerSetVisible(GunVisualLayers.Receiver, false);
        }

    }

    private void OnReceiverVisualsChange(EntityUid uid, ReceiverVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var sprite = args.Sprite;
        var receiver = GetReceiverEntity(uid); // Retrieve the receiver entity

        if (!args.AppearanceData.TryGetValue(AmmoVisuals.Receiver, out var receiverAttached) || receiverAttached is not bool attached)
            return;

        if (attached && receiver != null)
        {
            // If a receiver is attached, update the receiver layer's appearance
            if (TryComp<SpriteComponent>(receiver.Value, out var receiverSprite))
            {
                // Get the sprite state of the receiver
                var receiverState = receiverSprite.LayerGetState(0); // Assuming the receiver sprite uses the base layer (0)

                if (receiverState != null)
                {
                    var attachmentStateName = $"attachment-{receiverState}";
                    // Set the gun Receiver layer to match the Receiver's sprite state
                    if (sprite.LayerMapTryGet(GunVisualLayers.Receiver, out var layer))
                    {
                        sprite.LayerSetState(layer, new RSI.StateId(attachmentStateName));
                        sprite.LayerSetVisible(layer, true);
                    }
                }
            }
        }
        else
        {
            // If no Receiver is attached, hide the Receiver layer
            if (sprite.LayerMapTryGet(GunVisualLayers.Receiver, out var layer))
            {
                sprite.LayerSetVisible(layer, false);
            }
        }
    }
}

