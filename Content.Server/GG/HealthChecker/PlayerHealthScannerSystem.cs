using Content.Shared.Actions;
using Content.Shared.Actions.Events;
using Content.Server.Medical;
using Content.Shared.Movement.Events;
using Content.Shared.MedicalScanner;
using Robust.Server.GameObjects;
using Robust.Shared.GameObjects;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Content.Server.Body.Components;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Server.Temperature.Components;
using Content.Shared.Damage;
using Content.Server.Actions;
using Content.Shared.HealthScanner;
using Content.Shared.Toggleable;
using Robust.Shared.Timing;
using Content.Shared.Implants.Components;

namespace Content.Server.HealthScanner
{
    public partial class PlayerHealthAnalyzerSystem : EntitySystem
    {
        [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
        [Dependency] private readonly UserInterfaceSystem _uiSystem = default!;
        [Dependency] private readonly HealthAnalyzerSystem _healthAnalyzerSystem = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        [Dependency] private readonly ActionsSystem _actions = default!;

        [Dependency] private readonly SolutionContainerSystem _solutionContainerSystem = default!;

        [Dependency] private readonly IGameTiming _timing = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<PlayerHealthAnalyzerComponent, MapInitEvent>(OnAnalyzerMapInit);
            SubscribeLocalEvent<PlayerHealthAnalyzerComponent, ComponentShutdown>(OnAnalyzerShutdown);
            SubscribeLocalEvent<PlayerHealthAnalyzerComponent, ActivateImplantEvent>(OnAnalyzerToggle);
            //SubscribeLocalEvent<PlayerHealthAnalyzerComponent, HealthAnalyzerActionEvent>(OnUIActivate);
        }


        public override void Update(float frameTime)
        {
            var analyzerQuery = EntityQueryEnumerator<PlayerHealthAnalyzerComponent, TransformComponent>();
            while (analyzerQuery.MoveNext(out var uid, out var component, out var transform))
            {
                //Update rate limited to 1 second
                if (component.NextUpdate > _timing.CurTime)
                    continue;

                if (component.ActionEntity is not {} patient)
                    continue;

                component.NextUpdate = _timing.CurTime + component.UpdateInterval;

                //Get distance between health analyzer and the scanned entity

                UpdateScannedUser(uid, patient, true);
            }
        }
        private void OnAnalyzerMapInit(EntityUid uid, PlayerHealthAnalyzerComponent component, MapInitEvent args)
        {
            if (!_prototypeManager.TryIndex<EntityPrototype>(component.Action, out var entityPrototype))
            {
                Logger.ErrorS("PlayerHealthAnalyzer", $"Failed to load {component.Action} prototype.");
                return;
            }

            _actions.AddAction(uid, ref component.ActionEntity, component.Action, uid);
        }

        private void OnAnalyzerShutdown(EntityUid uid, PlayerHealthAnalyzerComponent component, ComponentShutdown args)
        {
            _actions.RemoveAction(uid, component.ActionEntity);
        }

        private void OnAnalyzerToggle(EntityUid uid, PlayerHealthAnalyzerComponent component, ref ActivateImplantEvent args)
        {
            Logger.ErrorS("PlayerHealthAnalyzer", $"Failed to toggle {component.Action} prototype.");
            // if (args.Handled)
            //     return;

            // args.Handled = true;
            OpenUserInterface(args.Performer, uid);
        }


        private void OpenUserInterface(EntityUid user, EntityUid analyzer)
        {
            Logger.ErrorS("PlayerHealthAnalyzer", $"Failed to open  prototype.");
            if (!_uiSystem.HasUi(analyzer, HealthAnalyzerUiKey.Key))
                return;

            _uiSystem.OpenUi(analyzer, HealthAnalyzerUiKey.Key, user);
            UpdateScannedUser(analyzer, user, true);
        }


        public void UpdateScannedUser(EntityUid healthAnalyzer, EntityUid target, bool scanMode)
        {
            if (!_uiSystem.HasUi(healthAnalyzer, HealthAnalyzerUiKey.Key))
                return;

            if (!HasComp<DamageableComponent>(target))
                return;

            var bodyTemperature = float.NaN;

            if (TryComp<TemperatureComponent>(target, out var temp))
                bodyTemperature = temp.CurrentTemperature;

            var bloodAmount = float.NaN;
            var bleeding = false;

            if (TryComp<BloodstreamComponent>(target, out var bloodstream) &&
                _solutionContainerSystem.ResolveSolution(target, bloodstream.BloodSolutionName,
                    ref bloodstream.BloodSolution, out var bloodSolution))
            {
                bloodAmount = bloodSolution.FillFraction;
                bleeding = bloodstream.BleedAmount > 0;
            }

            _uiSystem.ServerSendUiMessage(healthAnalyzer, HealthAnalyzerUiKey.Key, new HealthAnalyzerScannedUserMessage(
                GetNetEntity(target),
                bodyTemperature,
                bloodAmount,
                scanMode,
                bleeding
            ));
        }
    }


}
