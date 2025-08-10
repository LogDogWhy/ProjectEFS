using Content.Shared.Drugs;
using Robust.Shared.Player;
using Content.Shared.Camera;
using Content.Shared.GG.Drugs;
using Content.Server._ES14.Weight.Components;
using Content.Server._ES14.Weight.Events;
using Content.Server._ES14.Weight.EntitySystems;

namespace Content.Server.GG.Drugs;

public sealed class StrengthBufferSystem : EntitySystem
{

    [Dependency] private readonly ESWeightSystem _weight = default!;
    [Dependency] private readonly EntityManager _entityManager = default!;
    public override void Initialize()
    {

        SubscribeLocalEvent<StrengthBufferComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<StrengthBufferComponent, ComponentShutdown>(OnShutdown);
    }


    private void OnInit(EntityUid uid, StrengthBufferComponent component, ComponentInit args)
    {
        // _entityManager.RemoveComponent<CameraRecoilComponent>(uid);
        if(_entityManager.TryGetComponent<ESWeightOverloadComponent>(uid, out  var comp) && comp != null)
            comp.Overload += 15;
        if(_entityManager.TryGetComponent<ESWeightComponent>(uid, out  var compUid) && compUid != null)
            _weight.TryUpdateWeight((uid, compUid));
    }

    private void OnShutdown(EntityUid uid, StrengthBufferComponent component, ComponentShutdown args)
    {
        if(_entityManager.TryGetComponent<ESWeightOverloadComponent>(uid, out  var comp) && comp != null)
            comp.Overload -= 15;
        if(_entityManager.TryGetComponent<ESWeightComponent>(uid, out  var compUid) && compUid != null)
            _weight.TryUpdateWeight((uid, compUid));
    }
}
