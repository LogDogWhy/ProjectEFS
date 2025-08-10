using Content.Shared.Drugs;
using Robust.Shared.Player;
using Content.Shared.Camera;

namespace Content.Shared.GG.Drugs;

public sealed class RecoilReducerSystem : EntitySystem
{

    [Dependency] private readonly EntityManager _entityManager = default!;
    public override void Initialize()
    {

        SubscribeLocalEvent<RecoilReducerComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<RecoilReducerComponent, ComponentShutdown>(OnShutdown);
    }


    private void OnInit(EntityUid uid, RecoilReducerComponent component, ComponentInit args)
    {
        // _entityManager.RemoveComponent<CameraRecoilComponent>(uid);
        if(_entityManager.TryGetComponent<CameraRecoilComponent>(uid, out  var comp) && comp != null)
            comp.Modificator = 0.5f;
    }

    private void OnShutdown(EntityUid uid, RecoilReducerComponent component, ComponentShutdown args)
    {
        if(_entityManager.TryGetComponent<CameraRecoilComponent>(uid, out  var comp) && comp != null)
            comp.Modificator = 1f;
        // _entityManager.AddComponent<CameraRecoilComponent>(uid);
    }
}
