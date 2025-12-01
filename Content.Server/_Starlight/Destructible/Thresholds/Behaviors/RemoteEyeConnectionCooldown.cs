using Content.Server.Destructible.Thresholds.Behaviors;
using Content.Server.Destructible;
using Content.Server._Starlight.Computers.RemoteEye;
using Content.Shared._Starlight.Computers.RemoteEye;

namespace Content.Server._Starlight.Destructible.Thresholds.Behaviors;

[Serializable]
[DataDefinition]
public sealed partial class RemoteEyeConnectionCooldown : IThresholdBehavior
{
    [DataField("cooldown")]
    public TimeSpan Cooldown = TimeSpan.FromMinutes(1);
    public void Execute(EntityUid owner, DestructibleSystem system, EntityUid? cause = null)
    {
        var _entityManager = system.EntityManager;
        var _remoteEye = _entityManager.EntitySysManager.GetEntitySystem<RemoteEyeSystem>();
        if (!_remoteEye.TryGetRemoteEyeActor(owner, out var actor) 
            || !_entityManager.TryGetComponent<RemoteEyeActorComponent>(actor.Value, out var actorComp) 
            || actorComp.VirtualItem == null)
            return;

        _remoteEye.TrySetRemoteConnectionCooldown(actorComp.VirtualItem.Value, Cooldown);
    }
}