using Content.Shared.CCVar;
using Content.Shared.Drugs;
using Content.Shared.GG.Drugs;
using Content.Shared.StatusEffect;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;
using Content.Shared.FixedPoint;


namespace Content.Client.GG.Drugs;

public sealed class TunnelVisionOverlay : Overlay
{
    [Dependency] private readonly IConfigurationManager _config = default!;

    private ISawmill _sawmill = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntitySystemManager _sysMan = default!;

[Dependency] private readonly ILogManager _logManager = default!;
    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;
    private readonly ShaderInstance _tunnelShader;

    public float TunnelLevel = 0f;
    private float _oldTunnelLevel = 0f;

    public float TimeTicker = 0.0f;
    private const float VisualThreshold = 10.0f;
    private const float PowerDivisor = 25.0f;

    private float EffectScale => Math.Clamp((TunnelLevel + 50 - VisualThreshold) / PowerDivisor, 0.0f, 1.0f);

    public TunnelVisionOverlay()
    {
        IoCManager.InjectDependencies(this);
       _tunnelShader = _prototypeManager.Index<ShaderPrototype>("GradientCircleMask").InstanceUnique();
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        var playerEntity = _playerManager.LocalEntity;

        if (playerEntity == null)
        {
            return;
        }


        if (!_entityManager.HasComponent<TunnelVisionComponent>(playerEntity)
            || !_entityManager.TryGetComponent<StatusEffectsComponent>(playerEntity, out var status))
            {
                return;
            }


        var statusSys = _sysMan.GetEntitySystem<StatusEffectsSystem>();
        if (!statusSys.TryGetTime(playerEntity.Value, TunnelVisionOverlaySystem.TunnelVisionKey, out var time, status))
        {
            return;

        }

        var timeLeft = (float) (time.Value.Item2 - time.Value.Item1).TotalSeconds;
        TimeTicker += args.DeltaSeconds;

        if (timeLeft - TimeTicker > timeLeft / 8f)
        {
            TunnelLevel += (timeLeft - TunnelLevel) * args.DeltaSeconds / 8f;

        }
        else
        {

           TunnelLevel -= (timeLeft - TunnelLevel) * args.DeltaSeconds;
        }


    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (!_entityManager.TryGetComponent(_playerManager.LocalEntity, out EyeComponent? eyeComp))
            return false;

        if (args.Viewport.Eye != eyeComp.Eye)
            return false;


        return EffectScale > 0;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (!_entityManager.TryGetComponent(_playerManager.LocalEntity, out EyeComponent? eyeComp))
            return;


        if (args.Viewport.Eye != eyeComp.Eye)
            return;

        var viewport = args.WorldAABB;
        var handle = args.WorldHandle;
        var distance = args.ViewportBounds.Width;

        var time = (float) _timing.RealTime.TotalSeconds;
        var lastFrameTime = (float) _timing.FrameTime.TotalSeconds;

        if (!MathHelper.CloseTo(_oldTunnelLevel, TunnelLevel, 0.001f))
        {
            var diff = TunnelLevel - _oldTunnelLevel;
            _oldTunnelLevel += GetDiff(diff, lastFrameTime);
        }
        else
        {
            _oldTunnelLevel = TunnelLevel;
        }

        // float level = _oldTunnelLevel / 10f;
        float level = _oldTunnelLevel / 4;

        if (level > 0f)
        {
            var pulseRate = 2f;
            var adjustedTime = time * pulseRate;
            float outerMaxLevel = 2.0f * distance;
            float outerMinLevel = 0.8f * distance;
            float innerMaxLevel = 0.36f * distance;
            float innerMinLevel = 0.18f * distance;

            var spookyass = level < 1.1f ? level : 1.1f;

            var outerRadius = outerMaxLevel - spookyass * (outerMaxLevel - outerMinLevel);
            var innerRadius = innerMaxLevel - spookyass * (innerMaxLevel - innerMinLevel);
            // float outerDarkness;

            // outerDarkness = MathF.Min(0.098f, 0.3f * MathF.Log(level) + 0.1f);
            var pulse = MathF.Max(0f, MathF.Sin(adjustedTime));
            var outerDarkness = MathF.Min(0.98f, 0.3f * MathF.Log(level) + 1f);

            _tunnelShader.SetParameter("time", pulse);
            _tunnelShader.SetParameter("color", new Vector3(0f, 0f, 0f));
            _tunnelShader.SetParameter("darknessAlphaOuter", outerDarkness);
            _tunnelShader.SetParameter("outerCircleRadius", innerRadius );
            _tunnelShader.SetParameter("outerCircleMaxRadius", innerRadius + 0.3f * distance );
            handle.UseShader(_tunnelShader);
            handle.DrawRect(viewport, Color.Black);

        }

        handle.UseShader(null);
    }

    private float GetDiff(float value, float lastFrameTime)
    {
        var adjustment = value * 5f * lastFrameTime;

        if (value < 0f)
            adjustment = Math.Clamp(adjustment, value, -value);
        else
            adjustment = Math.Clamp(adjustment, -value, value);

        return adjustment;
    }
}
