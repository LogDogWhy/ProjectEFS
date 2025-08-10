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

    private void InitializeScopeVisuals()
    {
        SubscribeLocalEvent<ScopeVisualsComponent, ComponentInit>(OnScopeVisualsInit);
        SubscribeLocalEvent<ScopeVisualsComponent, AppearanceChangeEvent>(OnScopeVisualsChange);

    }

    private void OnScopeVisualsInit(EntityUid uid, ScopeVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (sprite.LayerMapTryGet(GunVisualLayers.Scope, out _))
        {
            sprite.LayerSetVisible(GunVisualLayers.Scope, false);
        }

    }

    private void OnScopeVisualsChange(EntityUid uid, ScopeVisualsComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        var sprite = args.Sprite;
        var scope = GetScopeEntity(uid); // Retrieve the scope entity

        if (!args.AppearanceData.TryGetValue(AmmoVisuals.Scope, out var scopeAttached) || scopeAttached is not bool attached)
            return;

        if (attached && scope != null)
        {
            // If a scope is attached, update the scope layer's appearance
            if (TryComp<SpriteComponent>(scope.Value, out var scopeSprite))
            {
                // Get the sprite state of the scope
                var scopeState = scopeSprite.LayerGetState(0); // Assuming the scope sprite uses the base layer (0)

                if (scopeState != null)
                {
                    var attachmentStateName = $"attachment-{scopeState}";
                    // Set the gun scope layer to match the scope's sprite state
                    if (sprite.LayerMapTryGet(GunVisualLayers.Scope, out var layer))
                    {
                        sprite.LayerSetState(layer, new RSI.StateId(attachmentStateName));
                        sprite.LayerSetVisible(layer, true);
                    }
                }
            }
        }
        else
        {
            // If no Scope is attached, hide the Scope layer
            if (sprite.LayerMapTryGet(GunVisualLayers.Scope, out var layer))
            {
                sprite.LayerSetVisible(layer, false);
            }
        }
    }
}

