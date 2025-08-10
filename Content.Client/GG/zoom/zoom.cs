using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared._Sunrise.SniperZoom;
using Robust.Shared.Enums;
using Content.Client.Viewport;
using Robust.Client.Input;
using System.Numerics;
using System;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.Input;
using Robust.Shared.GameObjects;
using Robust.Shared.Map;
using Robust.Shared.Maths;
using Robust.Shared.Prototypes;
using System.Collections.Generic;
using Content.Shared.Weapons.Ranged.Components;
using Content.Shared._Sunrise.SniperZoom;

namespace Content.Client._Sunrise.SniperZoom
{
    public sealed class SniperZoomOverlay : Overlay
    {
        private readonly IEntityManager _entityManager;
        private readonly IPlayerManager _playerManager;
        private readonly TransformSystem _transformSystem;
        private readonly IInputManager _inputManager;
        private readonly IEyeManager _eyeManager;
        private readonly ShaderInstance _blurShader;

        public override OverlaySpace Space => OverlaySpace.WorldSpace;

        public SniperZoomOverlay(IEntityManager entityManager, IPlayerManager playerManager, IPrototypeManager prototypeManager, TransformSystem transformSystem, IInputManager inputManager, IEyeManager eyeManager)
        {
            _entityManager = entityManager;
            _playerManager = playerManager;
            _transformSystem = transformSystem;
            _inputManager = inputManager;
            _eyeManager = eyeManager;

            _blurShader = prototypeManager.Index<ShaderPrototype>("GreyscaleFullscreen").Instance();
        }

        protected override void Draw(in OverlayDrawArgs args)
        {
            var player = _playerManager.LocalPlayer?.ControlledEntity;
            if (player == null || !_entityManager.TryGetComponent(player.Value, out GunComponent? gunComponent))
                return;

            if (!_entityManager.TryGetComponent(player.Value, out ZoomableGunComponent? zoomable) || !zoomable.Enabled)
                return;

            var angle = (float) gunComponent.MaxAngle.Degrees; // Угол обзора для треугольника, преобразованный в float.

            // Координаты персонажа в мире.
            var xform = _entityManager.GetComponent<TransformComponent>(player.Value);
            var worldPosition = _transformSystem.GetWorldPosition(xform);

            // Получение текущей камеры (глаз игрока).
            var screenPosition = _eyeManager.WorldToScreen(worldPosition);


            // Положение курсора на экране.
            var mouseScreenPosition = _inputManager.MouseScreenPosition.Position;
            var direction = (mouseScreenPosition - screenPosition).Normalized();

            // Рассчитываем точки треугольника.
            var baseLength = 300.0f; // Длина основания треугольника.
            var height = 600.0f; // Высота треугольника.

            // Вращение вектора с использованием тригонометрических функций.
            var leftDirection = RotateVector(direction, -(float)angle / 2);
            var rightDirection = RotateVector(direction, (float)angle / 2);

            var leftPoint = screenPosition + leftDirection * baseLength;
            var rightPoint = screenPosition + rightDirection * baseLength;
            var topPoint = screenPosition + direction * height;

            // Рисуем серое размытие с треугольной областью видимости.
            args.ScreenHandle.UseShader(_blurShader);
            args.ScreenHandle.DrawRect(args.ViewportBounds, Color.Gray.WithAlpha(0.6f));

            // Создаем путь для треугольной области.
            var triangle = new List<Vector2> { screenPosition, leftPoint, topPoint, rightPoint };
            args.ScreenHandle.DrawPrimitives(DrawPrimitiveTopology.TriangleFan, triangle, Color.Transparent);
        }

        private Vector2 RotateVector(Vector2 vector, float angle)
        {
            var radians = MathHelper.DegreesToRadians(angle);
            var cos = MathF.Cos(radians);
            var sin = MathF.Sin(radians);

            var rotatedX = cos * vector.X - sin * vector.Y;
            var rotatedY = sin * vector.X + cos * vector.Y;

            return new Vector2(rotatedX, rotatedY);
        }
    }
}

// Код регистрации оверлея в системе.
