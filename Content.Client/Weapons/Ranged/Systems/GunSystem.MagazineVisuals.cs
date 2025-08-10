using Content.Client.Weapons.Ranged.Components;
using Content.Shared.Rounding;
using Content.Shared.Weapons.Ranged.Systems;
using Robust.Client.GameObjects;
using System.IO;
using Robust.Client.Graphics;

namespace Content.Client.Weapons.Ranged.Systems;

public sealed partial class GunSystem
{
    private void InitializeMagazineVisuals()
    {
        SubscribeLocalEvent<MagazineVisualsComponent, ComponentInit>(OnMagazineVisualsInit);
        SubscribeLocalEvent<MagazineVisualsComponent, AppearanceChangeEvent>(OnMagazineVisualsChange);
    }

    private void OnMagazineVisualsInit(EntityUid uid, MagazineVisualsComponent component, ComponentInit args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite)) return;

        if (sprite.LayerMapTryGet(GunVisualLayers.Mag, out _))
        {
            sprite.LayerSetState(GunVisualLayers.Mag, $"{component.MagState}-{component.MagSteps - 1}");
            sprite.LayerSetVisible(GunVisualLayers.Mag, false);
        }

        if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, out _))
        {
            sprite.LayerSetState(GunVisualLayers.MagUnshaded, $"{component.MagState}-unshaded-{component.MagSteps - 1}");
            sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, false);
        }
    }

    private void OnMagazineVisualsChange(EntityUid uid, MagazineVisualsComponent component, ref AppearanceChangeEvent args)
    {
        var sprite = args.Sprite;

        if (sprite == null) return;

        if (component.DynamicSprite == false)
        {
            // Существующая логика для статического магазина
            if (!args.AppearanceData.TryGetValue(AmmoVisuals.MagLoaded, out var magloaded) ||
                magloaded is true)
            {
                if (!args.AppearanceData.TryGetValue(AmmoVisuals.AmmoMax, out var capacity))
                {
                    capacity = component.MagSteps;
                }

                if (!args.AppearanceData.TryGetValue(AmmoVisuals.AmmoCount, out var current))
                {
                    current = component.MagSteps;
                }

                var step = ContentHelpers.RoundToLevels((int)current, (int)capacity, component.MagSteps);

                if (step == 0 && !component.ZeroVisible)
                {
                    if (sprite.LayerMapTryGet(GunVisualLayers.Mag, out _))
                    {
                        sprite.LayerSetVisible(GunVisualLayers.Mag, false);
                    }

                    if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, out _))
                    {
                        sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, false);
                    }

                    return;
                }

                if (sprite.LayerMapTryGet(GunVisualLayers.Mag, out _))
                {
                    sprite.LayerSetVisible(GunVisualLayers.Mag, true);
                    sprite.LayerSetState(GunVisualLayers.Mag, $"{component.MagState}-{step}");
                }

                if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, out _))
                {
                    sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, true);
                    sprite.LayerSetState(GunVisualLayers.MagUnshaded, $"{component.MagState}-unshaded-{step}");
                }
            }
            else
            {
                if (sprite.LayerMapTryGet(GunVisualLayers.Mag, out _))
                {
                    sprite.LayerSetVisible(GunVisualLayers.Mag, false);
                }

                if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, out _))
                {
                    sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, false);
                }
            }
        }
        else
        {
            // Начало новой системы для динамического отображения магазина
            var magazine = GetMagazineEntity(uid);
            if (magazine != null && TryComp<SpriteComponent>(magazine.Value, out var magazineSprite))
            {
                string rsiName = string.Empty;

                if (magazineSprite.BaseRSI != null)
                {
                    // Получаем путь к ресурсу RSI как строку
                    var rsiPath = magazineSprite.BaseRSI.Path.ToString();

                    if (!string.IsNullOrEmpty(rsiPath))
                    {
                        // Извлекаем имя файла без расширения с помощью работы со строками
                        int lastSlashIndex = rsiPath.LastIndexOf('/');
                        if (lastSlashIndex != -1 && lastSlashIndex + 1 < rsiPath.Length)
                        {
                            rsiName = rsiPath.Substring(lastSlashIndex + 1);
                        }

                        // Убираем расширение .rsi
                        int extensionIndex = rsiName.LastIndexOf('.');
                        if (extensionIndex != -1)
                        {
                            rsiName = rsiName.Substring(0, extensionIndex);
                        }
                    }
                }

                // Если имя папки пустое, то присваиваем стандартное состояние "mag"
                string newMagState;
                newMagState = $"{rsiName}mag";

                // Устанавливаем новое состояние для слоя магазина
                if (sprite.LayerMapTryGet(GunVisualLayers.Mag, out var layer))
                {
                    sprite.LayerSetVisible(layer, true);
                    sprite.LayerSetState(layer, new RSI.StateId(newMagState));
                }
            }
            else
            {
                if (sprite.LayerMapTryGet(GunVisualLayers.Mag, out _))
                {
                    sprite.LayerSetVisible(GunVisualLayers.Mag, false);
                }

                if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, out _))
                {
                    sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, false);
                }

                return;
            }

            // Обновление отображения патронов в магазине
            if (!args.AppearanceData.TryGetValue(AmmoVisuals.AmmoMax, out var capacity))
            {
                capacity = component.MagSteps;
            }

            if (!args.AppearanceData.TryGetValue(AmmoVisuals.AmmoCount, out var current))
            {
                current = component.MagSteps;
            }

            var step = ContentHelpers.RoundToLevels((int)current, (int)capacity, component.MagSteps);

            if (step == 0 && !component.ZeroVisible)
            {
                if (sprite.LayerMapTryGet(GunVisualLayers.Mag, out _))
                {
                    sprite.LayerSetVisible(GunVisualLayers.Mag, false);
                }

                if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, out _))
                {
                    sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, false);
                }

                return;
            }

            if (sprite.LayerMapTryGet(GunVisualLayers.MagUnshaded, out _))
            {
                sprite.LayerSetVisible(GunVisualLayers.MagUnshaded, true);
                sprite.LayerSetState(GunVisualLayers.MagUnshaded, $"{component.MagState}-unshaded-{step}");
            }
        }
    }
}
