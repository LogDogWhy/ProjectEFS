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

    private void InitializeStockVisuals()
    {
        SubscribeLocalEvent<StockVisualsComponent, ComponentInit>(OnStockVisualsInit);
        SubscribeLocalEvent<StockVisualsComponent, AppearanceChangeEvent>(OnStockVisualsChange);

    }

    private void OnStockVisualsInit(EntityUid uid, StockVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (sprite.LayerMapTryGet(GunVisualLayers.Stock, out _))
        {
            sprite.LayerSetVisible(GunVisualLayers.Stock, false);
        }

    }

private void OnStockVisualsChange(EntityUid uid, StockVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var sprite = args.Sprite;
        var stock = GetStockEntity(uid); // Retrieve the stock entity

        if (!args.AppearanceData.TryGetValue(AmmoVisuals.Stock, out var stockAttached) || stockAttached is not bool attached)
            return;

        if (attached && stock != null)
        {
            // If a stock is attached, update the stock layer's appearance
            if (TryComp<SpriteComponent>(stock.Value, out var stockSprite))
            {
                // Get the sprite state of the stock
                var stockState = stockSprite.LayerGetState(0); // Assuming the stock sprite uses the base layer (0)

                if (stockState != null)
                {
                    var attachmentStateName = $"attachment-{stockState}";
                    // Set the gun stock layer to match the stock's sprite state
                    if (sprite.LayerMapTryGet(GunVisualLayers.Stock, out var layer))
                    {
                        sprite.LayerSetState(layer, new RSI.StateId(attachmentStateName));
                        sprite.LayerSetVisible(layer, true);
                    }
                }
            }
        }
        else
        {
            // If no Stock is attached, hide the Stock layer
            if (sprite.LayerMapTryGet(GunVisualLayers.Stock, out var layer))
            {
                sprite.LayerSetVisible(layer, false);
            }
        }
    }
}

